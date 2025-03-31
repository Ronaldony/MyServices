using System.Numerics;

namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using Server_World.Core.WorldMsg;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor
    {
        /// <summary>
        /// 프레임 업데이트.
        /// </summary>
        private void UpdateFrame(WorldMsg_UpdateFrame msg)
        {
            // TODO: 몬스터 이동 처리.
            UpdateFrame_Move(msg);

            // TODO: 충돌 처리.
        }

        /// <summary>
        /// 프레임 업데이트 - 이동 처리.
        /// </summary>s
        private void UpdateFrame_Move(WorldMsg_UpdateFrame msg)
        {
            // 섹션을 루프하여 플레이어, 적 이동 처리.
            Parallel.ForEach(_sectionPlayerDict, sectionPlayerPair =>
            {
                // 섹션의 적, 플레이어 데이터 가져오기.
                var section = sectionPlayerPair.Key;
                var enemies = _sectionEnemyDict[section];
                var players = _sectionPlayerDict[section];

                // 플레이어 이동 처리.
                foreach (var worldPlayer in players.Values)
                {
                    // 플레이어가 이동 중이지 않은 경우, 처리하지 않음.
                    if (worldPlayer.PlayInfo.Position.MoveDirection == GameCore.Type_MoveDirection.None)
                    {
                        continue;
                    }

                    var playInfo = worldPlayer.PlayInfo;
                    var pos = playInfo.Position.Pos;

                    // Calculation.
                    Calc_PlayerPosition(ref pos, playInfo.Position.MoveDirection, msg.FrameDeltaTime, playInfo.Stat.Velocity);

                    // Section 업데이트.
                    var updateSection = ((int)pos.X / GameData.SECTION_WIDTH, (int)pos.Y / GameData.SECTION_HEIGHT);

                    if (section != updateSection)
                    {
                        _sectionPlayerDict[section].Remove(worldPlayer.PlayInfo.Pid, out var _);
                        _sectionPlayerDict[updateSection].AddOrUpdate(worldPlayer.PlayInfo.Pid, worldPlayer, (_, _) => worldPlayer);
                    }
                }

                // 적 이동 처리.
                foreach (var enemy in enemies.Values)
                {
                    var position = enemy.Position;
                    var pos = position.Pos;

                    // Calculation.
                    var isUpdateSection = Calc_EnemyPosition(ref pos, position.NextPos, position.NextPosDir, msg.FrameDeltaTime, position.Veltocity, section);

                    // 섹션 업데이트.
                    if (isUpdateSection)
                    {
                        // Section 업데이트.
                        var updateSection = ((int)pos.X / GameData.SECTION_WIDTH, (int)pos.Y / GameData.SECTION_HEIGHT);

                        if (section != updateSection)
                        {
                            _sectionEnemyDict[section].Remove(enemy.EnemeyUid, out var _);
                            _sectionEnemyDict[updateSection].AddOrUpdate(enemy.EnemeyUid, enemy, (_, _) => enemy);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Calculation position of player.
        /// </summary>
        private void Calc_PlayerPosition(ref Vector2 oriPos, Type_MoveDirection moveDir, double diffSeconds, int velocity)
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
                        var calcDist = (float)(diffSeconds * velocity * 0.8);

                        oriPos.X += calcDist;
                        oriPos.Y += calcDist;
                    }
                    break;
                case Type_MoveDirection.LeftTop:
                    {
                        var calcDist = (float)(diffSeconds * velocity * 0.8);

                        oriPos.X -= calcDist;
                        oriPos.Y += calcDist;
                    }
                    break;
                case Type_MoveDirection.RightDown:
                    {
                        var calcDist = (float)(diffSeconds * velocity * 0.8);

                        oriPos.X += calcDist;
                        oriPos.Y -= calcDist;
                    }
                    break;
                case Type_MoveDirection.LeftDown:
                    {
                        var calcDist = (float)(diffSeconds * velocity * 0.8);

                        oriPos.X -= calcDist;
                        oriPos.Y -= calcDist;
                    }
                    break;
            }
        }

        /// <summary>
        /// Calculation position of enemy.
        /// </summary>
        private bool Calc_EnemyPosition(ref Vector2 oriPos, Vector2 nextPos, Vector2 moveDir, float frameDeltaTime, int velocity, (int x, int y) section)
        {
            var isUpdateSection = false;

            // X 이동.
            if (moveDir.X > 0)
            {
                // 목표 위치가 오른쪽인 경우.
                // 현재 위치가 더 왼쪽인 경우만 처리.
                if (oriPos.X < nextPos.X)
                { 
                    oriPos.X += moveDir.X * frameDeltaTime * velocity;
                    isUpdateSection = true;
                }
            }
            else if (moveDir.X < 0)
            {
                // 목표 위치가 왼쪽인 경우.
                // 현재 위치가 더 오른쪽인 경우만 처리.
                if (oriPos.X > nextPos.X) 
                { 
                    oriPos.X += moveDir.X * frameDeltaTime * velocity; 
                    isUpdateSection = true;
                }
            }

            // Y 이동.
            if (moveDir.Y > 0)
            {
                // 목표 위치가 위인 경우.
                // 현재 위치가 더 아래인 경우만 처s리.
                if (oriPos.Y < nextPos.Y) 
                { 
                    oriPos.Y += moveDir.Y * frameDeltaTime * velocity; 
                    isUpdateSection = true;
                }
            }
            else if (moveDir.Y < 0)
            {
                // 목표 위치가 아래인 경우.
                // 현재 위치가 더 위인 경우만 처리.
                if (oriPos.Y > nextPos.Y) 
                { 
                    oriPos.Y += moveDir.Y * frameDeltaTime * velocity; 
                    isUpdateSection = true;
                }
            }

            return isUpdateSection;
        }
    }
}
