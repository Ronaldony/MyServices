
namespace Server_Realm.AI
{
    using GameCore;
    using Server_Realm.AI.Actions;
    using Server_Realm.Units.Mob;

    /// <summary>
    /// AIBrain.
    /// </summary>
    public class AIBrain : IAIBrain
    {
        /// <summary>
        /// AIActionCollection.
        /// </summary>
        private readonly AIActionCollection _actionCollection;

        /// <summary>
        /// Now AI Action.
        /// </summary>
        private AIAction _nowAIAction;

        public AIBrain(IServiceProvider serviceProvider, MobUnit owner)
        {
            _actionCollection = new AIActionCollection(serviceProvider, owner);
        }

        /// <summary>
        /// Update action.
        /// </summary>
        public void Update(double deltaTime)
        {
            _nowAIAction?.Update(deltaTime);
        }

        /// <summary>
        /// Update state.
        /// </summary>
        public void ChangeState(Type_AIState state)
        {
            _nowAIAction?.Stop();

            var aiAction = _actionCollection.GetAIActoin(state);

            aiAction.Start();

            _nowAIAction = aiAction;
        }
    }
}
