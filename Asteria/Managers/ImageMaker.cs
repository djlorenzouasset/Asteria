using SkiaSharp;

namespace Asteria.Managers;

public class ImageMaker
{
    private string _image;
    private string _background;
    private string _saveIn;

    public ImageMaker(string imageName, string background, string saveName)    
    {
        _image = imageName;
        _background = background;
        _saveIn = Path.Combine(DirectoryManager.output, saveName + ".png");
    }

    public void MakeImage()
    {
        SKBitmap? background = SKBitmap.Decode(_background);
        SKBitmap? icon = SKBitmap.Decode(_image); // this sometimes return null idk why

        var surface = SKSurface.Create(new SKImageInfo(background.Width, background.Height));
        var canvas = surface.Canvas;

        canvas.DrawBitmap(background, new SKRect(0, 0, background.Width, background.Height));

        float scale = Math.Min((float)background.Width / icon.Width, (float)background.Height / icon.Height) * 0.6f;
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
    }
}