namespace Asteria.Managers;

public class DirectoryManager
{
    public static string cache = Path.Combine(Environment.CurrentDirectory, ".cache");
    public static string output = Path.Combine(Environment.CurrentDirectory, ".output");
    public static string mappings = Path.Combine(Environment.CurrentDirectory, ".mappings");
    public static string logs = Path.Combine(Environment.CurrentDirectory, ".logs");
}

