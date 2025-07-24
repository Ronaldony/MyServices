
namespace Server_Realm.AI.Actions
{
    using GameCore;
    using Server_Realm.Entities;

    /// <summary>
    /// AIAction.
    /// </summary>
    public class AIAction : IAIAction
    {
        public AIAction(Unit owner, Type_AIAction aiAction)
        {
            Owner = owner;
            AIActionType = aiAction;
        }

        /// <summary>
        /// Unit.
        /// </summary>
        public Unit Owner { get; }

        /// <summary>
        /// AIActionType.
        /// </summary>
        public Type_AIAction AIActionType { get; }

        /// <summary>
        /// Start.
        /// </summary>
        public void Start()
        {

        }

        /// <summary>
        /// Update.
        /// </summary>
        public void Update()
        {

        }

        /// <summary>
        /// Stop.
        /// </summary>
        public void Stop()
        {

        }

        /// <summary>
        /// Reset.
        /// </summary>
        public void Reset()
        {

        }
    }
}
