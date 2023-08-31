using Asteria.ViewModels;

namespace Asteria.Service;

public static class ApplicationService
{
    public static ViewModel AppVModel = new();
    public static DiscordRichPresence Discord = new();
}