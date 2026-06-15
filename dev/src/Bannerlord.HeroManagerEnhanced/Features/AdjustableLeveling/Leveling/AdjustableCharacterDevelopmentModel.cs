using AdjustableLeveling.Settings;
using AdjustableLeveling.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace AdjustableLeveling.Leveling
{
	public class AdjustableCharacterDevelopmentModel : DefaultCharacterDevelopmentModel
	{
		private readonly int[] _skillsRequiredForLevel;

		public override int MaxAttribute =>
			MCMSettings.Settings.MaxAttribute;
		public override int MaxFocusPerSkill =>
			MCMSettings.Settings.MaxFocusPerSkill;
		public override int FocusPointsPerLevel =>
			MCMSettings.Settings.FocusPointsPerLevel;
		public override int FocusPointsAtStart =>
			MCMSettings.Settings.FocusPointsAtStart;
		public override int AttributePointsAtStart =>
			MCMSettings.Settings.AttributePointsAtStart;
		public override int LevelsPerAttributePoint =>
			MCMSettings.Settings.LevelsPerAttributePoint;

		public AdjustableCharacterDevelopmentModel() : base()
		{
			this.Initialize(out _skillsRequiredForLevel);
			GeneralUtility.Trace($"{nameof(AdjustableCharacterDevelopmentModel)}: initialized (fasterCurve={MCMSettings.Settings.UseFasterLevelingCurve}, maxLevel={MCMSettings.Settings.MaxCharacterLevel}, levelXpModifier={MCMSettings.Settings.LevelXPModifier:0.###})");
		}

		public override int SkillsRequiredForLevel(int level)
		{
			var result = AdjustableCharDevModelUtility.SkillsRequiredForLevel(level, _skillsRequiredForLevel);
			GeneralUtility.TraceThrottled($"SkillsRequiredForLevel:{level}", $"{nameof(AdjustableCharacterDevelopmentModel)}.{nameof(SkillsRequiredForLevel)}: level={level}, required={result}", 10f);
			return result;
		}

		public override ExplainedNumber CalculateLearningLimit(IReadOnlyPropertyOwner<CharacterAttribute> characterAttributes, int focusValue, SkillObject skill, bool includeDescriptions = false)
		{
			var result = AdjustableCharDevModelUtility.CalculateLearningLimit(characterAttributes, focusValue, skill, includeDescriptions);
			GeneralUtility.TraceThrottled($"LearningLimit:{skill?.Name}", $"{nameof(AdjustableCharacterDevelopmentModel)}.{nameof(CalculateLearningLimit)}: skill='{skill?.Name}', focus={focusValue}, value={result.ResultNumber:0.###}", 2f);
			return result;
		}

		public override ExplainedNumber CalculateLearningRate(IReadOnlyPropertyOwner<CharacterAttribute> characterAttributes, int focusValue, int skillValue, SkillObject skill, bool includeDescriptions = false)
		{
			var result = AdjustableCharDevModelUtility.CalculateLearningRate(characterAttributes, focusValue, skillValue, skill, includeDescriptions);
			GeneralUtility.TraceThrottled($"LearningRate:{skill?.Name}", $"{nameof(AdjustableCharacterDevelopmentModel)}.{nameof(CalculateLearningRate)}: skill='{skill?.Name}', focus={focusValue}, skillValue={skillValue}, value={result.ResultNumber:0.###}", 2f);
			return result;
		}
	}
}
