namespace GameCore
{
    /// <summary>
    /// 게임 에러 코드.
    /// </summary>
    public static class GameErrorCode
    {
        //////////////////////////////////////////////////////////////////
        /// Server internal.
        public const int DBError = -2;
        public const int ServerException = -1;

        public const int NoError = 0;


        //////////////////////////////////////////////////////////////////
        /// Error.

        // Auth.
        public const int Auth_EmptyUserId = 1000;                       // UserId 없음.
        public const int Auth_NotExistUserId = 1001;                    // UserId가 존재하지 않음.
        public const int Auth_NotExistIdentity = 1002;                  // Identity가 존재하지 않음.
        public const int Auth_NotMatchUid = 1003;                       // Uid 불일치.
        public const int Auth_ExpiredToken = 1004;                      // Token 만료.
        public const int Auth_NotMatchToken = 1005;                     // Token 불일치.
        public const int Auth_NotExistPid = 1006;                       // Pid가 존재하지 않음.
        public const int Auth_NotCreatedSession = 1007;                 // Session 생성 실패.
        public const int Auth_NotExistPlayerInfo = 1008;                // PlayerInfo가 존재하지 않음.
        public const int Auth_NotCreateOrUpdatePlayer = 1009;           // 플레이어 객체 생성 혹은 업데이트 실패.


        // Player.
        public const int CreatePlayer_NotExistUserId = 1020;            // UserId가 존재하지 않음.
        public const int CreatePlayer_NotMatchUid = 1021;               // Uid 불일치.
        public const int CreatePlayer_NotExistIdentity = 1022;          // Identity가 존재하지 않음.
        public const int CreatePlayer_ExpiredToken = 1023;              // Token 만료.
        public const int CreatePlayer_NotMatchToken = 1024;             // Token 불일치.
        public const int CreatePlayer_NotCreatedPlayer = 1025;          // PlayerInfo 생성 실패.
    }
}
