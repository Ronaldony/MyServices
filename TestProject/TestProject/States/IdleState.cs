using TestProject.Objects;
using TestProject.Types;
using TestProject.Utils;

namespace TestProject.States
{
    /// <summary>
    /// IdleState.
    /// </summary>
    public class IdleState : AIState_Base
    {
        public override AIState StateType => AIState.Idle;

        public override void OnEnter(Creature creature)
        {
            creature.Movement.Stop();
            creature.Combat.ResetCombat();
        }

        public override void Update(Creature creature, float deltaTime)
        {
            // Look for nearby enemies
            var nearbyEnemies = creature.Detection.GetDetectedEnemies();
            if (nearbyEnemies.Count > 0)
            {
                var closestEnemy = creature.Detection.GetClosestEnemy(nearbyEnemies);
                if (closestEnemy != null && creature.Detection.IsInAggroRange(closestEnemy))
                {
                    creature.AI.TransitionToState(AIState.Combat);
                    creature.Combat.SetTarget(closestEnemy);
                    return;
                }
            }

            // Random patrol behavior for some creature types
            if (creature.AI.AIType == AIType.Aggressive &&
                creature.AI.StateTimer > 10f &&
                UnityEngine.Random.value < 0.3f)
            {
                creature.AI.TransitionToState(AIState.Patrol);
            }
        }
    }
}
