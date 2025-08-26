namespace Server_Realm.Context.AI.Actions.Interfaces
{
    using GameCore;
    using Server_Realm.Context.AI.Actions;
    using Server_Realm.Context.Units;

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
