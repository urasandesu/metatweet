﻿// -*- mode: csharp; encoding: utf-8; -*-
/* MetaTweet
 *   Hub system for micro-blog communication services
 * MetaTweetObjectModel
 *   Object model and Storage interface for MetaTweet and other systems
 *   Part of MetaTweet
 * Copyright © 2008-2009 Takeshi KIRIYA, XSpect Project <takeshik@xspect.org>
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
using System.Linq;

namespace XSpect.MetaTweet.ObjectModel
{
    /// <summary>
    /// アカウントを表します。
    /// </summary>
    /// <remarks>
    /// <p>アカウントは MetaTweet のデータ構造の頂点に位置する構造で、個々のサービスを利用するユーザを表現します。</p>
    /// <p>アカウントは <see cref="AccountId"/> によって一意に識別されます。</p>
    /// </remarks>
    [Serializable()]
    public class Account
        : StorageObject<StorageDataSet.AccountsDataTable, StorageDataSet.AccountsRow>,
          IComparable<Account>
    {
        /// <summary>
        /// アカウントを一意に識別するグローバル一意識別子 (GUID) 値を取得または設定します。
        /// </summary>
        /// <value>
        /// アカウントを一意に識別するグローバル一意識別子 (GUID) 値。
        /// </value>
        public Guid AccountId
        {
            get
            {
                return this.UnderlyingDataRow.AccountId;
            }
            set
            {
                this.UnderlyingDataRow.AccountId = value;
            }
        }

        /// <summary>
        /// アカウントに関連付けられているサービスを表す文字列を取得または設定します。
        /// </summary>
        /// <value>
        /// アカウントに関連付けられているサービスを表す文字列。
        /// </value>
        /// <remarks>
        /// Realm はアカウントに関連付けられているサービスを識別する要素です。
        /// 通常、Realm はサービスの完全修飾ドメイン名 (FQDN) を逆順に並べた文字列を先頭に配置します (Java のパッケージ命名規約と同じ)。
        /// Realm の値はユーザによって提示されます。
        /// </remarks>
        public String Realm
        {
            get
            {
                return this.UnderlyingDataRow.Realm;
            }
            set
            {
                this.UnderlyingDataRow.Realm = value;
            }
        }

        /// <summary>
        /// 指定されたカテゴリに属する、アカウントの最新のアクティビティの値を取得します。
        /// </summary>
        /// <param name="category">検索するカテゴリ。</param>
        /// <returns>指定されたカテゴリに属する、アカウントの最新のアクティビティの値。</returns>
        /// <remarks>
        /// このプロパティの返す値とは <see cref="Activity.Value"/> です。
        /// </remarks>
        public String this[String category]
        {
            get
            {
                return this.GetActivityOf(category).Value;
            }
        }

        /// <summary>
        /// このアカウントがお気に入りとしてマークしたアクティビティの関係の一覧を取得します。
        /// </summary>
        /// <value>
        /// このアカウントがお気に入りとしてマークしたアクティビティの関係の一覧。
        /// </value>
        public IEnumerable<FavorElement> FavoringMap
        {
            get
            {
                return this.Storage.GetFavorElements(this.UnderlyingDataRow.GetFavorMapRows());
            }
        }

        /// <summary>
        /// このアカウントがお気に入りとしてマークしたアクティビティの一覧を取得します。
        /// </summary>
        /// <value>
        /// このアカウントがお気に入りとしてマークしたアクティビティの一覧。
        /// </value>
        public IEnumerable<Activity> Favoring
        {
            get
            {
                return this.FavoringMap.Select(e => e.FavoringActivity);
            }
        }

        /// <summary>
        /// このアカウントがフォローしているアカウントの関係の一覧を取得します。
        /// </summary>
        /// <value>
        /// このアカウントがフォローしているアカウントの関係の一覧。
        /// </value>
        public IEnumerable<FollowElement> FollowingMap
        {
            get
            {
                return this.Storage.GetFollowElements(this.UnderlyingDataRow.GetFollowMapRows());
            }
        }

        /// <summary>
        /// このアカウントがフォローしているアカウントの一覧を取得します。
        /// </summary>
        /// <value>
        /// このアカウントがフォローしているアカウントの一覧。
        /// </value>
        public IEnumerable<Account> Following
        {
            get
            {
                return this.FollowingMap.Select(e => e.FollowingAccount);
            }
        }

        /// <summary>
        /// このアカウントがフォローされているアカウントの関係の一覧を取得します。
        /// </summary>
        /// <value>
        /// このアカウントがフォローされているアカウントの関係の一覧。
        /// </value>
        public IEnumerable<FollowElement> FollowersMap
        {
            get
            {
                return this.Storage.GetFollowElements(this.UnderlyingDataRow.GetFollowMapRowsByFK_AccountsFollowing_FollowMap());
            }
        }

        /// <summary>
        /// このアカウントがフォローされているアカウントの一覧を取得します。
        /// </summary>
        /// <value>
        /// このアカウントがフォローされているアカウントの一覧。
        /// </value>
        public IEnumerable<Account> Followers
        {
            get
            {
                return this.FollowersMap.Select(e => e.Account);
            }
        }

        /// <summary>
        /// このアカウントのアクティビティの一覧を取得します。
        /// </summary>
        /// <value>
        /// このアカウントのアクティビティの一覧。
        /// </value>
        public IEnumerable<Activity> Activities
        {
            get
            {
                return this.Storage.GetActivities(this.UnderlyingDataRow.GetActivitiesRows());
            }
        }

        /// <summary>
        /// 現在の <see cref="T:System.Object"/> を表す <see cref="T:System.String"/> を返します。
        /// </summary>
        /// <returns>
        /// 現在の <see cref="T:System.Object"/> を表す <see cref="T:System.String"/>。
        /// </returns>
        public override String ToString()
        {
            return String.Format("{0}@{1}", this.AccountId.ToString("d"), this.Realm);
        }

        /// <summary>
        /// 現在のオブジェクトを同じ型の別のオブジェクトと比較します。
        /// </summary>
        /// <param name="other">このオブジェクトと比較するオブジェクト。</param>
        /// <returns>
        /// 比較対象オブジェクトの相対順序を示す 32 ビット符号付き整数。戻り値の意味は次のとおりです。
        /// 値
        /// 意味
        /// 0 より小さい値
        /// このオブジェクトが <paramref name="other"/> パラメータより小さいことを意味します。
        /// 0
        /// このオブジェクトが <paramref name="other"/> と等しいことを意味します。
        /// 0 より大きい値
        /// このオブジェクトが <paramref name="other"/> よりも大きいことを意味します。
        /// </returns>
        public Int32 CompareTo(Account other)
        {
            return this.AccountId.CompareTo(other.AccountId);
        }

        /// <summary>
        /// 指定されたアクティビティをお気に入りとしてマークします。
        /// </summary>
        /// <param name="activity">お気に入りとしてマークするアクティビティ。</param>
        public void AddFavorite(Activity activity)
        {
            FavorElement element = this.Storage.NewFavorElement();
            element.Account = this;
            element.FavoringActivity = activity;
            element.Update();
        }

        /// <summary>
        /// 指定されたアクティビティへのお気に入りのマークを削除します。
        /// </summary>
        /// <param name="activity">お気に入りのマークを削除するアクティビティ。</param>
        public void RemoveFavorite(Activity activity)
        {
            FavorElement element = this.FavoringMap.Single(e => e.FavoringActivity == activity);
            element.Delete();
            element.Update();
        }

        /// <summary>
        /// 指定されたアカウントをフォローしている関係として追加します。
        /// </summary>
        /// <param name="account">フォローしている関係として追加するアカウント</param>
        public void AddFollowing(Account account)
        {
            FollowElement element = this.Storage.NewFollowElement();
            element.Account = this;
            element.FollowingAccount = account;
            element.Update();
        }

        /// <summary>
        /// 指定されたアカウントをフォローされている関係として追加します。
        /// </summary>
        /// <param name="account">フォローされている関係として追加するアカウント</param>
        public void AddFollower(Account account)
        {
            FollowElement element = this.Storage.NewFollowElement();
            element.Account = account;
            element.FollowingAccount = this;
            element.Update();
        }

        /// <summary>
        /// 指定されたアカウントとのフォロー関係を削除します。
        /// </summary>
        /// <param name="account">フォロー関係を削除するアカウント。</param>
        public void RemoveFollowing(Account account)
        {
            FollowElement element = this.FollowingMap.Single(e => e.FollowingAccount == account);
            element.Delete();
            element.Update();
        }

        /// <summary>
        /// 指定されたアカウントからのフォロー関係を削除します。
        /// </summary>
        /// <param name="account">フォロー関係を削除されるアカウント。</param>
        public void RemoveFollower(Account account)
        {
            FollowElement element = this.FollowersMap.Single(e => e.Account == account);
            element.Delete();
            element.Update();
        }

        /// <summary>
        /// このアカウントに新しいアクティビティを追加します。
        /// </summary>
        /// <returns>新しいアクティビティ。</returns>
        public Activity NewActivity()
        {
            Activity activity = this.Storage.NewActivity();
            activity.Account = this;
            return activity;
        }

        /// <summary>
        /// 指定されたカテゴリに属する、アカウントの最新のアクティビティを取得します。
        /// </summary>
        /// <param name="category">検索するカテゴリ。</param>
        /// <returns>指定されたカテゴリに属する、アカウントの最新のアクティビティ。</returns>
        public Activity GetActivityOf(String category)
        {
            return this.Activities.Where(a => a.Category == category).OrderByDescending(a => a.Timestamp).FirstOrDefault();
        }
    }
}