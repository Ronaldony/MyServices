namespace Server_World.Actors.HuntZone
{
    using Dapper;
    using GameCore;
    using GameCore.Datas;
    using GameCore.Datas.Play;
    using GameCore.NetPacket;
    using Server_World.Actors.HuntZone.Message;
    using ServerEngine.Network.UDP;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor
    {
        /// <summary>
        /// 플레이어 이동 처리.
        /// </summary>
        public void LoadPlayer(HuntZoneMsg_LoadPlayer packet)
        {
            var pid = packet.PlayInfo.Pid;
            var gameSession = packet.GameSession;

            // Check dictionary.
            if (_playInfoDict.ContainsKey(pid))
            {
                _playInfoDict.Remove(pid);
            }

            if (_sessionDict.ContainsKey(pid))
            {
                _sessionDict.Remove(pid);
            }

            // Send packet.
            var sendPacket = _memoryPoolService.Rent<NetPacketSC_LoadPlayer>();
            sendPacket.Users = _playInfoDict.Values.AsList();

            _worldServer.Send(gameSession.SessionId, gameSession.SessionUid, GamePacketCode.SC_LoadPlayer, _dataSerializer.Serialize(sendPacket));

            _memoryPoolService.Release(sendPacket);

            // Add player datas.
            _sessionDict.Add(pid, gameSession);
            _playInfoDict.Add(pid, packet.PlayInfo);
        }
    }
}
