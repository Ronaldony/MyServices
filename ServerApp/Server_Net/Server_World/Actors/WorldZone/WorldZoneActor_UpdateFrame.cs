using System.Collections.Concurrent;
using System.Numerics;

namespace Server_World.Actors.HuntZone
{
    using Framework.Core.Session;
    using GameCore;
    using GameCore.NetPacket;
    using Microsoft.Extensions.Logging;
    using Server_World.Core.Worlds;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// WorldZoneActor.
    /// </summary>
    public partial class WorldZoneActor
    {
        /// <summary>
        /// 프레임 업데이트.
        /// </summary>
        private void UpdateFrame(WorldMsg_UpdateFrame msg)
        {
            // 이동 처리.
            var moveWorldSection = UpdateFrame_Move(msg);

            // 이동 섹션 처리.
            UpdateFrame_Section(moveWorldSection);
        }

        /// <summary>
        /// 프레임 업데이트 - 이동 처리.
        /// </summary>
        private ConcurrentDictionary<((int sectionX, int sectionY) oriSection, (int sectionX, int sectionY) section), World_Section>
            UpdateFrame_Move(WorldMsg_UpdateFrame worldMsg)
        {
            var moveWorldSection = new ConcurrentDictionary<((int sectionX, int sectionY) oriSection, (int sectionX, int sectionY) section), World_Section>();

            // 섹션을 루프하여 플레이어, 적 이동 처리.
            Parallel.ForEach(_worldGroup.SectionPlayerDict, pair =>
            {
                // 섹션의 적, 플레이어 데이터 가져오기.
                var section = pair.Key;
                var players = pair.Value;

                ///////////////////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////////////////
                /// 이동 처리.

                // 플레이어 이동 처리.
                foreach (var worldPlayer in players.Values)
                {
                    var playInfo = worldPlayer.PlayInfo;
                    var location = playInfo.Location;

                    // 정지, 대시인 경우, 처리하지 않음.
                    switch (playInfo.Action)
                    {
                        case Type_Action.Move:
                            {
                                var position = location.Position;

                                // 위치 계산.
                                if (false == Calc_PlayerPosition(ref position, location.MoveDirection, worldMsg.FrameDeltaTime, playInfo.Stat.Velocity))
                                {
                                    continue;
                                }

                                // 위치 업데이트.
                                location.Position = position;
                            }
                            break;
                        case Type_Action.MoveEnd:
                            {
                                playInfo.Action = Type_Action.None;

                                // 위치 업데이트.
                                location.Position = location.PositionEnd;
                            }
                            break;
                        default:
                            continue;
                    }

                    // 섹션 업데이트.
                    var moveSection = GetSection(location.Position);

                    if (false == section.Equals(moveSection))
                    {
                        _worldGroup.SectionPlayerDict[section].Remove(worldPlayer.PlayInfo.Pid, out var _);
                        _worldGroup.SectionPlayerDict[moveSection].AddOrUpdate(worldPlayer.PlayInfo.Pid, worldPlayer, (key, old) => worldPlayer);

                        // 섹션이 이동된 플레이어 정보는 이동된 섹션 주변에 이동 정보를 보냄.
                        var keySection = (section, moveSection);

                        if (false == moveWorldSection.ContainsKey(keySection))
                        {
                            moveWorldSection.AddOrUpdate(keySection, new World_Section(), (_, _) => new World_Section());
                        }

                        moveWorldSection[keySection].PlayInfos.Add(worldPlayer.PlayInfo);
                    }
                }

                // 적 이동 처리.
                var enemies = _worldGroup.SectionEnemyDict[section];

                foreach (var enemy in enemies.Values)
                {
                    var position = enemy.Position;
                    var pos = enemy.Position.Pos;

                    // 적 위치 계산.
                    if (false == Calc_EnemyPosition(ref pos, position.NextPos, position.NextPosDir, worldMsg.FrameDeltaTime, position.Veltocity, out var moveSection))
                    {
                        continue;
                    }

                    // 위치 업데이트.
                    enemy.Position.Pos = pos;

                    // 섹션 업데이트.
                    if (false == section.Equals(moveSection))
                    {
                        _worldGroup.SectionEnemyDict[section].Remove(enemy.EnemeyUid, out var _);
                        _worldGroup.SectionEnemyDict[moveSection].AddOrUpdate(enemy.EnemeyUid, enemy, (_, _) => enemy);

                        // 섹션이 이동된 적 정보는 이동된 섹션 주변 플레이어에 이동 정보를 보냄.
                        var keySection = (section, moveSection);

                        if (false == moveWorldSection.ContainsKey(keySection))
                        {
                            moveWorldSection.AddOrUpdate(keySection, new World_Section(), (_, _) => new World_Section());
                        }

                        moveWorldSection[keySection].Enemies.Add(enemy);
                    }
                }
            });

            return moveWorldSection;
        }

        /// <summary>
        /// 업데이트 프레임 - 섹션.
        /// </summary>
        private void UpdateFrame_Section(
            ConcurrentDictionary<((int sectionX, int sectionY) oriSection, (int sectionX, int sectionY) section), World_Section> worldSectionDict
            )
        {
            var sessionDict = new Dictionary<GameSession, World_Section>();

            // 세션을 이동한 플레이어 정보 전달.
            foreach (var pair in worldSectionDict)
            {
                var oriSection = pair.Key.oriSection;
                var section = pair.Key.section;
                var worldSection = pair.Value;

                // Around sections.
                var oriAroundSections = GetAroundSections(oriSection);
                var aroundSections = GetAroundSections(section);
                var newAroundSections = aroundSections.Where(section => false == oriAroundSections.Contains(section));

                // 업데이트 대상 섹션의 플레이어 저장.
                foreach (var newSection in newAroundSections)
                {
                    // 기존 플레이어들에게 새로 진입한 플레이어 및 적 업데이트.
                    var sectionPlayers = GetSectionPlayers(newSection);

                    if (null != sectionPlayers)
                    {
                        foreach (var player in sectionPlayers)
                        {
                            if (false == sessionDict.ContainsKey(player.GameSession))
                            {
                                // 세션 추가. 
                                sessionDict.Add(player.GameSession, new World_Section());
                            }

                            // 플레이어 정보.
                            if (worldSection.PlayInfos.Any())
                            {
                                sessionDict[player.GameSession].PlayInfos.AddRange(worldSection.PlayInfos);
                            }

                            // 적 정보.
                            if (worldSection.Enemies.Any())
                            {
                                sessionDict[player.GameSession].Enemies.AddRange(worldSection.Enemies);
                            }
                        }
                    }

                    // 새로 진입한 플레이어에 플레이어 및 적 정보 업데이트.
                    var enemies = GetSectionEnemies(newSection);

                    foreach (var movePlayer in worldSection.PlayInfos)
                    {
                        var session = GetSession(movePlayer.Pid);

                        if (null == session)
                        {
                            continue;
                        }

                        // 새로운 섹션 적.
                        if (null != enemies)
                        {
                            if (false == sessionDict.ContainsKey(session))
                            {
                                sessionDict.Add(session, new World_Section());
                            }

                            sessionDict[session].Enemies.AddRange(enemies);
                        }

                        // 새로운 섹션 플레이어.
                        if (null != sectionPlayers)
                        {
                            if (false == sessionDict.ContainsKey(session))
                            {
                                sessionDict.Add(session, new World_Section());
                            }

                            sessionDict[session].PlayInfos.AddRange(sectionPlayers.Select(d => d.PlayInfo));
                        }
                    }
                }
            }

            // 섹션 업데이트 패킷 전송.
            var sendPacket = new NetPacketSC_UpdateSection();
            
            foreach (var pair in sessionDict)
            {
                sendPacket.Enemies = pair.Value.Enemies;
                sendPacket.PlayInfos = pair.Value.PlayInfos;

                SendPacket(pair.Key.SessionId, pair.Key.SessionUid, NetPacketCode.SC_UpdateSection, _dataSerializer.Serialize(sendPacket));
            }
        }

        /// <summary>
        /// Calculation position of player.
        /// </summary>
        private bool Calc_PlayerPosition(ref Vector2 oriPos, Type_MoveDirection moveDir, double diffSeconds, int velocity)
        {
            // 이동 방향 계산 시퀀스. 
            // 1. 이동 방향 확인.
            // 2. 이동 방향에 따라 이동 오차 범위 값 계산.
            // 3. x, y 좌표에 이동 오차 범위 값을 더하여 범위 내인 경우 true, 범위 밖인 경우 false.
            switch (moveDir)
            {
                case Type_MoveDirection.Top:
                    {
                        oriPos.Y += (float)(diffSeconds * velocity);
                    }
                    break;
                case Type_MoveDirection.Down:
                    {
                        oriPos.Y -= (float)(diffSeconds * velocity);
                    }
                    break;
                case Type_MoveDirection.Right:
                    {
                        oriPos.X += (float)(diffSeconds * velocity);
                    }
                    break;
                case Type_MoveDirection.Left:
                    {
                        oriPos.X -= (float)(diffSeconds * velocity);
                    }
                    break;
                case Type_MoveDirection.RightTop:
                    {
                        var calcDist = (float)(diffSeconds * velocity * 0.71);

                        oriPos.X += calcDist;
                        oriPos.Y += calcDist;
                    }
                    break;
                case Type_MoveDirection.LeftTop:
                    {
                        var calcDist = (float)(diffSeconds * velocity * 0.71);

                        oriPos.X -= calcDist;
                        oriPos.Y += calcDist;
                    }
                    break;
                case Type_MoveDirection.RightDown:
                    {
                        var calcDist = (float)(diffSeconds * velocity * 0.71);

                        oriPos.X += calcDist;
                        oriPos.Y -= calcDist;
                    }
                    break;
                case Type_MoveDirection.LeftDown:
                    {
                        var calcDist = (float)(diffSeconds * velocity * 0.71);

                        oriPos.X -= calcDist;
                        oriPos.Y -= calcDist;
                    }
                    break;
            }

            // Validate section.
            if (false == IsValidSection(GetSection(oriPos)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calculation position of enemy.
        /// </summary>
        private bool Calc_EnemyPosition(ref Vector2 oriPos, Vector2 nextPos, Vector2 moveDir, float frameDeltaTime, int velocity, out (int x, int y) updateSection)
        {
            // X 이동.
            if (moveDir.X > 0)
            {
                // 목표 위치가 오른쪽인 경우.
                // 현재 위치가 더 왼쪽인 경우만 처리.
                if (oriPos.X < nextPos.X) { oriPos.X += moveDir.X * frameDeltaTime * velocity; }
            }
            else if (moveDir.X < 0)
            {
                // 목표 위치가 왼쪽인 경우.
                // 현재 위치가 더 오른쪽인 경우만 처리.
                if (oriPos.X > nextPos.X) { oriPos.X += moveDir.X * frameDeltaTime * velocity; }
            }

            // Y 이동.
            if (moveDir.Y > 0)
            {
                // 목표 위치가 위인 경우.
                // 현재 위치가 더 아래인 경우만 처리.
                if (oriPos.Y < nextPos.Y) { oriPos.Y += moveDir.Y * frameDeltaTime * velocity; }
            }
            else if (moveDir.Y < 0)
            {
                // 목표 위치가 아래인 경우.
                // 현재 위치가 더 위인 경우만 처리.
                if (oriPos.Y > nextPos.Y) { oriPos.Y += moveDir.Y * frameDeltaTime * velocity; }
            }

            // Validate section.
            updateSection = GetSection(oriPos);

            if (false == IsValidSection(updateSection))
            {
                return false;
            }

            return true;
        }
    }
}
