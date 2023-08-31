using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse_Conversion.Sounds;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Exports.Sound;
using CUE4Parse.UE4.Assets.Exports.Animation;
using CUE4Parse.UE4.Assets.Exports.Sound.Node;
using CUE4Parse.GameTypes.FN.Assets.Exports.Sound;
using Asteria.ViewModels;
using Asteria.UTypes;
using Asteria.Managers;

namespace Asteria.Exporters;

public static class Sounds
{
    public static string? SaveSound(UObject _object)
    {
        if (!_object.TryGetValue(out UObject soundPath, "FrontEndLobbyMusic"))
        {
            return null;
        }

        string soundDefinition = soundPath.GetPathName().Split(".").First() + ".uasset";
        UObject? _sound = AppVModel.Dataminer.Provider?.LoadAllObjects(soundDefinition).LastOrDefault();

        if (_sound is null || _sound.ExportType != "SoundNodeWavePlayer")
        {
            return null;
        }

        if (!_sound.TryGetValue(out USoundWave soundWave, "SoundWaveAssetPtr"))
        {
            return null;
        }

        soundWave.Decode(true, out string format, out byte[]? data);
        if (data is null || string.IsNullOrEmpty(format) || soundWave.Owner is null)
        {
            return null;
        }

        FileStream stream = new FileStream(Path.Combine(DirectoryManager.cache, soundWave.Name) + "." + format, FileMode.Create, FileAccess.Write);
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(data);
        writer.Flush();
        writer.Close();
        Log.Information("Saved {soundType} as {path}", soundWave.ExportType, Path.Combine(DirectoryManager.cache, soundWave.Name + "." + format));
        return Path.Combine(DirectoryManager.cache, soundWave.Name + "." + format);
    }

    // https://github.com/halfuwu/FortnitePorting/blob/master/FortnitePorting/Exports/Types/DanceExportData.cs#L36
    // thanks to half for the help on this 
    public static string? SaveSoundEmote(UObject _object)
    {
        string? finalPath = null;
        var soundNotifies = new List<FAnimNotifyEvent>();

        if (!_object.TryGetValue(out UAnimMontage montage, "Animation"))
        {
            return null;
        }

        var montageNotifies = montage.GetOrDefault("Notifies", new List<FAnimNotifyEvent>());
        foreach (var notify in montageNotifies)
        {
            var notifyName = notify.NotifyName.Text;
            if (notifyName.Contains("FortEmoteSound") || notifyName.Contains("Fort Anim Notify State Emote Sound"))
            {
                soundNotifies.Add(notify);
            }
        }

        foreach (var soundNotify in soundNotifies)
        {
            var time = soundNotify.TriggerTimeOffset;
            var notifyData = soundNotify.NotifyStateClass.Load<UObject>();
            var firstNode = notifyData?.Get<USoundCue>("EmoteSound1P")?.FirstNode?.Load<USoundNode>();
            if (firstNode is null) continue;

            var sounds = HandleAudioTree(firstNode, time);
            foreach (var sound in sounds)
            {
                sound.SoundWave.Decode(true, out string format, out byte[]? data);

                if (data is null || string.IsNullOrEmpty(format) || sound?.SoundWave?.Owner is null)
                {
                    return null;
                }

                FileStream stream = new FileStream(Path.Combine(DirectoryManager.cache, sound.SoundWave.Name) + "." + format, FileMode.Create, FileAccess.Write);
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(data);
                writer.Flush();
                writer.Close();
                finalPath = Path.Combine(DirectoryManager.cache, sound.SoundWave.Name + "." + format);
                Log.Information("Saved {soundType} in {path}", sound.SoundWave.ExportType, finalPath);
            }
        }
        return finalPath;
    }

    // https://github.com/halfuwu/FortnitePorting/blob/master/FortnitePorting/Exports/ExportHelpers.cs#L899
    // thanks to Half for the sound exporter 
    private static List<Sound> HandleAudioTree(USoundNode node, float offset = 0f)
    {
        var sounds = new List<Sound>();
        Random RandomGenerator = new();

        switch (node)
        {
            case USoundNodeWavePlayer player:
                {
                    sounds.Add(LoadSound(player, offset));
                    break;
                }
            case USoundNodeDelay delay:
                {
                    foreach (var nodeObject in delay.ChildNodes)
                    {
                        sounds.AddRange(HandleAudioTree(nodeObject.Load<USoundNode>(), offset + delay.Get<float>("DelayMin")));
                    }

                    break;
                }
            case USoundNodeRandom random:
                {
                    var index = RandomGenerator.Next(0, random.ChildNodes.Length);
                    sounds.AddRange(HandleAudioTree(random.ChildNodes[index].Load<USoundNode>(), offset));
                    break;
                }
            case UFortSoundNodeLicensedContentSwitcher switcher:
                {
                    sounds.AddRange(HandleAudioTree(switcher.ChildNodes.Last().Load<USoundNode>(), offset));
                    break;
                }
            case USoundNodeDialoguePlayer dialoguePlayer:
                {
                    var dialogueWaveParameter = dialoguePlayer.Get<FStructFallback>("DialogueWaveParameter");
                    var dialogueWave = dialogueWaveParameter.Get<UDialogueWave>("DialogueWave");
                    var contextMappings = dialogueWave.Get<FStructFallback[]>("ContextMappings");
                    var soundWave = contextMappings.First().Get<USoundWave>("SoundWave");
                    sounds.Add(LoadSound(soundWave));
                    break;
                }
            case USoundNode generic:
                {
                    foreach (var nodeObject in generic.ChildNodes)
                    {
                        sounds.AddRange(HandleAudioTree(nodeObject.Load<USoundNode>(), offset));
                    }

                    break;
                }
        }
        return sounds;
    }

    private static Sound LoadSound(USoundNodeWavePlayer player, float timeOffset = 0)
    {
        var soundWave = player.SoundWave?.Load<USoundWave>();
        return new Sound(soundWave, timeOffset, player.GetOrDefault("bLooping", false));
    }

    private static Sound LoadSound(USoundWave soundWave, float timeOffset = 0)
    {
        return new Sound(soundWave, timeOffset, false);
    }
}
