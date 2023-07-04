using System.Windows;
using AdonisUI.Controls;
using Asteria.ViewModels;
using Asteria.Managers;

using MessageBox = AdonisUI.Controls.MessageBox;
using MessageBoxImage = AdonisUI.Controls.MessageBoxImage;

namespace Asteria.Views;

public partial class Settings
{
    public Settings()
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

    private void SelectMappingFile(object sender, RoutedEventArgs e)
    {
        if (DirectoryManager.FileSelector(out string mappingFile, mapping: true))
        {
            AppVModel.SettingsVM.Mappings = mappingFile;
        }
    }

    private async void OnClickSave(object sender, RoutedEventArgs e)
    {
        if (AppVModel.SettingsVM.restartNeeded)
        {
            var messageBox = new MessageBoxModel
            {
                Caption = "Restart Required.",
                Icon = MessageBoxImage.Exclamation,
                Text = "You changed settings that requires a restart to take effect.",
                Buttons = new[] { MessageBoxButtons.Ok() }
            };

            MessageBox.Show(messageBox);
            AppVModel.Restart();
        }

        else if (AppVModel.SettingsVM.aesChanged)
        {
            if (AppVModel.Dataminer is not null)
            {
                await AppVModel.Dataminer.UpdateMainKey(AppVModel.SettingsVM.MainKey);
            }
        }
        Close();
    }
}
