using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using Asteria.ViewModels;

namespace Asteria.Views;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        AppVModel.MainVM = new MainViewModel();
        DataContext = AppVModel.MainVM;
        Discord.UpdateWithImages("In Menu");
    }

    private void OnCosmeticPathChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(AppVModel.MainVM.CosmeticPath))
        {
            AppVModel.MainVM.BoxNotEmpty = true;
        }
        else
        {
            AppVModel.MainVM.BoxNotEmpty = false;
        }
    }

    private void OnClickConvert(object sender, RoutedEventArgs e)
    {
        Task.Run(() =>
        {
            AppVModel.Dataminer.Extract(AppVModel.MainVM.CosmeticPath);
        });
    }
}
