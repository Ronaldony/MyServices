namespace GameCore
{
    /// <summary>
    /// 허브 패킷 코드.
    /// </summary>
    public static class HubPacketCode
    {
        //////////////////////////////////////////////////////////////////////
        /// Hub

        public const int CS_Enter = 10;
        public const int SC_Enter = 11;

        // Posts.
        public const int CS_PushPost = 101;
        public const int SC_PushPost = 102;
        public const int CS_RetrievePost = 103;
        public const int SC_RetrievePost = 104;
        public const int CS_DeletePost = 105;
        public const int SC_DeletePost = 106;
    }
}
