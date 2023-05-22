using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Versions;
using CUE4Parse.MappingsProvider;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Assets.Exports;

using Asteria.Rest;
using Asteria.Models;
using Asteria.Manager;


namespace Asteria.Managers;

public class Dataminer
{
    private DefaultFileProvider? _provider;
    private Loggers loggers = new Loggers();

    private string? ffmpeg;
    private string? bg_path;

    public void Initialize(string mappings, string filesPath, string ffmpeg_path, string bgPath)
    {
        ffmpeg = ffmpeg_path;
        bg_path = bgPath;

        Console.Write("Initializing provider.. ");
        Console.WriteLine($"Loading Paks files from local directory {loggers.pathDefinition(filesPath)}");

        _provider = new(filesPath, SearchOption.TopDirectoryOnly, true, new VersionContainer(EGame.GAME_UE5_2));
        _provider.MappingsContainer = new FileUsmapTypeMappingsProvider(mappings);
        _provider.Initialize();

        Console.WriteLine("Provider initialized.");
        LoadKeys();

        Console.Write($"Insert here a {loggers.pathDefinition("music pack or an emote path")}: ");
        Extract(Console.ReadLine());
    }


    private void LoadKeys()
    {
        Console.WriteLine("Decrypting available paks.");
        var AesKeys = Requests.GetAesKeys();
        _provider.SubmitKey(new FGuid(), new FAesKey(AesKeys.mainKey));
        foreach (DynamicKey key in AesKeys.dynamicKeys)
        {
            _provider.SubmitKey(new FGuid(key.guid), new FAesKey(key.key));
        }
    }


    private void Extract(string filePath)
    {
        UObject? exports = new();

        try
        {
            exports = _provider.LoadAllObjects(filePath).FirstOrDefault();
        }
        catch (KeyNotFoundException) 
        {
            Console.Write($"No game file found for {loggers.pathDefinition(filePath)}. ");
            Console.Write("Want to extract another file instead? (Y/N): ");
            askInError(Console.ReadLine());
            Environment.Exit(0);
        }


        // check if the export type is supported
        if (IsSoundWave(exports) || IsEmote(exports))
        {
            ExportPackage exporter = new ExportPackage();

            string? texture = exporter.SavePackage(_provider, exports, ExportType.Texture);
            string? sound;

            if (IsEmote(exports))
            {
                sound = exporter.SavePackage(_provider, exports, ExportType.Sound, SoundType.Emote);
            }
            else
            {
                sound = exporter.SavePackage(_provider, exports, ExportType.Sound);
            }


            if (texture is null)
            {
                Console.WriteLine(loggers.errorDefinition("The texture for the cosmetic isn't available.") + "Cant create the video.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            else if (sound is null)
            {
                Console.WriteLine(loggers.errorDefinition("The sound for the cosmetic isn't available. ") + "Cant create the video.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            string soundName = sound.Split("\\").Last().Split(".").First();
            string output = Path.Combine(".output", soundName + ".mp4");
            string img = Path.Combine(".output", soundName + ".png");

            // istantizes managers
            ImageMaker imageMaker = new ImageMaker(texture, bg_path, soundName);
            VideoMaker videoMaker = new VideoMaker(ffmpeg, output, img, sound);

            Console.Write($"Starting making image for {loggers.typeDefinition(soundName)} ~ ");
            imageMaker.MakeImage();
            Console.WriteLine("Image finished.");

            Console.WriteLine($"Starting making video for {loggers.typeDefinition(soundName)}..");
            videoMaker.MakeVideo();
            Console.WriteLine("Video finished.");
            Console.WriteLine($"Output saved in {loggers.pathDefinition(output)}");
            Console.ReadKey();
        }

        else
        {
            Console.WriteLine($"This cosmetic is not a Music Pack and not an Emote. Got \"{loggers.typeDefinition(exports.ExportType)}\", expected \"{loggers.typeDefinition("AthenaMusicPackItemDefinition")}\" or \"{loggers.typeDefinition("AthenaDanceItemDefinition")}\" istead.");
            Console.Write("Want to extract another file instead? (Y/N): ");
            askInError(Console.ReadLine());
            Environment.Exit(0);
        }
    }


    private void askInError(string option)
    {
        switch (option.ToLower())
        {
            case "y":
                Console.Write($"Insert here a {loggers.pathDefinition("music pack or an emote path")}: ");
                Extract(Console.ReadLine());
                break;

            default:
                Console.WriteLine("Press a key for close the program.");
                Console.ReadKey();
                Environment.Exit(0);
                break;
        }
        return;
    }


    private bool IsSoundWave(UObject _object) => _object.ExportType == "AthenaMusicPackItemDefinition";


    private bool IsEmote(UObject _object) => _object.ExportType == "AthenaDanceItemDefinition";

}