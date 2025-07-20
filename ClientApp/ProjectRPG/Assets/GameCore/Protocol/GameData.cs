namespace GameCore
{
    /// <summary>
    /// 게임 데이터.
    /// </summary>
    public static class GameData
    {
        // 이동.
        public const double MOVEMENT_ERROR = 3;                      // 초당 이동 오차 범위.
        public const double MOVEMENT_FIXED_AXIS_ERROR = 2;           // 고정축 이동 오차 범위.
        public const double MOVEMENT_TIME_ERROR = 0.3;               // 이동 시간(초) 오차 범위.
        public const int MOVEMENT_RANGE_LIMIT = 5;                   // 이동 범위 제한.

        // 섹션.
        public const int SECTION_WIDTH = 15;                         // 섹션 가로 길이.
        public const int SECTION_HEIGHT = 10;                        // 섹션 세로 길이.

        // Mob.
        public const int MAX_ENEMY_SPAWN = 50;                       // 최대 적 스폰 수.

        // KnockBack.
        public const float KNOCKBACK_THRUST = 10f;                  // 넉백 쓰러스트.
        public const float KNOCKBACK_TIME = 0.2f;                   // 넉백 시간.
        public const float KNOCKBACK_MASS = 1f;                     // 넉백 질량.
        public const float KNOCKBACK_ERROR_DISTANCE = 3f;           // 넉백 오차 거리.
        public const float KNOCKBACK_ERROR_DISTANCE_SQUARE = 9f;    // 넉백 오차 거리 제곱근.(거리 비교용)

        // Dash.
        public const float DASH_TIME = 0.2f;                        // 대시 시간.
        public const float DASH_COOLDOWN = 0.25f;                   // 대시 쿨타임.
        public const int DASH_SPEED = 4;                            // 대시 속도.
        public const float DASH_ERROR = 3;                          // 대시 오차.

        // Hit.
        public const int HIT_DELAY_MS = 400;                        // 히트 딜레이.

        // Projectile.
        public const int PROJECTILE_SPAWN_DISTANCE = 10;            // 투사체 스폰 거리.
        public const int PROJECTILE_SPAWN_DISTANCE_SQUARE = 100;    // 투사체 스폰 거리 제곱근. (거리 비교용)
    }
}
