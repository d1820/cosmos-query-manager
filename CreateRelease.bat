set version=%1


echo Publishing Cosmos Manager...
msbuild .\CosmosManager\CosmosManager.csproj /target:publish /p:configuration=release

echo Creating Installer
"C:\Program Files\7-Zip\7z.exe" a -sfx7z.sfx ".\Releases\CosmosManagerInstaller-%version%.exe" ".\CosmosManager\publish\*"


