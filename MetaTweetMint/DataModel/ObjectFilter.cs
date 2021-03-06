// -*- mode: csharp; encoding: utf-8; tab-width: 4; c-basic-offset: 4; indent-tabs-mode: nil; -*-
// vim:set ft=cs fenc=utf-8 ts=4 sw=4 sts=4 et:
// $Id$
/* MetaTweet
 *   Hub system for micro-blog communication services
 * MetaTweetMint
 *   Extensible GUI client for MetaTweet
 *   Part of MetaTweet
 * Copyright © 2009-2010 Takeshi KIRIYA (aka takeshik) <takeshik@users.sf.net>
 * All rights reserved.
 * 
 * This file is part of MetaTweetMint.
 * 
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or (at your
 * option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but
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
using XSpect.Collections;
using XSpect.Configuration;

namespace XSpect.MetaTweet.Clients.Mint.DataModel
{
    public class ObjectFilter
        : Object
    {
        public XmlConfiguration.Entry<ObjectFilterConfiguration> Configuration
        {
            get;
            private set;
        }

        public String Name
        {
            get;
            private set;
        }

        public ObjectView ParentView
        {
            get;
            private set;
        }

        public ObjectFilter ParentFilter
        {
            get;
            private set;
        }

        public HybridDictionary<String, ObjectFilter> ChildFilters
        {
            get;
            private set;
        }

        public Predicate<IList<Object>> Predicate
        {
            get;
            set;
        }

        private ObjectFilter(String name)
        {
            this.ChildFilters = new HybridDictionary<String, ObjectFilter>((i, f) => f.Name);
            this.Name = name;
        }

        public ObjectFilter(String name, ObjectView parent)
            : this(name)
        {
            this.ParentView = parent;
            this.ParentFilter = null;
        }

        public ObjectFilter(String name, ObjectFilter parent)
            : this(name)
        {
            this.ParentView = parent.ParentView;
            this.ParentFilter = parent;
        }
    }
}
