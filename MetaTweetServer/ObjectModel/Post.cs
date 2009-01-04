﻿// -*- mode: csharp; encoding: utf-8; -*-
/* MetaTweet
 *   Hub system for micro-blog communication services
 * MetaTweetServer
 *   Server library of MetaTweet
 *   Part of MetaTweet
 * Copyright © 2008-2009 Takeshi KIRIYA, XSpect Project <takeshik@xspect.org>
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

namespace XSpect.MetaTweet.ObjectModel
{
    [Serializable()]
    public class Post
        : StorageObject<StorageDataSet.PostsRow>,
          IComparable<Post>
    {
        private Activity _activity;

        private String _postId;

        private Nullable<DateTime> _timestamp;
        
        private String _text;
        
        private String _source;
        
        private Nullable<Int32> _favoriteCount;
        
        private Nullable<Boolean> _isRead;
        
        private Nullable<Boolean> _isFavorited;
        
        private Nullable<Boolean> _isReply;

        private Nullable<Boolean> _isRestricted;

        private ReplyMap _replyMap;

        public Activity Activity
        {
            get
            {
                // Activity must be set in constructing.
                return this._activity;
            }
            set
            {
                this.UnderlyingDataRow.AccountId = value.Account.AccountId;
                this._activity = value;
            }
        }
        
        public String PostId
        {
            get
            {
                return this._postId ?? (this._postId = this.UnderlyingDataRow.PostId);
            }
            set
            {
                this.UnderlyingDataRow.PostId = value;
                this._postId = value;
            }
        }

        public DateTime Timestamp
        {
            get
            {
                if (!this._timestamp.HasValue)
                {
                    this._timestamp = this.UnderlyingDataRow.Timestamp;
                }
                return this._timestamp.Value;
            }
            set
            {
                this.UnderlyingDataRow.Timestamp = value;
                this._timestamp = value;
            }
        }

        public String Text
        {
            get
            {
                return this._text ?? (this._text = this.UnderlyingDataRow.Text);
            }
            set
            {
                this.UnderlyingDataRow.Text = value;
                this._text = value;
            }
        }

        public String Source
        {
            get
            {
                return this._source ?? (this._source = this.UnderlyingDataRow.Source);
            }
            set
            {
                this.UnderlyingDataRow.Source = value;
                this._source = value;
            }
        }

        public Nullable<Int32> FavoriteCount
        {
            get
            {
                if (this.UnderlyingDataRow.IsFavoriteCountNull())
                {
                    return null;
                }
                else if (!this._favoriteCount.HasValue)
                {
                    this._favoriteCount = this.UnderlyingDataRow.FavoriteCount;
                }
                return this._favoriteCount;
            }
            set
            {
                if (value == null)
                {
                    this.UnderlyingDataRow.SetFavoriteCountNull();
                }
                else
                {
                    this._favoriteCount = value;
                }
            }
        }

        public Boolean IsRead
        {
            get
            {
                if (!this._isRead.HasValue)
                {
                    this._isRead = this.UnderlyingDataRow.IsRead;
                }
                return this._isRead.Value;
            }
            set
            {
                this.UnderlyingDataRow.IsRead = value;
                this._isRead = value;
            }
        }

        public Boolean IsFavorited
        {
            get
            {
                if (!this._isFavorited.HasValue)
                {
                    this._isFavorited = this.UnderlyingDataRow.IsFavorited;
                }
                return this._isFavorited.Value;
            }
            set
            {
                this.UnderlyingDataRow.IsFavorited = value;
                this._isFavorited = value;
            }
        }

        public Boolean IsReply
        {
            get
            {
                if (!this._isReply.HasValue)
                {
                    this._isReply = this.UnderlyingDataRow.IsReply;
                }
                return this._isReply.Value;
            }
            set
            {
                this.UnderlyingDataRow.IsReply = value;
                this._isReply = value;
            }
        }

        public Boolean IsRestricted
        {
            get
            {
                if (!this._isRestricted.HasValue)
                {
                    this._isRestricted = this.UnderlyingDataRow.IsRestricted;
                }
                return _isRestricted.Value;
            }
            set
            {
                this.UnderlyingDataRow.IsRestricted = value;
                this._isRestricted = value;
            }
        }

        public ReplyMap ReplyMap
        {
            get
            {
                return this._replyMap ?? (this._replyMap = this.Storage.GetReplyMap(this));
            }
        }

        public IEnumerable<Post> Replies
        {
            get
            {
                return this._replyMap.GetReplies(this);
            }
        }

        public IEnumerable<Post> Replying
        {
            get
            {
                return this._replyMap.GetReplying(this);
            }
        }

        public Int32 CompareTo(Post other)
        {
            Int32 result;
            if ((result = this._activity.CompareTo(other._activity)) != 0)
            {
                return result;
            }
            else
            {
                return this._postId.CompareTo((other as Post)._postId);
            }
        }

        public override Boolean Equals(Object obj)
        {
            Post other = obj as Post;
            return this._activity == other.Activity && this._postId == other._postId;
        }

        public override Int32 GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}