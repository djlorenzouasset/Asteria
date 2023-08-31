using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Asteria.ViewModels;

public partial class CosmeticLoadingViewModel : ObservableObject
{
    [ObservableProperty] private string cosmeticId;
    [ObservableProperty] private BitmapImage cosmeticIcon;

    public void Update(string cosmetic, BitmapImage? icon = null)
    {
        CosmeticId = cosmetic;
        if (icon is null) return;
        CosmeticIcon = icon;
    }
}
