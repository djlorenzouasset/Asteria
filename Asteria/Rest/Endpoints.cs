using System.IO;
using System.Threading.Tasks;
using RestSharp;

namespace Asteria.Rest;

public static class Endpoints
{
    private static readonly RestClient _client = new();

    public static readonly FNCentralEndpoints FNCentral = new(_client);

    public static readonly EpicGamesEnpoints Epic = new(_client);

    public static readonly AsteriaEndpoints Asteria = new(_client);

    public static async Task<bool> DownloadFileAsync(string url, string path)
    {
        var request = new RestRequest(url);
        var data = await _client.DownloadDataAsync(request);
        if (data is not null)
        {
            await File.WriteAllBytesAsync(path, data);
            return true;
        }
        return false;
    }
}