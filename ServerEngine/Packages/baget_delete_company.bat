:: Nuget key (company)
set NUGET_KEY=oy2fysyppj7ci5skfg3a7lxbuoyz6t4ulhzwic3w7may6u

:: Nuget server (company)
set NUGET_SERVER_INDEX_JSON=http://10.0.60.10:5555/v3/index.json

:: Push 
dotnet nuget delete ServerEngine.AsciiBanner -s %NUGET_SERVER_INDEX_JSON% "*.nupkg" -k %NUGET_KEY% 
dotnet nuget delete ServerEngine.Config -s %NUGET_SERVER_INDEX_JSON% "*.nupkg" -k %NUGET_KEY% 
dotnet nuget delete ServerEngine.Core  -s %NUGET_SERVER_INDEX_JSON% "*.nupkg" -k %NUGET_KEY% 
dotnet nuget delete ServerEngine.GeoIP -s %NUGET_SERVER_INDEX_JSON% "*.nupkg" -k %NUGET_KEY% 
dotnet nuget delete ServerEngine.MessageBus -s %NUGET_SERVER_INDEX_JSON% "*.nupkg" -k %NUGET_KEY%
dotnet nuget delete ServerEngine.Network -s %NUGET_SERVER_INDEX_JSON% "*.nupkg" -k %NUGET_KEY% 

pause