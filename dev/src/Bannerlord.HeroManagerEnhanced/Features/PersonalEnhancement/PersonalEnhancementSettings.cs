using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;

namespace Bannerlord.PersonalEnhancement;

public sealed class PersonalEnhancementSettings : AttributeGlobalSettings<PersonalEnhancementSettings>
{
    public override string Id => "HeroManagerEnhanced.PersonalEnhancement";

    public override string DisplayName => new TextObject("{=PE_MCM_MOD_NAME}Personal Enhancement").ToString();

    public override string FolderName => "Bannerlord.HeroManagerEnhanced/PersonalEnhancement";

    public override string FormatType => "json2";

    [SettingPropertyBool(
        "{=PE_MCM_LIFESTEAL_NAME}Life steal from dealt damage",
        RequireRestart = false,
        HintText = "{=PE_MCM_LIFESTEAL_HINT}When enabled, 15% of damage dealt in battle is restored as your own health.")]
    [SettingPropertyGroup("{=PE_MCM_GROUP_COMBAT}Combat")]
    public bool LifeStealOnDamageEnabled { get; set; } = true;

    [SettingPropertyBool(
        "{=PE_MCM_REGEN_NAME}Regenerate 1% health per second",
        RequireRestart = false,
        HintText = "{=PE_MCM_REGEN_HINT}When enabled, recover 1% of maximum health every second while in battle and not at full health.")]
    [SettingPropertyGroup("{=PE_MCM_GROUP_COMBAT}Combat")]
    public bool PerSecondRegenerationEnabled { get; set; } = true;
}
