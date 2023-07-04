using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Asteria.ViewModels;

public partial class FinishedViewModel : ObservableObject
{
    [ObservableProperty]
    private string? messageText;

    [ObservableProperty]
    private BitmapImage? image;

    private RelayCommand<string>? clickedCommand;

    public IRelayCommand<string> ClickedCommand => clickedCommand ??= new RelayCommand<string>(AppVModel.MenuCommandHandler);

    public string? outputPath;

    public void UpdateMessage(string message, BitmapImage? icon = null)
    {
        MessageText = message;
        Image = icon;
    }
}