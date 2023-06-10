using Newtonsoft.Json;
using Serilog;

using Asteria.Models;
using Asteria.Rest;
using Asteria.Managers;

namespace Asteria;

public class Program
{
    public static Loggers logger = new Loggers(); // colors for logs


    public static void Main()
    {
        // create logger
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(Path.Combine(DirectoryManager.logs, $"Asteria-Log-{DateTime.Now:dd-MM-yyyy}.txt"),
                          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}") // https://github.com/serilog/serilog/wiki/Configuration-Basics#output-templates
            .CreateLogger();


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
            Log.Information("Settings loaded.");
        }
        else
        {
            bool useRarityBg = false;
            bool valid = false;

            Console.Write($"Insert the path to your {logger.pathDefinition("game files")}: ");
            string filesPath = Console.ReadLine().Replace("\"", "");
            if (!Directory.Exists(filesPath))
            {
                Console.WriteLine("This path " + logger.errorDefinition("not exist"));
                Console.WriteLine("Press any key for close the program.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            string[] globalExist = Directory.GetFiles(filesPath);
            foreach (string path in globalExist)
            {
                if (path.EndsWith(".utoc") || path.EndsWith(".ucas"))
                    valid = true;
            }
            if (!valid) 
            {
                Console.WriteLine("This path " + logger.errorDefinition("have no paks."));
                Console.WriteLine("Press any key for close the program.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            Console.Write($"Insert the path for the {logger.pathDefinition("custom background")} to use or write " + logger.typeDefinition("rarity") +  " for use a background based on the cosmetic rarity: ");
            string? bgPath = Console.ReadLine().Replace("\"", "");
            if (bgPath.ToLower() != "rarity")
            {
                if (!File.Exists(bgPath))
                {
                    Log.Error("No background found at \"{insertedPath}\"", bgPath);
                    Console.WriteLine(logger.errorDefinition("Background not found. ") + "Press any key for close the program.");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
            else
            {
                bgPath = null;
                useRarityBg = true;
            }

            settings = new UserSettings() { path = filesPath, background = bgPath, useRarity = useRarityBg };
            SaveSettings(settings);
            Log.Information("Settings saved");
        }


        #endregion

        if (ffmpeg is null) 
        {
            Log.Error("FFMpeg not found in the pc, closing the program");
            Console.WriteLine(logger.errorDefinition("FFMpeg not found in this PC. Download it and try again."));
            Console.ReadKey();
            Log.Information("Program closed.");
            Environment.Exit(0);
        }

        else 
        { 
            Console.WriteLine($"Downloading mappings in {logger.pathDefinition(DirectoryManager.mappings)} folder.");
            Log.Information($"Downloading mappings in {DirectoryManager.mappings} folder.");

            string ? mappings = GetMappings();

            if (mappings is null)
            {
                Console.Write(logger.errorDefinition("Invalid response from mappings. ") + "Want use a custom mappings file istead? (Y/N): ");
                switch (Console.ReadLine().ToLower())
                {
                    case "y":
                        Console.Write($"Insert the path to your {logger.pathDefinition("mappings file")}: ");
                        mappings = Console.ReadLine().Replace("\"", "");
                        Log.Information("Loading mappings from file \"{mappingsPath}\"", mappings);
                        break;

                    default:
                        Console.WriteLine("Press any key for close the program.");
                        Console.ReadKey();
                        Log.Information("Program closed.");
                        Environment.Exit(0);
                        break;
                }
            }

            bool useRarity = false;
            if (settings.useRarity) useRarity = true;

            Dataminer dataminer = new Dataminer();
            dataminer.Initialize(mappings, settings.path, ffmpeg, settings.background, useRarity);
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
                Log.Information("Creating missing directory {dirPath}", folder);
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

        string[] environmentPaths = Environment.GetEnvironmentVariable("PATH").Split(";");

        foreach (string environmentsPath in environmentPaths)
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
