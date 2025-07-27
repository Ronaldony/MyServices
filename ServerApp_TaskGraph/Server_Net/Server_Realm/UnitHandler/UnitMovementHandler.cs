using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Util;
using System.Numerics;

namespace Server_Realm.UnitHandler
{
    using GameCore;
    using GameCore.Datas.Plays;
    using Server_Realm.Entities;
    using Server_Realm.Worlds;

    /// <summary>
    /// UnitMovementHandler.
    /// </summary>
    public class UnitMovementHandler
    {
        private readonly ILogger<UnitMovementHandler> _logger;

        // Unit.
        private readonly Unit _owner;

        // DTO.
        private readonly DTO_Location _location;
        private readonly DTO_Stat _stat;

        // 최근 이동 시작 시간.
        private DateTime _lastMoveTime;

        // Sections.
        public SectionId Section { get; private set; }
        public SectionId OldSection { get; private set; }

        // Move status.
        private bool _isMoveing;

        #region get_status
        public Vector2 NowPos() => _location.Pos;
        public Vector2 NowDir() => _location.MoveDirection;
        public bool IsMoving() => _isMoveing;
        #endregion

        public UnitMovementHandler(IServiceProvider serviceProvider,  Unit owner, DTO_Location location, DTO_Stat stat)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UnitMovementHandler>>();

            _owner = owner;
            _location = location;
            _stat = stat;

            _lastMoveTime = TimeUtil.Now;

            Section = SectionId.GetSection(_location.Pos);
            OldSection = Section;
        }

        #region move
        /// <summary>
        /// Move start.
        /// </summary>
        public void MoveStart(Vector2 moveDireciton)
        {
            _lastMoveTime = TimeUtil.Now;
            _isMoveing = true;
            _location.MoveDirection = moveDireciton;
        }

        /// <summary>
        /// Update.
        /// </summary>
        public void Update(double deltaTime)
        {
            if (_isMoveing)
            {
                var deltaPos = _location.MoveDirection * (float)deltaTime * _stat.Velocity;
                var movePos = _location.Pos + deltaPos;

                // valid section.

                // Refresh postion.
                _location.Pos = movePos;

                // Post process.
                PostProcess();
            }
        }

        /// <summary>
        /// Move stop.
        /// </summary>
        public bool MoveStop(Vector2 endPos)
        {
            var nowTime = TimeUtil.Now;

            // 이동 거리 계산 및 유효성 검사.
            var deltaTime = (nowTime - _lastMoveTime).TotalSeconds + GameData.MOVEMENT_TIME_ERROR;

            if (false == IsValidPosition(deltaTime, _stat.Velocity, _location.MoveDirection, _location.Pos, endPos))
            {
                return false;
            }

            // 이동 정보 갱신.
            _lastMoveTime = nowTime;
            _location.Pos = endPos;
            _isMoveing = false;

            // Post process.
            PostProcess();

            return true;
        }

        /// <summary>
        /// Move stop.
        /// </summary>
        public bool MoveStop()
        {
            _isMoveing = false;

            return true;
        }
        #endregion

        #region knockback
        /// <summary>
        /// Knockback.
        /// </summary>
        public bool Knockback(Vector2 knockbackPos)
        {
            // 운동량(P) = 질량(MASS) * 속도(V)
            // 위 공식은 Unity AddForce(Impulse)와 동일한 결과를 내기 위한 공식.
            var momentum = Vector2.Normalize(knockbackPos - _location.Pos) * GameData.KNOCKBACK_THRUST * GameData.KNOCKBACK_MASS;

            // 넉백 예상 위치 = 현재 위치 + (운동량 * 움직인 시간)
            var calcPosition = _location.Pos + (momentum * GameData.KNOCKBACK_TIME);

            // 계산된 위치와 전달 받은 위치의 거리가 오차 허용 범위 내인지 검사.
            var errorPos = Vector2.DistanceSquared(calcPosition, knockbackPos);

            // 오차 허용 범위 검사.
            if (GameData.KNOCKBACK_ERROR_DISTANCE_SQUARE < errorPos)
            {
                _logger.LogError($"KnockBack - calc: {calcPosition}, recv: {knockbackPos}, momentum: {momentum} error: {errorPos}");
                return false;
            }

            // 위치 갱신.
            _location.Pos = knockbackPos;

            // 포스트 업데이트.
            PostProcess();

            return true;
        }
        #endregion

        #region dash
        public bool Dash(Vector2 dashPos)
        {
            // 대시 결과 유효성 검사.
            if (false == IsValidDash(_stat.Velocity, _location.MoveDirection, _location.Pos, dashPos))
            {
                return false;
            }

            // 위치 갱신.
            _location.Pos = dashPos;

            // 포스트 업데이트.
            PostProcess();

            return true;
        }
        #endregion

        #region private
        /// <summary>
        /// Post udpate.
        /// </summary>
        private void PostProcess()
        {
            // 섹션 이동한 경우 갱신.
            var newSection = SectionId.GetSection(_location.Pos);

            if (newSection != Section)
            {
                OldSection = _owner.Section;
                _owner.Section = newSection;
            }

            // Collision 갱신.
            _owner.UnitCollision.Update(_location.Pos);
        }

        /// <summary>
        /// Validation for position from packet.
        /// </summary>
        private bool IsValidPosition(double diffSeconds, int velocity, Vector2 direction, Vector2 lastPos, Vector2 endPos)
        {
            var dirY = direction.Y;
            var dirX = direction.X;

            // Y축 계산.
            if (dirY == 0)
            {
                // 고정축 계산.
                if (endPos.Y > lastPos.Y + GameData.MOVEMENT_FIXED_AXIS_ERROR) { return false; }
                if (endPos.Y + GameData.MOVEMENT_FIXED_AXIS_ERROR < lastPos.Y) { return false; }
            }
            else
            {
                var calcDist = Math.Abs(dirY * diffSeconds * velocity + GameData.MOVEMENT_ERROR);

                if (dirY > 0)
                {
                    // Top.
                    if (endPos.Y > lastPos.Y + calcDist) { return false; }
                }
                else
                {
                    // Down.
                    if (endPos.Y + calcDist < lastPos.Y) { return false; }
                }
            }

            // X축 계산.
            if (dirX == 0)
            {
                // 고정축 계산.
                if (endPos.X > lastPos.X + GameData.MOVEMENT_FIXED_AXIS_ERROR) { return false; }
                if (endPos.X + GameData.MOVEMENT_FIXED_AXIS_ERROR < lastPos.X) { return false; }
            }
            else
            {
                var calDist = Math.Abs(dirX * diffSeconds * velocity + GameData.MOVEMENT_ERROR);

                if (dirX > 0)
                {
                    // Right.
                    if (endPos.X > lastPos.X + calDist) { return false; }
                }
                else
                {
                    // Left.
                    if (endPos.X + calDist < lastPos.X) { return false; }
                }
            }

            return true;
        }

        /// <summary>
        /// Validation for position from packet.
        /// </summary>
        private bool IsValidDash(int velocity, Vector2 direction, Vector2 playerPos, Vector2 dashedPos)
        {
            var dirY = direction.Y;
            var dirX = direction.X;

            // Y 축 계산.
            if (dirY == 0)
            {
                // 고정축 범위 계산.
                if (dashedPos.Y > playerPos.Y + GameData.MOVEMENT_ERROR) { return false; }
                if (dashedPos.Y + GameData.MOVEMENT_ERROR < playerPos.Y) { return false; }
            }
            else
            {
                var calcDist = dirY * (GameData.DASH_TIME * GameData.DASH_SPEED * velocity + GameData.DASH_ERROR);

                // Y 좌표 검사.
                if (dirY > 0)
                {
                    // Top.
                    if (dashedPos.Y > playerPos.Y + calcDist) { return false; }
                }
                else
                {
                    // Down.
                    if (dashedPos.Y + calcDist < playerPos.Y) { return false; }
                }
            }

            // X 축 계산.
            if (dirX == 0)
            {
                // 고정축 범위 계산.
                if (dashedPos.X > playerPos.X + GameData.MOVEMENT_ERROR) { return false; }
                if (dashedPos.X + GameData.MOVEMENT_ERROR < playerPos.X) { return false; }
            }
            else
            {
                var calcDist = dirX * (GameData.DASH_TIME * GameData.DASH_SPEED * velocity + GameData.DASH_ERROR);

                if (dirX > 0)
                {
                    // Right.
                    if (dashedPos.X > playerPos.X + calcDist) { return false; }
                }
                else
                {
                    // Left.
                    if (dashedPos.X + calcDist < playerPos.X) { return false; }
                }
            }

            return true;
        }
        #endregion
    }
}
