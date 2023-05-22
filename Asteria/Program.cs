using Newtonsoft.Json;
using Asteria.Models;
using Asteria.Rest;
using Asteria.Managers;


namespace Asteria;

public class Program
{
    public static Loggers logger = new Loggers(); // colors for logs


    public static void Main()
    {
        Console.Title = "Asteria";
        CreateFolders();


        #region user settings and other configuration

        UserSettings settings;

        // check if FFMpeg is installed in the pc
        // is required for the video creation.
        string? ffmpeg = IsFFMpegInstalled();

        if (HaveSettings())
        {
            settings = LoadSettings();
        }

        else
        {
            Console.Write($"Insert the path to your {logger.pathDefinition("game files")}: ");
            string filesPath = Console.ReadLine().Replace("\"", "");

            Console.Write($"Insert the path for the {logger.pathDefinition("custom background")} to use: ");

            // the image path have to be without white spaces 
            // or the program will generate an exeption because
            // it cant find the correct image
            string bgPath = Console.ReadLine().Replace("\"", "");


            if (!File.Exists(bgPath))
            {
                Console.WriteLine(logger.errorDefinition("Background not found. ") + "Press any key for close the program.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            settings = new UserSettings() { path = filesPath, background = bgPath };
            SaveSettings(settings);
        }

        #endregion

        if (ffmpeg is null) 
        {
            Console.WriteLine(logger.errorDefinition("FFMpeg not found in this PC. Download it and try again."));
            Console.ReadKey();
            Environment.Exit(0);
        }

        else 
        { 
            Console.WriteLine($"Downloading mappings in {logger.pathDefinition(DirectoryManager.mappings)} folder.");
            string ? mappings = GetMappings();

            if (mappings is null)
            {
                Console.Write(logger.errorDefinition("Invalid response from mappings. ") + "Want use a custom mappings file istead? (Y/N): ");
                switch (Console.ReadLine().ToLower())
                {
                    case "y":
                        Console.Write($"Insert the path to your {logger.pathDefinition("mappings file")}: ");
                        mappings = Console.ReadLine().Replace("\"", "");
                        break;

                    default:
                        Console.WriteLine("Press a key for close the program.");
                        Console.ReadKey();
                        Environment.Exit(0);
                        break;
                }
            }

            Dataminer dataminer = new Dataminer();
            dataminer.Initialize(mappings, settings.path, ffmpeg, settings.background);
        }
    }


    public static bool HaveSettings() => File.Exists("settings.json");


    public static UserSettings LoadSettings() => JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText("settings.json"));


    public static void SaveSettings(UserSettings settings) => File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings, Formatting.Indented));


    public static void CreateFolders()
    {
        string[] folders = { DirectoryManager.cache, DirectoryManager.mappings, DirectoryManager.output };

        foreach (string folder in folders) 
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            continue;
        }
    }


    public static string ? IsFFMpegInstalled() 
    {
        // get the path of FFMpeg from the
        // Environment variables situated
        // in the pc. FFMpeg is required
        // for the video generation
        string[] environmentsPaths = Environment.GetEnvironmentVariable("PATH").Split(";");

        foreach (string environmentsPath in environmentsPaths)
        {
            string path = Path.Combine(environmentsPath, "ffmpeg.exe");

            if (File.Exists(path)) 
            {
                return path;
            }
        }

        return null;
    }


    public static string ? GetMappings()
    {
        var mappingsResponse = Requests.TryGetMappings();

        if (mappingsResponse == null || !mappingsResponse.FirstOrDefault().IsValid)
        {
            return null; 
        }

        string fileName = mappingsResponse.FirstOrDefault().fileName;
        string url = mappingsResponse.FirstOrDefault().url;

        Requests.downloadMappings(url, fileName);
        return Path.Combine(DirectoryManager.mappings, fileName);
    }
}