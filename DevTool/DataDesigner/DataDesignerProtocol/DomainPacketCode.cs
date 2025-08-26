namespace GameCore
{
    /// <summary>
    /// 게임 패킷 코드.
    /// </summary>
    public static class DomainPacketCode
    {
        //////////////////////////////////////////////////////////////////////
        /// PlayDomain

        public const int CS_EnterPlayDomain = 10;
        public const int SC_EnterPlayDomain = 11;

        public const int CS_RetrievePost = 100;
        public const int SC_RetrievePost = 101;
        public const int CS_DeletePost = 102;
        public const int SC_DeletePost = 103;
        public const int CS_DeletePostAll = 104;
        public const int SC_DeletePostAll = 105;
    }
}
