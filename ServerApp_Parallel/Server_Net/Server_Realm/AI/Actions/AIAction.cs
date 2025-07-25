
namespace Server_Realm.AI.Actions
{
    using Server_Realm.AI.Actions.Interfaces;
    using Server_Realm.Entities;

    /// <summary>
    /// AIAction.
    /// </summary>
    public class AIAction : IAIAction
    {
        public AIAction(Unit owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// Unit.
        /// </summary>
        public Unit Owner { get; }

        #region Action
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
        #endregion
    }
}
