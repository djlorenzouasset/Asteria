using System.Collections.Generic;

namespace Asteria.Models;

public class AsteriaUpdate
{
    public string Version { get; set; }

    public string ChangeLog { get; set; }

    public string DownloadUrl { get; set; }

    public string UpdateDate { get; set; }

    public List<string> Messages { get; set; }

    public Notice Notice { get; set; }
}

public class Notice
{
    public string? Title { get; set; }

    public string? Text { get; set; }

    public bool? Warn { get; set; }
}