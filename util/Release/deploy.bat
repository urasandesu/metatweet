@echo off

mkdir ..\..\bin\Release\
mkdir ..\..\bin\Release\module
mkdir ..\..\bin\Release\cache
mkdir ..\..\bin\Release\conf
cd ..\..\bin\Release
copy /Y ..\..\lib\Achiral.dll
copy /Y ..\..\lib\log4net.dll
copy /Y ..\..\lib\System.Data.SQLite.dll
copy /Y ..\..\lib\TidyNet.dll
copy /Y ..\..\MetaTweetClient\bin\Release\MetaTweetClient.exe
copy /Y ..\..\MetaTweetHostService\bin\Release\MetaTweetHostService.exe
copy /Y ..\..\MetaTweetObjectModel\bin\Release\MetaTweetObjectModel.dll
copy /Y ..\..\MetaTweetServer\bin\Release\MetaTweetServer.dll
copy /Y ..\..\MetaTweetTest\Sample\init.cs
copy /Y ..\..\MetaTweetTest\Sample\rc.cs
copy /Y ..\..\XSpectCommonFramework\bin\Release\XSpectCommonFramework.dll
cd module
copy /Y ..\..\..\LocalServant\bin\Release\LocalServant.dll
copy /Y ..\..\..\RemotingServant\bin\Release\RemotingServant.dll
copy /Y ..\..\..\SQLiteStorage\bin\Release\SQLiteStorage.dll
copy /Y ..\..\..\StorageFlow\bin\Release\StorageFlow.dll
copy /Y ..\..\..\TwitterApiFlow\bin\Release\TwitterApiFlow.dll
cd ..\conf
copy /Y ..\..\..\MetaTweetTest\Sample\RemotingServant-remoting.conf.xml
copy /Y ..\..\..\MetaTweetTest\Sample\SQLiteStorage-main.conf.xml
copy /Y ..\..\..\MetaTweetTest\Sample\TwitterApiInput-twitter.conf.xml
copy /Y ..\..\..\MetaTweetTest\Sample\TwitterApiOutput-twitter.conf.xml
