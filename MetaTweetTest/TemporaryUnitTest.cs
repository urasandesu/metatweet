﻿// -*- mode: csharp; encoding: utf-8; -*-
/* MetaTweet
 *   Hub system for micro-blog communication services
 * MetaTweetTest
 *   Test codes for MetaTweet
 *   Part of MetaTweet
 * Copyright © 2008-2009 Takeshi KIRIYA, XSpect Project <takeshik@xspect.org>
 * All rights reserved.
 * 
 * This file is part of MetaTweetTest.
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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XSpect.MetaTweet.ObjectModel;
using System.Xml;
using System.IO;

namespace XSpect.MetaTweet.Test
{
    /// <summary>
    /// UnitTest の概要の説明
    /// </summary>
    [TestClass]
    public class TemporaryUnitTest
    {
        public TemporaryUnitTest()
        {
            //
            // TODO: コンストラクタ ロジックをここに追加します
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 追加のテスト属性
        //
        // テストを作成する際には、次の追加属性を使用できます:
        //
        // クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 各テストを実行する前に、TestInitialize を使用してコードを実行してください
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 各テストを実行した後に、TestCleanup を使用してコードを実行してください
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ServerCoreTest()
        {
            var c = new ServerCore();
            c.LoadAssembly("sqlite", "SQLiteStorage.dll");
            c.LoadAssembly("sqlite", "SQLiteStorage.dll");
            c.LoadModule("sqlite", "XSpect.MetaTweet.SQLiteStorage");
            c.LoadAssembly("remoting", "RemotingServant.dll");
            c.LoadModule("remoting", "XSpect.MetaTweet.RemotingServant");
            var i = new TwitterApiInput();
            using (StreamReader reader = new StreamReader(@"C:\mtw-credential"))
            {
                i.Initialize(new Dictionary<String, String>()
                {
                    {"username", reader.ReadLine()},
                    {"password", reader.ReadLine()},
                });
            }
            var xres = i.InvokeRest(new System.Uri("http://twitter.com/statuses/friends_timeline.xml?id=takeshik"), "POST");
            i.Host = c;
            i.Name = "twitterclient";
        }

        [TestMethod]
        public void TwitterApiInputTest()
        {
            var c = new ServerCore();
            var s = new SQLiteStorage();
            s.Host = c;
            s.Name = "sqlite";
            s.Initialize(@"data source=C:\MetaTweet.db;binaryguid=False");
            s.DropTables();
            s.CreateTables();
            s.Connect();
            var i = new TwitterApiInput();
            i.Host = c;
            i.Name = "twitter";
            i.Realm = "com.twitter";
            using (StreamReader reader = new StreamReader(@"C:\mtw-credential"))
            {
                i.Initialize(new Dictionary<String, String>()
                {
                    {"username", reader.ReadLine()},
                    {"password", reader.ReadLine()},
                });
            }
            var r = i.FetchFriendsTimeline("", s, new Dictionary<String, String>()
            {
                {"count", "100"},
            }).ToList();
        }

        [TestMethod]
        public void RunMain()
        {
            Program.Main(new String[0]);
        }
    }
}
　