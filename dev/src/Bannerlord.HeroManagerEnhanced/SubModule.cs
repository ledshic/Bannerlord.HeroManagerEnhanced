using System;
using System.Reflection;
using AdjustableLeveling.General;
using Bannerlord.AutoAmmoPickup;
using Bannerlord.GimeAllPerks;
using Bannerlord.PersonalEnhancement;
using Bannerlord.UIExtenderEx;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.HeroManagerEnhanced;

public sealed class SubModule : MBSubModuleBase
{
    private const string HarmonyId = "Bannerlord.HeroManagerEnhanced";

    private Harmony? _harmony;
    private UIExtender? _uiExtender;

    public override void OnSubModuleLoad()
    {
        base.OnSubModuleLoad();

        try
        {
            _uiExtender = UIExtender.Create("Bannerlord.HeroManagerEnhanced");
            _uiExtender.Register(Assembly.GetExecutingAssembly());
            _uiExtender.Enable();

            _harmony = new Harmony(HarmonyId);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());

            AdjustableLevelingBootstrap.OnSubModuleLoad();
            Debug.Print($"[Bannerlord.HeroManagerEnhanced] SubModule loaded. v{typeof(SubModule).Assembly.GetName().Version}");
        }
        catch (Exception ex)
        {
            Debug.Print($"[HeroManagerEnhanced] ERROR in OnSubModuleLoad: {ex}");
            InformationManager.DisplayMessage(new InformationMessage(
                new TextObject("{=HME_INIT_FAIL}Hero Manager Enhanced failed to initialize. Check logs.").ToString(),
                Colors.Red));
        }
    }

    public override void OnSubModuleUnloaded()
    {
        base.OnSubModuleUnloaded();

        try
        {
            _uiExtender?.Disable();
            _uiExtender?.Deregister();
            _uiExtender = null;

            _harmony?.UnpatchAll(HarmonyId);
            _harmony = null;
        }
        catch (Exception ex)
        {
            Debug.Print($"[HeroManagerEnhanced] ERROR in OnSubModuleUnloaded: {ex}");
        }
    }

    public override void OnBeforeInitialModuleScreenSetAsRoot()
    {
        base.OnBeforeInitialModuleScreenSetAsRoot();

        AdjustableLevelingBootstrap.OnBeforeInitialModuleScreenSetAsRoot();

        var loadedMsg = new TextObject("{=HME_LOADED}Hero Manager Enhanced loaded (Ammo Pickup, Perk Concierge, Adjustable Leveling, Personal Enhancement).");
        InformationManager.DisplayMessage(new InformationMessage(loadedMsg.ToString(), Colors.Cyan));
    }

    public override void OnGameStart(Game game, IGameStarter gameStarter)
    {
        base.OnGameStart(game, gameStarter);

        if (game.GameType is Campaign && gameStarter is CampaignGameStarter campaignStarter)
        {
            campaignStarter.AddBehavior(new PlayerPerksCampaignBehavior(PerkIgnoreConfig.LoadIgnoreEntries()));
            AdjustableLevelingBootstrap.OnGameStart(game, campaignStarter);
            Debug.Print("[Bannerlord.HeroManagerEnhanced] Campaign behaviors and models registered.");
        }
    }

    public override void OnMissionBehaviorInitialize(Mission mission)
    {
        base.OnMissionBehaviorInitialize(mission);

        if (mission == null)
            return;

        mission.AddMissionBehavior(new AutoAmmoPickupMissionBehavior());
        mission.AddMissionBehavior(new CombatEnhancementMissionBehavior());
    }

    public override void OnNewGameCreated(Game game, object initializerObject)
    {
        base.OnNewGameCreated(game, initializerObject);
        AdjustableLevelingBootstrap.OnNewGameCreated(game);
    }

    public override void OnGameLoaded(Game game, object initializerObject)
    {
        base.OnGameLoaded(game, initializerObject);
        AdjustableLevelingBootstrap.OnGameLoaded(game);
    }

    public override void OnGameEnd(Game game)
    {
        base.OnGameEnd(game);
        AdjustableLevelingBootstrap.OnGameEnd(game);
    }
}
