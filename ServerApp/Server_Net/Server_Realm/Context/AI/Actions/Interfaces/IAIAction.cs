namespace Server_Realm.Context.AI.Actions.Interfaces
{
    /// <summary>
    /// IAIAction.
    /// </summary>
    public interface IAIAction
    {
        /// <summary>
        /// Status is updated.
        /// </summary>
        bool StatusUpdated { get; set; }

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
    }
}
