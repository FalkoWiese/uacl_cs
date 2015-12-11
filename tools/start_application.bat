rem @echo off
setlocal

if "%1"=="" goto ARG_ERROR
set APPL_DIR=%~dp0\..\%1\bin\Debug
set APPL_EXE=%1.exe

if "%APPL_DIR%\%APPL_EXE%"=="" goto APPL_ERROR

cd %APPL_DIR%
start %APPL_EXE%

goto END

:ARG_ERROR
echo .
echo ..  You have to give the application name as parameter into %~fp!
goto USAGE

:APPL_ERROR
echo .
echo ..  %APPL_DIR%\%APPL_EXE% not available, please compile the application!

:USAGE
echo ... Usage:
echo ..            %~dp\%~fp <application name>
echo .

:END
endlocal