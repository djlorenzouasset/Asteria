using System.Windows;
using Asteria.Rest;
using Asteria.Managers;
using Asteria.ViewModels;
using Asteria.Models;

namespace Asteria.Views;

public partial class StartUp
{
    public StartUp()
    {
        InitializeComponent();
        AppVModel.SettingsVM = new SettingsViewModel();
        DataContext = AppVModel.SettingsVM;
    }

    private void SelectPaksFolder(object sender, RoutedEventArgs e)
    {
        if (DirectoryManager.FolderSelector(out string paksFolder))
        {
            AppVModel.SettingsVM.PaksPath = paksFolder;
        }
    }

    private void SelectBackgroundFile(object sender, RoutedEventArgs e)
    {
        if (DirectoryManager.FileSelector(out string bgFile))
        {
            AppVModel.SettingsVM.Background = bgFile;
        }
    }

    private async void OnClickContinue(object sender, RoutedEventArgs e)
    {
        if (UserSettings.Settings.InstallType is EInstallType.LocalArchives && string.IsNullOrEmpty(UserSettings.Settings.PaksPath))
        {
            AppVModel.Warn("Invalid Informations.", "The Path to the local installation you inserted is not valid.");
            return;
        }

        Close();
        Updater.updateNotes = await Endpoints.Asteria.GetChangelogAsync();
        await AppVModel.LoadingVM.InitializeProgram();
    }
}
