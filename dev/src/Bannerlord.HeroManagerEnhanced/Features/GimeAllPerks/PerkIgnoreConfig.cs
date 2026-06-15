using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace Bannerlord.GimeAllPerks;

public static class PerkIgnoreConfig
{
    private const string ModuleId = "Bannerlord.HeroManagerEnhanced";
    private const string IgnoreConfigRelativePath = "ModuleData/ignore_perks.txt";

    public static HashSet<string> LoadIgnoreEntries()
    {
        string modulePath = Path.Combine(BasePath.Name, "Modules", ModuleId);
        string ignoreFilePath = Path.Combine(modulePath, IgnoreConfigRelativePath);
        HashSet<string> entries = new(StringComparer.OrdinalIgnoreCase);

        if (!File.Exists(ignoreFilePath))
        {
            return entries;
        }

        foreach (string rawLine in File.ReadAllLines(ignoreFilePath))
        {
            string line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            entries.Add(line);
        }

        return entries;
    }

    public static bool IsIgnored(PerkObject perk, HashSet<string> ignoreEntries)
    {
        if (ignoreEntries.Count == 0)
        {
            return false;
        }

        string stringId = ((MBObjectBase)perk).StringId;
        string name = perk.Name?.ToString() ?? string.Empty;

        return ignoreEntries.Contains(stringId) || ignoreEntries.Contains(name);
    }

    public static string FormatPerk(PerkObject perk)
    {
        string stringId = ((MBObjectBase)perk).StringId;
        string name = perk.Name?.ToString() ?? "(unnamed)";
        return $"{name} [{stringId}]";
    }
}
