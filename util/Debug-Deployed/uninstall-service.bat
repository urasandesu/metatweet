@echo off
cd %~DP0

C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\InstallUtil.exe /u ..\..\dist\Debug\bin\MetaTweetHostService.exe || (
    echo Press ENTER key to exit.
    pause > nul
)