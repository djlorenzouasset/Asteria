using System.Windows;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Asteria.Views;
using Asteria.Managers;

using MessageBox = AdonisUI.Controls.MessageBox;
using MessageBoxImage = AdonisUI.Controls.MessageBoxImage;
using MessageBoxButton = AdonisUI.Controls.MessageBoxButton;

namespace Asteria.ViewModels;

public partial class LoadingViewModel : ObservableObject
{
    [ObservableProperty]
    private string loadingMessage = "Asteria is starting..";

    public void UpdateText(string text)
    {
        LoadingMessage = text;
    }

    public async Task InitializeProgram()
    {
        await Task.Run(async () =>
        {
            AppVModel.Dataminer = new Dataminer();
            await AppVModel.Dataminer.Init();

            Application.Current.Dispatcher.Invoke(() =>
            {
                WindowManager.Open<MainWindow>();
                WindowManager.Close<Loading>();
            });
        });

        if (Updater.asteriaUpdate is not null && Updater.asteriaUpdate.Messages.Count > 0)
        {
            foreach (string message in Updater.asteriaUpdate.Messages)
            {
                Log.Warning(message);
            }
        }

        if (Updater.asteriaUpdate?.Notice.Title is not null)
        {
            var data = Updater.asteriaUpdate.Notice;
            var icon = data.Warn ? MessageBoxImage.Warning : MessageBoxImage.Information;
            MessageBox.Show(data.Text, data.Title, MessageBoxButton.OK, icon);
        }

        if (AppVModel.Dataminer.AssetsError)
        {
            AppVModel.Warn(
                "Errors accoured while loading.",
                "Asset Registries were not loaded correctly. It will not be possible to search for cosmetics by name or ID until the AES Key is available."
            );
        }
    }
}
