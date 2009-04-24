﻿// -*- mode: csharp; encoding: utf-8; -*-
// $Id$
/* MetaTweet
 *   Hub system for micro-blog communication services
 * MetaTweetObjectModel
 *   Object model and Storage interface for MetaTweet and other systems
 *   Part of MetaTweet
 * Copyright © 2008-2009 Takeshi KIRIYA, XSpect Project <takeshik@users.sf.net>
 * All rights reserved.
 * 
 * This file is part of MetaTweetObjectModel.
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
using System.Collections;
using System.Collections.Generic;

namespace XSpect.MetaTweet.ObjectModel
{
    partial class FollowElement
    {
        /// <summary>
        /// <see cref="FollowElement"/> のデータのバックエンドとなるデータ行の主キーのシーケンスを表します。このクラスは継承できません。
        /// </summary>
        [Serializable()]
        public sealed class PrimaryKeyCollection
            : Object,
              IEnumerable<Object>,
              IComparable<PrimaryKeyCollection>
        {
            private readonly FollowElement _element;

            /// <summary>
            /// <see cref="StorageDataSet.FollowMapRow.AccountId"/> の値を取得または設定します。
            /// </summary>
            /// <remarks>このプロパティは <see cref="FollowElement.Account"/> の <see cref="ObjectModel.Account.AccountId"/> に対応します。</remarks>
            public Guid AccountId
            {
                get
                {
                    return this._element.UnderlyingDataRow.AccountId;
                }
                set
                {
                    this._element.UnderlyingDataRow.AccountId = value;
                }
            }

            /// <summary>
            /// <see cref="StorageDataSet.FollowMapRow.FollowingAccountId"/> の値を取得または設定します。
            /// </summary>
            /// <remarks>このプロパティは <see cref="FollowElement.FollowingAccount"/> の <see cref="ObjectModel.Account.AccountId"/> に対応します。</remarks>
            public Guid FollowingAccountId
            {
                get
                {
                    return this._element.UnderlyingDataRow.FollowingAccountId;
                }
                set
                {
                    this._element.UnderlyingDataRow.FollowingAccountId = value;
                }
            }

            /// <summary>
            /// <see cref="PrimaryKeyCollection"/> の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="element">参照する <see cref="FollowElement"/>。</param>
            public PrimaryKeyCollection(FollowElement element)
            {
                this._element = element;
            }

            /// <summary>
            /// <see cref="PrimaryKeyCollection"/> を反復処理する列挙子を返します。 
            /// </summary>
            /// <returns>コレクションを反復処理するために使用できる <see cref="IEnumerable{Object}"/>。</returns>
            public IEnumerator<Object> GetEnumerator()
            {
                yield return this.AccountId;
                yield return this.FollowingAccountId;
            }

            /// <summary>
            /// <see cref="PrimaryKeyCollection"/> を反復処理する列挙子を返します。 
            /// </summary>
            /// <returns>コレクションを反復処理するために使用できる <see cref="IEnumerable"/>。</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            /// <summary>
            /// この主キーのシーケンスを別の主キーのシーケンスと比較します。
            /// </summary>
            /// <param name="other">この主キーのシーケンスと比較する主キーのシーケンス。</param>
            /// <returns>
            /// 比較対象ポストの相対順序を示す 32 ビット符号付き整数。戻り値の意味は次のとおりです。<br/>
            /// 値<br/>
            /// 意味<br/>
            /// 0 より小さい値<br/>
            /// この主キーのシーケンスが <paramref name="other"/> パラメータより前に序列されるべきであることを意味します。<br/>
            /// 0<br/>
            /// この主キーのシーケンスが <paramref name="other"/> と等しいことを意味します。<br/>
            /// 0 より大きい値<br/>
            /// この主キーのシーケンスが <paramref name="other"/> パラメータより後に序列されるべきであることを意味します。<br/>
            /// </returns>
            public Int32 CompareTo(PrimaryKeyCollection other)
            {
                Int32 ret;
                if ((ret = this.AccountId.CompareTo(other.AccountId)) != 0)
                {
                    return ret;
                }
                else
                {
                    return this.FollowingAccountId.CompareTo(other.FollowingAccountId);
                }
            }
        }
    }
}