using MMORPGServer.AI;
using System.Numerics;
using TestProject.Objects;
using TestProject.Types;

namespace TestProject.States
{
    /// <summary>
    /// EvadeState.
    /// </summary>
    public class EvadeState : AIState_Base
    {
        public override AIState StateType => AIState.Evade;

        public override void OnEnter(Creature creature)
        {
            creature.Combat.ClearAllThreat();
            creature.Combat.ResetCombat();
            creature.Health.SetHealthPercent(1.0f); // Full heal on evade
        }

        public override void Update(Creature creature, float deltaTime)
        {
            // Move back to spawn position
            creature.Movement.MoveTowards(creature.AI.SpawnPosition);

            float distanceFromSpawn = Vector3.Distance(creature.Position, creature.AI.SpawnPosition);
            if (distanceFromSpawn < 2.0f)
            {
                creature.AI.TransitionToState(AIState.Return);
            }
        }
    }
}
