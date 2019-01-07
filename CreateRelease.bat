set version=%1

echo Currently you must manually run Porject.Publish from context menu.
echo Publishing Cosmos Manager...

rem del /f ".\Releases\CosmosManagerInstaller-%version%.exe"

rem rd /s /q ".\CosmosManager\publish"

rem msbuild .\CosmosManager\CosmosManager.csproj /target:clean /p:configuration=Release

REM msbuild .\CosmosManager\CosmosManager.csproj  /p:configuration=Release

rem msbuild .\CosmosManager\CosmosManager.csproj /target:publish /p:Configuration=Release /p:VisualStudioVersion=15.0

rem xcopy /s /y ".\CosmosManager\bin\Release\app.publish\*" ".\CosmosManager\publish\"

rem del /f ".\CosmosManager\publish\CosmosManager2019.exe"

echo Creating Installer
"C:\Program Files\7-Zip\7z.exe" a -sfx7z.sfx ".\Releases\CosmosManagerInstaller-%version%.exe" ".\CosmosManager\publish\*"


