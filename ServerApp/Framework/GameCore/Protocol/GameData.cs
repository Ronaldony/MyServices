namespace GameCore
{
    /// <summary>
    /// 게임 데이터.
    /// </summary>
    public static class GameData
    {
        // 이동.
        public const double MOVENT_ERROR = 0.45;            // 초당 이동 오차 범위.

        // 섹션.
        public const int SECTION_WIDTH = 7;                 // 섹션 가로 길이.
        public const int SECTION_HEIGHT = 5;                // 섹션 세로 길이.

        // Mob.
        public const int MAX_ENEMY_SPAWN = 50;              // 최대 적 스폰 수.

        // KnockBack.
        public const float KNOCKBACK_THRUST = 10f;          // 넉백 쓰러스트.
        public const float KNOCKBACK_TIME = 0.2f;           // 넉백 시간.
        public const float KNOCKBACK_MASS = 1f;             // 넉백 질량.
        public const float KNOCKBACK_ERROR_DISTANCE = 3f;   // 넉백 오차 거리.

        // Dash.
        public const float DASH_TIME = 0.2f;                // 대시 시간.
        public const float DASH_COOLDOWN = 0.25f;           // 대시 쿨타임.
        public const int DASH_SPEED = 4;                    // 대시 속도.
        public const float DASH_ERROR = 1;                  // 대시 오차.

        // Hitbox.
        public const float HITBOX_LEFT = 0.33f;
        public const float HITBOX_RIGHT = 0.33f;
        public const float HITBOX_TOP = 0.86f;
    }
}
