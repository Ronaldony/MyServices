namespace GameCore
{
    /// <summary>
    /// 네트워크 패킷 코드.
    /// </summary>
    public static class NetPacketCode
    {
        //////////////////////////////////////////////////////////////////////
        /// UDP 

        // Auth.
        public const int CS_Login = 10;                     // 로그인.
        public const int SC_Login = 11;                     // 로그인.

        // Spawn.
        public const int CS_SpawnPlayer = 20;               // 플레이어 스폰.
        public const int SC_SpawnPlayer = 21;               // 플레이어 스폰.
        public const int CS_SpawnEnemy = 22;                // 적 스폰.
        public const int SC_SpawnEnemy = 23;                // 적 스폰.

        // 이동.
        public const int CS_MoveStart = 100;                // 이동 시작.
        public const int SC_MoveStart = 101;                // 이동 시작.
        public const int CS_MoveEnd = 102;                  // 이동 종료.
        public const int SC_MoveEnd = 103;                  // 이동 종료.
        public const int CS_MoveFixed = 104;                // 이동 수정.
        public const int SC_MoveFixed = 105;                // 이동 수정.

        // 지역 이동.
        public const int CS_Move_Area = 110;                // 장소 이동.
        public const int SC_Move_Area = 111;                // 장소 이동.

        // 공격.
        public const int CS_Attack = 120;                   // 공격.
        public const int SC_Attack = 121;                   // 공격.
    }
}
