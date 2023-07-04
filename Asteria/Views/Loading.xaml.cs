using System.Windows;
using System.Windows.Input;
using Asteria.ViewModels;
using Asteria.Managers;
using Asteria.Models;

namespace Asteria.Views;

public partial class Loading
{ 
    public Loading()
    {
        InitializeComponent();
        AppVModel.LoadingVM = new LoadingViewModel();
        DataContext = AppVModel.LoadingVM;
    }

    private async void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        if (UserSettings.Settings.FirstOpening)
        {
            UserSettings.Settings.FirstOpening = false;
            WindowManager.Open<StartUp>();
            return;
        }

        AppVModel.LoadingVM.UpdateText("Checking Asteria Updates");

        bool needUpdate = await Updater.UpdateAvailable();
        if (needUpdate) 
        {
            Updater.Check();
        }

        await AppVModel.LoadingVM.InitializeProgram();
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}
