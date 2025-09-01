namespace Server_Realm.Core.Types
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
        FailRetrievePost = 102,
        DeletePost = 103,
    }
}
