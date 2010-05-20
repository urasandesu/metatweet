// -*- mode: csharp; encoding: utf-8; tab-width: 4; c-basic-offset: 4; indent-tabs-mode: nil; -*-
// vim:set ft=cs fenc=utf-8 ts=4 sw=4 sts=4 et:
// $Id$
/* MetaTweet
 *   Hub system for micro-blog communication services
 * MetaTweetServer
 *   Server library of MetaTweet
 *   Part of MetaTweet
 * Copyright © 2008-2010 Takeshi KIRIYA (aka takeshik) <takeshik@users.sf.net>
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
using XSpect;
using XSpect.Extension;
using XSpect.Hooking;
using XSpect.MetaTweet.Objects;

namespace XSpect.MetaTweet.Modules
{
    /// <summary>
    /// フィルタ フロー モジュールの抽象基本クラスを提供します。
    /// </summary>
    /// <remarks>
    /// フィルタ フロー モジュールとは、ストレージ オブジェクトを入力とし、何らかの変換処理を適用しストレージ オブジェクトとして出力する、パイプラインの中途に位置するフロー モジュールを指します。
    /// </remarks>
    public abstract class FilterFlowModule
        : FlowModule
    {
        /// <summary>
        /// <see cref="Filter"/> のフック リストを取得します。
        /// </summary>
        /// <value>
        /// <see cref="Filter"/> のフック リスト。
        /// </value>
        public FuncHook<FilterFlowModule, String, Object, StorageModule, IDictionary<String, String>, Object> FilterHook
        {
            get;
            private set;
        }

        /// <summary>
        /// <see cref="FilterFlowModule"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        protected FilterFlowModule()
        {
            this.FilterHook = new FuncHook<FilterFlowModule, String, Object, StorageModule, IDictionary<String, String>, Object>(this._Filter);
        }

        /// <summary>
        /// フィルタ処理を行います。
        /// </summary>
        /// <param name="selector">モジュールに対し照合のために提示するセレクタ文字列。</param>
        /// <param name="input">フィルタ処理の入力として与えるストレージ オブジェクトのシーケンス。</param>
        /// <param name="storage">ストレージ オブジェクトの入出力先として使用するストレージ。</param>
        /// <param name="arguments">フィルタ処理の引数のリスト。</param>
        /// <returns>フィルタ処理の結果となる出力のシーケンス。</returns>
        public Object Filter(String selector, Object input, StorageModule storage, IDictionary<String, String> arguments)
        {
            this.CheckIfDisposed();
            return this.FilterHook.Execute(selector, input, storage, arguments);
        }

        private Object _Filter(String selector, Object input, StorageModule storage, IDictionary<String, String> arguments)
        {
            String param;
            return this.GetFlowInterface(selector, input.GetType(), null, out param).Invoke<IEnumerable<StorageObject>>(
                this,
                input,
                storage,
                param,
                arguments
            );
        }

        /// <summary>
        /// 非同期のフィルタ処理を開始します。
        /// </summary>
        /// <param name="selector">モジュールに対し照合のために提示するセレクタ文字列。</param>
        /// <param name="input">フィルタ処理の入力として与えるストレージ オブジェクトのシーケンス。</param>
        /// <param name="storage">ストレージ オブジェクトの入出力先として使用するストレージ。</param>
        /// <param name="arguments">フィルタ処理の引数のリスト。</param>
        /// <param name="callback">フィルタ処理完了時に呼び出されるオプションの非同期コールバック。</param>
        /// <param name="state">この特定の非同期フィルタ処理要求を他の要求と区別するために使用するユーザー指定のオブジェクト。</param>
        /// <returns>非同期のフィルタ処理を表す <see cref="System.IAsyncResult"/>。まだ保留状態の場合もあります。</returns>
        public IAsyncResult BeginFilter(
            String selector,
            Object input,
            StorageModule storage,
            IDictionary<String, String> arguments,
            AsyncCallback callback,
            Object state
        )
        {
            return new Func<String, Object, StorageModule, IDictionary<String, String>, Object>(this.Filter).BeginInvoke(
                selector,
                input,
                storage,
                arguments,
                callback,
                state
            );
        }

        /// <summary>
        /// 保留中の非同期フィルタ処理が完了するまで待機します。
        /// </summary>
        /// <param name="asyncResult">終了させる保留状態の非同期リクエストへの参照。</param>
        /// <returns>フィルタ処理の結果となる出力のシーケンス。</returns>
        public Object EndFilter(IAsyncResult asyncResult)
        {
            return asyncResult.GetAsyncDelegate<Func<String, Object, IDictionary<String, String>, Object>>()
                .EndInvoke(asyncResult);
        }
    }
}