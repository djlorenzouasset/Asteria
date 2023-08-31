using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Sound;

namespace Asteria.UTypes;

public class FortAnimNotifyState_EmoteSound : UObject
{
    public USoundCue? EmoteSound1P { get; private set; }
    public USoundCue? EmoteSound3P { get; private set; }

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        EmoteSound1P = GetOrDefault<USoundCue>(nameof(EmoteSound1P));
        EmoteSound3P = GetOrDefault<USoundCue>(nameof(EmoteSound3P));
    }
}
