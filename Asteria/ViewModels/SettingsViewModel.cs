using CUE4Parse.UE4.Versions;
using CommunityToolkit.Mvvm.ComponentModel;
using Asteria.Models;

namespace Asteria.ViewModels;

public class SettingsViewModel : ObservableObject
{
    public bool restartNeeded = false;
    public bool aesChanged = false;

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

    public string Background
    {
        get => UserSettings.Settings.BackgroundPath;
        set
        {
            UserSettings.Settings.BackgroundPath = value;
            OnPropertyChanged();
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

    public bool CustomBackground
    {
        get => !UserSettings.Settings.RarityBackground;
        set
        {
            CustomBackground = value;
        }
    }

    public bool CustomMappings
    {
        get => UserSettings.Settings.UseCustomMappings;
        set
        {
            CustomMappings = !value;
        }
    }

    public bool UseRarityBackground
    {
        get => UserSettings.Settings.RarityBackground;
        set
        {
            UserSettings.Settings.RarityBackground = value;
            OnPropertyChanged(nameof(CustomBackground));
        }
    }

    public bool UseCustomMappings
    {
        get => UserSettings.Settings.UseCustomMappings;
        set
        {
            UserSettings.Settings.UseCustomMappings = value;
            OnPropertyChanged(nameof(CustomMappings));
            restartNeeded = true;
        }
    }

    public bool UseDiscordPresence
    {
        get => UserSettings.Settings.DiscordPresence;
        set
        {
            UserSettings.Settings.DiscordPresence = value;
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
}