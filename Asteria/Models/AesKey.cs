using System.Collections.Generic;

namespace Asteria.Models;

public class AesKey
{
    public string Version { get; set; }

    public string MainKey { get; set; }

    public List<DynamicKey> DynamicKeys { get; set; }
}

public class DynamicKey
{
    public string Key { get; set; }
    public string Guid { get; set; }
}
