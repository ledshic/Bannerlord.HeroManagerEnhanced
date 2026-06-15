using AdjustableLeveling.Settings;
using AdjustableLeveling.Utility;
using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.TwoDimension;

namespace AdjustableLeveling.Other;

[HarmonyPatch(typeof(TroopRoster), "AddXpToTroopAtIndex")]
internal static class PatchAddXpToTroopAtIndex
{
    public static void Prefix(ref int xpAmount)
    {
        var originalXp = xpAmount;
        xpAmount = (int)Mathf.Round(xpAmount * MCMSettings.Settings.TroopXPModifier);

        GeneralUtility.TraceThrottled(
            nameof(PatchAddXpToTroopAtIndex),
            $"{nameof(PatchAddXpToTroopAtIndex)}: xp={originalXp}->{xpAmount} modifier={MCMSettings.Settings.TroopXPModifier:0.###}",
            0.5f);
    }
}

