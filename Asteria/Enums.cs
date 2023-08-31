using System.ComponentModel;

namespace Asteria;

public enum EInstallType : byte
{
    [Description("Local Installation")]
    LocalArchives = 0,

    [Description("Fortnite Live")]
    FortniteLive = 1
}

public enum Design : byte 
{
    [Description("Custom Background")]
    CustomBackground = 0,

    [Description("Rarity Background")]
    RarityBackground = 1
}

public enum DiscordPresence : byte
{
    [Description("Always")]
    Always = 0,

    [Description("Disabled")]
    Disabled = 1
}

public enum CosmeticTypes : byte
{
    MusicPack = 0,
    Dance = 1,
    Invalid = 2
}

public enum ExportType : byte
{
    Texture = 0,
    Sound = 1
}

public enum SoundType : byte
{
    Emote = 0,
    MusicPack = 1
}

