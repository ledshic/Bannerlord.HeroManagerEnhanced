using AdjustableLeveling.Leveling;
using AdjustableLeveling.Settings;
using AdjustableLeveling.Utility;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace AdjustableLeveling.General;

public static class AdjustableLevelingBootstrap
{
    private static CharacterDevelopmentModel? _characterDevelopmentModel;
    public static CharacterDevelopmentModel? CharacterDevelopmentModel => _characterDevelopmentModel;

    private static bool _isInitialized;
    private static bool _shownLargeLogWarning;
    private static Func<CharacterDevelopmentModel>? _cdmInitializer;

    public static void OnSubModuleLoad()
    {
        GeneralUtility.Trace($"{nameof(AdjustableLevelingBootstrap)}.{nameof(OnSubModuleLoad)}: ready (Harmony applied by HeroManagerEnhanced SubModule)");
    }

    public static void OnBeforeInitialModuleScreenSetAsRoot()
    {
        GeneralUtility.Trace($"{nameof(AdjustableLevelingBootstrap)}.{nameof(OnBeforeInitialModuleScreenSetAsRoot)}: begin");

        try
        {
            if (_isInitialized)
                return;
            _isInitialized = true;

            MCMSettings.Settings = new MCMSettings();

            var moduleNames = Utilities.GetModulesNames();
            GeneralUtility.Trace($"{nameof(AdjustableLevelingBootstrap)}: loaded modules: {string.Join(", ", moduleNames)}");
            CheckCompatibilityRequired(moduleNames, "TOR_Core", TORUtility.InitializeCompatibility);

            MCMSettings.Settings.Build();
            GeneralUtility.Trace($"{nameof(AdjustableLevelingBootstrap)}.{nameof(OnBeforeInitialModuleScreenSetAsRoot)}: settings built");
        }
        catch (Exception exc)
        {
            GeneralUtility.Message($"ERROR: Adjustable Leveling failed at ({nameof(OnBeforeInitialModuleScreenSetAsRoot)}): {exc.GetType()}: {exc.Message}\n{exc.StackTrace}");
        }
    }

    public static void OnGameStart(Game game, CampaignGameStarter gameStarter)
    {
        GeneralUtility.Trace($"{nameof(AdjustableLevelingBootstrap)}.{nameof(OnGameStart)}: gameType={game?.GameType?.GetType().Name}");

        try
        {
            ShowLargeLogWarningIfNeeded();

            if (game.GameType is Campaign)
            {
                MCMSettings.Settings.OnGameStart();

                _characterDevelopmentModel = _cdmInitializer?.Invoke() ?? new AdjustableCharacterDevelopmentModel();
                gameStarter.AddModel(_characterDevelopmentModel);
                GeneralUtility.Trace($"{nameof(AdjustableLevelingBootstrap)}.{nameof(OnGameStart)}: registered model={_characterDevelopmentModel?.GetType().FullName}");
            }
        }
        catch (Exception exc)
        {
            GeneralUtility.Message($"ERROR: Adjustable Leveling failed at ({nameof(OnGameStart)}): {exc.GetType()}: {exc.Message}\n{exc.StackTrace}");
        }
    }

    public static void OnNewGameCreated(Game game)
    {
        GeneralUtility.Trace($"{nameof(AdjustableLevelingBootstrap)}.{nameof(OnNewGameCreated)}: gameType={game?.GameType?.GetType().Name}");

        try
        {
            if (game.GameType is Campaign)
            {
                MCMSettings.Settings.OnNewGameCreated();
                GameCreatedOrLoaded(game);
            }
        }
        catch (Exception exc)
        {
            GeneralUtility.Message($"ERROR: Adjustable Leveling failed at ({nameof(OnNewGameCreated)}): {exc.GetType()}: {exc.Message}\n{exc.StackTrace}");
        }
    }

    public static void OnGameLoaded(Game game)
    {
        GeneralUtility.Trace($"{nameof(AdjustableLevelingBootstrap)}.{nameof(OnGameLoaded)}: gameType={game?.GameType?.GetType().Name}");

        try
        {
            if (game.GameType is Campaign)
            {
                MCMSettings.Settings.OnGameLoaded();
                GameCreatedOrLoaded(game);
            }
        }
        catch (Exception exc)
        {
            GeneralUtility.Message($"ERROR: Adjustable Leveling failed at ({nameof(OnGameLoaded)}): {exc.GetType()}: {exc.Message}\n{exc.StackTrace}");
        }
    }

    public static void OnGameEnd(Game game)
    {
        GeneralUtility.Trace($"{nameof(AdjustableLevelingBootstrap)}.{nameof(OnGameEnd)}: gameType={game?.GameType?.GetType().Name}");

        try
        {
            if (game.GameType is Campaign)
            {
                MCMSettings.Settings.OnGameEnd();
            }
        }
        catch (Exception exc)
        {
            GeneralUtility.Message($"ERROR: Adjustable Leveling failed at ({nameof(OnGameEnd)}): {exc.GetType()}: {exc.Message}\n{exc.StackTrace}");
        }
    }

    private static void GameCreatedOrLoaded(Game game)
    {
        var totalXpField = typeof(HeroDeveloper).GetField("_totalXp", BindingFlags.NonPublic | BindingFlags.Instance);
        var characters = game.ObjectManager.GetObjectTypeList<CharacterObject>();
        var totalHeroesChecked = 0;
        var totalHeroesAdjusted = 0;
        var adjustedExamples = new StringBuilder();
        foreach (var character in characters)
        {
            if (character.IsHero
                && character.HeroObject is Hero hero
                && hero.HeroDeveloper is HeroDeveloper heroDeveloper)
            {
                totalHeroesChecked++;
                var level = hero.Level;
                var totalXp = heroDeveloper.TotalXp;
                var currXp = heroDeveloper.GetXpRequiredForLevel(level);
                var nextXp = heroDeveloper.GetXpRequiredForLevel(level + 1);

                if (level > 0 && (totalXp < currXp || totalXp > nextXp))
                {
                    var newTotalXp = (currXp + nextXp) / 2;
                    totalXpField?.SetValue(heroDeveloper, newTotalXp);
                    totalHeroesAdjusted++;
                    if (totalHeroesAdjusted <= 5)
                        adjustedExamples.Append($" [{hero?.StringId ?? hero?.Name?.ToString()} lvl={level} xp={totalXp}->{newTotalXp}]");
                }
            }
        }

        GeneralUtility.Trace($"{nameof(AdjustableLevelingBootstrap)}.{nameof(GameCreatedOrLoaded)}: heroesChecked={totalHeroesChecked}, heroesAdjusted={totalHeroesAdjusted}.{adjustedExamples}");
    }

    private static void CheckCompatibilityRequired(string[] moduleNames, string moduleName, Func<Func<CharacterDevelopmentModel>> initializeCompatibility)
    {
        if (!moduleNames.Contains(moduleName))
            return;

        GeneralUtility.Trace($"{nameof(AdjustableLevelingBootstrap)}: detected compatibility module '{moduleName}'");

        if (_cdmInitializer != null)
        {
            GeneralUtility.Message($"ERROR: Adjustable Leveling found '{moduleName}', compatibility conflict detected!", false, Colors.Red, false);
            return;
        }

        _cdmInitializer = initializeCompatibility();
        GeneralUtility.Message($"INFO: Adjustable Leveling found '{moduleName}', applying compatibility", false, Colors.White, false);
        GeneralUtility.Trace($"{nameof(AdjustableLevelingBootstrap)}: compatibility initializer assigned for '{moduleName}'");
    }

    private static void ShowLargeLogWarningIfNeeded()
    {
        if (_shownLargeLogWarning)
            return;

        try
        {
            const long thresholdBytes = 100L * 1024L * 1024L;
            var (totalBytes, fileCount) = DebugTraceUtility.GetCombinedLogSize();
            if (fileCount <= 0 || totalBytes <= thresholdBytes)
                return;

            var sizeMb = totalBytes / (1024f * 1024f);
            var logDir = DebugTraceUtility.LogDirectoryPath ?? "(unknown)";
            var text = new TextObject("{=adjlvl_log_warning_body}Adjustable Leveling detected large log files.\\n\\nCombined size: {SIZE} MB across {COUNT} file(s).\\nLocation: {DIR}\\n\\nConsider disabling logging in MCM when not troubleshooting and deleting old log files.");
            text.SetTextVariable("SIZE", sizeMb.ToString("0.00"));
            text.SetTextVariable("COUNT", fileCount);
            text.SetTextVariable("DIR", logDir);

            InformationManager.ShowInquiry(new InquiryData(
                new TextObject("{=adjlvl_log_warning_title}Adjustable Leveling Log Size Warning").ToString(),
                text.ToString(),
                true,
                false,
                new TextObject("{=adjlvl_ok}OK").ToString(),
                string.Empty,
                () => { },
                () => { }));

            _shownLargeLogWarning = true;
        }
        catch (Exception exc)
        {
            GeneralUtility.Message($"ERROR: Adjustable Leveling failed at ({nameof(ShowLargeLogWarningIfNeeded)}): {exc.GetType()}: {exc.Message}\n{exc.StackTrace}");
        }
    }
}