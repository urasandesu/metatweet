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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Achiral;
using Achiral.Extension;
using Microsoft.Scripting.Hosting;
using XSpect.Configuration;
using XSpect.Extension;

namespace XSpect.MetaTweet.Clients.Mint
{
    public partial class SplashForm
        : Form
    {
        public ClientCore Client
        {
            get;
            private set;
        }

        public SplashForm(ClientCore client)
        {
            this.Client = client;
            InitializeComponent();
        }

        private void SplashForm_Shown(Object sender, EventArgs e)
        {
            Application.DoEvents();
            FileInfo[] codes = this.Client.Directories.ConfigDirectory
                .CreateSubdirectory("Mint")
                .CreateSubdirectory("code.d")
                .GetFiles();
            this.progressBar.Maximum = codes.Length;
            this.ExecuteScripts(codes);
            this.Client.MainForm.Show();
            this.Close();
        }

        private void ExecuteScripts(IEnumerable<FileInfo> codes)
        {
            codes.SingleOrDefault(f => f.Name.StartsWith("init."))
                .If(f => f != null,
                    f => f.ToEnumerable().Do(_ => _.Concat(codes.Except(_))).ForEach(ExecuteScript),
                    _ => Initializer.Initialize(Make.Dictionary<Object>(host => this.Client))
                );
        }

        private void ExecuteScript(FileInfo code)
        {
            this.progressLabel.Text = "Evaluating: " + code.Name;
            Application.DoEvents();
            this.Client.CodeManager.Execute<Object>(code, Make.Dictionary<Object>(host => this.Client));
            this.progressBar.Increment(1);
        }
    }
}
