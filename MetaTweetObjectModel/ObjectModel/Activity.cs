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
    /// アクティビティを表します。
    /// </summary>
    /// <remarks>
    /// <p>アクティビティはアカウントの行動を表現します。行動には、名前などを含むユーザ情報の変更および発言の投稿を含みます。
    /// 個々のアクティビティは行動が行われた日時、行動の種別を表す文字列、文字列およびバイト列の値によって構成されます。</p>
    /// <p>アクティビティは <see cref="Account"/>、<see cref="Timestamp"/> および <see cref="Category"/> によって一意に識別されます。</p>
    /// </remarks>
    [Serializable()]
    public class Activity
        : StorageObject<StorageDataSet.ActivitiesDataTable, StorageDataSet.ActivitiesRow>,
          IComparable<Activity>
    {
        /// <summary>
        /// このアクティビティの主体であるアカウントを取得または設定します。
        /// </summary>
        /// <value>
        /// このアクティビティの主体であるアカウント。
        /// </value>
        public Account Account
        {
            get
            {
                return this.Storage.GetAccount(this.UnderlyingDataRow.AccountsRow);
            }
            set
            {
                this.UnderlyingDataRow.AccountsRow = value.UnderlyingDataRow;
            }
        }
        
        /// <summary>
        /// このアクティビティの行われた日時を取得または設定します。
        /// </summary>
        /// <value>
        /// このアクティビティの行われた日時。
        /// </value>
        /// <remarks>
        /// 日時は協定世界時 (UTC) として表されます。
        /// </remarks>
        public DateTime Timestamp
        {
            get
            {
                return this.UnderlyingDataRow.Timestamp;
            }
            set
            {
                this.UnderlyingDataRow.Timestamp = value;
            }
        }

        /// <summary>
        /// このアクティビティの種別を表す文字列を取得または設定します。
        /// </summary>
        /// <value>
        /// このアクティビティの種別を表す文字列。
        /// </value>
        /// <remarks>
        /// どのような文字列を種別として使用するかに関しては、はインスタンスを操作する側が自由に決定できます。
        /// </remarks>
        public String Category
        {
            get
            {
                return this.UnderlyingDataRow.Category;
            }
            set
            {
                this.UnderlyingDataRow.Category = value;
            }
        }

        /// <summary>
        /// このアクティビティに関連付けられている文字列の値を取得または設定します。
        /// </summary>
        /// <value>
        /// このアクティビティに関連付けられている文字列の値。存在しない場合は <c>null</c>。
        /// </value>
        public String Value
        {
            get
            {
                return this.UnderlyingDataRow.IsValueNull()
                    ? null
                    : this.UnderlyingDataRow.Value;
            }
            set
            {
                if (value != null)
                {
                    this.UnderlyingDataRow.Value = value;
                }
                else
                {
                    this.UnderlyingDataRow.SetValueNull();
                }
            }
        }

        /// <summary>
        /// このアクティビティに関連付けられているバイト列の値を取得または設定します。
        /// </summary>
        /// <value>
        /// このアクティビティに関連付けられているバイト列の値。存在しない場合は <c>null</c>。
        /// </value>
        public Byte[] Data
        {
            get
            {
                return this.UnderlyingDataRow.IsDataNull()
                    ? null
                    : this.UnderlyingDataRow.Data;
            }
            set
            {
                if (value != null)
                {
                    this.UnderlyingDataRow.Data = value;
                }
                else
                {
                    this.UnderlyingDataRow.SetDataNull();
                }
            }
        }

        /// <summary>
        /// このアクティビティをお気に入りとしているアカウントとの関係の一覧を取得します。
        /// </summary>
        /// <value>
        /// このアクティビティをお気に入りとしているアカウントとの関係の一覧。
        /// </value>
        public IEnumerable<FavorElement> FavorersMap
        {
            get
            {
                return this.Storage.GetFavorElements(this.UnderlyingDataRow.GetFavorMapRows());
            }
        }

        /// <summary>
        /// このアクティビティをお気に入りとしているアカウントの一覧を取得します。
        /// </summary>
        /// <value>
        /// このアクティビティをお気に入りとしているアカウントの一覧を取得します。
        /// </value>
        public IEnumerable<Account> Favorers
        {
            get
            {
                return this.FavorersMap.Select(e => e.Account);
            }
        }

        /// <summary>
        /// このアクティビティに付与されているタグとの関係の一覧を取得します。
        /// </summary>
        /// <value>
        /// このアクティビティに付与されているタグとの関係の一覧。
        /// </value>
        public IEnumerable<TagElement> TagMap
        {
            get
            {
                return this.Storage.GetTagElements(this.UnderlyingDataRow.GetTagMapRows());
            }
        }

        /// <summary>
        /// このアクティビティに付与されているタグの一覧を取得します。
        /// </summary>
        /// <value>
        /// このアクティビティに付与されているタグの一覧。
        /// </value>
        public IEnumerable<String> Tags
        {
            get
            {
                return this.TagMap.Select(e => e.Tag);
            }
        }

        public Activity(
            Account account,
            DateTime timestamp,
            String category
        )
        {
            this.Account = account;
            this.Timestamp = timestamp;
            this.Category = category;
            this.Store();
        }

        public Activity(StorageDataSet.ActivitiesRow row)
        {
            this.UnderlyingDataRow = row;
        }

        /// <summary>
        /// このアクティビティを別のアクティビティと比較します。
        /// </summary>
        /// <param name="other">このアクティビティと比較するアクティビティ。</param>
        /// <returns>
        /// 比較対象アクティビティの相対順序を示す 32 ビット符号付き整数。戻り値の意味は次のとおりです。
        /// 値
        /// 意味
        /// 0 より小さい値
        /// このアクティビティが <paramref name="other"/> パラメータより前に序列されるべきであることを意味します。
        /// 0
        /// このアクティビティが <paramref name="other"/> と等しいことを意味します。
        /// 0 より大きい値
        /// このアクティビティが <paramref name="other"/> パラメータより後に序列されるべきであることを意味します。
        /// </returns>
        public virtual Int32 CompareTo(Activity other)
        {
            Int32 result;
            if ((result = this.Timestamp.CompareTo(other.Timestamp)) != 0)
            {
                return result;
            }
            else if ((result = this.Account.CompareTo(other.Account)) != 0)
            {
                return result;
            }
            else
            {
                return this.Category.CompareTo(other.Category);
            }
        }

        /// <summary>
        /// このアクティビティを表す <see cref="T:System.String"/> を返します。
        /// </summary>
        /// <returns>
        /// このアクティビティを表す <see cref="T:System.String"/>。
        /// </returns>
        public override String ToString()
        {
            return String.Format(
                "{0}: {1} = \"{2}\"",
                this.Timestamp.ToString("s"),
                this.Category,
                this.Value != null ? this.Value : "(null)"
            );
        }

        /// <summary>
        /// 指定されたアカウントをこのアクティビティをお気に入りとしている関係として追加します。
        /// </summary>
        /// <param name="account">お気に入りとしてマークしている関係として追加するアカウント</param>
        public void AddFavorer(Account account)
        {
            this.Storage.NewFavorElement(account, this);
        }

        /// <summary>
        /// 指定されたアカウントとのお気に入りとしている関係を削除します。
        /// </summary>
        /// <param name="account">お気に入りとしている関係を削除するアカウント</param>
        public void RemoveFavorer(Account account)
        {
            this.FavorersMap.Single(e => e.Account == account).Delete();
        }

        /// <summary>
        /// 指定された文字列をこのアクティビティにタグとして付与します。
        /// </summary>
        /// <param name="tag">タグとして付与する文字列。</param>
        public void AddTag(String tag)
        {
            this.Storage.NewTagElement(this, tag);
        }

        /// <summary>
        /// 指定された文字列のタグをこのアクティビティから剥奪します。
        /// </summary>
        /// <param name="tag">剥奪するタグの文字列。</param>
        public void RemoveTag(String tag)
        {
            this.TagMap.Single(e => e.Tag == tag).Delete();
        }

        /// <summary>
        /// このアクティビティと一対一で関連付けられるポストを取得します。存在しない場合は、新たに作成されます。
        /// </summary>
        /// <returns>このアクティビティと一対一で関連付けられるポスト。存在しなかった場合は、新たに作成されたポスト。</returns>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Category"/> が Post ではありません。
        /// </exception>
        /// <remarks>
        /// カテゴリが Post であるアクティビティはポストと関連付けられます。このメソッドはカテゴリが Post であるメソッドにおいて
        /// 関連付けられた <see cref="Post"/> を取得し、または存在しない場合新しく作成し、それを返します。カテゴリが Post 以外の
        /// 場合は例外 <see cref="InvalidOperationException"/> がスローされます。
        /// </remarks>
        /// <seealso cref="Post"/>
        public Post ToPost()
        {
            if (this.Category != "Post")
            {
                // TODO: exception string resource
                throw new InvalidOperationException();
            }
            StorageDataSet.PostsRow row = this.UnderlyingDataRow.GetPostsRows().SingleOrDefault();
            if (row != null)
            {
                return this.Storage.GetPost(row);
            }
            else
            {
                return this.NewPost();
            }
        }

        /// <summary>
        /// このアクティビティと一対一で関連付けられるポストを作成します。
        /// </summary>
        /// <returns>このアクティビティと一対一で関連付けられる新しいポスト。</returns>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Category"/> が Post ではありません。
        /// </exception>
        /// <seealso cref="ToPost"/>
        /// <seealso cref="Post"/>
        public Post NewPost()
        {
            if (this.Category != "Post")
            {
                // TODO: exception string resource
                throw new InvalidOperationException();
            }
            return this.Storage.NewPost(this);
        }
    }
}