using MMORPGServer.AI;
using System.Numerics;
using TestProject.Objects;
using TestProject.States;
using TestProject.Types;

namespace TestProject.AI
{
    /// <summary>
    /// AIBrain.
    /// </summary>
    public class AIBrain
    {
        private readonly Creature _owner;
        private readonly Dictionary<AIState, AIState_Base> _states;

        public AIState CurrentState { get; private set; }
        public AIType AIType { get; set; }
        public float StateTimer { get; private set; }
        public Vector3 SpawnPosition { get; set; }
        public float LeashRange { get; set; } = 40f;

        public AIBrain(Creature owner)
        {
            _owner = owner;
            _states = new Dictionary<AIState, AIState_Base>
            {
                { AIState.Idle, new IdleState() },
                { AIState.Combat, new CombatState() },
                { AIState.Chase, new ChaseState() },
                { AIState.Evade, new EvadeState() },
                { AIState.Return, new ReturnState() }
            };

            CurrentState = AIState.Idle;
            SpawnPosition = owner.Position;
        }

        public void Update(float deltaTime)
        {
            StateTimer += deltaTime;

            if (_states.TryGetValue(CurrentState, out var state))
            {
                state.Update(_owner, deltaTime);
            }
        }

        public void TransitionToState(AIState newState)
        {
            if (CurrentState == newState) return;

            if (_states.TryGetValue(CurrentState, out var currentState) &&
                !currentState.CanTransitionTo(newState))
            {
                return;
            }

            // Exit current state
            currentState?.OnExit(_owner);

            // Enter new state
            var previousState = CurrentState;
            CurrentState = newState;
            StateTimer = 0f;

            if (_states.TryGetValue(newState, out var nextState))
            {
                nextState.OnEnter(_owner);
            }

            OnStateChanged?.Invoke(previousState, newState);
        }

        public event Action<AIState, AIState> OnStateChanged;
    }
}
