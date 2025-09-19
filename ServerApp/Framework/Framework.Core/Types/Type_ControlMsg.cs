namespace Framework.Core.Types
{
    /// <summary>
    /// Type_PlayHubMsg.
    /// </summary>
    public enum Type_ControlMsg
    {
        None = 0,

        AddQueue = 1,

        ClaimPost = 10,
        ClaimPostFail = 11,
        ClaimPostComplete = 12,
    }
}
