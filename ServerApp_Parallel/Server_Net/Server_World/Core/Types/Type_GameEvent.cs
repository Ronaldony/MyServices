namespace Server_World.Core.Types
{
    /// <summary>
    /// Type_GameEvent.
    /// </summary>
    public enum Type_GameEvent
    {
        None = 0,

        // Packet.
        ReceivePacket = 1,          // 패킷 수신.
        BroadcastPacket = 2,        // 주변 섹션 패킷 전파.

        // Content.
        MoveSection = 10,           // 섹션 이동.
        MoveDistrict = 11,          // 구역 이동.
        Attack = 12,                // 공격.
    }
}
