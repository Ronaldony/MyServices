namespace Framework.Core.Types
{
    /// <summary>
    /// Post 처리 단계.
    /// </summary>
    public enum Type_PostStep
    {
        None = 0,           // 미처리.
        Progress = 1,       // 처리중.
        Complete = 2,       // 처리 완료.
    }
}
