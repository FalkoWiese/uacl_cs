@echo off

:: python -m easy_install --always-unzip http://downloads.sourceforge.net/project/pywin32/pywin32/Build%20218/pywin32-218.win32-py2.7.exe
:: python -m easy_install --always-unzip http://www.stickpeople.com/projects/python/win-psycopg/2.6.1/psycopg2-2.6.1.win32-py2.7-pg9.4.4-release.exe

:: some python packages require a compiler but assume Visual Studio 2008
:: this sets the needed environment variable
:: SET VS90COMNTOOLS=%VS100COMNTOOLS%
:: or install VC..Py.msi and have a look in your user directory, maybe in ...
:: $HOME\AppData\Local\Programs\Common\Microsoft\Visual C++ for Python\9.0
:: point the above mentioned env var to this directory.

pip install --use-wheel --requirement %~dp0..\requirements_development.txt
pip install --use-wheel --requirement %~dp0..\requirements.txt

python -m wheel install-scripts ipython Sphinx Fabric PyInstaller PySide

:: check status of pip installations:
:: pip install --no-install --no-download -r requirements.txt
:: echo %ERRORLEVEL%
:: 0 - all good
:: >0 - some lib is not installed
