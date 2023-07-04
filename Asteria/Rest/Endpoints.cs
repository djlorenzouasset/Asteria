using RestSharp;

namespace Asteria.Rest;

public static class Endpoints
{
    private static readonly RestClient _client = new();

    public static readonly FNCentralEndpoints FNCentral = new(_client);

    public static readonly AsteriaEndpoints Asteria = new(_client);
}