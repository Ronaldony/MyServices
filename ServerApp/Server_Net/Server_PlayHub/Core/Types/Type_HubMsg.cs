namespace Server_PlayHub.Core.Types
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

        // Post.
        PushPost = 100,
        RetrievePost = 101,
        DeletePost = 102,
    }
}
