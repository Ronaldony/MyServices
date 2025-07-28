
namespace Server_Realm.Zones.HuntZone
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Worlds.Msg;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// 더미 업데이트.
        /// </summary>
        private void UpdateDummy(WorldMsg_UpdateDummy msg)
        {
            var packet = _memoryPoolService.Rent<GamePacketSC_DummyMove>();

            foreach (var section in _worldGroup.AroundSectionDict.Keys)
            {
                var ws = _worldGroup.WorldSectionDict[section];

                // set dummy.
                packet.PlayInfos = ws.GetPlayerUnits().ToDictionary(d => d.UnitId, d => d.PlayInfo);
                packet.Mobs = ws.GetMobUnits().ToDictionary(d => d.UnitId, d => d.Mob);

                SendPacket_AroundSections(section, GamePacketCode.SC_DummyMove, packet);

                // Dispose.
                packet.Dispose();
            }

            _memoryPoolService.Release(packet);
        }
    }
}
