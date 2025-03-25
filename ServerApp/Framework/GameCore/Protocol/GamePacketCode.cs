namespace GameCore
{
    /// <summary>
    /// 게임 패킷 코드.
    /// </summary>
    public static class GamePacketCode
    {
        //////////////////////////////////////////////////////////////////////
        /// UDP 

        // Auth
        public const int CS_Login = 10;                     // 로그인.
        public const int SC_Login = 11;                     // 로그인.

        // Load
        public const int CS_LoadPlayer = 20;                // 플레이어 로드.
        public const int SC_LoadPlayer = 21;                // 플레이어 로드.

        // 이동.
        public const int CS_MoveStart = 100;                // 이동 시작.
        public const int SC_MoveStart = 101;                // 이동 시작.
        public const int CS_MoveEnd = 102;                  // 이동 종료.
        public const int SC_MoveEnd = 103;                  // 이동 종료.

        // 지역 이동.
        public const int CS_Move_Area = 110;                // 장소 이동.
        public const int SC_Move_Area = 111;                // 장소 이동.

        // 공격.
        public const int CS_Attack = 120;                   // 공격.
        public const int SC_Attack = 121;                   // 공격.
    }
}
