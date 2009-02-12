﻿// -*- mode: csharp; encoding: utf-8; -*-
/* MetaTweet
 *   Hub system for micro-blog communication services
 * MetaTweetServer
 *   Server library of MetaTweet
 *   Part of MetaTweet
 * Copyright © 2008-2009 Takeshi KIRIYA, XSpect Project <takeshik@users.sf.net>
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

using System.Reflection;
using System;
using XSpect.MetaTweet.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Achiral;

namespace XSpect.MetaTweet.Modules
{
    public class FlowInterfaceInfo
    {
        private MethodInfo _method;

        private FlowInterfaceAttribute _attribute;

        public FlowInterfaceInfo(MethodInfo method, FlowInterfaceAttribute attribute)
        {
            this._method = method;
            this._attribute = attribute;
        }

        public String Id
        {
            get
            {
                return this._attribute.Id;
            }
        }

        public String Summary
        {
            get
            {
                return this._attribute.Summary;
            }
        }

        public String Remarks
        {
            get
            {
                return this._attribute.Remarks;
            }
        }

        public Type OutputType
        {
            get
            {
                return this._method.ReturnType;
            }
        }

        public String GetParameter(String selector)
        {
            return selector.Substring(
                this._attribute.Id.Length + (this._attribute.Id.EndsWith("/") ? 1 : 0)
            );
        }

        public TOutput Invoke<TOutput>(
            FlowModule module,
            IEnumerable<StorageObject> source,
            Storage storage,
            String parameter,
            IDictionary<String, String> arguments
        )
        {
            var x = (source != null
                        ? source
                        : Enumerable.Empty<StorageObject>()
                    )
                        .Cast<Object>()
                        .Concat(Make.Array<Object>(
                            storage,
                            parameter,
                            arguments
                        )).ToArray();





            return (TOutput) this._method.Invoke(
                module,
                (source != null
                    ? source
                    : Enumerable.Empty<StorageObject>()
                )
                    .Cast<Object>()
                    .Concat(Make.Array<Object>(
                        storage,
                        parameter,
                        arguments
                    )).ToArray()
            );
        }

        public IEnumerable<StorageObject> Invoke(
            FlowModule module,
            IEnumerable<StorageObject> source,
            Storage storage,
            String parameter,
            IDictionary<String, String> arguments
        )
        {
            return this.Invoke<IEnumerable<StorageObject>>(module, source, storage, parameter, arguments);
        }
    }
}