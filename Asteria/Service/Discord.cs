using System;
using DiscordRPC;

namespace Asteria.Service;

public class DiscordRichPresence
{
    private static DiscordRpcClient? Client;

    private static readonly RichPresence Default = new()
    {
        State = "Idling",
        Timestamps = new() { Start = DateTime.UtcNow },
        Assets = new() { LargeImageKey = "logo", LargeImageText = $"Asteria {Globals.ASTERIA_VERSION}" },
        Buttons = new[]
        {
            new Button { Label = "Download", Url = "https://github.com/djlorenzouasset/Asteria" }
        }
    };

    public void Initialize()
    {
        Client = new DiscordRpcClient(Globals.APP_ID);
        Client.OnReady += (_, args) => Log.Information("Discord Rich Presence Started for {Username}", args.User.Username);
        Client.OnError += (_, args) => throw new Exception($"Error while starting Discord RPC: {args.Message}");

        Client.Initialize();
        Client.SetPresence(Default);
    }

    public void Update(string text)
    {
        Client?.UpdateState(text);
    }

    public void UpdateWithImages(string text, string iconName = "xxxx", string key = "")
    {
        Update(text);
        Client?.UpdateSmallAsset(iconName, key);
    }
}
