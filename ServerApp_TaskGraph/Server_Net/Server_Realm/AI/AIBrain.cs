
namespace Server_Realm.AI
{
    using GameCore;
    using Server_Realm.AI.Actions;
    using Server_Realm.Entities;

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
        private AIAction _curAIAction;

        public AIBrain(Unit owner)
        {
            _actionCollection = new AIActionCollection(owner);
        }

        /// <summary>
        /// Update action.
        /// </summary>
        public void Update(double deltaTime)
        {
            _curAIAction?.Update(deltaTime);
        }

        /// <summary>
        /// Update state.
        /// </summary>
        public void ChangeState(Type_AIState state)
        {
            _curAIAction?.Stop();
            _curAIAction?.Reset();

            var aiAction = _actionCollection.GetAIActoin(state);

            aiAction.Start();

            _curAIAction = aiAction;
        }
    }
}
