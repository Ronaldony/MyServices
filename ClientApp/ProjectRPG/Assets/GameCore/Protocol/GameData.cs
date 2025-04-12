namespace GameCore
{
    /// <summary>
    /// 게임 데이터.
    /// </summary>
    public static class GameData
    {
        // 이동.
        public const double MOVENT_ERROR = 0.45;        // 초당 이동 오차 범위.

        // 섹션.
        public const int SECTION_WIDTH = 7;             // 섹션 가로 길이.
        public const int SECTION_HEIGHT = 5;            // 섹션 세로 길이.

        // Mob.
        public const int MAX_ENEMY_SPAWN = 50;          // 최대 적 스폰 수.

        // KnockBack.
        public const float KNOCKBACK_THRUST = 10f;
        public const float KNOCKBACK_TIME = 0.2f;
        public const float KNOCKBACK_MASS = 0.4f;
        public const float KNOCKBACK_ERROR_DISTANCE = 2f;
    }
}
