﻿// -*- mode: csharp; encoding: utf-8; tab-width: 4; c-basic-offset: 4; indent-tabs-mode: nil; -*-
// vim:set ft=cs fenc=utf-8 ts=4 sw=4 sts=4 et:
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
using System.Collections.Generic;

namespace XSpect.MetaTweet.Objects
{
    /// <summary>
    /// エンティティ モデルに依存しないアクティビティの基本実装を表します。
    /// </summary>
    public interface IActivity
        : IComparable<IActivity>
    {
        /// <summary>
        /// このアクティビティを行ったアカウントの ID を取得または設定します。
        /// </summary>
        /// <value>
        /// このアクティビティを行ったアカウントの ID。
        /// </value>
        Guid AccountId
        {
            get;
            set;
        }

        /// <summary>
        /// このアクティビティが行われた時刻を取得または設定します。
        /// </summary>
        /// <value>
        /// このアクティビティが行われた時刻。
        /// </value>
        DateTime Timestamp
        {
            get;
            set;
        }

        /// <summary>
        /// このアクティビティが表す行動の種類を表す文字列を取得または設定します。
        /// </summary>
        /// <value>
        /// このアクティビティが表す行動の種類を表す文字列。
        /// </value>
        String Category
        {
            get;
            set;
        }

        /// <summary>
        /// このアクティビティを、同一アカウント・時刻・カテゴリの他のアクティビティと識別するための文字列、または、このアクティビティを一意に識別する文字列を取得または設定します。
        /// </summary>
        /// <value>
        /// このアクティビティを、同一アカウント・時刻・カテゴリの他のアクティビティと識別するための文字列、または、このアクティビティを一意に識別する文字列。
        /// </value>
        String SubId
        {
            get;
            set;
        }

        /// <summary>
        /// このアクティビティを行ったユーザ エージェントを識別する文字列を取得または設定します。
        /// </summary>
        /// <value>
        /// このアクティビティを行ったユーザ エージェントを識別する文字列。
        /// </value>
        String UserAgent
        {
            get;
            set;
        }

        /// <summary>
        /// このアクティビティの値となる文字列を取得または設定します。
        /// </summary>
        /// <value>
        /// このアクティビティの値となる文字列。
        /// </value>
        String Value
        {
            get;
            set;
        }

        /// <summary>
        /// このアクティビティの値となるバイト列を取得または設定します。
        /// </summary>
        /// <value>
        /// このアクティビティの値となるバイト列。
        /// </value>
        Byte[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// このアクティビティを行ったアカウントを取得または設定します。
        /// </summary>
        /// <value>
        /// このアクティビティを行ったアカウント。
        /// </value>
        Account Account
        {
            get;
            set;
        }

        /// <summary>
        /// このアクティビティに関連付けられたタグのシーケンスを取得します。
        /// </summary>
        /// <value>
        /// このアクティビティに関連付けられたタグのシーケンス。
        /// </value>
        IEnumerable<Tag> Tags
        {
            get;
        }

        /// <summary>
        /// このアクティビティに関連付けられたタグの意味となる文字列のシーケンスを取得します。
        /// </summary>
        /// <value>
        /// このアクティビティに関連付けられたタグの意味となる文字列のシーケンス。
        /// </value>
        IEnumerable<String> Tagging
        {
            get;
        }

        /// <summary>
        /// このアクティビティに関連付けられたリファレンスのシーケンスを取得します。
        /// </summary>
        /// <value>
        /// このアクティビティに関連付けられたリファレンスのシーケンス。
        /// </value>
        IEnumerable<Reference> References
        {
            get;
        }

        /// <summary>
        /// このアクティビティが対象として関連付けられたリファレンスのシーケンスを取得します。
        /// </summary>
        /// <value>
        /// このアクティビティが対象として関連付けられたリファレンスのシーケンス。
        /// </value>
        IEnumerable<Reference> ReverseReferences
        {
            get;
        }

        /// <summary>
        /// このアクティビティに関連付けられたリファレンスの意味と、対象となるアクティビティの組のシーケンスを取得します。
        /// </summary>
        /// <value>
        /// このアクティビティに関連付けられたリファレンスの意味と、対象となるアクティビティの組のシーケンス。
        /// </value>
        IEnumerable<KeyValuePair<String, Activity>> Referring
        {
            get;
        }

        /// <summary>
        /// このアクティビティが対象として関連付けられたリファレンスの意味と、関連付けたアクティビティの組のシーケンスを取得します。
        /// </summary>
        /// <value>
        /// このアクティビティが対象として関連付けられたリファレンスの意味と、関連付けたアクティビティの組のシーケンス。
        /// </value>
        IEnumerable<KeyValuePair<String, Activity>> Referrers
        {
            get;
        }

        /// <summary>
        /// このアクティビティに関連付けられたマークのシーケンスを取得します。
        /// </summary>
        /// <value>
        /// このアクティビティに関連付けられたマークのシーケンス。
        /// </value>
        IEnumerable<Mark> Marks
        {
            get;
        }

        /// <summary>
        /// このアクティビティが対象として関連付けられたマークの意味と、関連付けたアカウントの組のシーケンスを取得します。
        /// </summary>
        /// <value>
        /// このアクティビティが対象として関連付けられたマークの意味と、関連付けたアカウントの組のシーケンス。
        /// </value>
        IEnumerable<KeyValuePair<String, Account>> Markers
        {
            get;
        }

        /// <summary>
        /// 意味を指定して、このアクティビティに関連付けられたリファレンスの対象となるアクティビティのシーケンスを取得します。
        /// </summary>
        /// <param name="name">リファレンスの意味。</param>
        /// <returns>このアクティビティに、指定した意味で関連付けられたリファレンスの対象となるアクティビティのシーケンス。</returns>
        IEnumerable<Activity> ReferringOf(String name);

        /// <summary>
        /// 意味を指定して、このアクティビティが対象として関連付けられたリファレンスを関連付けたアクティビティのシーケンスを取得します。
        /// </summary>
        /// <param name="name">リレーションの意味。</param>
        /// <returns>このアクティビティに、指定した意味でこのアクティビティが対象として関連付けられたリファレンスを関連付けたアクティビティのシーケンス。</returns>
        IEnumerable<Activity> ReferrersOf(String name);

        /// <summary>
        /// 意味を指定して、このアクティビティが対象として関連付けられたマークを関連付けたアカウントのシーケンスを取得します。
        /// </summary>
        /// <param name="name">マークの意味。</param>
        /// <returns>このアクティビティに、指定した意味でこのアクティビティが対象として関連付けられたマークを関連付けたアカウントのシーケンス。</returns>
        IEnumerable<Account> MarkersOf(String name);

        /// <summary>
        /// このアクティビティに、指定した意味でタグが関連付けられているかどうかを示す値を取得します。
        /// </summary>
        /// <param name="name">タグの意味。</param>
        /// <returns>
        /// このアクティビティに、指定した意味でタグが関連付けられている場合は <c>true</c>。それ以外の場合は <c>false</c>。
        /// </returns>
        Boolean IsTagging(String name);

        /// <summary>
        /// このアクティビティに、指定した意味とアクティビティでリファレンスが関連付けられているかどうかを示す値を取得します。
        /// </summary>
        /// <param name="name">リファレンスの意味。</param>
        /// <param name="activity">対象とするアカウント。</param>
        /// <returns>
        /// このアクティビティに、指定した意味とアクティビティでリファレンスが関連付けられている場合は <c>true</c>。それ以外の場合は <c>false</c>。
        /// </returns>
        Boolean IsReferring(String name, Activity activity);

        /// <summary>
        /// 指定したアクティビティに、指定した意味でこのアクティビティを対象としてリファレンスが関連付けられているかどうかを示す値を取得します。
        /// </summary>
        /// <param name="name">リファレンスの意味。</param>
        /// <param name="activity">リファレンスが関連付けられているかどうかを取得するアクティビティ。</param>
        /// <returns>
        /// 指定したアクティビティに、指定した意味でこのアクティビティを対象としてリファレンスが関連付けられている場合は <c>true</c>。それ以外の場合は <c>false</c>。
        /// </returns>
        Boolean IsReferred(String name, Activity activity);

        /// <summary>
        /// 指定したアカウントに、指定した意味でこのアクティビティを対象としてマークが関連付けられているかどうかを示す値を取得します。
        /// </summary>
        /// <param name="name">マークの意味。</param>
        /// <param name="account">マークが関連付けられているかどうかを取得するアカウント。</param>
        /// <returns>
        /// 指定したアカウントに、指定した意味でこのアクティビティを対象としてマークが関連付けられている場合は <c>true</c>。それ以外の場合は <c>false</c>。
        /// </returns>
        Boolean IsMarked(String name, Account account);

        /// <summary>
        /// このアクティビティにタグを関連付けます。
        /// </summary>
        /// <param name="name">タグの意味。</param>
        /// <returns></returns>
        Tag Tag(String name);

        /// <summary>
        /// このアクティビティにリファレンスを関連付けます。
        /// </summary>
        /// <param name="name">リファレンスの意味。</param>
        /// <param name="referTo">リファレンスの対象となるアクティビティ。</param>
        /// <returns>関連付けられたリファレンス。</returns>
        Reference Refer(String name, Activity referTo);

        /// <summary>
        /// このアクティビティを対象として、指定したアクティビティにリファレンスを関連付けます。
        /// </summary>
        /// <param name="name">リファレンスの意味。</param>
        /// <param name="referredFrom">リファレンスを関連付けるアクティビティ。</param>
        /// <returns>関連付けられたリファレンス。</returns>
        Reference Referred(String name, Activity referredFrom);

        /// <summary>
        /// このアクティビティを対象として、指定したアカウントにマークを関連付けます。
        /// </summary>
        /// <param name="name">マークの意味。</param>
        /// <param name="markedFrom">マークを関連付けるアカウント。</param>
        /// <returns>関連付けられたマーク。</returns>
        Mark Marked(String name, Account markedFrom);
    }
}