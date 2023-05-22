namespace Asteria.Models;

public class MappingsResponse
{
    public string url { get; set; }
    public string fileName { get; set; }

    public string hash { get; set; }

    public long length { get; set; }

    public string uploaded { get; set; }

    public Meta meta { get; set; }

    public MappingsResponse()
    {
        this.url = string.Empty;
        this.fileName = string.Empty;
    }

    public bool IsValid => !string.IsNullOrEmpty(this.url) && !string.IsNullOrEmpty(this.fileName);
}

public class Meta
{
    public string version { get; set; }

    public string compressionMethod { get; set; }

    public string platform { get; set; }
}
