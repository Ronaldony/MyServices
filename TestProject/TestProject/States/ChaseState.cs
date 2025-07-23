using System.Numerics;
using TestProject.Objects;
using TestProject.Types;

namespace TestProject.States
{

    public class ChaseState : AIState_Base
    {
        public override AIState StateType => AIState.Chase;

        public override void Update(Creature creature, float deltaTime)
        {
            var target = creature.Combat.CurrentTarget;
            if (target == null || target.IsDead)
            {
                creature.AI.TransitionToState(AIState.Evade);
                return;
            }

            // Check leash range
            float distanceFromSpawn = Vector3.Distance(creature.Position, creature.AI.SpawnPosition);
            if (distanceFromSpawn > creature.AI.LeashRange)
            {
                creature.AI.TransitionToState(AIState.Return);
                return;
            }

            float distanceToTarget = Vector3.Distance(creature.Position, target.Position);

            // If close enough to attack, switch to combat
            if (distanceToTarget <= creature.Combat.AttackRange)
            {
                creature.AI.TransitionToState(AIState.Combat);
                return;
            }

            // Move towards target
            creature.Movement.MoveTowards(target.Position);
        }
    }
}
