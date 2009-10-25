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
using System.Linq;

namespace XSpect.MetaTweet.Objects
{
    partial class Relation
        : IRelation
    {
        /// <summary>
        /// <see cref="Relation"/> の新しいインスタンスを初期化します。
        /// </summary>
        private Relation()
        {
        }

        /// <summary>
        /// <see cref="Relation"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="storage">The storage.</param>
        internal Relation(Storage storage)
            : base(storage)
        {
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override String ToString()
        {
            return String.Format(
                "Rel [{0}]: {1} -> [{2}]",
                this.Account,
                this.Name,
                this.RelatingAccount
            );
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This object is less than the <paramref name="other"/> parameter.
        /// Zero
        /// This object is equal to <paramref name="other"/>.
        /// Greater than zero
        /// This object is greater than <paramref name="other"/>.
        /// </returns>
        public override Int32 CompareTo(StorageObject other)
        {
            if (!(other is Relation))
            {
                throw new ArgumentException("other");
            }
            return this.CompareTo(other as Relation);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This object is less than the <paramref name="other"/> parameter.
        /// Zero
        /// This object is equal to <paramref name="other"/>.
        /// Greater than zero
        /// This object is greater than <paramref name="other"/>.
        /// </returns>
        public Int32 CompareTo(IRelation other)
        {
            // Account -> Name -> RelatingAccount
            Int32 result;
            return other == null
                ? 1
                : (result = this.Account.CompareTo(other.Account)) != 0
                      ? result
                      : (result = this.Name.CompareTo(other.Name)) != 0
                            ? result
                            : this.RelatingAccount.CompareTo(other.RelatingAccount);
        }

        // NOTE: Alternative implementation.
        /// <summary>
        /// このリレーションが関連付けられる先のアカウントを取得または設定します。
        /// </summary>
        /// <value>このリレーションが関連付けられる先のアカウント。</value>
        public Account RelatingAccount
        {
            get
            {
                return this.Storage.GetAccounts(this.RelatingAccountId).FirstOrDefault();
            }
            set
            {
                this.RelatingAccountId = value.AccountId;
            }
        }
    }
}