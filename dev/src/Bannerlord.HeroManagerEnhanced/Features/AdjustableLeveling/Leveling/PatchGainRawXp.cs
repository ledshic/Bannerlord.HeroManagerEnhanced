using AdjustableLeveling.Settings;
using AdjustableLeveling.Utility;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace AdjustableLeveling.Leveling;

[HarmonyPatch(typeof(HeroDeveloper), "GainRawXp")]
internal static class PatchGainRawXp
{
	public static void Prefix(HeroDeveloper __instance, ref float rawXp)
	{
		var originalRawXp = rawXp;
		rawXp *= MCMSettings.Settings.LevelXPModifier;

		GeneralUtility.TraceThrottled(
			$"{nameof(PatchGainRawXp)}:{__instance?.Hero?.GetHashCode() ?? 0}",
			$"{nameof(PatchGainRawXp)}: hero='{__instance?.Hero?.Name}' levelXp={originalRawXp:0.###}->{rawXp:0.###} modifier={MCMSettings.Settings.LevelXPModifier:0.###}",
			0.5f);
	}
}
