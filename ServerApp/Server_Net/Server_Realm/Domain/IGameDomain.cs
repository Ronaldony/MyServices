namespace Server_Realm.Domain
{
    /// <summary>
    /// IGameDomain.
    /// </summary>
    public interface IGameDomain
    {
        /// <summary>
        /// Tell.
        /// </summary>
        void Tell<T>(T msg);
    }
}
