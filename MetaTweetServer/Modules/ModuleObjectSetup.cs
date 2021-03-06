﻿// -*- mode: csharp; encoding: utf-8; tab-width: 4; c-basic-offset: 4; indent-tabs-mode: nil; -*-
// vim:set ft=cs fenc=utf-8 ts=4 sw=4 sts=4 et:
// $Id$
/* MetaTweet
 *   Hub system for micro-blog communication services
 * MetaTweetServer
 *   Server library of MetaTweet
 *   Part of MetaTweet
 * Copyright © 2008-2010 Takeshi KIRIYA (aka takeshik) <takeshik@users.sf.net>
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
using System.Collections.ObjectModel;

namespace XSpect.MetaTweet.Modules
{
    [Serializable()]
    public sealed class ModuleObjectSetup
        : Object
    {
        private String _key;

        private String _typeName;

        private Collection<String> _options;

        public String Key
        {
            get
            {
                return this._key;
            }
            set
            {
                this._key = value ?? String.Empty;
            }
        }

        public String TypeName
        {
            get
            {
                return this._typeName;
            }
            set
            {
                this._typeName = value ?? String.Empty;
            }
        }

        public Collection<String> Options
        {
            get
            {
                return this._options;
            }
            set
            {
                this._options = value ?? new Collection<String>();
            }
        }

        public ModuleObjectSetup()
        {
            this.Options = new Collection<String>();
        }
    }
}