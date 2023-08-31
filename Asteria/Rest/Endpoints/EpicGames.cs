﻿using System.Threading.Tasks;
using RestSharp;
using Asteria;
using Asteria.Models;
using Asteria.Rest;

public class EpicGamesEnpoints : RestBase
{
    public static AuthResponse? AuthResponse;

    public EpicGamesEnpoints(RestClient client) : base(client)
    {
    }

    public async Task<AuthResponse?> GetAuthAsync()
    {
        var request = new RestRequest(Globals.AUTH, Method.Post);
        request.AddHeader("Authorization", Globals.BASIC);
        request.AddParameter("grant_type", "client_credentials");
        var response = await _client.ExecuteAsync<AuthResponse>(request).ConfigureAwait(false);
        Log.Information("[{Method}] {StatusDescription} ({StatusCode}): {URI}", request.Method, response.StatusDescription, (int)response.StatusCode, request.Resource);

        if (response is null || !response.IsSuccessful || string.IsNullOrEmpty(response.Content)) return null;
        AuthResponse = response.Data;
        return response.Data;
    }

    public async Task<RestResponse?> GetManifestAsync()
    {
        var request = new RestRequest(Globals.MANIFEST, Method.Get);
        request.AddHeader("Authorization", "Bearer " + AuthResponse?.access_token);
        var response = await _client.ExecuteAsync(request).ConfigureAwait(false);
        Log.Information("[{Method}] {StatusDescription} ({StatusCode}): {URI}", request.Method, response.StatusDescription, (int)response.StatusCode, request.Resource);

        if (response is null || !response.IsSuccessful || string.IsNullOrEmpty(response.Content)) return null;
        return response;
    }
}