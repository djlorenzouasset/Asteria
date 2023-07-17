using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Asteria.Managers;

// https://github.com/halfuwu/FortnitePorting/blob/master/FortnitePorting/AppUtils/AppHelper.cs
public static class WindowManager
{
    public static void Open<T>() where T : Window, new()
    {
        new T().Show();
    }

    public static void Close<T>() where T : Window, new()
    {
        if (!IsWindowOpen<T>()) return;

        GetWindow<T>().Close();
    }

    private static T GetWindow<T>() where T : Window
    {
        return Application.Current.Windows.OfType<T>().First();
    }

    private static bool IsWindowOpen<T>() where T : Window
    {
        return Application.Current.Windows.OfType<T>().Any();
    }

    public static void StartProcess(string processName, bool shellExecute = true)
    {
        Process.Start(new ProcessStartInfo { FileName = processName, UseShellExecute = shellExecute });
        Log.Information("Process Started: {processArg}", processName);
    }
}