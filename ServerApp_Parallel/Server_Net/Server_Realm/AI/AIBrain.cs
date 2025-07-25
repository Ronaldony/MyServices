
namespace Server_Realm.AI
{
    using GameCore;
    using Server_Realm.AI.Actions;
    using Server_Realm.AI.Actions.Interfaces;
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
        /// Unit.
        /// </summary>
        private readonly Unit _owner;

        public AIBrain(Unit owner)
        {
            _owner = owner;

            _actionCollection = new AIActionCollection(owner);
        }

        /// <summary>
        /// Update action.
        /// </summary>
        public IAIAction UpdateAction()
        {
            throw new NotImplementedException();
        }

        public AIAction UpdateState(Type_AIState state)
        {
            throw new NotImplementedException();
        }
    }
}
