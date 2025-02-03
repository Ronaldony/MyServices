namespace DataDesignerServer.Core.Common
{
    /// <summary>
    /// ServerErrorCode.
    /// </summary>
    public static class ServerErrorCode
    {
        public const int OccurServerException = -1000;              // 서버 Exception 발생.

        // Configuration
        public const int ConfigData_CreateSchema = 100;             // Configuration 스키마 생성 실패.
        public const int ConfigData_AlreadyExistSchema = 101;       // 이미 Configuration 스키마 존재.
        public const int ConfigData_NoSchemaName = 102;             // 스키마 name이 없음.
        public const int ConfigData_NoColumnName = 103;             // 컬럼 name이 없음.
        public const int ConfigData_NotExistSchema = 104;           // 스키마가 존재하지 않음.
    }
}
