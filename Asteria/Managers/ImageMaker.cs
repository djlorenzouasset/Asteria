using System;
using System.IO;
using SkiaSharp;
using Asteria.Models;

namespace Asteria.Managers;

public class ImageMaker
{
    private string _image;
    private string _rarity;
    private string _saveIn;

    public ImageMaker(string imageName, string rarity, string saveIn)    
    {
        _image = imageName;
        _rarity = rarity;
        _saveIn = saveIn;
    }

    public void MakeImage()
    {
        SKBitmap? background;

        if (UserSettings.Settings.ImageDesign == Design.RarityBackground)
        {
            byte[]? rarityBg = Resources.Resource.ResourceManager.GetObject(_rarity) as byte[];
            background = SKBitmap.Decode(rarityBg);
        }
        else if (UserSettings.Settings.ImageDesign == Design.CustomBackground && string.IsNullOrEmpty(UserSettings.Settings.BackgroundPath))
        {
            Log.Warning("Rarity background is disabled and a custom background is not set. Using Cosmetic Rarity instead, make sure to change this setting.");
            byte[]? rarityBg = Resources.Resource.ResourceManager.GetObject(_rarity) as byte[];
            background = SKBitmap.Decode(rarityBg);
        }
        else 
        {
            background = SKBitmap.Decode(UserSettings.Settings.BackgroundPath);
        }

        SKBitmap? icon = SKBitmap.Decode(_image); // this sometimes return null idk why
        var surface = SKSurface.Create(new SKImageInfo(background.Width, background.Height));
        var canvas = surface.Canvas;
        canvas.DrawBitmap(background, new SKRect(0, 0, background.Width, background.Height));

        float scale = Math.Min((float)background.Width / icon.Width, (float)background.Height / icon.Height) * 0.4f;
        int newWidth = (int)(icon.Width * scale);
        int newHeight = (int)(icon.Height * scale);
        float x = (background.Width - newWidth) / 2;
        float y = (background.Height - newHeight) / 2;
        canvas.DrawBitmap(icon, new SKRect(x, y, x + newWidth, y + newHeight));

        var data = surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100);
        using (var stream = File.OpenWrite(_saveIn))
        {
            data.SaveTo(stream);
            stream.Close();
        }
        Log.Information("Image generated and saved in {path}", _saveIn);
    }
}