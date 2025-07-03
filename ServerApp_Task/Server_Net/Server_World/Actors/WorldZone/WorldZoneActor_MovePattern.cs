using System.Numerics;

namespace Server_World.Actors.HuntZone
{
    using Framework.Core.Session;
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.NetPacket;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// WorldZoneActor.
    /// </summary>
    public partial class WorldZoneActor
    {
        /// <summary>
        /// 적 이동 패턴 업데이트.
        /// </summary>
        private void UpdateMovePattern(WorldMsg_UpdateMovePattern msg)
        {
            var movePos = msg.MovePos;
            var movePattern = msg.MovePattern;
            var updateSessions = new Dictionary<GameSession, List<DTO_Enemy>>();

            // 이동 패턴 업데이트.
            foreach (var pair in _worldGroup.SectionEnemyDict)
            {
                var section = pair.Key;
                var worldEnemies = pair.Value.Values;
                var updateEnemies = new List<DTO_Enemy>();

                foreach (var worldEnemy in worldEnemies)
                {
                    var enemy = worldEnemy.Enemy;

                    if (movePattern != enemy.Location.MovePattern)
                    {
                        continue;
                    }

                    // 다음 이동 패턴 업데이트.
                    var location = enemy.Location;

                    location.NextPos = new Vector2(location.SpawnPos.X + movePos.X, location.SpawnPos.Y + movePos.Y);
                    location.NextPosDir = Vector2.Normalize(location.NextPos - location.Pos);

                    // 업데이트 대상 리스트업.
                    updateEnemies.Add(enemy);
                }

                // 업데이트 대상 세션에 추가.
                if (updateEnemies.Any())
                {
                    var aroundPlayers = GetAroundPlayers(section);

                    foreach (var player in aroundPlayers)
                    {
                        if (false == updateSessions.ContainsKey(player.GameSession))
                        {
                            updateSessions.Add(player.GameSession, new List<DTO_Enemy>());
                        }

                        updateSessions[player.GameSession].AddRange(updateEnemies);
                    }
                }
            }

            // 네트워크 전송.
            var netPakcet = new NetPacketSC_UpdateMovePattern();

            foreach (var pair in updateSessions)
            {
                netPakcet.Enemies = pair.Value;
                SendPacket(pair.Key.SessionId, pair.Key.SessionUid, NetPacketCode.SC_UpdateMovePattern, _dataSerializer.Serialize(netPakcet));
            }
        }
    }
}
