using Dapper;

namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using GameCore.Datas.Play;
    using GameCore.NetPacket;
    using Server_World.Core.Utilities;
    using Server_World.Core.WorldMsg;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor
    {
        /// <summary>
        /// 스폰 플레이어.
        /// </summary>
        private void SpawnPlayer(World_Player player)
        {
            var pid = player.PlayInfo.Pid;
            var session = player.GameSession;

            // Check dictionary.
            if (_playerDict.ContainsKey(pid))
            {
                // 중복 플레이어 처리.
                _playerDict.Remove(pid);
            }

            // Send load player pakcet.
            NetPacketUtility.SendPacket<NetPacketSC_SpawnPlayer>(pakcet =>
            {
                // Send player.
                pakcet.Users = _playerDict.Values.Select(d => d.PlayInfo).AsList();
                SendPacket(session.SessionId, session.SessionUid, NetPacketCode.SC_SpawnPlayer, _dataSerializer.Serialize(pakcet));

                // Send other players.
                pakcet.Users = new List<DTO_PlayInfo> { player.PlayInfo };
                BroadCast(session.SessionId, session.SessionUid, NetPacketCode.SC_SpawnPlayer, _dataSerializer.Serialize(pakcet));
            });

            if (_playerDict.Count == 0)
            {
                _schedulerEnemy.Start();
            }

            // Add player datas.
            if (false == AddPlayerToSection(pid, player))
            {
                return;
            }

            _playerDict.Add(pid, player);
        }
    }
}
