:: dotnet clean
cd DataDesignerServer
dotnet restore --no-cache

:: dotnet  publish
call dotnet publish DataDesignerServer.csproj --self-contained -p:PublishSingleFile=true -r win-x64 -p:PublishReadyToRun=false -f net9.0 -c Release --output ../publish

:: Eletron build
cd ..
cd DataDesignerApp
call npm install
call npm run build

echo "Finished build publish"
pause