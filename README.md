# Asteria

An open-source program that allow you to generate audio-videos of Music Packs and Emotes of Fortnite (NOW WITH RARITIES BACKGROUNDS!).
If you want contribute to the project, feel free to help! 

> ⚠️ If you encounter problems, I ask you to open an issue in the repository so I can fix it.


# Install the program

### Requirements

* <a href='https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime'>.NET 6.0 Runtime</a>
* <a href='https://ffmpeg.org/download.html'>FFMPeg</a>

### Build

1. Download the source code or clone it with <a href='https://git-scm.com/download/win'>Git</a>.

```
git clone https://github.com/djlorenzouasset/Asteria --recursive
```

2. Build the program
```
cd Asteria
cd Asteria
dotnet build
```

## Settings

- When the program opens, will ask for your <b>local installation of Fortnite</b>. Insert the path and press enter. After this, the program will ask for a <b>custom background</b> (1920x1080 if possible). You can find good backgrounds in high quality on <a href='https://fortnite.gg/assets?category=backgrounds'>Fortnite.gg</a> or write "rarity" for use an automatic background based on the cosmetic rarity.

- When you completed these steps, the program will load local files and when finish you can insert a path for a Music Pack or an Emote (you search in Fortnite files with the help of <a href='https://github.com/4sval/FModel'>FModel</a>). Here is an example of the program when is ready to extract:

<img src='https://github.com/djlorenzouasset/Asteria/blob/main/.github/project-preview.png' alt="Asteria Program">


## How can I change my settings?

You can change your settings by opening the <code>settings.json</code> file.

```jsonc
{
  "path": "your_path_to_files",
  "background": "your_path_to_background",
  "useRarity": false // depends if you want use a custom background or the cosmetic rarity
}
```

## What the program do?

The program basically extract emotes/music packs icons and music for make audio-videos presentation for the cosmetic!

- Video examples are <a href="https://twitter.com/djlorenzouasset/status/1660572148845379587?s=20">here</a>

- Image examples:

<img src="https://github.com/djlorenzouasset/Asteria/blob/main/.github/Athena_Emote_Bunny_Hop_02.png"> 
<img src="https://github.com/djlorenzouasset/Asteria/blob/main/.github/MusicPack_Showdown.png">


In the <code>.output</code> folder will be saved the finished images and videos. In the <code>.cache</code> folder will be saved audios and jsons of the extracted objects!

<b>Json output example:</b>

```json
{
  "Type": "AthenaMusicPackItemDefinition",
  "Name": "MusicPack_034_SXRocketEventRaisin",
  "Class": "UScriptClass'AthenaMusicPackItemDefinition'",
  "Properties": {
    "FrontEndLobbyMusic": {
      "AssetPathName": "/Game/Athena/Sounds/MusicPacks/MusicPack_SXRocketEvent_Cue.MusicPack_SXRocketEvent_Cue",
      "SubPathString": ""
    },
    "CoverArtImage": {
      "AssetPathName": "/Game/2dAssets/Music/Season11/Textures/T_Music_SXRocketEvent.T_Music_SXRocketEvent",
      "SubPathString": ""
    },
    "DynamicInstallBundleName": "CB_86614FA64B218EE0640F38A8A68FA0AC",
    "DisplayName": {
      "Namespace": "",
      "Key": "956F5CFC4B208F1563AFF2B8BE5DFAC1",
      "SourceString": "The End",
      "LocalizedString": "The End"
    },
    "ShortDescription": {
      "Namespace": "",
      "Key": "D9C3F99140A50418AB32AA962ABCEC42",
      "SourceString": "Music",
      "LocalizedString": "Music"
    },
    "Description": {
      "Namespace": "",
      "Key": "243E5A4B4C5011EEB268E8A297F032B3",
      "SourceString": "Press play and begin again.",
      "LocalizedString": "Press play and begin again."
    },
    "GameplayTags": [
      "Cosmetics.Source.Season11.BattlePass.Paid",
      "Cosmetics.Filter.Season.11",
      "Athena.ValetRaisin.Source"
    ],
    "SmallPreviewImage": {
      "AssetPathName": "/Game/2dAssets/Music/Season11/PreviewImages/T_Music_PreviewImages_Season11-T-Music-SXRocketEvent.T_Music_PreviewImages_Season11-T-Music-SXRocketEvent",
      "SubPathString": ""
    },
    "LargePreviewImage": {
      "AssetPathName": "/Game/2dAssets/Music/Season11/PreviewImages/T_Music_PreviewImages_Season11-T-Music-SXRocketEvent-L.T_Music_PreviewImages_Season11-T-Music-SXRocketEvent-L",
      "SubPathString": ""
    },
    "Rarity": "EFortRarity::Rare"
  }
}
```

# Credits

- Some Audios functions from <a href='https://github.com/halfuwu/FortnitePorting'>Fortnite-Porting</a>, a project of <a href='https://github.com/halfuwu'>@halfuwu</a>
- Background of example images from <a href='https://fortnite.gg/assets?category=backgrounds'>fortnite.gg</a>
