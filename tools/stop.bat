@echo off

wmic process where name="OfficeConsole.exe" call terminate
wmic process where name="ServerConsole.exe" call terminate