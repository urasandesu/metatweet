@echo off
cd %~DP0

mkdir ..\report
"C:\Program Files\Microsoft Visual Studio 9.0\Team Tools\Performance Tools\VSPerfCmd.exe" /U /START:SAMPLE /OUTPUT:"..\report\Sample_%DATE:/%DATE:/=- %TIME::=-%.vsp"
"C:\Program Files\Microsoft Visual Studio 9.0\Team Tools\Performance Tools\VSPerfCmd.exe" /ATTACH:MetaTweetHostService.exe
