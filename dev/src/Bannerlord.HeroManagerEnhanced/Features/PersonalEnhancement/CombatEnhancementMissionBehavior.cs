using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using Bannerlord.HeroManagerEnhanced.Features.Shared;

namespace Bannerlord.PersonalEnhancement;

public sealed class CombatEnhancementMissionBehavior : MissionLogic
{
    private const float LifeStealRate = 0.15f;
    private const float RegenPerSecondRate = 0.01f;
    private const float RegenTickIntervalSeconds = 1f;

    private float _regenTickAccumulator;

    public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

    public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon affectorWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
    {
        base.OnAgentHit(affectedAgent, affectorAgent, in affectorWeapon, in blow, in attackCollisionData);

        PersonalEnhancementSettings? settings = PersonalEnhancementSettings.Instance;
        if (settings == null || !settings.LifeStealOnDamageEnabled)
            return;

        if (!ShouldRunInCurrentMission())
            return;

        Agent? player = Agent.Main;
        if (player == null || !player.IsActive())
            return;

        if (affectorAgent != player)
            return;

        if (blow.InflictedDamage <= 0)
            return;

        ApplyHeal(player, blow.InflictedDamage * LifeStealRate);
    }

    public override void OnMissionTick(float dt)
    {
        base.OnMissionTick(dt);

        PersonalEnhancementSettings? settings = PersonalEnhancementSettings.Instance;
        if (settings == null || !settings.PerSecondRegenerationEnabled)
            return;

        if (!ShouldRunInCurrentMission())
            return;

        Agent? player = Agent.Main;
        if (player == null || !player.IsActive())
            return;

        if (player.Health >= player.HealthLimit)
            return;

        _regenTickAccumulator += dt;
        if (_regenTickAccumulator < RegenTickIntervalSeconds)
            return;

        _regenTickAccumulator = 0f;
        ApplyHeal(player, player.HealthLimit * RegenPerSecondRate);
    }

    private static void ApplyHeal(Agent player, float amount)
    {
        if (amount <= 0f)
            return;

        float targetHealth = player.Health + amount;
        if (targetHealth > player.HealthLimit)
            targetHealth = player.HealthLimit;

        player.Health = targetHealth;
    }

    private static bool ShouldRunInCurrentMission()
    {
        return MissionCombatScope.CanRunInCombatMission(Mission.Current, includeHideoutRaid: true);
    }
}
