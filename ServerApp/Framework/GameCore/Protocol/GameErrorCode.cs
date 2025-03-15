namespace GameCore
{
    public static class GameErrorCode
    {
        //////////////////////////////////////////////////////////////////
        /// Server internal.
        public const int DBError = -2;
        public const int ServerException = -1;

        public const int NoError = 0;


        //////////////////////////////////////////////////////////////////
        /// Error.
        public const int Login_EmptyUserId = 1000;
        public const int Login_NotExistUserId = 1001;
    }
}
