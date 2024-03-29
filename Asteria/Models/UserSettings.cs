﻿using System;
using System.IO;
using Newtonsoft.Json;
using CUE4Parse.UE4.Versions;
using CommunityToolkit.Mvvm.ComponentModel;
using Asteria.Managers;

namespace Asteria.Models;

public partial class UserSettings : ObservableObject
{
    private static string SettingsFile = Path.Combine(DirectoryManager.data, "settings.json");

    public static UserSettings Settings;

    public static void LoadSettings()
    {
        if (File.Exists(SettingsFile))
        {
            Settings = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(SettingsFile));
            Log.Information("Settings loaded.");
        }
        Settings ??= new UserSettings();
    }

    public static void SaveSettings()
    {
        File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(Settings, Formatting.Indented));
        Log.Information("Settings saved.");
    }

    public static void IsFFMpegInstalled()
    {
        string[] environmentPaths = Environment.GetEnvironmentVariable("PATH").Split(";");

        foreach (string environmentsPath in environmentPaths)
        {
            string path = Path.Combine(environmentsPath, "ffmpeg.exe");

            if (File.Exists(path))
            {
                Settings.ffmpegPath = path;
            }
        }
    }

    public static void ExtractFFmpegZip()
    {
        // TODO: Automatically save FFmpeg in the PC.
    }

    [ObservableProperty] private DiscordPresence discordPresence = DiscordPresence.Always;

    [ObservableProperty] private bool saveRawData = true;

    [ObservableProperty] private EInstallType installType = EInstallType.FortniteLive;

    [ObservableProperty] private ELanguage localizationLanguage = ELanguage.English;

    [ObservableProperty] private EGame ueVersion = EGame.GAME_UE5_3;

    [ObservableProperty] private string paksPath = string.Empty;

    [ObservableProperty] private string mainKey = string.Empty;

    [ObservableProperty] private Design imageDesign = Design.RarityBackground;

    [ObservableProperty] private string backgroundPath = string.Empty;

    [ObservableProperty] private bool useCustomMappings = false;

    [ObservableProperty] private string customMappings = string.Empty;

    [ObservableProperty] private string? ffmpegPath = null;

    [ObservableProperty] private bool firstOpening = true;

    [ObservableProperty] private bool showChangelog = false;
}
