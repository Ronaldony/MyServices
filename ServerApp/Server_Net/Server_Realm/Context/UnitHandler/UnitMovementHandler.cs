using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Util;
using System.Numerics;

namespace Server_Realm.Context.UnitHandler
{
    using Framework.Core.Utilities;
    using GameCore;
    using GameCore.Datas.Plays;
    using Server_Realm.Context.Units;
    using Server_Realm.Worlds;

    /// <summary>
    /// UnitMovementHandler.
    /// </summary>
    public class UnitMovementHandler : UnitHandlerBase
    {
        private readonly ILogger<UnitMovementHandler> _logger;

        // Unit.
        private Unit _owner;

        // DTO.
        private DTO_Location _location;
        private DTO_Stat _stat;

        // Sections.
        public SectionId Section { get; private set; }
        public SectionId OldSection { get; private set; }

        // 최근 이동 시작 시간.
        private DateTime _lastMoveTime;

        // Move status.
        private bool _isMoveing;

        // 섹션 업데이트 여부.
        private bool _isSectionUpdated;

        // 이동 상태 변화.
        private bool _isMoveStatusUpdated;

        public Vector2 NowPos() => _location.Pos;
        public Vector2 NowDir() => _location.MoveDirection;
        public bool IsMoving() => _isMoveing;

        public UnitMovementHandler(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UnitMovementHandler>>();
        }

        /// <summary>
        /// 기본 상태 초기화.
        /// </summary>
        public void Setup(Unit owner, DTO_Location location, DTO_Stat stat)
        {
            _owner = owner;
            _location = location;
            _stat = stat;

            Section = SectionId.ToSection(_location.Pos);
            OldSection = Section;

            _isMoveing = false;
            _isSectionUpdated = false;
            _isMoveStatusUpdated = false;
            _lastMoveTime = TimeUtil.Now;
        }

        #region move
        /// <summary>
        /// Move start.
        /// </summary>
        public void MoveStart(Vector2 moveDireciton)
        {
            _lastMoveTime = TimeUtil.Now;
            _isMoveing = true;

            _location.MoveDirection = new Vector2(MathF.Round(moveDireciton.X, 2), MathF.Round(moveDireciton.Y, 2));

            // 상태 업데이트.
            _isMoveStatusUpdated = true;
        }

        /// <summary>
        /// Move start.
        /// </summary>
        public void MoveStart(Vector2 moveDireciton, Vector2 endPos)
        {
            _lastMoveTime = TimeUtil.Now;
            _isMoveing = true;

            _location.MoveDirection = new Vector2(MathF.Round(moveDireciton.X, 2), MathF.Round(moveDireciton.Y, 2));
            _location.PosEnd = endPos;

            // 상태 업데이트.
            _isMoveStatusUpdated = true;
        }

        /// <summary>
        /// Update.
        /// </summary>
        public void Update(double deltaTime)
        {
            // 유닛 죽음 판정 확인.
            if (_owner.IsDead) return;

            if (_isMoveing)
            {
                var deltaPos = _location.MoveDirection * (float)deltaTime * _stat.Velocity;
                var movePos = _location.Pos + deltaPos;

                if (false == _owner.ZoneProcessor.IsValidSection(movePos))
                {
                    return;
                }

                _location.Pos = movePos;

                if (_owner.UnitType == Core.Types.Type_Unit.Mob)
                {
                    //_logger.LogDebug($"Update movement. pos: {_location.Pos}, dir: {_location.MoveDirection}");
                }

                // Post process.
                PostMove();
            }
        }

        /// <summary>
        /// Move stop.
        /// </summary>
        public bool MoveStop(Vector2 endPos)
        {
            if (false == _owner.ZoneProcessor.IsValidSection(endPos))
            {
                return false;
            }

            var nowTime = TimeUtil.Now;

            // 이동 거리 계산 및 유효성 검사.
            var deltaTime = (nowTime - _lastMoveTime).TotalSeconds + GameData.MOVEMENT_TIME_ERROR;

            if (false == IsValidMove(deltaTime, _stat.Velocity, _location.MoveDirection, _location.Pos, endPos))
            {
                return false;
            }

            // 이동 정보 갱신.
            _location.Pos = endPos;

            // Stop move.
            MoveStop();

            // Post process.
            PostMove();

            return true;
        }

        /// <summary>
        /// Move stop.
        /// </summary>
        public void MoveStop()
        {
            _location.MoveDirection = Vector2.Zero;
            _lastMoveTime = TimeUtil.Now;

            // 상태 업데이트.
            _isMoveing = false;
            _isMoveStatusUpdated = true;

            if (_owner.UnitType == Core.Types.Type_Unit.Mob)
            {
                //_logger.LogDebug($"Update movement. pos: {_location.Pos}, dir: {_location.MoveDirection}");
            }
        }
        #endregion

        #region knockback & dash
        /// <summary>
        /// Knockback.
        /// </summary>
        public bool Knockback(Vector2 knockbackPos)
        {
            if (false == _owner.ZoneProcessor.IsValidSection(knockbackPos))
            {
                return false;
            }

            // 운동량(P) = 질량(MASS) * 속도(V)
            // 위 공식은 Unity AddForce(Impulse)와 동일한 결과를 내기 위한 공식.
            var momentum = VectorUtility.SafeNormalize(knockbackPos, _location.Pos) * GameData.KNOCKBACK_THRUST * GameData.KNOCKBACK_MASS;

            // 넉백 예상 위치 = 현재 위치 + (운동량 * 움직인 시간)
            var calcPosition = _location.Pos + momentum * GameData.KNOCKBACK_TIME;

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
            PostMove();

            return true;
        }

        /// <summary>
        /// Dash.
        /// </summary>
        public bool Dash(Vector2 dashPos)
        {
            if (false == _owner.ZoneProcessor.IsValidSection(dashPos))
            {
                return false;
            }

            // 대시 결과 유효성 검사.
            if (false == IsValidDash(_stat.Velocity, _location.MoveDirection, _location.Pos, dashPos))
            {
                return false;
            }

            // 위치 갱신.
            _location.Pos = dashPos;

            // 포스트 업데이트.
            PostMove();

            return true;
        }
        #endregion

        #region common
        /// <summary>
        /// Section updated.
        /// </summary>
        public bool IsSectionUpdated()
        {
            var oldStatus = _isSectionUpdated;
            _isSectionUpdated = false;
            return oldStatus;
        }

        /// <summary>
        /// Move status updated.
        /// </summary>
        public bool IsMoveStatusUpdated()
        {
            var oldStatus = _isMoveStatusUpdated;
            _isMoveStatusUpdated = false;
            return oldStatus;
        }
        #endregion

        #region private
        /// <summary>
        /// Post udpate.
        /// </summary>
        private void PostMove()
        {
            // 섹션 이동한 경우 갱신.
            var newSection = SectionId.ToSection(_location.Pos);

            if (newSection != Section)
            {
                OldSection = Section;
                Section = newSection;

                // Section updated.
                _isSectionUpdated = true;
            }

            // Collision 갱신.
            _owner.UnitCollision.Update(_location.Pos);
        }

        /// <summary>
        /// Validation for position from packet.
        /// </summary>
        private bool IsValidMove(double diffSeconds, int velocity, Vector2 direction, Vector2 lastPos, Vector2 endPos)
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
                var calcDist = Math.Abs(dirY * diffSeconds * velocity) + GameData.MOVEMENT_ERROR;

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
                var calDist = Math.Abs(dirX * diffSeconds * velocity) + GameData.MOVEMENT_ERROR;

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
                    if (dashedPos.Y < playerPos.Y + calcDist) { return false; }
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
                    if (dashedPos.X < playerPos.X + calcDist) { return false; }
                }
            }

            return true;
        }
        #endregion

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
        }
    }
}
