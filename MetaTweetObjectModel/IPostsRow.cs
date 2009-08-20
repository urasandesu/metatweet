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
using XSpect.MetaTweet.ObjectModel;

namespace XSpect.MetaTweet
{
    /// <summary>
    /// Posts テーブルの行を表します。
    /// </summary>
    public interface IPostsRow
        : IRow
    {
        /// <summary>
        /// <c>AccountId</c> 列の値を取得または設定します。
        /// </summary>
        /// <value><c>AccountId</c> 列の値。</value>
        /// <remarks>このプロパティは <see cref="Post.Activity"/> の <see cref="ObjectModel.Activity.Account"/> の <see cref="ObjectModel.Account.AccountId"/> に対応します。</remarks>
        Guid AccountId
        {
            get;
            set;
        }

        /// <summary>
        /// <c>PostId</c> 列の値を取得または設定します。
        /// </summary>
        /// <value><c>PostId</c> 列の値。</value>
        /// <remarks>このプロパティは <see cref="Post.PostId"/> に対応します。</remarks>
        String PostId
        {
            get;
            set;
        }

        /// <summary>
        /// <c>Text</c> 列の値を取得または設定します。
        /// </summary>
        /// <value><c>Text</c> 列の値。</value>
        /// <remarks>このプロパティは <see cref="Post.Text"/> に対応します。</remarks>
        String Text
        {
            get;
            set;
        }

        /// <summary>
        /// <c>Source</c> 列の値を取得または設定します。
        /// </summary>
        /// <value><c>Source</c> 列の値。</value>
        /// <remarks>このプロパティは <see cref="Post.Source"/> に対応します。</remarks>
        String Source
        {
            get;
            set;
        }
    }
}