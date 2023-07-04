using System.IO;
using System.Threading.Tasks;
using RestSharp;
using Asteria.Models;
using Asteria.Managers;

namespace Asteria.Rest;

public class FNCentralEndpoints : RestBase
{
    private const string AESKEY = "https://fortnitecentral.genxgames.gg/api/v1/aes";
    private const string MAPPINGS = "https://fortnitecentral.genxgames.gg/api/v1/mappings";

    public FNCentralEndpoints(RestClient client) : base(client)
    {
    }

    public async Task<Mappings[]?> GetMappingsAsync()
    {
        var request = new RestRequest(MAPPINGS, Method.Get);
        var response = await _client.ExecuteAsync<Mappings[]>(request).ConfigureAwait(false);
        Log.Information("[{Method}] {StatusDescription} ({StatusCode}): {URI}", request.Method, response.StatusDescription, (int)response.StatusCode, request.Resource);

        if (!response.IsSuccessStatusCode || string.IsNullOrEmpty(response.Content)) return null;
        return response.Data;
    }

    public async Task<AesKey?> GetAesKeysAsync()
    {
        var request = new RestRequest(AESKEY, Method.Get);
        var response = await _client.ExecuteAsync<AesKey>(request).ConfigureAwait(false);
        Log.Information("[{Method}] {StatusDescription} ({StatusCode}): {URI}", request.Method, response.StatusDescription, (int)response.StatusCode, request.Resource);

        if (!response.IsSuccessStatusCode || string.IsNullOrEmpty(response.Content)) return null;
        return response.Data;
    }

    public async Task<bool> DownloadMappingsAsync(string url, string mappingName)
    {
        var request = new RestRequest(url);
        var data = await _client.DownloadDataAsync(request);
        if (data is not null) await File.WriteAllBytesAsync(Path.Combine(DirectoryManager.mappings, mappingName), data);
        return true;
    }
}
