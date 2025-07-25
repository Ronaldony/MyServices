
namespace Server_Realm.AI
{
    using GameCore;
    using Server_Realm.AI.Actions;
    using Server_Realm.AI.Actions.Interfaces;

    /// <summary>
    /// IAIBrain.
    /// </summary>
    public interface IAIBrain
    {
        /// <summary>
        /// Update action.
        /// </summary>
        IAIAction UpdateAction();

        AIAction UpdateState(Type_AIState state);
    }
}
