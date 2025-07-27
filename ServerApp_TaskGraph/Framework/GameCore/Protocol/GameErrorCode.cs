namespace GameCore
{
    /// <summary>
    /// 게임 에러 코드.
    /// </summary>
    public static class GameErrorCode
    {
        //////////////////////////////////////////////////////////////////
        /// Server internal.
        public const int NotExistUser = -3;
        public const int DBError = -2;
        public const int ServerException = -1;

        public const int NoError = 0;

        //////////////////////////////////////////////////////////////////
        /// Error.

        // Action filter.
        public const int AuthFilter_EmptyUid = 1000;                    // Action filter - Uid 없음.
        public const int AuthFilter_EmptyToken = 1001;                  // Action filter - Token 없음.
        public const int AuthFilter_NotmatchToken = 1002;               // Action filter - Token 불일치.
        public const int AuthFilter_ExpiredToken = 1003;                // Action filter - Token 만료.
        public const int AuthFilter_NotExistUser = 1004;                // Action filter - 유저 없음.

        // Auth.
        public const int Auth_EmptyUserId = 2000;                       // UserId 비어있음.
        public const int Auth_NotExistPlayer = 2001;                    // 플레이어가 존재하지 않음.

        // Login.
        public const int Login_NotExistUserId = 2010;                   // UserId가 존재하지 않음.
        public const int Login_NotExistIdentity = 2011;                 // Identity가 존재하지 않음.

        // Player.
        public const int CreatePlayer_NotCreatedPlayer = 2020;          // PlayerInfo 생성 실패.
        public const int CreatePlayer_UpdatePid = 2021;                 // Pid 업데이트 실패.
    }
}
