using System;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;

namespace Bannerlord.GimeAllPerks;

public sealed class ModSettings : AttributeGlobalSettings<ModSettings>
{
    public override string Id => "HeroManagerEnhanced.GimeAllPerks";

    public override string DisplayName => new TextObject("{=GIME_MCM_MOD_NAME}Perk Concierge").ToString();

    public override string FolderName => "Bannerlord.HeroManagerEnhanced/GimeAllPerks";

    public override string FormatType => "json2";

    [SettingPropertyBool("{=GIME_MCM_AUTO_NAME}Auto", RequireRestart = false,
        HintText = "{=GIME_MCM_AUTO_HINT}Automatically apply perks once after a campaign is loaded.")]
    [SettingPropertyGroup("{=GIME_MCM_ACTIONS}Actions")]
    public bool AutoApplyOnLoad { get; set; } = true;

    [SettingPropertyButton("{=GIME_MCM_APPLY_NAME}Perks Activation", Content = "{=GIME_MCM_APPLY_BUTTON}Apply", RequireRestart = false,
        HintText = "{=GIME_MCM_APPLY_HINT}Manually apply all available perks to the player now.")]
    [SettingPropertyGroup("{=GIME_MCM_ACTIONS}Actions")]
    public Action ApplyPerksButton { get; set; } = PlayerPerksCampaignBehavior.ApplyPerksFromMcm;
}
