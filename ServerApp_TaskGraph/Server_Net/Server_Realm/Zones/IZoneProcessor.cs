namespace Server_Realm.Zones
{
    /// <summary>
    /// IZoneProcessor.
    /// </summary>
    public interface IZoneProcessor
    {
        /// <summary>
        /// Self tell.
        /// </summary>
        void SelfTell<T>(T msg);
    }
}
