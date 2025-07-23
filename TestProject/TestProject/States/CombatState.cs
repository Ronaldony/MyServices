using MMORPGServer.AI;
using System.Numerics;
using TestProject.Objects;
using TestProject.Types;

namespace TestProject.States
{
    /// <summary>
    /// CombatState.
    /// </summary>
    public class CombatState : AIState_Base
    {
        public override AIState StateType => AIState.Combat;

        public override void OnEnter(Creature creature)
        {
            creature.Combat.EnterCombat();
        }

        public override void Update(Creature creature, float deltaTime)
        {
            var target = creature.Combat.CurrentTarget;
            if (target == null || target.IsDead)
            {
                // No valid target, look for new one
                var newTarget = creature.Combat.ThreatManager.GetHighestThreatTarget();
                if (newTarget != null)
                {
                    creature.Combat.SetTarget(newTarget);
                    target = newTarget;
                }
                else
                {
                    creature.AI.TransitionToState(AIState.Evade);
                    return;
                }
            }

            // Check leash range
            float distanceFromSpawn = Vector3.Distance(creature.Position, creature.AI.SpawnPosition);
            if (distanceFromSpawn > creature.AI.LeashRange)
            {
                creature.AI.TransitionToState(AIState.Return);
                return;
            }

            float distanceToTarget = Vector3.Distance(creature.Position, target.Position);

            // If target is too far, chase
            if (distanceToTarget > creature.Combat.AttackRange)
            {
                creature.AI.TransitionToState(AIState.Chase);
                return;
            }

            // Attack if in range and not on cooldown
            if (creature.Combat.CanAttack())
            {
                creature.Combat.PerformAttack(target);
            }

            // Face the target
            creature.Movement.FaceTarget(target);
        }

        public override void OnExit(Creature creature)
        {
            // Keep combat state active until fully disengaged
        }
    }
}
