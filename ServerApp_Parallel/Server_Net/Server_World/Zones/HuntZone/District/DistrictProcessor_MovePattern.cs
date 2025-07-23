using System.Numerics;

namespace Server_World.Zones.HuntZone.District
{
    using GameCore;
    using GameCore.Datas.Enemy;
    using GameCore.GamePacket;
    using Server_World.Core.Worlds;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// DistrictProcessor.
    /// </summary>
    public partial class DistrictProcessor
    {
        /// <summary>
        /// 적 이동 패턴 업데이트.
        /// </summary>
        private void UpdateMovePattern(WorldMsg_UpdateMovePattern msg)
        {
            var movePos = msg.MovePos;

            // 이동 패턴 업데이트.
            var updateEnemieDict = new Dictionary<SectionId, List<DTO_Enemy>>();

            foreach (var pair in _worldGroup.SectionEnemyDict)
            {
                var worldEnemies = pair.Value.Values;
                var section = pair.Key;

                updateEnemieDict.Add(section, new List<DTO_Enemy>());

                // 업데이트 이동 패턴 확인 및 대상 추가.
                foreach (var worldEnemy in worldEnemies)
                {
                    var enemy = worldEnemy.Enemy;

                    if (msg.MovePattern != enemy.Location.MovePattern)
                    {
                        continue;
                    }

                    // 다음 이동 패턴 업데이트.
                    var location = enemy.Location;

                    location.NextPos = new Vector2(location.SpawnPos.X + movePos.X, location.SpawnPos.Y + movePos.Y);
                    location.NextPosDir = Vector2.Normalize(location.NextPos - location.Pos);

                    // 업데이트 대상 리스트업.
                    updateEnemieDict[section].Add(enemy);
                }
            }

            // MovePattern 패킷 전송.
            var GamePacket = new GamePacketSC_UpdateMovePattern();

            foreach (var pair in updateEnemieDict)
            {
                GamePacket.Enemies = pair.Value;
                SendPacket_AroundSections(pair.Key, GamePacketCode.SC_UpdateMovePattern, GamePacket);
            }
        }
    }
}
