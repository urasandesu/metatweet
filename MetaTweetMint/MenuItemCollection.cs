﻿// -*- mode: csharp; encoding: utf-8; tab-width: 4; c-basic-offset: 4; indent-tabs-mode: nil; -*-
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
using System.Linq;
using System.Windows.Forms;
using Achiral;
using Achiral.Extension;
using XSpect.Collections;
using XSpect.Extension;

namespace XSpect.MetaTweet.Clients.Mint
{
    public class MenuItemCollection
        : HybridDictionary<String, ToolStripItem>
    {
        public MainForm Form
        {
            get;
            private set;
        }

        public MenuItemCollection(MainForm form)
            : base((idx, item) => item.Walk(t => t.OwnerItem).Reverse().Select(t => t.Name).Join("/"))
        {
            this.Form = form;
        }

        public ToolStripMenuItem Add(String key, String text)
        {
            return new ToolStripMenuItem(text)
            {
                Name = key,
            }.Let(
                t => t.Click += (sender, e) => this.Form.Client.EvaluateFunction("menu:" + t.Name, null, false),
                this.Add
            );
        }

        protected override void InsertItems(IEnumerable<Int32> indexes, IEnumerable<String> keys, IEnumerable<ToolStripItem> values, Boolean ensureKeysCompliant)
        {
            Create.Dictionary(keys, values).ForEach(p =>
                p.Key.LastIndexOf('/').Do(i => i > 0
                    ? (this[p.Key.Remove(i)] as ToolStripMenuItem).DropDown.Items
                    : this.Form.MainMenuStrip.Items
                ).Add(p.Value)
            );
            base.InsertItems(indexes, keys, values, ensureKeysCompliant);
        }

        protected override IEnumerable<Boolean> RemoveItems(IEnumerable<Int32> indexes)
        {
            // Expand remove-list into their descendant entries:
            return base.RemoveItems(indexes
                .Select(i => this[i].Key)
                // Be filtered itself (cf. "foo".StartsWith("foo")) and its descendants.
                .Where(k => this.Keys.Any(_ => _.StartsWith(k)))
                .Select(k => this.Keys.IndexOf(k))
            );
        }
    }
}