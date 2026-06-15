using AdjustableLeveling.Settings;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace AdjustableLeveling.Utility
{
	public static class AdjustableCharDevModelUtility
	{
		private static TextObject _skillFocusText;
		private static TextObject _attributeEffectText;
		private static TextObject _overLimitText;

		static AdjustableCharDevModelUtility()
		{
			_skillFocusText = new TextObject("Focus");
			_attributeEffectText = new TextObject("Attribute");
			_overLimitText = new TextObject("Over limit");

			try
			{
				_skillFocusText = (TextObject)AccessTools.Field(typeof(DefaultCharacterDevelopmentModel), nameof(_skillFocusText)).GetValue(null);
				_attributeEffectText = (TextObject)AccessTools.Field(typeof(DefaultCharacterDevelopmentModel), "_attributeEffectText").GetValue(null);
				_overLimitText = (TextObject)AccessTools.Field(typeof(DefaultCharacterDevelopmentModel), nameof(_overLimitText)).GetValue(null);
			}
			catch (Exception exc)
			{
				GeneralUtility.Message($"WARNING: Adjustable Leveling using fallback learning-limit text ({nameof(AdjustableCharDevModelUtility)} static constructor): {exc.GetType()}: {exc.Message}\n{exc.StackTrace}");
			}
		}

		public static void Initialize(this DefaultCharacterDevelopmentModel cdm, out int[] skillsRequiredForLevel)
		{
			skillsRequiredForLevel = new int[1025];
			try
			{
				//skillsRequiredForLevel[0] = 0;
				for (int i = 1; i < skillsRequiredForLevel.Length; i++)
					skillsRequiredForLevel[i] = skillsRequiredForLevel[i - 1] + (int)(MCMSettings.Settings.UseFasterLevelingCurve ? 500f * MathF.Pow(i, 2f) : 25f * MathF.Pow(i, 3f));

				// overwrite private _skillsRequiredForLevel-field
				AccessTools.Field(typeof(DefaultCharacterDevelopmentModel), "_skillsRequiredForLevel").SetValue(cdm, skillsRequiredForLevel);
				GeneralUtility.Trace($"{nameof(AdjustableCharDevModelUtility)}.{nameof(Initialize)}: generated level curve up to {skillsRequiredForLevel.Length - 1}, level1={skillsRequiredForLevel[1]}, level10={skillsRequiredForLevel[10]}, level60={skillsRequiredForLevel[60]}");
			}
			catch (Exception exc)
			{
				GeneralUtility.Message($"ERROR: Adjustable Leveling failed to initialize ({nameof(AdjustableCharDevModelUtility)}.{nameof(Initialize)}): {exc.GetType()}: {exc.Message}\n{exc.StackTrace}");
			}
		}

		public static int SkillsRequiredForLevel(int level, int[] skillsRequiredForLevel) =>
			level > MCMSettings.Settings.MaxCharacterLevel || level >= skillsRequiredForLevel.Length ? int.MaxValue : skillsRequiredForLevel[level];

		public static ExplainedNumber CalculateLearningLimit(
			IReadOnlyPropertyOwner<CharacterAttribute> characterAttributes,
			int focusValue,
			SkillObject skill,
			bool includeDescriptions = false)
		{
			var attributeValue = GetAverageAttributeValue(characterAttributes, skill);
			var explainedNumber = new ExplainedNumber(MCMSettings.Settings.BaseLearningLimit, includeDescriptions);
			explainedNumber.Add(
				focusValue * MCMSettings.Settings.LearningLimitIncreasePerFocusPoint,
				_skillFocusText);
			explainedNumber.Add(
				attributeValue * MCMSettings.Settings.LearningLimitIncreasePerAttributePoint,
				_attributeEffectText);
			explainedNumber.LimitMin(0f);
			return explainedNumber;
		}

		public static ExplainedNumber CalculateLearningRate(
			IReadOnlyPropertyOwner<CharacterAttribute> characterAttributes,
			int focusValue,
			int skillValue,
			SkillObject skill,
			bool includeDescriptions = false)
		{
			var attributeValue = GetAverageAttributeValue(characterAttributes, skill);
			int baseLimit = MCMSettings.Settings.BaseLearningLimit;
			int focusLimit = focusValue * MCMSettings.Settings.LearningLimitIncreasePerFocusPoint;
			int attributeLimit = attributeValue * MCMSettings.Settings.LearningLimitIncreasePerAttributePoint;

			int focusMax = baseLimit + focusLimit;
			int finalMax = focusMax + attributeLimit;

			var explainedNumber = new ExplainedNumber(0f, includeDescriptions);
			if (focusMax > 0 && skillValue < focusMax)
			{
				var factor = 1f - skillValue / (float)focusMax;
				explainedNumber.Add((0.25f + 0.5f * factor) * attributeValue, _attributeEffectText);
				explainedNumber.Add((0.50f + factor) * focusValue, _skillFocusText);
			}
			else if (attributeLimit > 0 && skillValue < finalMax)
			{
				var factor = 1f - (skillValue - focusMax) / (float)attributeLimit;
				explainedNumber.Add(0.25f * factor * attributeValue, _attributeEffectText);
				explainedNumber.Add(0.50f * factor * focusValue, _skillFocusText);
			}
			else
			{
				explainedNumber.Add(-1f, _overLimitText);
			}

			explainedNumber.LimitMin(MCMSettings.Settings.MinLearningRate);
			if (MCMSettings.Settings.MaxLearningRate > 0f)
				explainedNumber.LimitMax(MCMSettings.Settings.MaxLearningRate);
			return explainedNumber;
		}

		private static int GetAverageAttributeValue(IReadOnlyPropertyOwner<CharacterAttribute> characterAttributes, SkillObject skill)
		{
			if (characterAttributes == null || skill?.Attributes == null || skill.Attributes.Length == 0)
				return 0;

			var total = 0;
			foreach (var attribute in skill.Attributes)
				total += characterAttributes.GetPropertyValue(attribute);

			return total / skill.Attributes.Length;
		}
	}
}
