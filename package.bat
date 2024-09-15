del /F /Q GameData
dotnet build .\KwarterMaster.csproj
mkdir GameData\KwarterMaster\Plugins
copy Plugins\net472\KwarterMaster.dll GameData\KwarterMaster\Plugins
mkdir GameData\KwarterMaster\Textures
copy Textures GameData\KwarterMaster\Textures\