using System.IO;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Versions;
using CUE4Parse.MappingsProvider;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Objects.UObject;
using Asteria.Rest;
using Asteria.Models;
using Asteria.Managers;
using Asteria.Views;

namespace Asteria.ViewModels;

public class Dataminer 
{
    private DefaultFileProvider? Provider;

    private AssetsExpoter assetsExporter;

    public Dataminer()
    {
        if (string.IsNullOrEmpty(UserSettings.Settings.PaksPath))
        {
            Log.Warning("The paks folder path you inserted is invalid. Open the settings, change your paks path and restart.");
            AppVModel.Warn("Invalid Paks Folder Path", "The paks folder path you inserted is invalid. Open the settings, change your paks path and restart.");
            return;
        }

        Provider = new(UserSettings.Settings.PaksPath, SearchOption.TopDirectoryOnly, true);
        Provider.Versions = new VersionContainer(UserSettings.Settings.UeVersion);
        assetsExporter = new AssetsExpoter(Provider);
    }

    public async Task Init()
    {
        AppVModel.LoadingVM.UpdateText("Loading Paks");
        Provider.Initialize();
        AppVModel.LoadingVM.UpdateText("Loading Mappings");
        await LoadMappings();
        AppVModel.LoadingVM.UpdateText("Loading Encryption Keys");
        await LoadAesKeys();
    }

    // -------------------------------- extractors --------------------------------

    public void Extract(string gameFilePath)
    {
        UObject? exports = new();

        try
        {
            exports = Provider.LoadAllObjects(gameFilePath).FirstOrDefault();
        }
        catch (KeyNotFoundException)
        {
            AppVModel.OnPathNotFound($"No GameFile found for {gameFilePath}.");
            return;
        }

        CosmeticTypes check = GetExportType(exports);
        if (check == CosmeticTypes.Invalid)
        {
            AppVModel.OnInvalidCosmeticType(exports.ExportType, "AthenaMusicPackItemDefinition or AthenaDanceItemDefinition", exports.Name);
            return;
        }

        string? sound = (check == CosmeticTypes.Dance) ? assetsExporter.SavePackage(exports, ExportType.Sound, SoundType.Emote) : assetsExporter.SavePackage(exports, ExportType.Sound); ;
        string? texture = assetsExporter.SavePackage(exports, ExportType.Texture);

        if (texture is null)
        {
            AppVModel.OnInvalidCosmeticProperty(true, exports.Name);
            return;
        }
        else if (sound is null)
        {
            AppVModel.OnInvalidCosmeticProperty(false, exports.Name);
            return;
        }

        Application.Current.Dispatcher.Invoke(() => {
            WindowManager.Open<CosmeticLoading>();
            WindowManager.Close<MainWindow>();
        });

        BitmapImage? textureBitmap = assetsExporter.GetBitmapImage(exports);
        AppVModel.CosmeticVM.Update($"Loading {exports.Name}", textureBitmap);

        switch (check)
        {
            case CosmeticTypes.Dance:
                Discord.UpdateWithImages("Generating an Emote", "dance", exports.Name);
                break;
            case CosmeticTypes.MusicPack:
                Discord.UpdateWithImages("Generating a Music Pack", "music", exports.Name);
                break;
        }

        string soundName = sound.Split("\\").Last().Split(".").First();
        string output = Path.Combine(DirectoryManager.output, soundName + ".mp4");
        string img = Path.Combine(DirectoryManager.cache, exports.Name + ".png");
        string rarity = getRarityOrDefault(exports);

        ImageMaker imageMaker = new ImageMaker(texture, rarity, img);
        VideoMaker videoMaker = new VideoMaker(output, img, sound);

        // make the image
        AppVModel.CosmeticVM.Update("Making Image..");
        imageMaker.MakeImage();
        // make the video
        AppVModel.CosmeticVM.Update("Making Video..");
        videoMaker.MakeVideo();
        AppVModel.CosmeticVM.Update("Finished!");

        Application.Current.Dispatcher.Invoke(() =>
        {
            WindowManager.Open<Finished>();
            WindowManager.Close<CosmeticLoading>();
        });

        AppVModel.FinishedVM.UpdateMessage($"The cosmetic {exports.Name} has been generated successfully!", textureBitmap);
        AppVModel.FinishedVM.outputPath = output;   
    }

    public CosmeticTypes GetExportType(UObject uObject)
    {
        return (uObject.ExportType) switch
        {
            "AthenaMusicPackItemDefinition" => CosmeticTypes.MusicPack,
            "AthenaDanceItemDefinition" => CosmeticTypes.Dance,
            _ => CosmeticTypes.Invalid
        };
    }

    public string getRarityOrDefault(UObject uObject)
    {
        if (uObject.TryGetValue<UObject>(out var series, "Series"))
        {
            return series.Name;
        }
        else if (uObject.TryGetValue<FName>(out var _rarity, "Rarity"))
        {
            return formatRarity(_rarity.Text);
        }
        else
        {
            Log.Warning("Invalid rarity, using default \"{rarity}\"", "Common");
            return "Common";
        }
    }

    private string formatRarity(string rarity)
    {
        if (!rarity.Contains("::")) return "Common";
        return rarity.Split("::")[1];
    }

    // -------------------------------- provider things -------------------------------- //

    private async Task LoadMappings()
    {
        if (UserSettings.Settings.UseCustomMappings && !string.IsNullOrEmpty(UserSettings.Settings.CustomMappings))
        {
            Provider.MappingsContainer = new FileUsmapTypeMappingsProvider(UserSettings.Settings.CustomMappings);
            Log.Information("Loaded mappings from custom path {filePath}", UserSettings.Settings.CustomMappings);
            return;
        }

        Mappings[]? mappings = await Endpoints.FNCentral.GetMappingsAsync();
        if (mappings is null || mappings.Length == 0)
        {
            LocalLoad();
            return;
        }

        var mapping = mappings.FirstOrDefault();
        await Endpoints.FNCentral.DownloadMappingsAsync(mapping.Url, mapping.Filename);
        Log.Information("Loaded mappings from {filePath}", Path.Combine(DirectoryManager.mappings, mapping.Filename));
        Provider.MappingsContainer = new FileUsmapTypeMappingsProvider(Path.Combine(DirectoryManager.mappings, mapping.Filename));
    }

    private void LocalLoad()
    {
        if (!DirectoryManager.GetSavedMappings(out string path)) return;
        Provider.MappingsContainer = new FileUsmapTypeMappingsProvider(path);

        Log.Information("Loaded mappings from local file {filePath}", path);
    }

    private async Task LoadAesKeys()
    {
        AesKey? aesKeys = await Endpoints.FNCentral.GetAesKeysAsync();
        if (aesKeys is null)
        {
            if (!string.IsNullOrEmpty(UserSettings.Settings.MainKey))
            {
                await Provider.SubmitKeyAsync(new FGuid(), new FAesKey(UserSettings.Settings.MainKey));
                Log.Warning("Aes Keys response was unsuccessfull, using Main Key from settings.");
                return;
            }

            Log.Warning("Aes Keys response was unsuccessfull, the program may not work as expected.");
            return;
        }

        await Provider.SubmitKeyAsync(new FGuid(), new FAesKey(aesKeys.MainKey));
        foreach (DynamicKey key in aesKeys.DynamicKeys)
        {
            await Provider.SubmitKeyAsync(new FGuid(key.Guid), new FAesKey(key.Key));
        }

        Log.Information("Loaded {total} Encryption Keys", aesKeys.DynamicKeys.Count);
    }

    public async Task UpdateMainKey(string mainKey)
    {
        await Provider.SubmitKeyAsync(new FGuid(), new FAesKey(mainKey));
    }
}