echo off

rem ######################
rem Python Env
rem ######################
set PYTHON_ENV=c:\Users\Tester\cl_env\Scripts
set CL_WORKSPACE=c:\Workspaces\ua_utilities

rem set IPYTHONDIR=%CL_WORKSPACE%
rem set http_proxy=172.20.10.35:8080
rem set https_proxy=http://172.20.10.35:8080
rem set OPCUADIR=C:\UnifiedAutomation\UaSdkCppBundleSource_133
rem set VSDIR=c:\Programs\MicrosoftVisualStudio2010
rem set VS100COMNTOOLS=%VSDIR%\Common7\Tools\
rem set VS90COMNTOOLS=%VS100COMNTOOLS%
rem call %VSDIR%\VC\vcvarsall.bat
rem set PATH=%PATH%;%CL_WORKSPACE%;

call %PYTHON_ENV%\activate.bat
call cd %CL_WORKSPACE%

start cmd /K "title Compact Laser UA Convenience Layer"
