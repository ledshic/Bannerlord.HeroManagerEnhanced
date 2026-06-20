using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace Bannerlord.HeroManagerEnhanced.Features.Shared;

internal static class MissionCombatScope
{
    internal static bool CanRunInCombatMission(Mission? mission, bool includeHideoutRaid = false)
    {
        if (mission == null)
            return false;

        if (mission.IsFieldBattle || mission.IsSiegeBattle || mission.IsSallyOutBattle)
            return true;

        if (includeHideoutRaid && mission.GetMissionBehavior<HideoutPhasedMissionController>() != null)
            return true;

        if (mission.Mode == MissionMode.Battle || mission.Mode == MissionMode.Duel)
            return true;

        // Keep the original auto-pickup fallback: some combat-adjacent missions do not advertise battle modes
        // but still expose an active player agent.
        return Agent.Main != null && Agent.Main.IsActive();
    }
}
