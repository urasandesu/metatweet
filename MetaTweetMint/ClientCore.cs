// -*- mode: csharp; encoding: utf-8; tab-width: 4; c-basic-offset: 4; indent-tabs-mode: nil; -*-
// vim:set ft=cs fenc=utf-8 ts=4 sw=4 sts=4 et:
// $Id$
/* MetaTweet
 *   Hub system for micro-blog communication services
 * MetaTweetMint
 *   Extensible GUI client for MetaTweet
 *   Part of MetaTweet
 * Copyright © 2009-2010 Takeshi KIRIYA (aka takeshik) <takeshik@users.sf.net>
 * All rights reserved.
 * 
 * This file is part of MetaTweetMint.
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Achiral;
using Achiral.Extension;
using XSpect.Collections;
using XSpect.Configuration;
using XSpect.Extension;
using XSpect.MetaTweet.Clients.Mint.DataModel;
using XSpect.MetaTweet.Clients.Mint.Evaluating;
using XSpect.Reflection;

namespace XSpect.MetaTweet.Clients.Mint
{
    public sealed class ClientCore
        : MarshalByRefObject,
          IDisposable
    {
        private Boolean _disposed;

        /// <summary>
        /// クライアント オブジェクトの生成時に渡されたパラメータのリストを取得します。
        /// </summary>
        /// <value>
        /// クライアント オブジェクトの生成時に渡されたパラメータのリスト。
        /// </value>
        public IDictionary<String, String> Parameters
        {
            get;
            private set;
        }

        /// <summary>
        /// MetaTweet システム全体の設定を管理するオブジェクトを取得します。
        /// </summary>
        /// <value>
        /// MetaTweet システム全体の設定を管理するオブジェクト。
        /// </value>
        public XmlConfiguration GlobalConfiguration
        {
            get;
            private set;
        }

        public XmlConfiguration Configuration
        {
            get;
            private set;
        }

        /// <summary>
        /// MetaTweet システムの特別なディレクトリを取得するためのオブジェクトを取得します。
        /// </summary>
        /// <value>
        /// MetaTweet システムの特別なディレクトリを取得するためのオブジェクト。
        /// </value>
        public DirectoryStructure Directories
        {
            get;
            private set;
        }

        public FontConfiguration Fonts
        {
            get;
            private set;
        }

        /// <summary>
        /// アセンブリ マネージャを取得します。
        /// </summary>
        /// <value>アセンブリ マネージャ。</value>
        public CodeManager CodeManager
        {
            get;
            private set;
        }

        public HybridDictionary<String, ServerConnector> Connectors
        {
            get;
            private set;
        }

        public MainForm MainForm
        {
            get;
            private set;
        }

        public IDictionary<String, IEvaluatable> Functions
        {
            get;
            private set;
        }

        public KeyInputManager KeyInputManager
        {
            get;
            private set;
        }

        public IDictionary<String, Object> DefaultArgumentDictionary
        {
            get
            {
                return Create.Dictionary(
                    Make.Array("host", "args"),
                    new Object[]
                    {
                        this,
                        this.Parameters,
                    }
                );
            }
        }

        /// <summary>
        /// <see cref="ClientCore"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public ClientCore()
        {
            this.Connectors = new HybridDictionary<String, ServerConnector>((i, c) => c.Name);
            this.Functions = new Dictionary<String, IEvaluatable>();
            this.KeyInputManager = new KeyInputManager(this);
        }

        /// <summary>
        /// 対象のインスタンスの有効期間ポリシーを制御する、有効期間サービス オブジェクトを取得します。
        /// </summary>
        /// <returns>
        /// 対象のインスタンスの有効期間ポリシーを制御するときに使用する、<see cref="T:System.Runtime.Remoting.Lifetime.ILease"/> 型のオブジェクト。存在する場合は、このインスタンスの現在の有効期間サービス オブジェクトです。それ以外の場合は、<see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"/> プロパティの値に初期化された新しい有効期間サービス オブジェクトです。
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">直前の呼び出し元に、インフラストラクチャ アクセス許可がありません。</exception>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure"/>
        /// </PermissionSet>
        public override Object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// <see cref="ClientCore"/> によって使用されているすべてのリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <see cref="ClientCore"/> によって使用されているアンマネージ リソースを解放し、オプションでマネージ リソースも解放します。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 <c>true</c>、破棄されない場合は <c>false</c>。</param>
        private void Dispose(Boolean disposing)
        {
            this._disposed = true;
        }

        /// <summary>
        /// オブジェクトが破棄されているかどうかを確認し、破棄されている場合は例外を送出します。
        /// </summary>
        private void CheckIfDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        /// <summary>
        /// クライアント オブジェクトを初期化します。
        /// </summary>
        /// <param name="arguments">サーバ ホストからサーバ オブジェクトに渡す引数のリスト。</param>
        public void Initialize(IDictionary<String, String> arguments)
        {
            this.CheckIfDisposed();
            this.Parameters = arguments;
            if (this.Parameters.Contains(Create.KeyValuePair("debug", "true")))
            {
                Debugger.Launch();
            }

            this.GlobalConfiguration = XmlConfiguration.Load(this.Parameters["config"]);
            this.Directories = new DirectoryStructure(this.GlobalConfiguration.ResolveChild("directories"));

            this.Configuration = XmlConfiguration.Load(
                this.Directories.ConfigDirectory.CreateSubdirectory("Mint").File("MetaTweetMint.conf.xml")
            );
            this.Fonts = new FontConfiguration(this.Configuration.ResolveChild("fonts"));
            this.CodeManager = new CodeManager(this.Directories.ConfigDirectory.Directory("Mint").File("CodeManager.conf.xml"));
        }

        public void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            this.MainForm = new MainForm(this);
            new SplashForm(this).Show();
            Application.Run();
        }

        public Object EvaluateFunction(String name, IDictionary<String, String> args)
        {
            return this.EvaluateFunction(name, args, true);
        }

        public Object EvaluateFunction(String name, IDictionary<String, String> args, Boolean throwIfUndefined)
        {
            if (this.Functions.ContainsKey(name))
            {
                return this.Functions[name].Evaluate(this, args ?? new Dictionary<String, String>());
            }
            else if (throwIfUndefined &&
                // There are no intentions to evaluate any function; why this throw an exception?
                !name.IsNullOrEmpty()
            )
            {
                throw new KeyNotFoundException("Function is not defined.");
            }
            else
            {
                return null;
            }
        }
    }
}
