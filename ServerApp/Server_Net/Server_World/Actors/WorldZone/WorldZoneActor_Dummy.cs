namespace Server_World.Actors.HuntZone
{
    using Framework.Core.Session;
    using GameCore;
    using GameCore.NetPacket;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// WorldZoneActor.
    /// </summary>
    public partial class WorldZoneActor
    {
        /// <summary>
        /// 더미 업데이트.
        /// </summary>
        private void UpdateDummy(WorldMsg_UpdateDummy msg)
        {
            var packet = _memoryPoolService.Rent<NetPacketSC_DummyMove>();
            var gameSessions = new List<GameSession>();
            
            foreach (var worldPlayer in _worldGroup.PlayerDict.Values)
            {
                packet.PlayInfos.Add(worldPlayer.PlayInfo);
                gameSessions.Add(worldPlayer.GameSession);
            }

            foreach (var worldEnemy in _worldGroup.EnemyDict.Values)
            {
                packet.Enemies.Add(worldEnemy.Enemy);
            }

            foreach (var session in gameSessions)
            {
                SendPacket(session.SessionId, session.SessionUid, NetPacketCode.SC_DummyMove, _dataSerializer.Serialize(packet));
            }
        }
    }
}
