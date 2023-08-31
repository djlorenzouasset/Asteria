using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Versions;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.MappingsProvider;
using CUE4Parse.FileProvider.Vfs;
using CUE4Parse.UE4.AssetRegistry;
using CUE4Parse.FileProvider.Objects;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.AssetRegistry.Objects;
using CUE4Parse.UE4.Assets.Exports.Texture;
using RestSharp;
using Asteria.Rest;
using Asteria.Models;
using Asteria.Managers;
using Asteria.Converters;

namespace Asteria.ViewModels;

public class Dataminer 
{
    private ManifestDownloader? Manifest;
    public readonly AbstractVfsFileProvider? Provider;
    public readonly List<FAssetData> AssetRegistries = new();
    public readonly List<AssetItem> Items = new();
    public bool AssetsError = false;

    private readonly List<string> accepted = new() 
    { 
        "AthenaDanceItemDefinition", 
        "AthenaMusicPackItemDefinition" 
    };

    public Dataminer()
    {
        switch (UserSettings.Settings.InstallType)
        {
            case EInstallType.FortniteLive:
                Provider = new StreamedFileProvider("Fortnite-Live", true,
                    new VersionContainer(
                        UserSettings.Settings.UeVersion,
                        ETexturePlatform.DesktopMobile));
                break;

            case EInstallType.LocalArchives:
                {
                    if (string.IsNullOrEmpty(UserSettings.Settings.PaksPath) || !Directory.Exists(UserSettings.Settings.PaksPath))
                    {
                        Log.Warning("The paks folder path you inserted is invalid. Open the settings, change your paks path and restart.");
                        AppVModel.Warn("Invalid Paks Folder Path", "The paks folder path you inserted is invalid. Open the settings, change your paks path and restart.");
                        return;
                    }

                    Provider = new DefaultFileProvider(UserSettings.Settings.PaksPath, SearchOption.TopDirectoryOnly, true,
                        new VersionContainer(UserSettings.Settings.UeVersion,
                        ETexturePlatform.DesktopMobile));
                    break;
                }
        }
    }

    public async Task Init()
    {
        // paks
        AppVModel.LoadingVM.UpdateText("Loading Archives");
        await InitializeProvider();

        // mappings
        AppVModel.LoadingVM.UpdateText("Loading Mappings");
        await LoadMappings();

        // aes keys loading
        AppVModel.LoadingVM.UpdateText("Loading Encryption Keys");
        await LoadAesKeys();

        // localization loading
        AppVModel.LoadingVM.UpdateText($"Localization loaded for {UserSettings.Settings.LocalizationLanguage.GetDescription()}");
        Provider.LoadLocalization(UserSettings.Settings.LocalizationLanguage);

        AppVModel.LoadingVM.UpdateText("Loading Asset Registries");
        var assetRegistries = Provider.Files.Where(x => x.Key.Contains("AssetRegistry", StringComparison.OrdinalIgnoreCase)).ToList();

        foreach (var (_, file) in assetRegistries)
        {
            if (file.Path.Contains("UEFN", StringComparison.OrdinalIgnoreCase) || file.Path.Contains("Editor", StringComparison.OrdinalIgnoreCase)) 
                continue;
            await LoadAssetRegistry(file);
        }

        if (assetRegistries.Count == 0)
        {
            Log.Warning("Failed to load asset registries.");
            AssetsError = true;
            return;
        }

        // cosmetics loading
        AppVModel.LoadingVM.UpdateText("Loading Cosmetics");
        await LoadCosmetics();
    }

    private async Task InitializeProvider()
    {
        switch (Provider)
        {
            case DefaultFileProvider provider:
                provider.Initialize();
                break;
            case StreamedFileProvider provider:
                {
                    Manifest = new("http://epicgames-download1.akamaized.net/Builds/Fortnite/CloudDir/ChunksV4/");
                    await Endpoints.Epic.GetAuthAsync();
                    RestResponse? manifest = await Endpoints.Epic.GetManifestAsync();
                    if (manifest is null || string.IsNullOrEmpty(manifest.Content))
                    {
                        Log.Error("Invalid response from Fortnite Manifest.");
                        AppVModel.Quit("Invalid manifest response.", "The manifest response was invalid. Wait some minutes or open the program using the local installation.");
                    }
                    await LoadAllPaks(provider, manifest);
                    Provider.Mount();
                    break;
                }
        }
    }

    private async Task LoadAllPaks(StreamedFileProvider _provider, RestResponse manifestData)
    {
        await Manifest.DownloadManifest(new(manifestData.Content));
        foreach (var file in Manifest.ManifestFile.FileManifests)
        {
            if (!ManifestDownloader.PaksFinder.IsMatch(file.Name) || file.Name.Contains("optional")) continue;
            Manifest.LoadFileManifest(file, ref _provider);
        }
    }

    // thanks to Half for the function from FortnitePorting
    private async Task LoadAssetRegistry(GameFile file)
    {
        try
        {
            var assetArchive = await file.TryCreateReaderAsync();
            if (assetArchive is null) return;

            try
            {
                var assetRegistry = new FAssetRegistryState(assetArchive);
                AssetRegistries.AddRange(assetRegistry.PreallocatedAssetDataBuffers);
                Log.Information("Loaded Asset Registry: {0}", file.Path);
            }
            catch (Exception)
            {
                Log.Warning("Failed to load asset registry: {0}", file.Path);
            }
        }
        catch
        {
            return;
        }
    }

    private async Task LoadCosmetics()
    {
        var sw = new Stopwatch();
        sw.Start();

        var items = AssetRegistries.Where(x => accepted.Any(
            y => x.AssetClass.Text.Equals(y, StringComparison.OrdinalIgnoreCase))).ToList();

        foreach (var item in items) 
        {
            try
            {
                var _item = await Provider.LoadObjectAsync(item.ObjectPath);

                string DisplayName;
                string AssetName;
                string AssetPath;

                var displayName = _item.GetOrDefault("DisplayName", new FText(_item.Name));
                AssetName = _item.Name;
                AssetPath = _item.GetPathName();

                if (displayName.Text.Equals("TBD"))
                {
                    DisplayName = _item.Name;
                }
                else
                {
                    DisplayName = displayName.Text;
                }

                Items.Add(new()
                {
                    DisplayName = DisplayName,
                    Name = AssetName,
                    Path = AssetPath
                });
            }
            catch (Exception e)
            {
                Log.Error("Failed to load {path}", item.ObjectPath);
            }
        }

        sw.Stop();
        Log.Information("Loaded {type_1} and {type_2} in {tot}s", "Emotes", "Music Packs", Math.Round(sw.Elapsed.TotalSeconds, 2));
    }

    private async Task LoadMappings()
    {
        if (UserSettings.Settings.UseCustomMappings && !string.IsNullOrEmpty(UserSettings.Settings.CustomMappings))
        {
            Provider.MappingsContainer = new FileUsmapTypeMappingsProvider(UserSettings.Settings.CustomMappings);
            Log.Information("Loaded mappings from {filePath}", UserSettings.Settings.CustomMappings);
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

        Log.Information("Loaded mappings from {filePath}", path);
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
    }

    public async Task UpdateMainKey(string mainKey)
    {
        await Provider.SubmitKeyAsync(new FGuid(), new FAesKey(mainKey));
    }
}