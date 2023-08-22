using System.IO;
using System.Windows.Media.Imaging;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse_Conversion.Textures;
using CUE4Parse.UE4.Assets.Exports.Texture;
using SkiaSharp;
using Asteria.Managers;

namespace Asteria.Exporters;

public static class Textures
{
    public static BitmapImage? GetBitmapImage(UObject uObject)
    {
        try
        {
            if (!uObject.TryGetValue(out UTexture2D? texture, "LargePreviewImage")) return null;
            else if (texture == null) return null;

            var decoded = texture.Decode(ETexturePlatform.DesktopMobile);
            if (decoded == null) return null;

            var bitmap = decoded.Encode(SKEncodedImageFormat.Png, 100);
            var stream = new MemoryStream(bitmap.ToArray(), false);
            var finished = new BitmapImage();
            finished.BeginInit();
            finished.CacheOption = BitmapCacheOption.OnLoad;
            finished.StreamSource = stream;
            finished.EndInit();
            finished.Freeze();
            return finished;
        }
        catch
        {
            return null;
        }
    }

    public static string? SaveTexture(this UObject uObject)
    {
        if (!uObject.TryGetValue(out UTexture2D texture, "LargePreviewImage")) return null;
        return texture.ExtractTexture();
    }

    public static string ExtractTexture(this UTexture2D texture)
    {
        var savePath = Path.Combine(DirectoryManager.cache, texture.Name + ".png");
        var decoded = texture.Decode(ETexturePlatform.DesktopMobile);
        var encoded = decoded?.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
        var file = File.Create(savePath);
        var stream = encoded?.AsStream();
        stream?.CopyTo(file);
        stream?.Close();
        file.Close();
        Log.Information("Saved UTexture2D as {path}", savePath);
        return savePath;
    }
}
