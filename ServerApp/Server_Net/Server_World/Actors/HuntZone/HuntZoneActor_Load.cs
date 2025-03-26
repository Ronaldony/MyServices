using Dapper;

namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using GameCore.NetPacket;
    using Server_World.Actors.HuntZone.Message;
    using Server_World.Core.Utilities;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor
    {
        /// <summary>
        /// 플레이어 이동 처리.
        /// </summary>
        private void LoadPlayer(HuntZoneMsg_LoadPlayer msg)
        {
            var pid = msg.PlayInfo.Pid;
            var gameSession = msg.GameSession;

            // Check dictionary.
            if (_playInfoDict.ContainsKey(pid))
            {
                _playInfoDict.Remove(pid);
            }

            if (_sessionDict.ContainsKey(pid))
            {
                _sessionDict.Remove(pid);
            }

            // Add Session.
            _sessionDict.Add(pid, gameSession);

            // Send load player pakcet.
            NetPacketUtility.SendPacket<NetPacketSC_LoadPlayer>(pakcet =>
            {
                pakcet.Users = _playInfoDict.Values.AsList();
                SendPacket(pid, NetPacketCode.SC_LoadPlayer, _dataSerializer.Serialize(pakcet));
            });

            // Add player datas.
            _playInfoDict.Add(pid, msg.PlayInfo);
        }
    }
}
