
namespace Server_World.Zones.HuntZone.District
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// 더미 업데이트.
        /// </summary>
        private void UpdateDummy(WorldMsg_UpdateDummy msg)
        {
            var packet = _memoryPoolService.Rent<GamePacketSC_DummyMove>();

            foreach (var section in _worldGroup.ManagedSections)
            {
                var sectionPlayer = _worldGroup.SectionPlayerDict[section];
                var sectionEnemy = _worldGroup.SectionEnemyDict[section];

                // set dummy.
                packet.PlayInfos = sectionPlayer.Select(d => d.Value.PlayInfo).ToList();
                packet.Enemies = sectionEnemy.Select(d => d.Value.Enemy).ToList();

                SendPacket_AroundSections(section, GamePacketCode.SC_DummyMove, packet);

                // clear.
                packet.PlayInfos.Clear();
                packet.Enemies.Clear();
            }
        }
    }
}
