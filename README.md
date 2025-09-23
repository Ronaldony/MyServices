## MyServices

C# 논 타겟팅 MMORPG 서버 제작 프로젝트로, C# 으로 제작되었다.

## 프로젝트 환경 및 구성

1. 서버 엔진
   * 설명: 서버에서 공통으로 사용되는 기능 모듈
   * 환경: .NET 9.0
   * 구조
     ```bash
      └── ServerEngine
          ├── ServerEngine.Config: 초기화 모듈
          ├── ServerEngine.Core: Core 모듈
          ├── ServerEngine.Database: DB 모듈
          ├── ServerEngine.GeoIP: IP 모듈
          ├── ServerEngine.MessageBus: 메시징 시스템 모듈
          ├── ServerEngine.Network: 네트워크 모듈
          ├── ServerEngine.AsciiBanner: 폰트 제어
          └── ServerEngine.Test: 엔진 테스트 프로젝트
      
      ``` 
2. 서버
   * 설명: 로그인 서버, 렐름 서버, 컨트롤 서버로 구성되어 실제 클라이언트와 통신하는 주체
   * 환경: .NET 9.0
   * 구조
     ```bash
      └── ServerApp
          ├── Framework: 프레임워크 모음
          │   ├── Framework.Config: 서버 설정
          │   ├── Framework.Core: 서버 코어
          │   ├── GameCore: 게임 코어
          │   └── Framework.WebAPI: WebAPI 기능
          ├── Server_API: API 서버 모음
          │   └── Server_Login: 로그인 서버
          ├── Server_Net: DB 모듈
          │   ├── Server_Control: 컨트롤 서버
          │   └── Server_Realm: 렐름 서버
          └── ServerApp: 서버 솔루션 (Framework, Server_API, Server_Net 포함)
      
      ``` 
3. 클라이언트
   * 설명: Unity로 제작된 게임 클라이언트
   * 환경: Unity 5, Unity Editor v2022.3.12f1
   * 구조
     ```bash
      └── ClinetApp
          └── ProjectRPG: 클라이언트 프로젝트
      
      ``` 
4. 데이터 디자이너
   * 설명: 데이터를 시각적으로 편집할 수 있는 도구로, 서버 및 클라이언트에 동일 데이터, 코드를 제공
   * 환경: node.js v22.13.0, npm v10.9.2
   * 구조
     ```bash
      └── DevTool
          └── DataDesigner: 데이터 디자이너
              ├── DataDesignerApp: 데이터 디자이너 클라이언트
              ├── DataDesignerProtocol: 데이터 프로토콜
              ├── DataDesignerServer: 데이터 디자이너 서버
              ├── publish: 빌드 생성 폴더
              ├── execute_projects.bat: 데이터 디자이너 클라이언트, 서버 실행 배치 파일
              └── publish.bat: 프로젝트 빌드 배치 파일 (DataDesignerApp, DataDesignerServer)
      
      ```
5. 기타 환경
   * Linux Ubuntu: 24.04 LTS
   * MySQL: 8.0.43-0ubuntu0.24.04.1 Linux on x86_64
   * Couchbase: v7.2.0 community amd64
   * docker: v26.1.3
   * RabbitMQ: v4.1.4 (Docker)
   * Strapi: v5.21.0 (node.js v22.14.0)
