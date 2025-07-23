using MMORPGServer.AI;
using TestProject.Objects;
using TestProject.Types;

namespace TestProject.States
{
    /// <summary>
    /// ReturnState.
    /// </summary>
    public class ReturnState : AIState_Base
    {
        public override AIState StateType => AIState.Return;

        public override void OnEnter(Creature creature)
        {
            creature.Position = creature.AI.SpawnPosition;
            creature.Movement.Stop();
        }

        public override void Update(Creature creature, float deltaTime)
        {
            // Wait a moment before returning to idle
            if (creature.AI.StateTimer > 2.0f)
            {
                creature.AI.TransitionToState(AIState.Idle);
            }
        }
    }
}
