:: Nuget key
set NUGET_KEY=oy2fzpikruawetuo7qoqxrbkma6os67e7k5tfftbpgpqea

:: Nuget server
set NUGET_SERVER_INDEX_JSON=http://192.168.219.4:5555/v3/index.json

:: Push 
dotnet nuget push -s %NUGET_SERVER_INDEX_JSON% "*.nupkg" -k %NUGET_KEY%
pause