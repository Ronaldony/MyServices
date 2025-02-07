namespace DataDesignerServer.Core.Common
{
	/// <summary>
	/// ServerErrorCode.
	/// </summary>
	public static class ServerErrorCode
	{
		public const int OccurServerException = -1000;              // 서버 Exception 발생.

		// Common
		public const int Common_NotExistSchema = 100;               // 스키마가 존재하지 않음.

		// Configuration
		public const int ConfigData_CreateSchema = 200;             // 200. Configuration 스키마 생성 실패.
		public const int ConfigData_UpdateSchema = 201;             // 201. Configuration 스키마 갱신 실패.
		public const int ConfigData_DeleteSchema = 202;             // 202. Configuration 스키마 제거 실패.
		public const int ConfigData_AlreadyExistSchema = 203;       // 203. 이미 Configuration 스키마 존재.
		public const int ConfigData_NoSchemaName = 204;             // 204. 스키마 name이 없음.
		public const int ConfigData_NoColumnName = 205;             // 205. 컬럼 name이 없음.
		public const int ConfigData_NotExistSchema = 206;           // 206. 스키마가 존재하지 않음.
		public const int ConfigData_UpdateRow = 207;                // 207. 데이터 업데이트 실패.
	}
}
