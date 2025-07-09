namespace Server_World.Core.Types
{
    /// <summary>
    /// Type_GameEvent.
    /// </summary>
    public enum Type_GameEvent
    {
        None = 0,

        //////////////////////////////////////////
        /// Packet.
        Packet = 1,
        AroundPacket = 2,

        //////////////////////////////////////////
        /// Content.
        MoveSection = 10,
        Attack = 11,
    }
}
