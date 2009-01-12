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
using System.Linq;

namespace XSpect.MetaTweet.ObjectModel
{
    [Serializable()]
    public class Account
        : StorageObject<StorageDataSet.AccountsDataTable, StorageDataSet.AccountsRow>,
          IComparable<Account>
    {
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

        public String this[String category]
        {
            get
            {
                return this.GetActivityOf(category).Value;
            }
        }

        public IEnumerable<FollowElement> FollowingMap
        {
            get
            {
                return this.Storage.GetFollowElements(this.UnderlyingDataRow.GetFollowMapRowsByFK_Accounts_FollowMap());
            }
        }

        public IEnumerable<Account> Following
        {
            get
            {
                return this.FollowingMap.Select(e => e.FollowingAccount);
            }
        }

        public IEnumerable<FollowElement> FollowersMap
        {
            get
            {
                return this.Storage.GetFollowElements(this.UnderlyingDataRow.GetFollowMapRowsByFK_AccountsFollowing_FollowMap());
            }
        }

        public IEnumerable<Account> Followers
        {
            get
            {
                return this.FollowersMap.Select(e => e.Account);
            }
        }

        public IEnumerable<Activity> Activities
        {
            get
            {
                return this.Storage.GetActivities(this.UnderlyingDataRow.GetActivitiesRows());
            }
        }

        internal Account()
        {
        }

        public override String ToString()
        {
            return String.Format("{0}@{1}", this.AccountId.ToString("d"), this.Realm);
        }

        public Int32 CompareTo(Account other)
        {
            return this.AccountId.CompareTo(other.AccountId);
        }

        public void AddFollowing(Account account)
        {
            FollowElement element = this.Storage.NewFollowElement();
            element.Account = this;
            element.FollowingAccount = account;
            element.Update();
        }

        public void AddFollower(Account account)
        {
            FollowElement element = this.Storage.NewFollowElement();
            element.Account = account;
            element.FollowingAccount = this;
            element.Update();
        }

        public void RemoveFollowing(Account account)
        {
            FollowElement element = this.FollowingMap.Where(e => e.FollowingAccount == account).Single();
            element.Delete();
            element.Update();
        }

        public void RemoveFollower(Account account)
        {
            FollowElement element = this.FollowersMap.Where(e => e.Account == account).Single();
            element.Delete();
            element.Update();
        }

        public Activity NewActivity()
        {
            Activity activity = this.Storage.NewActivity();
            activity.Account = this;
            return activity;
        }

        public Activity GetActivityOf(String category)
        {
            return this.Activities.Where(a => a.Category == category).OrderByDescending(a => a.Timestamp).First();
        }
    }
}