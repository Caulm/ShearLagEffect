rd /s /q %~dp0bin\Release
rd /s /q %~dp0bin\Publish
pushd %~dp0source\ShearLagEffect
dotnet publish -c Release /p:PublishProfile=FolderProfile
popd
rd /s /q %~dp0bin\Release
del /f /s /q %~dp0bin\Publish\*.pdb
pause