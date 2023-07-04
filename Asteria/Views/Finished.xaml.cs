using System.Windows;
using Asteria.Managers;
using Asteria.ViewModels;

namespace Asteria.Views;

public partial class Finished
{
    public Finished()
    {
        InitializeComponent();
        AppVModel.FinishedVM = new FinishedViewModel();
        DataContext = AppVModel.FinishedVM;
    }

    private void OpenOutput(object sender, RoutedEventArgs e)
    {
        WindowManager.StartProcess(AppVModel.FinishedVM.outputPath, true);
        Log.Information("Opened output file at {path}", AppVModel.FinishedVM.outputPath);
    }

    private void GenerateNew(object sender, RoutedEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            WindowManager.Open<MainWindow>();
        });

        Close();
    }

    private void CloseProgram(object sender, RoutedEventArgs e)
    {
        AppVModel.Quit();
    }
}
