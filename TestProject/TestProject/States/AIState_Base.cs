using TestProject.Objects;
using TestProject.Types;

namespace TestProject.States
{
    // State pattern implementation for AI behaviors
    /// <summary>
    /// AIState_Base.
    /// </summary>
    public abstract class AIState_Base
    {
        public abstract AIState StateType { get; }

        public virtual void OnEnter(Creature creature) { }
        public virtual void OnExit(Creature creature) { }
        public virtual void Update(Creature creature, float deltaTime) { }
        public virtual bool CanTransitionTo(AIState newState) => true;
    }
}
