namespace Server_Realm.AI.Actions.Interfaces
{
    /// <summary>
    /// IAIAction.
    /// </summary>
    public interface IAIAction
    {
        /// <summary>
        /// Need to update.
        /// </summary>
        bool NeedsUpdate { get; set; }

        /// <summary>
        /// Start.
        /// </summary>
        void Start();

        /// <summary>
        /// Update.
        /// </summary>
        void Update(double deltaTime);

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
