using System;
using System.Windows;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Serilog.Sinks.SystemConsole.Themes;
using AdonisUI.Controls;
using Asteria.Managers;
using Asteria.Models;

using MessageBox = AdonisUI.Controls.MessageBox;
using MessageBoxImage = AdonisUI.Controls.MessageBoxImage;
using MessageBoxResult = AdonisUI.Controls.MessageBoxResult;

namespace Asteria;

public partial class App
{
    [DllImport("kernel32")]
    private static extern bool AllocConsole();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        AllocConsole();
        Console.Title = "Asteria";

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(Path.Combine(DirectoryManager.logs, $"Asteria-Log-{DateTime.Now:dd-MM-yyyy}.txt"),
                          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}") // https://github.com/serilog/serilog/wiki/Configuration-Basics#output-templates
            .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
            .CreateLogger();

        DirectoryManager.CreateFolders();
        UserSettings.LoadSettings();
        UserSettings.IsFFMpegInstalled();
        Updater.Initialize();

        if (UserSettings.Settings.FfmpegPath is null)
        {
            throw new Exception("FFMpeg not found in this PC. Download it and try again.");
        }

        if (UserSettings.Settings.DiscordPresence == DiscordPresence.Always)
        {
            Discord.Initialize();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        UserSettings.SaveSettings();
        Log.Information("------------------- Application Closed -------------------");
        Log.CloseAndFlush();
        Environment.Exit(0);
    }

    private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Error(e.Exception.Message);

        var messageBox = new MessageBoxModel
        {
            Caption = "An unhandled exception has occurred",
            Icon = MessageBoxImage.Error,
            Text = e.Exception.Message,
            Buttons = new[] { new MessageBoxButtonModel("Restart", MessageBoxResult.OK), new MessageBoxButtonModel("Close", MessageBoxResult.Cancel) },
        };

        MessageBox.Show(messageBox);
        if (messageBox.Result == MessageBoxResult.Cancel)
        {
            UserSettings.SaveSettings();
            Environment.Exit(0);

        }
        else if (messageBox.Result == MessageBoxResult.OK)
        {
            UserSettings.SaveSettings();
            AppVModel.Restart();
        }
    }
}
