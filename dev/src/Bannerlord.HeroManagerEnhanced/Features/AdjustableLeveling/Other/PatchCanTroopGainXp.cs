using AdjustableLeveling.Settings;
using AdjustableLeveling.Utility;
using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.TwoDimension;

namespace AdjustableLeveling.Other;

[HarmonyPatch(typeof(MobilePartyHelper), "CanTroopGainXp")]
internal static class PatchCanTroopGainXp
{
    public static void Postfix(ref int gainableMaxXp)
    {
        var originalGainableMaxXp = gainableMaxXp;
        gainableMaxXp = (int)Mathf.Round(gainableMaxXp / MCMSettings.Settings.TroopXPModifier);

        GeneralUtility.TraceThrottled(
            nameof(PatchCanTroopGainXp),
            $"{nameof(PatchCanTroopGainXp)}: gainableMaxXp={originalGainableMaxXp}->{gainableMaxXp} troopModifier={MCMSettings.Settings.TroopXPModifier:0.###}",
            0.5f);
    }
}
