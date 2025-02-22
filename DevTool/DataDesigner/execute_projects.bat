:: server
start devenv ./DataDesignerServer/DataDesignerServer.sln


:: electron
cd DataDesignerApp
call code .

echo "start install electron npm"
call npm install
echo "end install electron npm"
npm run start

echo done!
pause