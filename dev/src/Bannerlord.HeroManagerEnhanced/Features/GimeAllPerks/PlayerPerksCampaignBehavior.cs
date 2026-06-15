using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Bannerlord.GimeAllPerks;

public sealed class PlayerPerksCampaignBehavior : CampaignBehaviorBase
{
    private readonly HashSet<string> _ignoreEntries;
    private static PlayerPerksCampaignBehavior? _instance;

    public PlayerPerksCampaignBehavior(HashSet<string> ignoreEntries)
    {
        _ignoreEntries = ignoreEntries;
        _instance = this;
    }

    public override void RegisterEvents()
    {
        CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
    }

    public override void SyncData(IDataStore dataStore)
    {
    }

    public static void ApplyPerksFromMcm()
    {
        if (_instance == null)
        {
            var notReadyMsg = new TextObject("{=GIME_NOT_READY}Perk Concierge: campaign is not ready yet.");
            InformationManager.DisplayMessage(new InformationMessage(notReadyMsg.ToString()));
            return;
        }

        _instance.ApplyPerksToMainHero();
    }

    private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
    {
        ModSettings? settings = ModSettings.Instance;
        if (settings == null || !settings.AutoApplyOnLoad)
        {
            return;
        }

        ApplyPerksToMainHero();
    }

    private void ApplyPerksToMainHero()
    {
        _ignoreEntries.Clear();
        foreach (string entry in PerkIgnoreConfig.LoadIgnoreEntries())
        {
            _ignoreEntries.Add(entry);
        }

        Hero? mainHero = Hero.MainHero;
        if (mainHero == null || mainHero.HeroDeveloper == null)
        {
            var noHeroMsg = new TextObject("{=GIME_NO_HERO}Perk Concierge: main hero is unavailable in the current context.");
            InformationManager.DisplayMessage(new InformationMessage(noHeroMsg.ToString()));
            return;
        }

        int addedCount = 0;
        int ignoredCount = 0;
        List<string> resolvedIgnored = new();
        foreach (PerkObject perk in PerkObject.All)
        {
            if (PerkIgnoreConfig.IsIgnored(perk, _ignoreEntries))
            {
                ignoredCount++;
                resolvedIgnored.Add(PerkIgnoreConfig.FormatPerk(perk));
                continue;
            }

            if (mainHero.GetPerkValue(perk))
            {
                continue;
            }

            mainHero.HeroDeveloper.AddPerk(perk);
            addedCount++;
        }

        var addedMsg = new TextObject("{=GIME_ADDED}Perk Concierge: added {ADDED} perks to player. Ignored {IGNORED} perk(s).");
        addedMsg.SetTextVariable("ADDED", addedCount);
        addedMsg.SetTextVariable("IGNORED", ignoredCount);
        InformationManager.DisplayMessage(new InformationMessage(addedMsg.ToString()));

        if (resolvedIgnored.Count > 0)
        {
            string ignoredText = string.Join(", ", resolvedIgnored.Distinct());
            var ignoredMsg = new TextObject("{=GIME_IGNORED}Ignored perks: {LIST}");
            ignoredMsg.SetTextVariable("LIST", ignoredText);
            InformationManager.DisplayMessage(new InformationMessage(ignoredMsg.ToString()));
        }
    }
}
