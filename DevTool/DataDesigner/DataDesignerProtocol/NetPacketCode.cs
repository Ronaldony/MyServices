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
        public const int CS_MoveArea = 110;                 // 장소 이동.
        public const int SC_MoveArea = 111;                 // 장소 이동.

        // 이동 패턴.
        public const int CS_UpdateMovePattern = 120;        // 이동 패턴 업데이트.
        public const int SC_UpdateMovePattern = 121;        // 이동 패턴 업데이트.

        // 공격.
        public const int CS_Attack = 130;                   // 공격.
        public const int SC_Attack = 131;                   // 공격.
        public const int CS_HitEnemy = 132;                 // Enemy 피격.
        public const int SC_HitEnemy = 133;                 // Enemy 피격.

        // 섹션.
        public const int CS_UpdateSection = 140;            // 섹션 업데이트.
        public const int SC_UpdateSection = 141;            // 섹션 업데이트.

        // 넉백.
        public const int CS_KnockBack = 150;                // 넉백.
        public const int SC_KnockBack = 151;                // 넉백.

        // 대시.
        public const int CS_Dash = 160;                // 대시.
        public const int SC_Dash = 161;                // 대시.
    }
}
