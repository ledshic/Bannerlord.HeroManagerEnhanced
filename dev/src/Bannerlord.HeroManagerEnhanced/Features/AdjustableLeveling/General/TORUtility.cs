using AdjustableLeveling.Leveling;
using AdjustableLeveling.Settings;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;

namespace AdjustableLeveling.Utility;

public static class TORUtility
{
  public static Func<CharacterDevelopmentModel> InitializeCompatibility()
  {
    try
    {
      GeneralUtility.Trace($"{nameof(TORUtility)}.{nameof(InitializeCompatibility)}: begin");

      var torSkillsType = Type.GetType("TOR_Core.CharacterDevelopment.TORSkills, TOR_Core", throwOnError: false);
      if (torSkillsType == null)
      {
        GeneralUtility.Message("WARNING: TOR_Core detected but TOR skills type was not found. Falling back to native character development model.", false);
        GeneralUtility.Trace($"{nameof(TORUtility)}.{nameof(InitializeCompatibility)}: TOR skills type missing, fallback model will be used");
        return () => new AdjustableCharacterDevelopmentModel();
      }

      GeneralUtility.Trace($"{nameof(TORUtility)}.{nameof(InitializeCompatibility)}: TOR skills type found: {torSkillsType.FullName}");

      AddTORSkill(torSkillsType, "Faith", "{=tor_skill_faith_str}Faith");
      AddTORSkill(torSkillsType, "GunPowder", "{=tor_skill_gunpowder_str}Gunpowder");
      AddTORSkill(torSkillsType, "SpellCraft", "{=tor_skill_spellcraft_str}Spellcraft");

      GeneralUtility.Trace($"{nameof(TORUtility)}.{nameof(InitializeCompatibility)}: TOR skill registration completed");
    }
    catch (Exception exc)
    {
      GeneralUtility.Message($"ERROR: Adjustable Leveling failed at ({nameof(TORUtility)}.{nameof(InitializeCompatibility)}): {exc.GetType()}: {exc.Message}\n{exc.StackTrace}");
    }

    return () => new AdjustableCharacterDevelopmentModel();
  }

  private static void AddTORSkill(Type torSkillsType, string propertyName, string localizedName)
  {
    try
    {
      var skillProperty = torSkillsType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
      if (skillProperty == null || !typeof(SkillObject).IsAssignableFrom(skillProperty.PropertyType))
      {
        GeneralUtility.Message($"WARNING: TOR skill property '{propertyName}' was not found. Skipping.", false);
        GeneralUtility.Trace($"{nameof(TORUtility)}.{nameof(AddTORSkill)}: missing property '{propertyName}'");
        return;
      }

      MCMSettings.Settings.AddSkill(propertyName, localizedName, () => (SkillObject)skillProperty.GetValue(null));
      GeneralUtility.Trace($"{nameof(TORUtility)}.{nameof(AddTORSkill)}: registered '{propertyName}'");
    }
    catch (Exception exc)
    {
      GeneralUtility.Message($"ERROR: Failed to register TOR skill '{propertyName}': {exc.GetType()}: {exc.Message}\n{exc.StackTrace}");
    }
  }
}
