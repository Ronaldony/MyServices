using Dapper;
using PowerArgs;

namespace Server_World.Actors.HuntZone
{
    using Framework.Core.Session;
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.Datas.Play;
    using GameCore.NetPacket;
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
            // 섹션.
            var pid = player.PlayInfo.Pid;
            var session = player.GameSession;

            // 세션 정보 가져오기.
            var section = ((int)player.PlayInfo.Position.Pos.X / GameData.SECTION_WIDTH, (int)player.PlayInfo.Position.Pos.Y / GameData.SECTION_HEIGHT);
            var aroundSessions = GetAroundSessions(section);

            if (aroundSessions.Any())
            {
                // Send others.
                var otherPlayerPacket = new NetPacketSC_SpawnPlayer
                {
                    Users = new List<DTO_PlayInfo> { player.PlayInfo }
                };

                var packetBytes = _dataSerializer.Serialize(otherPlayerPacket);

                foreach (var otherSession in aroundSessions)
                {
                    SendPacket(otherSession.SessionId, otherSession.SessionUid, NetPacketCode.SC_SpawnPlayer, packetBytes);
                }
            }

            // send this player.
            var packet = new NetPacketSC_SpawnPlayer
            {
                Users = _playerDict.Values.Select(d => d.PlayInfo).AsList()
            };

            SendPacket(session.SessionId, session.SessionUid, NetPacketCode.SC_SpawnPlayer, _dataSerializer.Serialize(packet));

            // 섹션에 플레이어 저장.
            if (false == AddPlayerIntoWorld(pid, player, section.Item1, section.Item2))
            {
                return;
            }
        }

        /// <summary>
        /// 스폰 적.
        /// </summary>
        private void SpawnEnemy(WorldMsg_SpawnEnemy msg)
        {
            var sessionEnemyDict = new Dictionary<GameSession, List<DTO_Enemy>>();

            // 적 스폰 병렬 처리.
            foreach (var pair in msg.Enemies)
            {
                var section = pair.Key;
                var enemies = pair.Value;

                // 섹션에 적 정보 추가.
                var sectionEnemy = _sectionEnemyDict[section];

                foreach (var enemy in enemies)
                {
                    sectionEnemy.AddOrUpdate(enemy.EnemeyUid, enemy, (key, old) => enemy);
                }

                // 근처 세션 가져오기.
                var aroundSessions = GetAroundSessions(section);

                // 플레이어 세션에 전송할 스폰 적 저장.
                foreach (var session in aroundSessions)
                {
                    if (sessionEnemyDict.ContainsKey(session))
                    {
                        sessionEnemyDict.Add(session, new List<DTO_Enemy>());
                    }

                    sessionEnemyDict[session].AddRange(enemies);
                }
            }

            // send packet.
            if (false == sessionEnemyDict.Any())
            {
                return;
            }

            Parallel.ForEach(sessionEnemyDict, pair =>
            {
                var packet = new NetPacketSC_SpawnEnemy
                {
                    Enemies = pair.Value
                };

                SendPacket(pair.Key.SessionId, pair.Key.SessionUid, NetPacketCode.SC_SpawnEnemy, _dataSerializer.Serialize(packet));

            });
        }
    }
}
