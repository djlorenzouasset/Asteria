using System.Diagnostics;
using Asteria.Models;

namespace Asteria.Managers;

public class VideoMaker
{
    private string _output;
    private string _image;
    private string _audio;

    public VideoMaker(string output, string image, string audio)
    {
        _output = output;
        _image = image;
        _audio = audio;
    }

    public void MakeVideo()
    {
        string command = $"-y -loop 1 -framerate 1 -i \"{_image}\" -i \"{_audio}\" -map 0 -map 1:a -c:v libx264 -preset ultrafast -tune stillimage -vf fps=1,format=yuv420p -c:a aac -shortest -movflags +faststart \"{_output}\"";
        Log.Information("Starting making video using external terminal.");

        ProcessStartInfo startInfo = new ProcessStartInfo(UserSettings.Settings.FfmpegPath);
        startInfo.Arguments = command;
        startInfo.UseShellExecute = true;
        startInfo.RedirectStandardOutput = false; 

        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();
        process.WaitForExit();
        Log.Information("Video finished and saved in {path}", _output);
    }
}
