using System.Windows.Input;
using Asteria.ViewModels;

namespace Asteria.Views;

public partial class CosmeticLoading
{
    public CosmeticLoading()
    {
        InitializeComponent();
        AppVModel.CosmeticVM = new CosmeticLoadingViewModel();
        DataContext = AppVModel.CosmeticVM;
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}
