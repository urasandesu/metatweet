###
### Deploy.py
###
### Using: lib/ipy util/Deploy.py <Directory> <Configuration | "Clean">
###

import clr
import sys
from System import *
from System.IO import *

srcDir = DirectoryInfo(sys.argv[1])
dstDir = srcDir.CreateSubdirectory("dist")
config = sys.argv[2]

def distrib(path):
    path = path.Replace("<CONFIG>", config)
    distribAs(path, Path.GetFileName(path))

def distribIf(cond, path):
    if config.Contains(cond):
        distrib(path)

def distribAs(path, name):
    path = path.Replace("<CONFIG>", config)
    srcDir.GetFiles(path)[0].CopyTo(Path.Combine(dstDir.FullName, name), True)
    print "Deployed: " + "dist/" + config + "/" + path.Replace("\\", "/")

def distribIfAs(cond, path, name):
    if config.Contains(cond):
        distribAs(path, name)

def distribDir(path):
    path = path.Replace("<CONFIG>", config)
    for f in srcDir.GetDirectories(path)[0].GetFiles():
        distrib(f.FullName.Substring(srcDir.FullName.Length))

dstBase = dstDir.CreateSubdirectory(config)

if config == "Clean":
    dstBase.Delete(True)
    print "Deleted: dist/" + config + "/"
    sys.exit()

dstDir = dstBase
distrib("COPYING")
distrib("COPYING.ja")
distrib("lib/NOTICE")
distribAs("resource/manual/README.dist", "README")
distribAs("resource/manual/README.dist.ja", "README.ja")

dstDir = dstBase.CreateSubdirectory("bin")
distrib("MetaTweetMint/bin/<CONFIG>/MetaTweetMint.exe")
distribIf("Debug", "MetaTweetMint/bin/<CONFIG>/MetaTweetMint.pdb")
distribAs("MetaTweetMint/Properties/App.config", "MetaTweetMint.exe.config")

dstDir = dstBase.CreateSubdirectory("sbin")
distrib("MetaTweetHostService/bin/<CONFIG>/MetaTweetHostService.exe")
distribIf("Debug", "MetaTweetHostService/bin/<CONFIG>/MetaTweetHostService.pdb")
distribAs("MetaTweetHostService/Properties/App.config", "MetaTweetHostService.exe.config")

dstDir = dstBase.CreateSubdirectory("etc")
if dstDir.GetFiles("suppress_deploy").Length == 0:
    distrib("resource/configuration/MetaTweet.conf.xml")
    distrib("resource/configuration/MetaTweet.conf.default")
    distrib("resource/configuration/MetaTweetServer.conf.xml")
    distrib("resource/configuration/MetaTweetServer.conf.default")
    distrib("resource/configuration/ModuleManager.conf.xml")
    distrib("resource/configuration/ModuleManager.conf.default")
    distrib("resource/configuration/ModuleManager.init.conf.xml")
    distrib("resource/configuration/ModuleManager.init.conf.default")
    distrib("resource/configuration/log4net.config")

    dstDir = dstBase.CreateSubdirectory("etc/Mint")
    distrib("resource/configuration/Mint/CodeManager.conf.xml")
    distrib("resource/configuration/Mint/CodeManager.conf.default")
    distrib("resource/configuration/Mint/MetaTweetMint.conf.xml")
    distrib("resource/configuration/Mint/MetaTweetMint.conf.default")

    dstDir = dstBase.CreateSubdirectory("etc/modules.d")
    distrib("resource/configuration/LocalServant-local.conf.xml")
    distrib("resource/configuration/LocalServant-local.conf.default")
    distrib("resource/configuration/RemotingTcpServant-remoting.conf.xml")
    distrib("resource/configuration/RemotingTcpServant-remoting.conf.default")
    distrib("resource/configuration/RemotingIpcServant-remoting.conf.xml")
    distrib("resource/configuration/RemotingIpcServant-remoting.conf.default")
    distrib("resource/configuration/SQLiteStorage-main.conf.xml")
    distrib("resource/configuration/SQLiteStorage-main.conf.default")
    distrib("resource/configuration/SystemInput-sys.conf.xml")
    distrib("resource/configuration/SystemInput-sys.conf.default")
    distrib("resource/configuration/SystemFilter-sys.conf.xml")
    distrib("resource/configuration/SystemFilter-sys.conf.default")
    distrib("resource/configuration/SystemOutput-sys.conf.xml")
    distrib("resource/configuration/SystemOutput-sys.conf.default")
    distrib("resource/configuration/TwitterApiInput-twitter.conf.xml")
    distrib("resource/configuration/TwitterApiInput-twitter.conf.default")
    distrib("resource/configuration/TwitterApiOutput-twitter.conf.xml")
    distrib("resource/configuration/TwitterApiOutput-twitter.conf.default")
    distrib("resource/configuration/TwitterWebInput-twitter_w.conf.xml")
    distrib("resource/configuration/TwitterWebInput-twitter_w.conf.default")
    distrib("resource/configuration/TwitterWebInput-twitter_w.scrapingKeys.conf.xml")
    distrib("resource/configuration/TwitterWebInput-twitter_w.scrapingKeys.conf.default")

dstDir = dstBase.CreateSubdirectory("ja")
distrib("XSpectWindowsFormsSupplement/bin/<CONFIG>/ja/XSpectWindowsFormsSupplement.resources.dll")

dstDir = dstBase.CreateSubdirectory("lib")
distrib("lib/AchiralPlus.dll")
distrib("lib/Azuki.dll")
distrib("lib/IronPython.dll")
distrib("lib/IronPython.Modules.dll")
distrib("lib/Microsoft.Scripting.dll")
distrib("lib/Microsoft.Scripting.Core.dll")
distrib("lib/log4net.dll")
distrib("lib/WeifenLuo.WinFormsUI.Docking.dll")
distrib("MetaTweetObjectModel/bin/<CONFIG>/MetaTweetObjectModel.dll")
distribIf("Debug", "MetaTweetObjectModel/bin/<CONFIG>/MetaTweetObjectModel.pdb")
distrib("MetaTweetServer/bin/<CONFIG>/MetaTweetServer.dll")
distribIf("Debug", "MetaTweetServer/bin/<CONFIG>/MetaTweetServer.pdb")
distrib("Linx/Linx/bin/<CONFIG>/Linx.dll")
distribIf("Debug", "Linx/Linx/bin/<CONFIG>/Linx.pdb")
distrib("XSpectCommonFramework/bin/<CONFIG>/XSpectCommonFramework.dll")
distribIf("Debug", "XSpectCommonFramework/bin/<CONFIG>/XSpectCommonFramework.pdb")
distrib("XSpectWindowsFormsSupplement/bin/<CONFIG>/XSpectWindowsFormsSupplement.dll")
distribIf("Debug", "XSpectWindowsFormsSupplement/bin/<CONFIG>/XSpectWindowsFormsSupplement.pdb")

dstDir = dstBase.CreateSubdirectory("libexec/LocalServant")
distrib("LocalServant/bin/<CONFIG>/LocalServant.dll")
distribIf("Debug", "LocalServant/bin/Debug/LocalServant.pdb")

dstDir = dstBase.CreateSubdirectory("libexec/RemotingServant")
distrib("RemotingServant/bin/<CONFIG>/RemotingServant.dll")
distribIf("Debug", "RemotingServant/bin/Debug/RemotingServant.pdb")

dstDir = dstBase.CreateSubdirectory("libexec/SQLiteStorage")
distrib("SQLiteStorage/bin/<CONFIG>/SQLiteStorage.dll")
distribIf("Debug", "SQLiteStorage/bin/Debug/SQLiteStorage.pdb")
distrib("lib/System.Data.SQLite.dll")

dstDir = dstBase.CreateSubdirectory("libexec/SystemFlow")
distrib("SystemFlow/bin/<CONFIG>/SystemFlow.dll")
distribIf("Debug", "SystemFlow/bin/Debug/SystemFlow.pdb")

dstDir = dstBase.CreateSubdirectory("libexec/TwitterApiFlow")
distrib("TwitterApiFlow/bin/<CONFIG>/TwitterApiFlow.dll")
distribIf("Debug", "TwitterApiFlow/bin/Debug/TwitterApiFlow.pdb")
distrib("lib/DotNetOpenAuth.dll")
distribIf("Debug", "lib/DotNetOpenAuth.pdb")
distrib("lib/Kerr.Credentials.dll")
distrib("lib/LinqToTwitter.dll")
distribIf("Debug", "lib/LinqToTwitter.pdb")
distrib("TwitterApiFlow/bin/<CONFIG>/TwitterApiFlow.dll")
distrib("TwitterApiFlow/bin/<CONFIG>/TwitterApiFlow.dll")
distrib("TwitterApiFlow/bin/<CONFIG>/TwitterApiFlow.dll")

dstDir = dstBase.CreateSubdirectory("libexec/TwitterWebFlow")
distrib("TwitterWebFlow/bin/<CONFIG>/TwitterWebFlow.dll")
distribIf("Debug", "TwitterWebFlow/bin/Debug/TwitterWebFlow.pdb")
distrib("lib/TidyNet.dll")

dstDir = dstBase.CreateSubdirectory("share")
distrib("resource/COPYING")

dstDir = dstBase.CreateSubdirectory("share/doc")
distrib("resource/reference/MetaTweetLibs.chm")
distrib("resource/specification/MetaTweetObjectModel.png")
distrib("resource/specification/MetaTweetServer.png")

dstDir = dstBase.CreateSubdirectory("share/misc")
distribDir("resource/logo")

dstDir = dstBase.CreateSubdirectory("share/util")
distribDir("util/<CONFIG>-Deployed")