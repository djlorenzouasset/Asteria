using System.Windows;
using Asteria.Rest;
using Asteria.Managers;
using Asteria.ViewModels;

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
        Close();
        Updater.updateNotes = await Endpoints.Asteria.GetChangelogAsync();
        await AppVModel.LoadingVM.InitializeProgram();
    }
}
