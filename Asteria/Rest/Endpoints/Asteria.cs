using System.Threading.Tasks;
using RestSharp;
using Asteria;
using Asteria.Models;
using Asteria.Rest;

public class AsteriaEndpoints : RestBase
{
    public AsteriaEndpoints(RestClient client) : base(client)
    {
    }

    public async Task<AsteriaUpdate?> GetAsteriaVersionAsync()
    {
        var request = new RestRequest(Globals.ASTERIA_INFOS, Method.Get);
        var response = await _client.ExecuteAsync<AsteriaUpdate>(request).ConfigureAwait(false);
        Log.Information("[{Method}] {StatusDescription} ({StatusCode}): {URI}", request.Method, response.StatusDescription, (int)response.StatusCode, request.Resource);

        if (response is null) return null;
        return response.Data;
    }

    public async Task<string?> GetChangelogAsync()
    {
        var request = new RestRequest(Globals.CHANGELOG, Method.Get);
        var response = await _client.ExecuteAsync<string>(request).ConfigureAwait(false);
        Log.Information("[{Method}] {StatusDescription} ({StatusCode}): {URI}", request.Method, response.StatusDescription, (int)response.StatusCode, request.Resource);

        if (response is null) return null;
        return response.Content;
    }
}
