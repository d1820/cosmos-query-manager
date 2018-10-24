set version=%1


echo Publishing Cosmos Manager...

del /f ".\Releases\CosmosManagerInstaller-%version%.exe"

rd /s /q ".\CosmosManager\publish"

msbuild .\CosmosManager\CosmosManager.csproj /target:clean /p:configuration=Release

REM msbuild .\CosmosManager\CosmosManager.csproj  /p:configuration=Release

msbuild .\CosmosManager\CosmosManager.csproj /target:publish /p:Configuration=Release /p:VisualStudioVersion=15.0

xcopy /s /y ".\CosmosManager\bin\Release\app.publish\*" ".\CosmosManager\publish\"

del /f ".\CosmosManager\publish\CosmosManager2019.exe"

echo Creating Installer
"C:\Program Files\7-Zip\7z.exe" a -sfx7z.sfx ".\Releases\CosmosManagerInstaller-%version%.exe" ".\CosmosManager\publish\*"


