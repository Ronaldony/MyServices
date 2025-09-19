namespace Server_Control.Core.Types
{
    /// <summary>
    /// Type_HubMsg.
    /// </summary>
    public enum Type_HubMsg
    {
        None = 0,

        // Login.
        Enter = 1,
        Leave = 2,
        EnterZone = 3,

        // Post.
        PushPost = 100,
        ClaimPost = 101,
        ClaimPostFail = 102,
        ClaimPostComplete = 103,
        DeletePost = 104,
    }
}
