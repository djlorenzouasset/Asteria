using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Objects.Core.i18N;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Asteria.Views;
using Asteria.Models;
using Asteria.Managers;
using Asteria.Exporters;

namespace Asteria.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private RelayCommand<string>? clickedCommand;
    public IRelayCommand<string> ClickedCommand => clickedCommand ??= new RelayCommand<string>(AppVModel.MenuCommandHandler);

    private string cosmetic = string.Empty;
    private bool boxNotEmpty = false;

    public string Cosmetic
    {
        get => cosmetic;
        set
        {
            cosmetic = value;
        }
    }

    public bool BoxNotEmpty
    {
        get => boxNotEmpty;
        set
        {
            boxNotEmpty = value;
            OnPropertyChanged(nameof(BoxNotEmpty));
        }
    }

    public void Execute(string searchPattern)
    {
        AssetItem? item = Utils.TryExtract(searchPattern);
        if (item == null)
        {
            AppVModel.OnPathNotFound($"No GameFile found for {searchPattern}.");
            return;
        }
        var export = AppVModel.Dataminer.Provider?.LoadObject(item.Path);

        // check the type for the extractor
        CosmeticTypes exportType = export.ExportType switch
        {
            "AthenaMusicPackItemDefinition" => CosmeticTypes.MusicPack,
            "AthenaDanceItemDefinition" => CosmeticTypes.Dance,
            _ => CosmeticTypes.Invalid
        };

        if (exportType is CosmeticTypes.Invalid)
        {
            AppVModel.OnInvalidCosmeticType(export.Name);
            return;
        }

        string? texture = AssetsExpoter.SavePackage(export, ExportType.Texture);
        string? sound = (exportType == CosmeticTypes.Dance) ? 
            AssetsExpoter.SavePackage(export, ExportType.Sound, SoundType.Emote) : 
            AssetsExpoter.SavePackage(export, ExportType.Sound);

        if (texture is null)
        {
            AppVModel.OnInvalidCosmeticProperty(true, item.DisplayName);
            return;
        }
        else if (sound is null)
        {
            AppVModel.OnInvalidCosmeticProperty(false, item.DisplayName);
            return;
        }

        Application.Current.Dispatcher.Invoke(() => {
            WindowManager.Open<CosmeticLoading>();
            WindowManager.Close<MainWindow>();
        });

        BitmapImage? textureBitmap = Textures.GetBitmapImage(export);
        AppVModel.CosmeticVM.Update($"Loading {item.DisplayName}", textureBitmap);
        
        switch (exportType)
        {
            case CosmeticTypes.Dance:
                Discord.UpdateWithImages("Generating an Emote", "dance", item.DisplayName);
                break;
            case CosmeticTypes.MusicPack:
                Discord.UpdateWithImages("Generating a Music Pack", "music", item.DisplayName);
                break;
        }

        string soundName = sound.Split("\\").Last().Split(".").First();
        string img = Path.Combine(DirectoryManager.cache, export.Name + ".png");
        string output = Path.Combine(DirectoryManager.output, soundName + ".mp4");
        string rarity = Utils.GetRarityOrDefault(export);

        AppVModel.CosmeticVM.Update("Making Image..");
        ImageMaker imageMaker = new(texture, rarity, img);
        imageMaker.MakeImage();

        AppVModel.CosmeticVM.Update("Making Video..");
        VideoMaker videoMaker = new(output, img, sound);
        videoMaker.MakeVideo();
        AppVModel.CosmeticVM.Update("Finished!");

        Application.Current.Dispatcher.Invoke(() =>
        {
            WindowManager.Open<Finished>();
            WindowManager.Close<CosmeticLoading>();
        });

        AppVModel.FinishedVM.UpdateMessage($"The cosmetic \"{item.DisplayName}\" has been generated successfully!", textureBitmap);
        AppVModel.FinishedVM.outputPath = output;
    }
}
