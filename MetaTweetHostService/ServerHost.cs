﻿// -*- mode: csharp; encoding: utf-8; -*-
/* MetaTweet
 *   Hub system for micro-blog communication services
 * MetaTweetHostService
 *   Windows Service which hosts MetaTweetServer
 *   Part of MetaTweet
 * Copyright © 2008-2009 Takeshi KIRIYA, XSpect Project <takeshik@users.sf.net>
 * All rights reserved.
 * 
 * This file is part of MetaTweetServer.
 * 
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or (at your
 * option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
 * License for more details. 
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>,
 * or write to the Free Software Foundation, Inc., 51 Franklin Street,
 * Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Achiral.Extension;
using System.IO;
using System.Reflection;
using Achiral;

namespace XSpect.MetaTweet
{
    public partial class ServerHost
        : ServiceBase
    {
        public const String ServerDllName = "MetaTweetServer.dll";

        private readonly DirectoryInfo _rootDirectory;

        private AppDomain _serverDomain;

        private readonly Dictionary<String, String> _arguments;

        private Object _serverObject;

        public ServerHost()
        {
            this._rootDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
            this._arguments = new Dictionary<String, String>();
            this.InitializeComponent();
        }

        protected override void OnContinue()
        {
            this.StartServer();
        }

        protected override void OnPause()
        {
            this.StopServer();
        }

        protected override void OnStart(String[] args)
        {
            Environment.CurrentDirectory = this._rootDirectory.FullName;
            args
                .Select(s => "(-(?<key>[a-zA-Z0-9_]*)(=(?<value>(\"[^\"]*\")|('[^']*')|(.*)))?)*".RegexMatch(s))
                .Where(m => m.Success)
                .ForEach(m => this._arguments.Add(
                    m.Groups["key"].Value,
                    m.Groups["value"].Success ? m.Groups["value"].Value : "true"
                ));
            this.StartServer();
        }

        protected override void OnStop()
        {
            this.StopServer();
        }

        private TDelegate GetMethod<TDelegate>(String name)
            where TDelegate : class
        {
            return this._serverObject
                .GetType()
                .GetMethod(name, BindingFlags.Public | BindingFlags.Instance)
                .CreateDelegate<TDelegate, Object>(this._serverObject) as TDelegate;
        }

        private void StartServer()
        {
            this._serverDomain = AppDomain.CreateDomain(
                ServerDllName,
                AppDomain.CurrentDomain.Evidence,
                new AppDomainSetup()
                {
                    ApplicationName = "HostService",
                    LoaderOptimization = LoaderOptimization.MultiDomainHost,
                }
            );

            this._serverDomain.DoCallBack(() =>
            {
                this._serverObject = Assembly.LoadFrom(
                    this._rootDirectory.GetFiles(ServerDllName).Single().FullName
                ).CreateInstance("XSpect.MetaTweet.ServerCore");
            });
            this.GetMethod<Action<Dictionary<String, String>>>("Initialize")(this._arguments);
            this.GetMethod<Action>("Start")();
        }

        private void StopServer()
        {
            this.GetMethod<Action>("Stop")();
            this._serverObject = null;
            AppDomain.Unload(this._serverDomain);
        }

        private void StopServerGracefully()
        {
            this.GetMethod<Action>("StopGracefully")();
            this._serverObject = null;
            AppDomain.Unload(this._serverDomain);
        }
    }
}
