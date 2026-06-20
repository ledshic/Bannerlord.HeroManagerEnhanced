using System;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;

namespace Bannerlord.GimeAllPerks;

public sealed class ModSettings : AttributeGlobalSettings<ModSettings>
{
    private bool _autoApplyOnLoad = true;
    private bool _progressiveUnlockOnSkillLevelUp;

    public override string Id => "HeroManagerEnhanced.GimeAllPerks";

    public override string DisplayName => new TextObject("{=GIME_MCM_MOD_NAME}Perk Concierge").ToString();

    public override string FolderName => "Bannerlord.HeroManagerEnhanced/GimeAllPerks";

    public override string FormatType => "json2";

    [SettingPropertyBool("{=GIME_MCM_AUTO_NAME}Auto", RequireRestart = false,
        HintText = "{=GIME_MCM_AUTO_HINT}Automatically apply perks once after a campaign is loaded.")]
    [SettingPropertyGroup("{=GIME_MCM_ACTIONS}Actions")]
    public bool AutoApplyOnLoad
    {
        get => _autoApplyOnLoad;
        set
        {
            _autoApplyOnLoad = value;
            // Only enforce exclusivity when turning this option on.
            // Turning it off keeps the other option unchanged, so both-off is valid.
            if (value)
            {
                _progressiveUnlockOnSkillLevelUp = false;
            }
        }
    }

    [SettingPropertyBool("{=GIME_MCM_PROGRESSIVE_NAME}Progressive", RequireRestart = false,
        HintText = "{=GIME_MCM_PROGRESSIVE_HINT}When checked, newly eligible perks are automatically unlocked whenever the player gains skill levels. Incompatible with Auto mode.")]
    [SettingPropertyGroup("{=GIME_MCM_ACTIONS}Actions")]
    public bool ProgressiveUnlockOnSkillLevelUp
    {
        get => _progressiveUnlockOnSkillLevelUp;
        set
        {
            _progressiveUnlockOnSkillLevelUp = value;
            // Only enforce exclusivity when turning this option on.
            // Turning it off keeps the other option unchanged, so both-off is valid.
            if (value)
            {
                _autoApplyOnLoad = false;
            }
        }
    }

    [SettingPropertyButton("{=GIME_MCM_APPLY_NAME}Perks Activation", Content = "{=GIME_MCM_APPLY_BUTTON}Apply", RequireRestart = false,
        HintText = "{=GIME_MCM_APPLY_HINT}Manually apply all available perks to the player now.")]
    [SettingPropertyGroup("{=GIME_MCM_ACTIONS}Actions")]
    public Action ApplyPerksButton { get; set; } = PlayerPerksCampaignBehavior.ApplyPerksFromMcm;

    [SettingPropertyInteger("{=GIME_MCM_SKILL_DELTA_NAME}Increase/Decrease Skill Proficiency",
        -1000,
        1000,
        RequireRestart = false,
        HintText = "{=GIME_MCM_SKILL_DELTA_HINT}Value to add to each player skill level (e.g. One Handed).")]
    [SettingPropertyGroup("{=GIME_MCM_ACTIONS}Actions")]
    public int SkillProficiencyDelta { get; set; } = 0;

    [SettingPropertyButton("{=GIME_MCM_SKILL_APPLY_NAME}Apply Proficiency Change", Content = "{=GIME_MCM_APPLY_BUTTON}Apply", RequireRestart = false,
        HintText = "{=GIME_MCM_SKILL_APPLY_HINT}Apply the slider value to all player skill levels now.")]
    [SettingPropertyGroup("{=GIME_MCM_ACTIONS}Actions")]
    public Action ApplySkillProficiencyButton { get; set; } = PlayerPerksCampaignBehavior.ApplySkillProficiencyFromMcm;
}
