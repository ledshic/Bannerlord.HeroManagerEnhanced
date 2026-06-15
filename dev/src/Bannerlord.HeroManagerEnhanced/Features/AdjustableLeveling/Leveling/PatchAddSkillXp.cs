using AdjustableLeveling.Settings;
using AdjustableLeveling.Utility;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;

namespace AdjustableLeveling.Leveling;

[HarmonyPatch(typeof(HeroDeveloper), "AddSkillXp")]
internal static class PatchAddSkillXp
{
	public static void Prefix(HeroDeveloper __instance, SkillObject skill, ref float rawXp)
	{
		var originalRawXp = rawXp;
		var modifier = MCMSettings.Settings.GetSkillModifier(skill, __instance?.Hero);
		rawXp *= modifier;

		GeneralUtility.TraceThrottled(
			$"{nameof(PatchAddSkillXp)}:{__instance?.Hero?.GetHashCode() ?? 0}:{skill?.GetHashCode() ?? 0}",
			$"{nameof(PatchAddSkillXp)}: hero='{__instance?.Hero?.Name}' skill='{skill?.Name}' rawXp={originalRawXp:0.###}->{rawXp:0.###} modifier={modifier:0.###}",
			0.5f);
	}
}
