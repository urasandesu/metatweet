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

namespace XSpect.MetaTweet.ObjectModel
{
    /// <summary>
    /// アクティビティと、アクティビティに付与されている文字列との関係を表します。
    /// </summary>
    /// <remarks>
    /// このクラスはアクティビティと文字列との関係表の単一の行要素を表現し、その集合により多対多の関係を構成します。
    /// </remarks>
    [Serializable()]
    public partial class TagElement
        : StorageObject<StorageDataSet.TagMapDataTable, StorageDataSet.TagMapRow>
    {
        /// <summary>
        /// この関係のデータのバックエンドとなるデータ行の主キーのシーケンスを取得します。
        /// </summary>
        /// <value>この関係のデータのバックエンドとなるデータ行の主キーのシーケンス。</value>
        public override IEnumerable<Object> PrimaryKeys
        {
            get
            {
                return this.GetPrimaryKeyCollection();
            }
        }
        
        /// <summary>
        /// タグを付与されている主体であるアクティビティを取得または設定します。
        /// </summary>
        /// <value>
        /// タグを付与されている主体であるアクティビティ。
        /// </value>
        public Activity Activity
        {
            get
            {
                this.Storage.LoadActivitiesDataTable(
                    this.UnderlyingDataRow.AccountId,
                    this.UnderlyingDataRow.Timestamp,
                    this.UnderlyingDataRow.Category,
                    this.UnderlyingDataRow.Subindex
                );
                return this.ActivityInDataSet;
            }
            set
            {
                this.UnderlyingDataRow.ActivitiesRowParent = value.UnderlyingDataRow;
            }
        }

        /// <summary>
        /// データセット内に存在する、タグを付与されている主体であるアクティビティを取得または設定します。
        /// </summary>
        /// <value>
        /// データセット内に存在する、タグを付与されている主体であるアクティビティ。
        /// </value>
        public Activity ActivityInDataSet
        {
            get
            {
                return this.Storage.GetActivity(this.UnderlyingDataRow.ActivitiesRowParent);
            }
        }

        /// <summary>
        /// アクティビティに対し付与されているタグの文字列を取得または設定します。
        /// </summary>
        /// <value>
        /// アクティビティに対し付与されているタグの文字列。
        /// </value>
        public String Tag
        {
            get
            {
                return this.UnderlyingDataRow.Tag;
            }
            set
            {
                this.UnderlyingDataRow.Tag = value;
            }
        }

        /// <summary>
        /// <see cref="TagElement"/> の新しいインスタンスを初期化します。
        /// </summary>
        public TagElement()
        {
        }

        /// <summary>
        /// <see cref="TagElement"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="row">関係が参照するデータ列。</param>
        public TagElement(StorageDataSet.TagMapRow row)
        {
            this.UnderlyingDataRow = row;
        }

        /// <summary>
        /// この関係を表す <see cref="T:System.String"/> を返します。
        /// </summary>
        /// <returns>この関係を表す <see cref="T:System.String"/>。</returns>
        public override String ToString()
        {
            return String.Format("{0}: {1}", this.Activity.ToString(), this.Tag);
        }

        /// <summary>
        /// この関係のデータのバックエンドとなるデータ行の主キーのシーケンスを表すオブジェクトを取得します。
        /// </summary>
        /// <returns>この関係のデータのバックエンドとなるデータ行の主キーのシーケンスを表すオブジェクト。</returns>
        public PrimaryKeyCollection GetPrimaryKeyCollection()
        {
            return new PrimaryKeyCollection(this);
        }

        /// <summary>
        /// この関係を別のストレージへコピーします。
        /// </summary>
        /// <param name="destination">コピー先の <see cref="Storage"/>。</param>
        /// <returns>コピーされた関係。</returns>
        public TagElement Copy(Storage destination)
        {
            return destination.NewTagElement(
                this.Activity.Copy(destination),
                this.Tag
            );
        }
    }
}