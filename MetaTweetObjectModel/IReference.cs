// -*- mode: csharp; encoding: utf-8; tab-width: 4; c-basic-offset: 4; indent-tabs-mode: nil; -*-
// vim:set ft=cs fenc=utf-8 ts=4 sw=4 sts=4 et:
// $Id$
/* MetaTweet
 *   Hub system for micro-blog communication services
 * MetaTweetObjectModel
 *   Object model and Storage interface for MetaTweet and other systems
 *   Part of MetaTweet
 * Copyright © 2008-2010 Takeshi KIRIYA (aka takeshik) <takeshik@users.sf.net>
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

namespace XSpect.MetaTweet.Objects
{
    /// <summary>
    /// エンティティ モデルに依存しないリファレンスの基本実装を表します。
    /// </summary>
    public interface IReference
        : IComparable<IReference>,
          IEquatable<IReference>
    {
        /// <summary>
        /// このリファレンスが関連付けられているアクティビティのアカウント ID を取得または設定します。
        /// </summary>
        /// <value>
        /// このリファレンスが関連付けられているアクティビティのアカウント ID。
        /// </value>
        String AccountId
        {
            get;
            set;
        }

        /// <summary>
        /// このリファレンスが関連付けられているアクティビティのタイムスタンプを取得または設定します。
        /// </summary>
        /// <value>
        /// このリファレンスが関連付けられているアクティビティのタイムスタンプ。
        /// </value>
        DateTime Timestamp
        {
            get;
            set;
        }

        /// <summary>
        /// このリファレンスが関連付けられているアクティビティのカテゴリを取得または設定します。
        /// </summary>
        /// <value>
        /// このリファレンスが関連付けられているアクティビティのカテゴリ。
        /// </value>
        String Category
        {
            get;
            set;
        }

        /// <summary>
        /// このリファレンスが関連付けられているアクティビティのサブ ID を取得または設定します。
        /// </summary>
        /// <value>
        /// このリファレンスが関連付けられているアクティビティのサブ ID。
        /// </value>
        String SubId
        {
            get;
            set;
        }

        /// <summary>
        /// このリファレンスの意味となる文字列を取得または設定します。
        /// </summary>
        /// <value>
        /// このリファレンスの意味となる文字列。
        /// </value>
        String Name
        {
            get;
            set;
        }

        /// <summary>
        /// このリファレンスが関連付けられる先のアクティビティのアカウント ID を取得または設定します。
        /// </summary>
        /// <value>
        /// このリファレンスが関連付けられる先のアクティビティのアカウント ID。
        /// </value>
        String ReferringAccountId
        {
            get;
            set;
        }

        /// <summary>
        /// このリファレンスが関連付けられる先のアクティビティのタイムスタンプを取得または設定します。
        /// </summary>
        /// <value>
        /// このリファレンスが関連付けられる先のアクティビティのタイムスタンプ。
        /// </value>
        DateTime ReferringTimestamp
        {
            get;
            set;
        }

        /// <summary>
        /// このリファレンスが関連付けられる先のアクティビティのカテゴリを取得または設定します。
        /// </summary>
        /// <value>
        /// このリファレンスが関連付けられる先のアクティビティのカテゴリ。
        /// </value>
        String ReferringCategory
        {
            get;
            set;
        }

        /// <summary>
        /// このリファレンスが関連付けられる先のアクティビティのサブ ID を取得または設定します。
        /// </summary>
        /// <value>
        /// このリファレンスが関連付けられる先のアクティビティのサブ ID。
        /// </value>
        String ReferringSubId
        {
            get;
            set;
        }

        /// <summary>
        /// このリファレンスが関連付けられているアクティビティを取得または設定します。
        /// </summary>
        /// <value>
        /// このリファレンスが関連付けられているアクティビティ。
        /// </value>
        IActivity Activity
        {
            get;
            set;
        }

        /// <summary>
        /// このリファレンスが関連付けられる先のアクティビティを取得または設定します。
        /// </summary>
        /// <value>
        /// このリファレンスが関連付けられる先のアクティビティ。
        /// </value>
        IActivity ReferringActivity
        {
            get;
            set;
        }

        /// <summary>
        /// 指定したリファレンスが、このリファレンスと完全に等しいかどうかを判断します。
        /// </summary>
        /// <param name="other">このリファレンスと比較するリファレンス。</param>
        /// <returns>指定したリファレンスがこのリファレンスと完全に等しい場合は <c>true</c>。それ以外の場合は <c>false</c>。</returns>
        Boolean EqualsExact(IReference other);
    }
}