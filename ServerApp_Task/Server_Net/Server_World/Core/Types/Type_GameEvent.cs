namespace Server_World.Core.Types
{
    /// <summary>
    /// Type_GameEvent.
    /// </summary>
    public enum Type_GameEvent
    {
        None = 0,

        /// Packet.
        ReceivePacket = 1,          // 패킷 수신.
        RouteAroundPacket = 2,      // 주변 섹션 패킷 전송.

        /// Content.
        MoveSectionGroup = 10,      // 섹션 이동.
        Attack = 11,                // 공격.
    }
}
