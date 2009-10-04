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
    public interface IAccount
        : IComparable<IAccount>
    {
        Activity this[String category]
        {
            get;
        }

        Activity this[String category, DateTime baseline]
        {
            get;
        }

        Guid AccountId
        {
            get;
            set;
        }

        String Realm
        {
            get;
            set;
        }

        IEnumerable<Activity> Activities
        {
            get;
        }

        IEnumerable<Annotation> Annotations
        {
            get;
        }

        IEnumerable<String> Annotating
        {
            get;
        }

        IEnumerable<Relation> Relations
        {
            get;
        }

        IEnumerable<Relation> ReverseRelations
        {
            get;
        }

        IEnumerable<KeyValuePair<String, Account>> Relating
        {
            get;
        }

        IEnumerable<KeyValuePair<String, Account>> Relators
        {
            get;
        }

        IEnumerable<Mark> Marks
        {
            get;
        }

        IEnumerable<KeyValuePair<String, Activity>> Marking
        {
            get;
        }

        Boolean IsAnnotating(String name);

        IEnumerable<Account> RelatingOf(String name);

        IEnumerable<Account> RelatorsOf(String name);

        IEnumerable<Activity> MarkingOf(String name);

        Activity Act(DateTime timestamp, String category, String subId, String userAgent, String value, Byte[] data);

        Activity Act(DateTime timestamp, String category, String subId);

        Annotation Annotate(String name);

        Relation Relate(String name, Account relateTo);

        Relation Related(String name, Account relatedFrom);

        Mark Mark(String name, Activity markTo);
    }
}