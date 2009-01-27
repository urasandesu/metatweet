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

namespace XSpect.MetaTweet.ObjectModel
{
    /// <summary>
    /// ポストと、ポストの返信元のポストとの関係を表します。
    /// </summary>
    /// <remarks>
    /// このクラスは一方のポストと他方のポストとの関係表の単一の行要素を表現し、
    /// その集合により多対多の関係を構成します。
    /// </remarks>
    [Serializable()]
    public class ReplyElement
        : StorageObject<StorageDataSet.ReplyMapDataTable, StorageDataSet.ReplyMapRow>
    {
        /// <summary>
        /// 返信している主体であるポストを取得または設定します。
        /// </summary>
        /// <value>
        /// 返信している主体であるポストを取得または設定します。
        /// </value>
        public Post Post
        {
            get
            {
                return this.Storage.GetPost(this.UnderlyingDataRow.PostsRowParentByFK_Posts_ReplyMap);
            }
            set
            {
                this.UnderlyingDataRow.PostsRowParentByFK_Posts_ReplyMap = value.UnderlyingDataRow;
            }
        }

        /// <summary>
        /// ポストの返信元のポストを取得または設定します。
        /// </summary>
        /// <value>
        /// ポストの返信元のポスト。
        /// </value>
        public Post InReplyToPost
        {
            get
            {
                return this.Storage.GetPost(this.UnderlyingDataRow.PostsRowParentByFK_PostsInReplyTo_ReplyMap);
            }
            set
            {
                this.UnderlyingDataRow.PostsRowParentByFK_PostsInReplyTo_ReplyMap = value.UnderlyingDataRow;
            }
        }

        public ReplyElement(
            Post post,
            Post inReplyToPost
        )
        {
            this.Post = post;
            this.InReplyToPost = post;
            this.Store();
        }

        public ReplyElement(StorageDataSet.ReplyMapRow row)
        {
            this.UnderlyingDataRow = row;
        }

        /// <summary>
        /// この関係を表す <see cref="T:System.String"/> を返します。
        /// </summary>
        /// <returns>
        /// この関係を表す <see cref="T:System.String"/>。
        /// </returns>
        public override String ToString()
        {
            return String.Format("{0} => {1}", this.Post.ToString(), this.InReplyToPost.ToString());
        }
    }
}