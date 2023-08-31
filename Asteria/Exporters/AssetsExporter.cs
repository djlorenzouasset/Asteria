using System;
using System.IO;
using CUE4Parse.UE4.Assets.Exports;
using Newtonsoft.Json;
using Asteria.Models;
using Asteria.Managers;

namespace Asteria.Exporters;

public static class AssetsExpoter
{
    public static string? SavePackage(UObject _object, ExportType exportType, SoundType soundType = SoundType.MusicPack)
    {
        if (UserSettings.Settings.SaveRawData) 
            SavePackage(_object);

        if (exportType == ExportType.Sound && soundType == SoundType.Emote)
        {
            return Sounds.SaveSoundEmote(_object);
        }
        else if (exportType == ExportType.Sound && soundType == SoundType.MusicPack)
        {
            return Sounds.SaveSound(_object);
        }
        else if (exportType == ExportType.Texture)
        {
            return Textures.SaveTexture(_object);
        }
        else
        {
            // exception for debug
            Log.Error("Invalid operation");
            throw new Exception("Invalid operation");
        }
    }

    private static void SavePackage(UObject _object)
    {
        File.WriteAllText(Path.Combine(DirectoryManager.cache, _object.Name + ".json"), JsonConvert.SerializeObject(_object, Formatting.Indented));
        Log.Information("Saved package {pkgName} as {path}", _object.Name, Path.Combine(DirectoryManager.cache, _object.Name + ".json"));
    }
}