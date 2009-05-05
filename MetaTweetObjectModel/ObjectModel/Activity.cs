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
using System.Collections.Generic;
using System.Linq;

namespace XSpect.MetaTweet.ObjectModel
{
    /// <summary>
    /// アクティビティを表します。
    /// </summary>
    /// <remarks>
    /// <para>アクティビティはアカウントの行動を表現します。行動には、名前などを含むユーザ情報の変更および発言の投稿を含みます。個々のアクティビティは行動が行われた日時、行動の種別を表す文字列、サブインデックス (同一日付種別内での行動を一意に識別する数値)、文字列およびバイト列の値によって構成されます。</para>
    /// <para>アクティビティは <see cref="Account"/>、<see cref="Timestamp"/>、<see cref="Category"/> および <see cref="Subindex"/> によって一意に識別されます。</para>
    /// </remarks>
    [Serializable()]
    public partial class Activity
        : StorageObject<StorageDataSet.ActivitiesDataTable, StorageDataSet.ActivitiesRow>,
          IComparable<Activity>,
          IEquatable<Activity>
    {
        private readonly PrimaryKeyCollection _primaryKeys;

        /// <summary>
        /// このアクティビティのデータのバックエンドとなるデータ行の主キーのシーケンスを取得します。
        /// </summary>
        /// <value>このアクティビティのデータのバックエンドとなるデータ行の主キーのシーケンス。</value>
        public override IList<Object> PrimaryKeyList
        {
            get
            {
                return this.PrimaryKeys.ToList();
            }
        }

        /// <summary>
        /// このアクティビティのデータのバックエンドとなるデータ行の主キーのシーケンスを表すオブジェクトを取得します。
        /// </summary>
        /// <returns>このアクティビティのデータのバックエンドとなるデータ行の主キーのシーケンスを表すオブジェクト。</returns>
        public PrimaryKeyCollection PrimaryKeys
        {
            get
            {
                return this._primaryKeys;
            }
        }

        /// <summary>
        /// データセット内に存在する、このアクティビティの主体であるアカウントを取得または設定します。
        /// </summary>
        /// <value>
        /// データセット内に存在する、このアクティビティの主体であるアカウント。
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
                if (this.IsStored && this.Storage.Cache.Activies.Contains(this) && value < this.Timestamp)
                {
                    this.Storage.Cache.Activies.Remove(this);
                }
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
        /// このアクティビティのサブインデックスを取得または設定します。
        /// </summary>
        /// <value>
        /// このアクティビティのサブインデックス。
        /// </value>
        /// <remarks>
        /// <para>サブインデックスとは、<see cref="Account"/>、<see cref="Timestamp"/>、<see cref="Category"/> が同一のアクティビティをそれぞれ一意に識別するための整数値を指します。</para>
        /// <para>サブインデックスは 0 から開始します。<see cref="Account"/>、<see cref="Timestamp"/>、および <see cref="Category"/> が同一な他のアクティビティが存在しない場合、そのアクティビティのサブインデックスは常に 0 です。同一な他のアクティビティが追加された場合、追加される順にインクリメントされたサブインデックスが設定されます。</para>
        /// </remarks>
        public Int32 Subindex
        {
            get
            {
                return this.UnderlyingDataRow.Subindex;
            }
            set
            {
                this.UnderlyingDataRow.Subindex = value;
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
        /// データセット内に存在する、このアクティビティをお気に入りとしているアカウントとの関係のシーケンスを取得します。
        /// </summary>
        /// <value>
        /// データセット内に存在する、このアクティビティをお気に入りとしているアカウントとの関係のシーケンス。
        /// </value>
        public IEnumerable<FavorElement> FavorersMap
        {
            get
            {
                return this.Storage.GetFavorElements(this.UnderlyingDataRow.GetFavorMapRows());
            }
        }

        /// <summary>
        /// データセット内に存在する、このアクティビティをお気に入りとしているアカウントのシーケンスを取得します。
        /// </summary>
        /// <value>
        /// データセット内に存在する、このアクティビティをお気に入りとしているアカウントのシーケンス。
        /// </value>
        public IEnumerable<Account> Favorers
        {
            get
            {
                return this.FavorersMap.Select(e => e.Account);
            }
        }

        /// <summary>
        /// データセット内に存在する、このアクティビティに付与されているタグとの関係のシーケンスを取得します。
        /// </summary>
        /// <value>
        /// データセット内に存在する、このアクティビティに付与されているタグとの関係のシーケンス。
        /// </value>
        public IEnumerable<TagElement> TagMap
        {
            get
            {
                return this.Storage.GetTagElements(this.UnderlyingDataRow.GetTagMapRows());
            }
        }

        /// <summary>
        /// データセット内に存在する、このアクティビティに付与されているタグのシーケンスを取得します。
        /// </summary>
        /// <value>
        /// データセット内に存在する、このアクティビティに付与されているタグのシーケンス。
        /// </value>
        public IEnumerable<String> Tags
        {
            get
            {
                return this.TagMap.Select(e => e.Tag);
            }
        }

        /// <summary>
        /// <see cref="Activity"/> の新しいインスタンスを初期化します。
        /// </summary>
        public Activity()
        {
            this._primaryKeys = new PrimaryKeyCollection(this);
        }

        /// <summary>
        /// <see cref="Activity"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="row">アクティビティが参照するデータ行。</param>
        /// 
        public Activity(StorageDataSet.ActivitiesRow row)
            : this()
        {
            this.UnderlyingDataRow = row;
        }

        /// <summary>
        /// このアクティビティと、指定した別のアカウントが同一かどうかを判断します。
        /// </summary>
        /// <param name="obj">このアクティビティと比較するオブジェクト。</param>
        /// <returns>
        /// <paramref name="obj"/> パラメータの値がこのアクティビティと同じ場合は <c>true</c>。それ以外の場合は <c>false</c>。 
        /// </returns>
        public override Boolean Equals(Object obj)
        {
            return obj is Activity && this.Equals(obj as Activity);
        }

        /// <summary>
        /// このアクティビティのハッシュ コードを返します。 
        /// </summary>
        /// <returns>32 ビット符号付き整数ハッシュ コード。 </returns>
        public override Int32 GetHashCode()
        {
            return this.PrimaryKeys.GetHashCode();
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
                this.Value ?? "(null)"
            );
        }

        /// <summary>
        /// このアクティビティの参照するデータ行がデータ表に属していない場合、データ列を新たなデータ表に所属させます。
        /// </summary>
        public override void Store()
        {
            this.Storage.Cache.Activies.Add(this);
            base.Store();
        }

        /// <summary>
        /// このアクティビティを別のアクティビティと比較します。
        /// </summary>
        /// <param name="other">このアクティビティと比較するアクティビティ。</param>
        /// <returns>
        /// 比較対象アクティビティの相対順序を示す 32 ビット符号付き整数。戻り値の意味は次のとおりです。<br/>
        /// 値<br/>
        /// 意味<br/>
        /// 0 より小さい値<br/>
        /// このアクティビティが <paramref name="other"/> パラメータより前に序列されるべきであることを意味します。<br/>
        /// 0<br/>
        /// このアクティビティが <paramref name="other"/> と等しいことを意味します。<br/>
        /// 0 より大きい値<br/>
        /// このアクティビティが <paramref name="other"/> パラメータより後に序列されるべきであることを意味します。<br/>
        /// </returns>
        public virtual Int32 CompareTo(Activity other)
        {
            return new PrimaryKeyCollection(this).CompareTo(new PrimaryKeyCollection(other));
        }

        /// <summary>
        /// このアクティビティと、指定した別のアクティビティが同一かどうかを判断します。
        /// </summary>
        /// <param name="other">このアクティビティと比較するアクティビティ。</param>
        /// <returns>
        /// <paramref name="other"/> パラメータの値がこのアクティビティと同じ場合は <c>true</c>。それ以外の場合は <c>false</c>。 
        /// </returns>
        public Boolean Equals(Activity other)
        {
            return this.Storage == other.Storage && this.CompareTo(other) == 0;
        }

        /// <summary>
        /// このアクティビティを別のストレージへコピーします。
        /// </summary>
        /// <param name="destination">コピー先の <see cref="Storage"/>。</param>
        /// <returns>コピーされたアクティビティ。</returns>
        public Activity Copy(Storage destination)
        {
            Activity activity = destination.NewActivity(
                this.GetAccount().Copy(destination),
                this.Timestamp,
                this.Category
            );
            activity.Value = this.Value;
            activity.Data = this.Data;
            return activity;
        }

        /// <summary>
        /// <see cref="Subindex"/> 値を再設定します。
        /// </summary>
        /// <returns>再設定された <see cref="Subindex"/> の値。</returns>
        public Int32 FixSubindex()
        {
            this.Storage.LoadActivitiesDataTable(
                this.PrimaryKeys.AccountId, this.Timestamp, this.Category, null
            );
            this.Subindex = this.Storage.GetActivities(
                this.PrimaryKeys.AccountId, this.Timestamp, this.Category, null
            ).Count();
            return this.Subindex;
        }

        /// <summary>
        /// このアクティビティの主体であるアカウントを取得します。
        /// </summary>
        /// <returns>
        /// このアクティビティの主体であるアカウント。
        /// </returns>
        public Account GetAccount()
        {
            this.Storage.LoadAccountsDataTable(this.UnderlyingDataRow.AccountId);
            return this.Account;
        }

        /// <summary>
        /// このアクティビティをお気に入りとしているアカウントとの関係のシーケンスを取得します。
        /// </summary>
        /// <returns>
        /// このアクティビティをお気に入りとしているアカウントとの関係のシーケンス。
        /// </returns>
        public IEnumerable<FavorElement> GetFavorersMap()
        {
            this.Storage.LoadFavorMapDataTable(null, this.UnderlyingDataRow.AccountId, this.Timestamp, this.Category, this.Subindex);
            return this.FavorersMap;
        }

        /// <summary>
        /// このアクティビティをお気に入りとしているアカウントのシーケンスを取得します。
        /// </summary>
        /// <returns>
        /// このアクティビティをお気に入りとしているアカウントのシーケンス。
        /// </returns>
        public IEnumerable<Account> GetFavorers()
        {
            return this.GetFavorersMap().Select(e => e.Account);
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
            this.GetFavorersMap().Single(e => e.Account == account).Delete();
        }

        /// <summary>
        /// このアクティビティに付与されているタグとの関係のシーケンスを取得します。
        /// </summary>
        /// <returns>
        /// このアクティビティに付与されているタグとの関係のシーケンス。
        /// </returns>
        public IEnumerable<TagElement> GetTagMap()
        {
            this.Storage.LoadTagMapDataTable(this.UnderlyingDataRow.AccountId, this.Timestamp, this.Category, this.Subindex, null);
            return this.TagMap;
        }

        /// <summary>
        /// このアクティビティに付与されているタグのシーケンスを取得します。
        /// </summary>
        /// <returns>
        /// このアクティビティに付与されているタグのシーケンス。
        /// </returns>
        public IEnumerable<String> GetTags()
        {
            return this.GetTagMap().Select(e => e.Tag);
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
            this.GetTagMap().Single(e => e.Tag == tag).Delete();
        }

        /// <summary>
        /// このアクティビティと一対一で関連付けられるポストを取得します。存在しない場合は、新たに作成されます。
        /// </summary>
        /// <returns>このアクティビティと一対一で関連付けられるポスト。存在しなかった場合は、新たに作成されたポスト。</returns>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Category"/> が Post ではありません。
        /// </exception>
        /// <remarks>
        /// カテゴリが Post であるアクティビティはポストと関連付けられます。このメソッドはカテゴリが Post であるメソッドにおいて関連付けられた <see cref="Post"/> を取得し、または存在しない場合新しく作成し、それを返します。カテゴリが Post 以外の場合は例外 <see cref="InvalidOperationException"/> がスローされます。
        /// </remarks>
        /// <seealso cref="Post"/>
        public Post ToPost()
        {
            this.Storage.LoadPostsDataTable(this.UnderlyingDataRow.AccountId, this.Value);
            if (this.Category != "Post")
            {
                throw new InvalidOperationException("This activity's category is not \"Post\".");
            }
            StorageDataSet.PostsRow row = this.UnderlyingDataRow.GetPostsRows().SingleOrDefault();
            return row != null
                ? this.Storage.GetPost(row)
                : this.Storage.NewPost(this);
        }
    }
}