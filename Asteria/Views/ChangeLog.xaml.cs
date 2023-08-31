using System.Windows.Navigation;
using Asteria.Managers;

namespace Asteria.Views;

public partial class ChangeLog
{
    public string VersionTitle => "ASTERIA v" + Globals.ASTERIA_VERSION;
    public string ChangeLogText => Updater.updateNotes;

    public ChangeLog()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void OpenLink(object sender, RequestNavigateEventArgs e)
    {
        WindowManager.StartProcess(e.Uri.AbsoluteUri);
    }
}
