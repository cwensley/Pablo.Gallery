@echo off

rem get script dir
set DIR=%~dp0
set DIR=%DIR:~0,-1%
set OLDDIR=%CD%
set EF_VER=6.1.1
set EF_DIR=%DIR%\..\src\packages\EntityFramework.%EF_VER%

set OUTPUT_DIR=%DIR%\..\src\Pablo.Gallery\bin
set CONTEXT_DLL=Pablo.Gallery.dll

if not exist "%OUTPUT_DIR%\%CONTEXT_DLL%" (
	echo You must build the project first before running this script
	exit /B 1
)

echo Copying migrate.exe and entity framework dll's to a temp directory...
call :createTempDir
set TMP_DIR=%TMPDIR%
copy /Y %EF_DIR%\tools\*.* %TMP_DIR% > nul
copy /Y %EF_DIR%\lib\net40\*.* %TMP_DIR% > nul

echo Updating database...
cd %OUTPUT_DIR%
%TMP_DIR%\migrate.exe %CONTEXT_DLL% /startupDirectory="%OUTPUT_DIR%" /startupConfigurationFile=../Web.config %*

echo Cleaning up...
cd %OLDDIR%
rmdir /S /Q %TMP_DIR%

echo Done!
goto:eof

:createTempDir
set TMPDIR=%TMP%\pablo.gallery.migrate-%RANDOM%-%TIME:~6,5%.tmp
if exist "%TMPDIR%" GOTO :createTempDir
mkdir %TMPDIR%
goto:eof


:eof