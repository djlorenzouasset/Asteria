using CUE4Parse.UE4.Versions;
using CommunityToolkit.Mvvm.ComponentModel;
using Asteria.Models;

namespace Asteria.ViewModels;

public class SettingsViewModel : ObservableObject
{
    public bool restartNeeded = false;
    public bool aesChanged = false;

    // ASTERIA
    public DiscordPresence DiscordPresence
    {
        get => UserSettings.Settings.DiscordPresence;
        set
        {
            UserSettings.Settings.DiscordPresence = value;
            restartNeeded = true;
        }
    }

    public bool SaveRawData
    {
        get => UserSettings.Settings.SaveRawData;
        set
        {
            UserSettings.Settings.SaveRawData = value;
            OnPropertyChanged();
        }
    }

    // FORTNITE
    public bool SelectorsEnabled => UserSettings.Settings.InstallType != EInstallType.FortniteLive;
    public EInstallType InstallType
    {
        get => UserSettings.Settings.InstallType;
        set
        {
            UserSettings.Settings.InstallType = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectorsEnabled));
            restartNeeded = true;
        }
    }

    public ELanguage LocalizationLanguage
    {
        get => UserSettings.Settings.LocalizationLanguage;
        set
        {
            UserSettings.Settings.LocalizationLanguage = value;
            OnPropertyChanged();
            restartNeeded = true;
        }
    }

    public EGame UEVersion
    {
        get => UserSettings.Settings.UeVersion;
        set
        {
            UserSettings.Settings.UeVersion = value;
            restartNeeded = true;
        }
    }

    public string PaksPath
    {
        get => UserSettings.Settings.PaksPath;
        set
        {
            UserSettings.Settings.PaksPath = value;
            OnPropertyChanged();
            restartNeeded = true;
        }
    }

    public string MainKey
    {
        get => UserSettings.Settings.MainKey;
        set
        {
            UserSettings.Settings.MainKey = value;
            OnPropertyChanged();
            aesChanged = true;
        }
    }

    // OTHER
    public bool CustomBackground => UserSettings.Settings.ImageDesign == Design.CustomBackground;
    public Design ImageDesign
    {
        get => UserSettings.Settings.ImageDesign;
        set
        {
            UserSettings.Settings.ImageDesign = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CustomBackground));
        }
    }

    public string Background
    {
        get => UserSettings.Settings.BackgroundPath;
        set
        {
            UserSettings.Settings.BackgroundPath = value;
            OnPropertyChanged();
        }
    }

    public bool UseCustomMappings
    {
        get => UserSettings.Settings.UseCustomMappings;
        set
        {
            UserSettings.Settings.UseCustomMappings = value;
            OnPropertyChanged();
            restartNeeded = true;
        }
    }

    public string Mappings
    {
        get => UserSettings.Settings.CustomMappings;
        set
        {
            UserSettings.Settings.CustomMappings = value;
            OnPropertyChanged();
        }
    }
}