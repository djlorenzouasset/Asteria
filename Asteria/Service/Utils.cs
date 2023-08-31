using System;
using System.Linq;
using System.Collections.Generic;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Objects.Core.i18N;
using Asteria.Models;

public static class Utils
{
    public static AssetItem? TryExtract(string param)
    {
        if (AppVModel.Dataminer.AssetsError)
        {
            try
            {
                var export = AppVModel.Dataminer.Provider?.LoadAllObjects(param).First();
                return new() { DisplayName = export.GetOrDefault("DisplayName", new FText(export.Name)).Text, Name = export.Name, Path = export.GetPathName() };
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }
        else
        {
            return FindPath(param.Replace(".uasset", ""));
        }
    }

    private static AssetItem? FindPath(string search)
    {
        var cosmetic = AppVModel.Dataminer.Items.Where(x => x.DisplayName.Equals(search, StringComparison.OrdinalIgnoreCase) ||
            x.Path.Split('.')[0].Equals(search, StringComparison.OrdinalIgnoreCase) ||
            x.Name.Equals(search.Split('/').Last(), StringComparison.OrdinalIgnoreCase) ||
            x.Name.Equals(search, StringComparison.OrdinalIgnoreCase)
        );
        if (cosmetic.Count() > 0)
        {
            return cosmetic.First();
        }
        return null;
    }

    public static string GetRarityOrDefault(UObject uObject)
    {
        if (uObject.TryGetValue<UObject>(out var series, "Series"))
        {
            return series.Name;
        }
        else if (uObject.TryGetValue<FName>(out var _rarity, "Rarity"))
        {
            return FormatRarity(_rarity.Text);
        }
        else
        {
            Log.Warning("Invalid rarity, using default \"{rarity}\"", "Common");
            return "Common";
        }
    }

    private static string FormatRarity(string rarity)
    {
        if (!rarity.Contains("::")) return "Common";
        return rarity.Split("::")[1];
    }
}