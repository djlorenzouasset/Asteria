namespace Asteria.Models;

public class AesKeys
{
    public string version { get; set; }

    public string mainKey { get; set; }

    public List<DynamicKey> dynamicKeys { get; set; }
    public List<Unloaded> unloaded { get; set; }
}

public class DynamicKey
{
    public string name { get; set; }
    public string key { get; set; }
    public string guid { get; set; }
    public string keychain { get; set; }
    public int fileCount { get; set; }
    public bool hasHighResTextures { get; set; }
    public Size size { get; set; }
}

public class Size
{
    public int raw { get; set; }
    public string formatted { get; set; }
}

public class Unloaded
{
    public string name { get; set; }
    public string guid { get; set; }
    public int fileCount { get; set; }
    public bool hasHighResTextures { get; set; }
    public Size size { get; set; }
}
