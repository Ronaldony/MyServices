namespace Server_Realm.AI.Actions.Interfaces
{
    using GameCore;
    using Server_Realm.Entities;

    /// <summary>
    /// IAIActionCollection.
    /// </summary>
    public interface IAIActionCollection
    {
        /// <summary>
        /// Unit.
        /// </summary>
        Unit Owner { get; }

        /// <summary>
        /// Get AI Action by state.
        /// </summary>
        AIAction GetAIActoin(Type_AIState state);
    }
}
