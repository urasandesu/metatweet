@echo off
cd %~DP0

..\..\..\lib\ipy.exe ..\..\MSBuild.py Rebuild Debug x64 ..\..\..\MetaTweetSetup\MetaTweetSetup.wixproj 32
