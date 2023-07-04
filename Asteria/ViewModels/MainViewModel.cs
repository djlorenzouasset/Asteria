using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Asteria.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private string cosmeticPath = string.Empty;

    private bool boxNotEmpty = false;

    private RelayCommand<string>? clickedCommand;

    public IRelayCommand<string> ClickedCommand => clickedCommand ??= new RelayCommand<string>(AppVModel.MenuCommandHandler);

    public string CosmeticPath
    {
        get => cosmeticPath;
        set
        {
            cosmeticPath = value;
        }
    }

    public bool BoxNotEmpty
    {
        get => boxNotEmpty;
        set
        {
            boxNotEmpty = value;
            OnPropertyChanged(nameof(BoxNotEmpty));
        }
    }
}
