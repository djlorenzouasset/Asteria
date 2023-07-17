using System;
using System.Windows;
using AdonisUI.Controls;
using Asteria.Managers;
using Asteria.Views;

using MessageBox = AdonisUI.Controls.MessageBox;
using MessageBoxButtons = AdonisUI.Controls.MessageBoxButtons;
using MessageBoxImage = AdonisUI.Controls.MessageBoxImage;
using MessageBoxResult = AdonisUI.Controls.MessageBoxResult;

namespace Asteria.ViewModels;

public class ViewModel
{
    public MainViewModel MainVM;

    public LoadingViewModel LoadingVM;

    public SettingsViewModel SettingsVM;

    public Dataminer Dataminer;

    public CosmeticLoadingViewModel CosmeticVM;

    public FinishedViewModel FinishedVM;

    public void MenuCommandHandler(string command)
    {
        switch (command)
        {
            case "github_url":
                WindowManager.StartProcess(Globals.GITHUB_REPO);
                break;

            case "twitter_url":
                WindowManager.StartProcess(Globals.TWITTER);
                break;

            case "discord_url":
                WindowManager.StartProcess(Globals.DISCORD);
                break;

            case "settings":
                WindowManager.Open<Settings>();
                Log.Information("Opened AppSettings Window.");
                break;

            case "updateCheck":
                Log.Information("Checking for updates.");
                Updater.Check(true);
                break;

            case "changeLog":
                WindowManager.Open<ChangeLog>();
                Log.Information("Opened ChangeLog Window.");
                break;
        }
    }

    public void Warn(string title, string message)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var messageBox = new MessageBoxModel
            {
                Caption = title,
                Text = message,
                Icon = MessageBoxImage.Warning,
                Buttons = new[] { MessageBoxButtons.Ok() }
            };
            MessageBox.Show(messageBox);
        });
    }

    public void OnPathNotFound(string message)
    {
        Log.Error(message);
        Application.Current.Dispatcher.Invoke(() =>
        {
            var messageBox = new MessageBoxModel
            {
                Caption = "Cosmetic not Found!",
                Text = message,
                Icon = MessageBoxImage.Error,
                Buttons = new[] { new MessageBoxButtonModel("Try Again", MessageBoxResult.Custom), new MessageBoxButtonModel("Quit", MessageBoxResult.Cancel) }
            };
            MessageBox.Show(messageBox);

            if (messageBox.Result == MessageBoxResult.Custom)
            {
                WindowManager.Open<MainWindow>();
                WindowManager.Close<MainWindow>();
                return;
            }
            Quit();
        });
    }

    public void OnInvalidCosmeticProperty(bool textureError, string cosmeticId)
    {
        string message = textureError ? $"The texture for {cosmeticId} is not available." : $"The sound for {cosmeticId} is not available.";

        Log.Warning(message);
        Application.Current.Dispatcher.Invoke(() =>
        {
            var messageBox = new MessageBoxModel
            {
                Caption = "Invalid Cosmetic!",
                Text = message + "\n\nWant generate another cosmetic instead?",
                Icon = MessageBoxImage.Warning,
                Buttons = MessageBoxButtons.YesNo()
            };
            MessageBox.Show(messageBox);
            if (messageBox.Result == MessageBoxResult.Yes)
            {
                WindowManager.Open<MainWindow>();
                WindowManager.Close<MainWindow>();
                return;
            }
            Quit();
        });
    }

    public void OnInvalidCosmeticType(string got, string expected, string cosmeticId)
    {
        Log.Warning($"{cosmeticId}: Got {got}, Expected {expected}");

        Application.Current.Dispatcher.Invoke(() =>
        {
            var messageBox = new MessageBoxModel
            {
                Caption = "Invalid Cosmetic Type!",
                Text = $"This cosmetic is not a Music Pack // Emote (Got {got}, Expected {expected}).\n\nWant generate another cosmetic instead?",
                Icon = MessageBoxImage.Warning,
                Buttons = MessageBoxButtons.YesNo()
            };
            MessageBox.Show(messageBox);
            if (messageBox.Result == MessageBoxResult.Yes)
            {
                WindowManager.Open<MainWindow>();
                WindowManager.Close<MainWindow>();
                return;
            }
            Quit();
        });
    }

    public void Restart()
    {
        WindowManager.StartProcess(AppDomain.CurrentDomain.FriendlyName, shellExecute: false);
        Application.Current.Shutdown();
    }

    public void Quit()
    {
        Application.Current.Shutdown();
    }
}
