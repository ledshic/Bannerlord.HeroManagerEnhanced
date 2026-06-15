using System;
using System.Collections.Generic;
using System.Reflection;
using AdjustableLeveling.Settings;
using AdjustableLeveling.Utility;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Library;

namespace AdjustableLeveling.Smithing;

[HarmonyPatch(typeof(CraftingCampaignBehavior), "AddResearchPoints")]
internal static class PatchAddResearchPoints
{
	public static void Prefix(ref int researchPoints)
	{
		var originalResearchPoints = researchPoints;
		researchPoints = MathF.Round(researchPoints * MCMSettings.Settings.SmithingResearchModifier);

		GeneralUtility.TraceThrottled(
			nameof(PatchAddResearchPoints),
			$"{nameof(PatchAddResearchPoints)}: researchPoints={originalResearchPoints}->{researchPoints} modifier={MCMSettings.Settings.SmithingResearchModifier:0.###}",
			0.5f);
	}
}