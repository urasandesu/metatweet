﻿// -*- mode: csharp; encoding: utf-8; -*-
/* MetaTweet
 *   Hub system of Twitter-like communication service
 * MetaTweetServer
 *   Server library of MetaTweet
 *   Part of MetaTweet
 * Copyright © 2008 Takeshi KIRIYA, XSpect Project <takeshik@xspect.org>
 * All rights reserved.
 * 
 * This file is part of MetaTweetServer.
 * 
 * This library is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or (at your
 * option) any later version.
 * 
 * This library is distributed in the hope that it will be useful, but
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using XSpect.Reflection;
using System.Threading;
using XSpect.MetaTweet.Properties;

namespace XSpect.MetaTweet
{
    public sealed class ServerCore
        : Object,
          IDisposable
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(ServerCore));

        private readonly AssemblyManager _assemblyManager = new AssemblyManager();

        private readonly Dictionary<String, Listener> _listeners = new Dictionary<String, Listener>();

		private readonly Dictionary<String, Realm> _realms = new Dictionary<String, Realm>();

		public ILog Log
		{
			get
			{
				return this._log;
			}
		}

		public ServerCore()
		{
			this._log.InfoFormat(
				Resources.ServerInitialized,
				Assembly.GetExecutingAssembly().GetName().Version.ToString(),
				Environment.OSVersion.ToString(),
				Thread.CurrentThread.CurrentUICulture.ToString()
			);
		}

        public void Start(IDictionary<String, String> arguments)
        {
			this._log.Info(Resources.ServerStarting);
			this._log.Info(Resources.ServerStarted);
		}

        public void Stop()
        {
			this._log.Info(Resources.ServerStopping);
			this._log.Info(Resources.ServerStopped);
		}

        public void StopGracefully()
        {
			this._log.Info(Resources.ServerStopping);
			this._log.Info(Resources.ServerStopped);
		}

        public void Pause()
        {
			this._log.Info(Resources.ServerPausing);
			this._log.Info(Resources.ServerPaused);
		}

        public void PauseGracefully()
        {
			this._log.Info(Resources.ServerPausing);
			this._log.Info(Resources.ServerPaused);
		}

        public void Resume()
        {
			this._log.Info(Resources.ServerResuming);
			this._log.Info(Resources.ServerResumed);
		}

        public void Dispose()
        {
			this._log.Info(Resources.ServerTerminated);
        }

		public void AddListener(String id, Listener listener)
		{
			listener.Register(this, id);
			this._listeners.Add(id, listener);
			this._log.InfoFormat(
				Resources.ListenerAdded,
				id,
				listener.GetType().AssemblyQualifiedName,
				listener.GetType().Assembly.CodeBase
			);

		}

		public void RemoveListener(String id)
		{
			this._realms.Remove(id);
			this._log.InfoFormat(Resources.ListenerRemoved, id);
		}

		public void AddRealm(String id)
		{
			this._realms.Add(id, new Realm(this, id));
			this._log.InfoFormat(Resources.RealmAdded, id);
		}

		public void RemoveRealm(String id)
		{
			this._realms.Remove(id);
			this._log.InfoFormat(Resources.RealmAdded, id);
		}
    }
}