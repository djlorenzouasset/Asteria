using System;
using System.IO;
using System.Linq;
using Ookii.Dialogs.Wpf;

namespace Asteria.Managers;

public static class DirectoryManager
{
    public static string chunks = Path.Combine(Environment.CurrentDirectory, ".data");
    public static string cache = Path.Combine(Environment.CurrentDirectory, ".cache");
    public static string output = Path.Combine(Environment.CurrentDirectory, ".output");
    public static string mappings = Path.Combine(Environment.CurrentDirectory, ".mappings");
    public static string logs = Path.Combine(Environment.CurrentDirectory, ".logs");
    public static string data = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Asteria");

    public static void CreateFolders()
    {
        string[] folders = { chunks, cache, mappings, output, logs, data };

        foreach (string folder in folders)
        {
            if (!Directory.Exists(folder))
            {
                Log.Information("Creating missing directory {dirPath}", folder);
                Directory.CreateDirectory(folder);
            }
            continue;
        }
    }

    public static bool FolderSelector(out string selectedFolder)
    {
        var fileExplorer = new VistaFolderBrowserDialog { ShowNewFolderButton = true };

        if (fileExplorer.ShowDialog() == true)
        {
            selectedFolder = fileExplorer.SelectedPath;
            return true;
        }
        selectedFolder = string.Empty;
        return false;
    }

    public static bool FileSelector(out string selectedFile, bool mapping = false)
    { 
        var fileExplorer = new VistaOpenFileDialog()
        {
            Filter = mapping ? "Unreal Mappings|*.usmap" : "Image Files|*.jpg;*.jpeg;*.png"
        };

        if (fileExplorer.ShowDialog() == true)
        {
            selectedFile = fileExplorer.FileName;
            return true;
        }
        selectedFile = string.Empty;
        return false;
    }

    public static bool GetSavedMappings(out string mappingsPath)
    {
        DirectoryInfo mappingsDir = new(mappings);
        var recent = mappingsDir.GetFiles("*.usmap").OrderByDescending(f => f.LastWriteTime).First();

        if (recent is not null)
        {
            mappingsPath = recent.FullName;
            return true;
        }
        mappingsPath = string.Empty;
        return false;
    }
}
