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
        public const int ConfigData_CreateSchema = 200;             // 200. 스키마 생성 실패.
		public const int ConfigData_UpdateSchema = 201;             // 201. 스키마 갱신 실패.
		public const int ConfigData_DeleteSchema = 202;             // 202. 스키마 제거 실패.
		public const int ConfigData_AlreadyExistSchema = 203;       // 203. 이미 스키마 존재.
		public const int ConfigData_NoSchemaName = 204;             // 204. 스키마 name이 없음.
		public const int ConfigData_NoColumnName = 205;             // 205. 컬럼 name이 없음.
		public const int ConfigData_NotExistSchema = 206;           // 206. 스키마가 존재하지 않음.
		public const int ConfigData_UpdateRow = 207;                // 207. 데이터 업데이트 실패.
        public const int ConfigData_NotExistRow = 208;              // 208. 데이터 로우 존재하지 않음.

        // Type
        public const int TypeData_CreateSchema = 300;               // 300. 스키마 생성 실패.
        public const int TypeData_UpdateSchema = 301;               // 301. 스키마 갱신 실패.
        public const int TypeData_DeleteSchema = 302;               // 302. 스키마 제거 실패.
        public const int TypeData_AlreadyExistSchema = 303;         // 303. 이미 Configuration 스키마 존재.
        public const int TypeData_NoSchemaName = 304;               // 304. 스키마 name이 없음.
        public const int TypeData_NotExistSchema = 305;             // 305. 스키마가 존재하지 않음.
        public const int TypeData_UpdateRow = 306;                  // 306. 데이터 업데이트 실패.
        public const int TypeData_NotExistRow = 307;                // 307. 데이터 로우 존재하지 않음.

		// Table
		public const int TableData_CreateSchema = 400;             // 400. 스키마 생성 실패.
		public const int TableData_UpdateSchema = 401;             // 401. 스키마 갱신 실패.
		public const int TableData_DeleteSchema = 402;             // 402. 스키마 제거 실패.
		public const int TableData_AlreadyExistSchema = 403;       // 403. 이미 스키마 존재.
		public const int TableData_NoSchemaName = 404;             // 404. 스키마 name이 없음.
		public const int TableData_NoColumnName = 405;             // 405. 컬럼 name이 없음.
		public const int TableData_NotExistSchema = 406;           // 406. 스키마가 존재하지 않음.
		public const int TableData_UpdateRow = 407;                // 407. 데이터 업데이트 실패.
		public const int TableData_NotExistRow = 408;              // 408. 데이터 로우 존재하지 않음.

		// Generate
		public const int Generate_NoProfileName = 500;				// 500. 프로파일 이름 없음.
		public const int Generate_UpdateProfile = 501;				// 501. 프로파일 갱신 실패.
		public const int Generate_AddProfile = 502;					// 502. 프로파일 추가 실패.
		public const int Generate_DeleteProfile = 503;				// 503. 프로파일 삭제 실패.
		public const int Generate_NotExistProfile = 504;            // 504. 프로파일 존재하지 않음.
		public const int Generate_Data = 505;						// 505. 코드 생성 실패.
		public const int Generate_WrongTarget = 506;				// 506. 코드 생성 타겟 없음.
		public const int Generate_NotExistProtocol = 507;			// 507. 프로토콜 프로젝트 없음.
		public const int Generate_ProtocolCompile = 508;			// 508. 프로토콜 컴파일 실패.
		public const int Generate_NotCreateDatbase = 509;			// 509. 프로토콜 컴파일 실패.
    }
}
