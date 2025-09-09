:: Nuget key
set NUGET_KEY=oy2fzpikruawetuo7qoqxrbkma6os67e7k5tfftbpgpqea

:: Nuget server
set NUGET_SERVER_INDEX_JSON=http://localhost:8111/httpAuth/app/nuget/feed/ProjectRPG/teamcity_baget/v3/index.json

:: Push 
dotnet nuget push --skip-duplicate -s %NUGET_SERVER_INDEX_JSON% "*.nupkg"
pause