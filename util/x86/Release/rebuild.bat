@echo off
cd %~DP0

%WINDIR%\Microsoft.NET\Framework\v3.5\MSBuild.exe /target:Rebuild /property:Configuration=Release;Platform=x86 /consoleloggerparameters:NoSummary;ShowTimestamp /nodeReuse:False ..\..\..\MetaTweet.sln || (
    echo Press ENTER key to exit.
    pause > nul
)