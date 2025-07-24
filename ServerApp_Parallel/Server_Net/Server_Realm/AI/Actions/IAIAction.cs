namespace Server_Realm.AI.Actions
{
    using GameCore;
    using Server_Realm.Entities;

    /// <summary>
    /// IAIAction.
    /// </summary>
    public interface IAIAction
    {
        /// <summary>
        /// Onwer.
        /// </summary>
        Unit Owner { get; }

        /// <summary>
        /// Type_AIAction.
        /// </summary>
        Type_AIAction AIActionType { get; }

        /// <summary>
        /// Start.
        /// </summary>
        void Start();

        /// <summary>
        /// Update.
        /// </summary>
        void Update();

        /// <summary>
        /// Stop.
        /// </summary>
        void Stop();

        /// <summary>
        /// Reset.
        /// </summary>
        void Reset();
    }
}
