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
        public const int CS_EnterZone = 12;
        public const int SC_EnterZone = 13;

        // Posts.
        public const int CS_PushPost = 101;
        public const int SC_PushPost = 102;
        public const int CS_ClaimPost = 103;
        public const int SC_ClaimPost = 104;
        public const int CS_ClaimPostFail = 105;
        public const int SC_ClaimPostFail = 106;
        public const int CS_DeletePost = 107;
        public const int SC_DeletePost = 108;
    }
}
