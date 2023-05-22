using System.Diagnostics;

namespace Asteria.Managers;

public class VideoMaker
{
    private string _ffmpeg_path;
    private string _output;
    private string _image;
    private string _audio;

    public VideoMaker(string ffmpeg_path, string output, string image, string audio)
    {
        _ffmpeg_path = ffmpeg_path;
        _output = output;
        _image = image;
        _audio = audio;
    }

    public void MakeVideo()
    {
        string command = $"-y -loop 1 -framerate 1 -i \"{_image}\" -i \"{_audio}\" -map 0 -map 1:a -c:v libx264 -preset ultrafast -tune stillimage -vf fps=1,format=yuv420p -c:a aac -shortest -movflags +faststart \"{_output}\"";

        ProcessStartInfo startInfo = new ProcessStartInfo(_ffmpeg_path);
        startInfo.Arguments = command;
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = false;

        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        process.WaitForExit();
    }
}
