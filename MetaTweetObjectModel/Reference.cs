// -*- mode: csharp; encoding: utf-8; tab-width: 4; c-basic-offset: 4; indent-tabs-mode: nil; -*-
// vim:set ft=cs fenc=utf-8 ts=4 sw=4 sts=4 et:
// $Id$
/* MetaTweet
 *   Hub system for micro-blog communication services
 * MetaTweetObjectModel
 *   Object model and Storage interface for MetaTweet and other systems
 *   Part of MetaTweet
 * Copyright © 2008-2010 Takeshi KIRIYA (aka takeshik) <takeshik@users.sf.net>
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
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;

namespace XSpect.MetaTweet.Objects
{
    [KnownType(typeof(Activity))]
    partial class Reference
        : IReference,
          IComparable<Reference>,
          IEquatable<Reference>
    {
        /// <summary>
        /// オブジェクトの種類を取得します。
        /// </summary>
        /// <value>オブジェクトの種類を表す <see cref="StorageObjectTypes"/> 値。</value>
        public override StorageObjectTypes ObjectType
        {
            get
            {
                return StorageObjectTypes.Reference;
            }
        }

        protected override RelatedEnd ContextHolder
        {
            get
            {
                return this.ActivityReference;
            }
        }

        /// <summary>
        /// <see cref="Reference"/> の新しいインスタンスを初期化します。
        /// </summary>
        private Reference()
        {
        }

        /// <summary>
        /// <see cref="Reference"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="storage">オブジェクトが追加されるストレージ。</param>
        internal Reference(Storage storage)
            : base(storage)
        {
        }

        /// <summary>
        /// シリアル化したデータを使用して、<see cref="Reference"/> クラスの新しいインスタンスを初期化します。 
        /// </summary>
        /// <param name="info">シリアル化済みオブジェクト データを保持している <see cref="SerializationInfo"/>。</param>
        /// <param name="context">転送元または転送先に関するコンテキスト情報を含んでいる <see cref="StreamingContext"/>。</param>
        protected Reference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Activity = (Activity) info.GetValue("Activity", typeof(Activity));
            this.AccountId = this.Activity.AccountId;
            this.Timestamp = this.Activity.Timestamp;
            this.Category = this.Activity.Category;
            this.SubId = this.Activity.SubId;
            this.Name = (String) info.GetValue("Name", typeof(String));
            this.ReferringActivity = (Activity) info.GetValue("ReferringActivity", typeof(Activity));
            // ReferringAccountId, ReferringTimestamp, ReferringCategory and ReferringSubId must be
            // set at set_ReferringActivity.
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override Boolean Equals(Object obj)
        {
            return
                !ReferenceEquals(null, obj) &&
                ReferenceEquals(this, obj) ||
                ((obj is IReference) && this.Equals((IReference) obj));
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override Int32 GetHashCode()
        {
            return unchecked(
                (this._AccountId != null ? this._AccountId.GetHashCode() * 397 : 0) ^
                this._Timestamp.GetHashCode() * 397 ^
                (this._Category != null ? this._Category.GetHashCode() * 397 : 0) ^
                (this._SubId != null ? this._SubId.GetHashCode() * 397 : 0) ^
                (this._Name != null ? this._Name.GetHashCode() * 397 : 0) ^
                (this._ReferringAccountId != null ? this._ReferringAccountId.GetHashCode() * 397 : 0) ^
                this._ReferringTimestamp.GetHashCode() * 397 ^
                (this._ReferringCategory != null ? this._ReferringCategory.GetHashCode() * 397 : 0) ^
                (this._ReferringSubId != null ? this._ReferringSubId.GetHashCode() : 0)
            );
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
                "Ref [{0}]: {1} -> [{2}]",
                this.Activity,
                this.Name,
                this.ReferringActivity
            );
        }

        /// <summary>
        /// このオブジェクトの完全な内容を表す <see cref="String"/> を返します。
        /// </summary>
        /// <returns>このオブジェクトの完全な内容を表す <see cref="String"/>。</returns>
        public override String Describe()
        {
            return String.Format(
                "Ref [{0}]: {1} -> [{2}]",
                this.Activity.Describe(),
                this.Name,
                this.ReferringActivity.Describe()
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
            if (!(other is Reference))
            {
                throw new ArgumentException("other");
            }
            return this.CompareTo((Reference) other);
        }

        /// <summary>
        /// 指定したストレージ オブジェクトが、このストレージ オブジェクトと完全に等しいかどうかを判断します。
        /// </summary>
        /// <param name="other">このストレージ オブジェクトと比較するストレージ オブジェクト。</param>
        /// <returns>
        /// 指定したストレージ オブジェクトがこのストレージ オブジェクトと完全に等しい場合は <c>true</c>。それ以外の場合は <c>false</c>。
        /// </returns>
        public override Boolean EqualsExact(StorageObject other)
        {
            return other is Reference
                ? this.EqualsExact((Reference) other)
                : other is IReference
                      ? this.EqualsExact((IReference) other)
                      : false;
        }

        /// <summary>
        /// オブジェクトをシリアル化するために必要なデータをシリアル化情報オブジェクトに設定します。
        /// </summary>
        /// <param name="info">オブジェクトと関連付けられているシリアル化データを保持する <see cref="SerializationInfo"/>。</param>
        /// <param name="context">オブジェクトに関連付けられているシリアル化ストリームのソースおよびデスティネーションを格納している <see cref="StreamingContext"/>。</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Activity", this.Activity, typeof(Activity));
            info.AddValue("Name", this.Name, typeof(String));
            info.AddValue("ReferringActivity", this.ReferringActivity, typeof(Activity));
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
        public Int32 CompareTo(IReference other)
        {
            // Activity -> Name -> ReferringActivity
            Int32 result;
            return other == null
                ? 1
                : (result = this.Activity.CompareTo(other.Activity)) != 0
                      ? result
                      : (result = this.Name.CompareTo(other.Name)) != 0
                            ? result
                            : this.ReferringActivity.CompareTo(other.ReferringActivity);
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
        public Int32 CompareTo(Reference other)
        {
            return this.CompareTo((IReference) other);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public Boolean Equals(IReference other)
        {
            return !ReferenceEquals(other, null) && (
                ReferenceEquals(this, other)
                || this.Activity.Equals(other.Activity)
                && this.Name.Equals(other.Name)
                && this.ReferringActivity.Equals(other.ReferringActivity)
            );
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public Boolean Equals(Reference other)
        {
            return this.Equals((IReference) other);
        }

        /// <summary>
        /// 指定したリファレンスが、このリファレンスと完全に等しいかどうかを判断します。
        /// </summary>
        /// <param name="other">このリファレンスと比較するリファレンス。</param>
        /// <returns>
        /// 指定したリファレンスがこのリファレンスと完全に等しい場合は <c>true</c>。それ以外の場合は <c>false</c>。
        /// </returns>
        public Boolean EqualsExact(IReference other)
        {
            return !ReferenceEquals(other, null) && (
                ReferenceEquals(this, other)
                || this.Activity.EqualsExact(other.Activity)
                && this.Name.Equals(other.Name)
                && this.ReferringActivity.EqualsExact(other.ReferringActivity)
            );
        }

        /// <summary>
        /// 指定したリファレンスが、所属するストレージを含め、このリファレンスと完全に等しいかどうかを判断します。
        /// </summary>
        /// <param name="other">このリファレンスと比較するリファレンス。</param>
        /// <returns>
        /// 指定したリファレンスが、所属するストレージを含め、このリファレンスと完全に等しい場合は <c>true</c>。それ以外の場合は <c>false</c>。
        /// </returns>
        public Boolean EqualsExact(Reference other)
        {
            return this.Storage == other.Storage
                && this.EqualsExact((IReference) other);
        }

        #region Alternative Implementations

        /// <summary>
        /// このリファレンスが関連付けられる先のアクティビティを取得または設定します。
        /// </summary>
        /// <value>このリファレンスが関連付けられる先のアクティビティ。</value>
        public Activity ReferringActivity
        {
            get
            {
                return this.Storage.GetActivities(
                    this.ReferringAccountId,
                    this.ReferringTimestamp,
                    this.ReferringCategory,
                    this.ReferringSubId
                )
                    .Single();
            }
            set
            {
                this.ReferringAccountId = value.AccountId;
                this.ReferringTimestamp = value.Timestamp;
                this.ReferringCategory = value.Category;
                this.ReferringSubId = value.SubId;
            }
        }

        #endregion

        #region Implicit Implementations

        /// <summary>
        /// このリファレンスが関連付けられているアクティビティを取得または設定します。
        /// </summary>
        /// <value>このリファレンスが関連付けられているアクティビティ。</value>
        IActivity IReference.Activity
        {
            get
            {
                return this.Activity;
            }
            set
            {
                this.Activity = (Activity) value;
            }
        }

        /// <summary>
        /// このリファレンスが関連付けられる先のアクティビティを取得または設定します。
        /// </summary>
        /// <value>このリファレンスが関連付けられる先のアクティビティ。</value>
        IActivity IReference.ReferringActivity
        {
            get
            {
                return this.ReferringActivity;
            }
            set
            {
                this.ReferringActivity = (Activity) value;
            }
        }

        #endregion
    }
}