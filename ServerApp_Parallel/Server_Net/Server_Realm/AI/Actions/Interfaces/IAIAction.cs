namespace Server_Realm.AI.Actions.Interfaces
{
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
