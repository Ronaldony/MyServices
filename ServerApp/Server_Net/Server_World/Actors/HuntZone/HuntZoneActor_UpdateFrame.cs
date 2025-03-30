namespace Server_World.Actors.HuntZone
{
    using GameCore;
    using Server_World.Core.WorldMsg;
    using ServerEngine.Core.Util;
    using System.Linq;
    using System.Numerics;

    /// <summary>
    /// HuntZoneActor.
    /// </summary>
    public partial class HuntZoneActor
    {
        /// <summary>
        /// 프레임 업데이트.
        /// </summary>
        private void UpdateFrmae(WorldMsg_UpdateFrame msg)
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
            foreach (var sectionPlayerPair in _sectionPlayerDict)
            {
                // 플레이어가 1명이라도 존재하는 섹션 업데이트.
                if (0 == sectionPlayerPair.Value.Count)
                {
                    return;
                }

                // 섹션의 적, 플레이어 데이터 가져오기.
                var section = sectionPlayerPair.Key;
                var enemies = _sectionEnemyDict[section];
                var players = _sectionPlayerDict[section];

                var nowTime = TimeUtil.Now;

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
                    var deltaTime = (nowTime - playInfo.UpdateTime).TotalSeconds;

                    Calc_PlayerPosition(ref pos, playInfo.Position.MoveDirection, deltaTime, playInfo.Stat.Velocity);
                }

                // 적 이동 처리.
                nowTime = TimeUtil.Now;

                // 프레임 시간.
                var frameDeltaTime = msg.FrameDeltaTime;

                foreach (var enemy in enemies.Values)
                {
                    var position = enemy.Position;

                    position.FrameDeltaTime += frameDeltaTime;

                    // 이동.
                    Vector2.Lerp(position.Pos, position.NextPos, position.FrameDeltaTime * position.Veltocity / _confEnemy.RoamTimeSec);

                    // 다음 이동 패턴 업데이트.
                    if (position.MovePattern == msg.MovePattern)
                    {
                        var index = position.RoamingPos.IndexOf(position.NextPos);
                        position.NextPos = position.RoamingPos[(index + 1) % position.RoamingPos.Count];
                    }
                }
            }
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
    }
}
