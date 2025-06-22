using System.Collections.Concurrent;
using System.Numerics;

namespace Server_World.Actors.HuntZone
{
    using Framework.Core.Session;
    using GameCore;
    using GameCore.NetPacket;
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
            var updateMoveDict = UpdateFrame_Move(msg);

            // TODO: Update attak.

            // 이동 세션 처리.
            UpdateFrame_Session(updateMoveDict);
        }

        /// <summary>
        /// 프레임 업데이트 - 이동 처리.
        /// </summary>
        private ConcurrentDictionary<(int sectionX, int sectionY), World_UpdateMove>
            UpdateFrame_Move(WorldMsg_UpdateFrame worldMsg)
        {
            var updateMoveDict = new ConcurrentDictionary<(int sectionX, int sectionY), World_UpdateMove>();

            // 플레이어가 존재하는 섹션의 플레이어, 적 이동 처리.
            Parallel.ForEach(_worldGroup.SectionPlayerDict, pair =>
            {
                // 섹션의 적, 플레이어 데이터 가져오기.
                var section = pair.Key;

                // Player, Enemy.
                var players = pair.Value;
                var enemies = _worldGroup.SectionEnemyDict[section];

                // Player, Enemy 수 검사.
                if (false == players.Any() && false == enemies.Any())
                {
                    return;
                }

                // Set update move.
                updateMoveDict.AddOrUpdate(section, new World_UpdateMove(), (_, _) => new World_UpdateMove());

                var updateMoveSection = updateMoveDict[section];

                /////////////////////////////////////////////////////////////
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
                                var movePos = Calc_PlayerPosition(location.Position, location.MoveDirection, worldMsg.FrameDeltaTime, playInfo.Stat.Velocity);

                                // valid.
                                if (false == IsValidSection(GetSection(movePos)))
                                {
                                    continue;
                                }

                                location.Position = movePos;
                            }
                            break;
                        case Type_Action.MoveEnd:
                            {
                                // 위치 개인.
                                playInfo.Action = Type_Action.None;
                                location.Position = location.PositionEnd;
                            }
                            break;
                        default:
                            continue;
                    }

                    // 변경된 섹션 검사.
                    var moveSection = GetSection(location.Position);

                    if (false == section.Equals(moveSection))
                    {
                        // 플레이어 센션 업데이트.
                        _worldGroup.SectionPlayerDict[section].TryRemove(playInfo.Pid, out var _);
                        _worldGroup.SectionPlayerDict[moveSection].AddOrUpdate(playInfo.Pid, worldPlayer, (key, old) => worldPlayer);

                        // 섹션이 이동된 플레이어 정보는 이동된 섹션 주변에 이동 정보를 보냄.
                        if (false == updateMoveSection.MoveSectionUnit.ContainsKey(moveSection))
                        {
                            updateMoveSection.MoveSectionUnit.Add(moveSection, _memoryPoolService.Rent<World_SectionUnit>());
                        }

                        updateMoveSection.MoveSectionUnit[section].PlayInfos.Add(playInfo);
                    }
                }

                /////////////////////////////////////////////////////////////
                // 적 이동 처리.
                foreach (var wolrdEnemy in enemies.Values)
                {
                    var enemy = wolrdEnemy.Enemy;
                    var location = enemy.Location;

                    // 적 위치 계산.
                    var movePos = Calc_EnemyPosition(location.Pos, location.NextPos, location.NextPosDir, worldMsg.FrameDeltaTime, location.Veltocity);
                    var moveSection = GetSection(movePos);

                    if (false == IsValidSection(moveSection))
                    {
                        continue;
                    }

                    // 위치 업데이트.
                    enemy.Location.Pos = movePos;

                    // 섹션 업데이트.
                    if (false == section.Equals(moveSection))
                    {
                        _worldGroup.SectionEnemyDict[section].TryRemove(enemy.EnemeyUid, out var _);
                        _worldGroup.SectionEnemyDict[moveSection].AddOrUpdate(enemy.EnemeyUid, wolrdEnemy, (_, _) => wolrdEnemy);

                        // 섹션이 이동된 적 정보는 이동된 섹션 주변 플레이어에 이동 정보를 보냄.
                        if (false == updateMoveSection.MoveSectionUnit.ContainsKey(moveSection))
                        {
                            updateMoveSection.MoveSectionUnit.Add(moveSection, _memoryPoolService.Rent<World_SectionUnit>());
                        }

                        updateMoveSection.MoveSectionUnit[section].Enemies.Add(enemy);
                    }
                }
            });

            return updateMoveDict;
        }

        /// <summary>
        /// 업데이트 프레임 - 세션.
        /// </summary>
        private void UpdateFrame_Session(
           ConcurrentDictionary<(int sectionX, int sectionY), World_UpdateMove> updateMoveDict
            )
        {
            var sectionUnitDict = new Dictionary<GameSession, World_SectionUnit>();

            // 세션을 이동한 플레이어 정보 전달.
            foreach (var pair in updateMoveDict)
            {
                var oriSection = pair.Key;
                var oriAroundSections = GetAroundSections(oriSection);

                foreach (var updateMove in pair.Value.MoveSectionUnit)
                {
                    var section = updateMove.Key;
                    var sectionUnit = updateMove.Value;

                    // Around sections.
                    var aroundSections = GetAroundSections(section);
                    var newAroundSections = aroundSections.Where(section => false == oriAroundSections.Contains(section));

                    // 주변 섹션에 전파.
                    foreach (var newSection in newAroundSections)
                    {
                        // 이미 섹션에 존재하는 플레이어들에게 새로 진입한 플레이어, 적 업데이트.
                        var sectionPlayers = GetSectionPlayers(newSection);

                        if (null != sectionPlayers)
                        {
                            foreach (var player in sectionPlayers)
                            {
                                if (false == sectionUnitDict.ContainsKey(player.GameSession))
                                {
                                    // 세션 추가. 
                                    sectionUnitDict.Add(player.GameSession, _memoryPoolService.Rent<World_SectionUnit>());
                                }

                                // 플레이어 정보.
                                if (sectionUnit.PlayInfos.Any())
                                {
                                    sectionUnitDict[player.GameSession].PlayInfos.AddRange(sectionUnit.PlayInfos);
                                }

                                // 적 정보.
                                if (sectionUnit.Enemies.Any())
                                {
                                    sectionUnitDict[player.GameSession].Enemies.AddRange(sectionUnit.Enemies);
                                }
                            }
                        }

                        // 새로 진입한 플레이어에 플레이어 및 적 정보 업데이트.
                        var enemies = GetSectionEnemies(newSection);

                        foreach (var movePlayer in sectionUnit.PlayInfos)
                        {
                            var session = GetSession(movePlayer.Pid);

                            if (null == session)
                            {
                                continue;
                            }

                            // 새로운 섹션 적.
                            if (null != enemies)
                            {
                                if (false == sectionUnitDict.ContainsKey(session))
                                {
                                    sectionUnitDict.Add(session, _memoryPoolService.Rent<World_SectionUnit>());
                                }

                                foreach (var worldEnemy in enemies)
                                {
                                    sectionUnitDict[session].Enemies.Add(worldEnemy.Enemy);
                                }
                            }

                            // 새로운 섹션 플레이어.
                            if (null != sectionPlayers)
                            {
                                if (false == sectionUnitDict.ContainsKey(session))
                                {
                                    sectionUnitDict.Add(session, new World_SectionUnit());
                                }

                                foreach (var worldPlayer in sectionPlayers)
                                {
                                    sectionUnitDict[session].PlayInfos.Add(worldPlayer.PlayInfo);
                                }
                            }
                        }
                    }
                }
            }

            // 섹션 업데이트 패킷 전송.
            var sendPacket = new NetPacketSC_UpdateSection();
            
            foreach (var pair in sectionUnitDict)
            {
                sendPacket.Enemies = pair.Value.Enemies;
                sendPacket.PlayInfos = pair.Value.PlayInfos;

                SendPacket(pair.Key.SessionId, pair.Key.SessionUid, NetPacketCode.SC_UpdateSection, _dataSerializer.Serialize(sendPacket));
            }
        }

        /// <summary>
        /// Calculation position of player.
        /// </summary>
        private Vector2 Calc_PlayerPosition(Vector2 oriPos, Type_MoveDirection moveDir, double diffSeconds, int velocity)
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

            return oriPos;
        }

        /// <summary>
        /// Calculation position of enemy.
        /// </summary>
        private Vector2 Calc_EnemyPosition(Vector2 oriPos, Vector2 nextPos, Vector2 moveDir, float frameDeltaTime, int velocity)
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

            return oriPos;
        }
    }
}
