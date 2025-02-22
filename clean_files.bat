@echo off
set "folder_delete=.vs bin obj .vscode node_modules publish"

for %%f in (%folder_delete%) do (
	for /d /r %%d in (*) do (
    		if exist "%%d\%%f" (
        		echo Deleting "%%d\%%f"
        		rd /s /q "%%d\%%f"
    		)
	)
)

echo Done!
pause