namespace DataDesignerServer.Core.Common
{
    /// <summary>
    /// ServerErrorCode.
    /// </summary>
    public static class ServerErrorCode
    {
        public const int OccurServerException = -1000;          // 서버 Exception 발생.

        public const int FailCreateConfigSchema = 100;          // 설정 스키마 생성 실패.
    }
}
