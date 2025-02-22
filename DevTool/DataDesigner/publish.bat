:: dotnet clean
cd DataDesignerServer
dotnet restore --no-cache

:: dotnet  publish
dotnet publish DataDesignerServer.csproj --self-contained false -p:PublishSingleFile=true -r win-x64 -p:PublishReadyToRun=false -f net9.0 -c Release --output ../publish

:: Eletron build
cd ..
cd DataDesignerApp
npm run build

pause