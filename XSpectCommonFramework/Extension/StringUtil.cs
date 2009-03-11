﻿// -*- mode: csharp; encoding: utf-8; -*-
/* XSpect Common Framework - Generic Utility Class Library
 * Copyright © 2008-2009 Takeshi KIRIYA, XSpect Project <takeshik@users.sf.net>
 * All rights reserved.
 * 
 * This file is part of XSpect Common Framework.
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
using System.IO;
using Achiral.Extension;
using Achiral;

namespace XSpect.Extension
{
    public static class StringUtil
        : Object
    {
        public static String ForEachLine(this String str, Func<String, String> selector)
        {
            return str
                .Split(Make.Array(Environment.NewLine), StringSplitOptions.None)
                .Select(selector)
                .Join(Environment.NewLine);
        }

        public static String Indent(this String str, Int32 columnCount)
        {
            return str.ForEachLine(s => new String(' ', columnCount) + s);
        }

        public static String Unindent(this String str, Int32 columnCount)
        {
            return null;
        }
    }
}