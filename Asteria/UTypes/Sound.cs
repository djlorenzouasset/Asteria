using CUE4Parse.UE4.Assets.Exports.Sound;

namespace Asteria.UTypes;

// sound class from Fortnite Porting

public class Sound
{
    public USoundWave? SoundWave;
    public float Time;
    public bool Loop;

    public Sound(USoundWave? soundWave, float time, bool loop)
    {
        SoundWave = soundWave;
        Time = time;
        Loop = loop;
    }
}