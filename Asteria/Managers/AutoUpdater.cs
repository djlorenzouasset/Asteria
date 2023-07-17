using System;
using System.Threading.Tasks;
using AdonisUI.Controls;
using AutoUpdaterDotNET;
using Newtonsoft.Json;
using Asteria.Models;
using Asteria.Rest;
using Asteria.Views;

namespace Asteria.Managers;

public static class Updater
{
    public static AsteriaUpdate? asteriaUpdate;

    public static string? updateNotes;

    private static bool isFromMenu = false;

    public static void Initialize()
    {
        AutoUpdater.InstalledVersion = new Version(Globals.ASTERIA_VERSION);
        AutoUpdater.Synchronous = true;
        AutoUpdater.ParseUpdateInfoEvent += UpdateInfo;
        AutoUpdater.CheckForUpdateEvent += CheckForUpdate;
    }

    public static void Check(bool fromMenu = false)
    {
        isFromMenu = fromMenu;
        AutoUpdater.Start(Globals.ASTERIA_INFOS);
    }

    public async static Task<bool> UpdateAvailable()
    {
        updateNotes = await Endpoints.Asteria.GetChangelogAsync();
        asteriaUpdate = await Endpoints.Asteria.GetAsteriaVersionAsync();
        if (asteriaUpdate is null) return false;
        var currentVersion = new Version(Globals.ASTERIA_VERSION);
        var apiVersion = new Version(asteriaUpdate.Version);
        return currentVersion != apiVersion;
    }

    private static void UpdateInfo(ParseUpdateInfoEventArgs args)
    {
        asteriaUpdate = JsonConvert.DeserializeObject<AsteriaUpdate>(args.RemoteData);
        if (asteriaUpdate is null) return;

        args.UpdateInfo = new UpdateInfoEventArgs
        {
            CurrentVersion = asteriaUpdate.Version,
            DownloadURL = asteriaUpdate.DownloadUrl,
            ChangelogURL = asteriaUpdate.ChangeLog
        };
    }

    private static void CheckForUpdate(UpdateInfoEventArgs args)
    {
        var updateVersion = new Version(args.CurrentVersion);
        if (args.InstalledVersion == updateVersion)
        {
            if (isFromMenu)
            {
                MessageBox.Show($"Asteria is up to date.\n\nCurrent version installed: {args.InstalledVersion}\nLatest version: {args.CurrentVersion} ({asteriaUpdate?.UpdateDate})", "Update check finished.", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            return;
        }

        var messageBox = new MessageBoxModel
        {
            Text = $"Asteria has an update available from {args.InstalledVersion} to {updateVersion}. Would you like to update now?\n" + $"\nChangelog:\n{args.ChangelogURL}",
            Caption = "Update",
            Icon = MessageBoxImage.Exclamation,
            Buttons = MessageBoxButtons.YesNo()
        };
        MessageBox.Show(messageBox);

        if (messageBox.Result == MessageBoxResult.Yes)
        {
            if (AutoUpdater.DownloadUpdate(args))
            {
                UserSettings.Settings.ShowChangelog = true;
                Log.Information("Installed new Asteria Version ({newVersion}). Old version: {oldVersion}", updateVersion, args.InstalledVersion);
                AppVModel.Quit();
            }
        }
        return;
    }
}
