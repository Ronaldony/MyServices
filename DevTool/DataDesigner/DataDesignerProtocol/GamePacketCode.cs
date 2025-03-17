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
        public const int Login = 10;                // 로그인.

        // 플레이어.
        public const int Move = 100;                 // 이동.
        public const int Move_Area = 101;            // 장소 이동.
        public const int Attack = 102;               // 공격.
    }
}
