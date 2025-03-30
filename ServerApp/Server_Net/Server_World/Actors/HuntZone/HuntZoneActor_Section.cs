
using Dapper;

namespace Server_World.Actors.HuntZone
{
    using Framwork.Core.Session;
    using GameCore;
    using GameCore.Datas.Enemy;
    using Server_World.Core.WorldMsg;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor
    {
        /// <summary>
        /// Get section players.
        /// </summary>
        private List<GameSession> GetGameSessions(int sectionX, int sectionY)
        {
            if (false == _sectionPlayerDict.TryGetValue((sectionX, sectionY), out var playerDict))
            {
                return null;
            }

            return playerDict.Values.Select(d => d.GameSession).AsList();
        }

        /// <summary>
        /// Add player to section.
        /// </summary>
        private bool AddPlayerToSection(string pid, World_Player player)
        {
            // 유저 좌표 검사.
            var pos = player.PlayInfo.Position.Pos;
            var section = ((int)pos.X / GameData.SECTION_WIDTH, (int)pos.Y / GameData.SECTION_HEIGHT);

            if (false == _sectionPlayerDict.ContainsKey(section))
            {
                return false;
            }

            // 플레이러를 섹션에 추가.
            if (_sectionPlayerDict[section].TryGetValue(pid, out var oldValue))
            {
                _sectionPlayerDict[section][pid] = player;
                return true;
            }

            _sectionPlayerDict[section].Add(pid, player);

            return true;
        }

        /// <summary>
        /// Initialize seciton players.
        /// </summary>
        private void Initialize_Sections(
            ref Dictionary<(int, int), Dictionary<string, World_Player>> sectionPlayerDict,
            ref Dictionary<(int ,int), Dictionary<ulong, DTO_Enemy>> sectionEnemyDict
            )
        {
            // 지역 정보 불러오기.
            var confArea = _tdbService.First<CONF_Area>(d => d.Area.Equals(Type_Area.HuntZone));

            // 지역 좌표를 계산하여 섹션 분리.

            // 가로, 세로 섹션의 최소, 최댓 값을 사용하여 섹션 분리.
            var sectionMinX = confArea.MinX / GameData.SECTION_WIDTH;
            var sectionMaxX = confArea.MaxX / GameData.SECTION_WIDTH;

            var sectionMinY = confArea.MinY / GameData.SECTION_HEIGHT;
            var sectionMaxY = confArea.MaxY / GameData.SECTION_HEIGHT;

            // 가로, 세로 섹션 저장.
            for (var sectionX = sectionMinX; sectionX <= sectionMaxX; ++sectionX)
            {
                for (var sectionY = sectionMinY; sectionY <= sectionMaxY; ++sectionY)
                {
                    sectionPlayerDict.Add((sectionX, sectionY), new Dictionary<string, World_Player>());
                    sectionEnemyDict.Add((sectionX, sectionY), new Dictionary<ulong, DTO_Enemy>());
                }
            }

            return;
        }
    }
}
