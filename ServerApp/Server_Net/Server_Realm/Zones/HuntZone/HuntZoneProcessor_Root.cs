namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Entities;

    /// <summary>
    /// HuntZoneProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 플레이어 루팅 처리.
        /// </summary>
        private void RootItem(PlayerUnit pu, GamePacketCS_RootItem packet)
        {
            // Root item.
            if (false == pu.RootItem(packet.DropId))
            {
                // 루팅 실패.
                var packetRootItem = _memoryPoolService.Rent<GamePacketSC_RootItem>();
                packetRootItem.IsFailed = true;

                SendPacket(pu.Pid, GamePacketCode.SC_RootItem, packetRootItem);

                return;
            }
        }
    }
}
