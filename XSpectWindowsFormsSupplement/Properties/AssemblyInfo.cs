﻿// -*- mode: csharp; encoding: utf-8; -*-
/* XSpect Windows Forms Supplement - Supplemental library for Windows Forms
 * Copyright © 2009 Takeshi KIRIYA, XSpect Project <takeshik@users.sf.net>
 * All rights reserved.
 * 
 * This file is part of XSpect Windows Forms Supplement.
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
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: CLSCompliant(true)]

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("XSpect Windows Forms Supplement")]
[assembly: AssemblyDescription("Supplemental library for Windows Forms")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("XSpect Project")]
[assembly: AssemblyProduct("XSpect Windows Forms Supplement")]
[assembly: AssemblyCopyright("Copyright © 2009 Takeshi KIRIYA, XSpect Project <takeshik@users.sf.net>, All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("e71c2457-1ffd-4163-b0f2-4bd554fb10c6")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

#pragma warning disable 1699

// HACK: Copied /XSpect.snk to /XSpectWindowsFormsSupplement/XSpect.snk
// HACK: Specified "XSpect.snk" as the path of the key instead of "../XSpect.snk"
// because the satellite assemblies couldn't sign.
[assembly: AssemblyKeyFile(@"XSpect.snk")]
[assembly: AssemblyDelaySign(false)]

#pragma warning restore 1699
