:: Nuget key (company)
set NUGET_KEY=oy2fysyppj7ci5skfg3a7lxbuoyz6t4ulhzwic3w7may6u

:: Nuget server (company)
set NUGET_SERVER_INDEX_JSON=http://10.0.60.10:5555/v3/index.json

:: Push 
dotnet nuget push -s %NUGET_SERVER_INDEX_JSON% "*.nupkg" -k %NUGET_KEY% --skip-duplicate

pause