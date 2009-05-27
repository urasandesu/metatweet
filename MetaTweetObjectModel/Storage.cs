﻿// -*- mode: csharp; encoding: utf-8; -*-
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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using XSpect.MetaTweet.ObjectModel;

namespace XSpect.MetaTweet
{
    /// <summary>
    /// オブジェクト モデルの生成、格納、操作を提供する基本クラスです。
    /// </summary>
    /// <remarks>
    /// <para>MetaTweet のオブジェクト モデルは、ストレージ オブジェクト、データセット、バックエンドの三層で構成されます。</para>
    /// <para>バックエンドはオブジェクト モデルを効率的に外部記憶に格納し、データセットに対しデータの提供を行います。通常、リレーショナルデータベースの使用が期待されています。</para>
    /// <para>データセットはバックエンドから取得したデータ構造を表形式で保持し、追加、修正、および削除を行い、バックエンドに対し変更点の更新を行います。</para>
    /// <para>ストレージ オブジェクトはデータセット上のデータ行を参照し、表形式のデータ構造を通常のオブジェクト構造として公開し、データの抽象的な追加、修正、および削除の機能を提供します。</para>
    /// <para>データセット、ストレージ オブジェクトとデータセットはオブジェクト モデルにおいて厳密に定義されています。バックエンドと他の層に関してはインターフェイスのみ厳密に定義されています。</para>
    /// <para>ストレージは、バックエンドとの接続、切断、およびデータセット間の入出力のインターフェイス、データセット内のデータを検索し、また表形式のデータからストレージ オブジェクトを生成する機能を提供します。</para>
    /// </remarks>
    public abstract class Storage
        : MarshalByRefObject,
          IDisposable
    {
        private Boolean _disposed;

        /// <summary>
        /// バックエンドから取得し、またはストレージ オブジェクトにより追加されたデータ行を格納するデータセットを取得または設定します。
        /// </summary>
        public StorageDataSet UnderlyingDataSet
        {
            get;
            private set;
        }

        /// <summary>
        /// このストレージのキャッシュを取得または設定します。
        /// </summary>
        /// <value>
        /// このストレージのキャッシュ。
        /// </value>
        public StorageCache Cache
        {
            get;
            set;
        }

        /// <summary>
        /// <see cref="Storage"/> の新しいインスタンスを初期化します。
        /// </summary>
        protected Storage()
        {
            this.UnderlyingDataSet = new StorageDataSet();
            this.Cache = new StorageCache(this);
        }

        /// <summary>
        /// 対象のインスタンスの有効期間ポリシーを制御する、有効期間サービス オブジェクトを取得します。
        /// </summary>
        /// <returns>
        /// 対象のインスタンスの有効期間ポリシーを制御するときに使用する、<see cref="T:System.Runtime.Remoting.Lifetime.ILease"/> 型のオブジェクト。存在する場合は、このインスタンスの現在の有効期間サービス オブジェクトです。それ以外の場合は、<see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"/> プロパティの値に初期化された新しい有効期間サービス オブジェクトです。
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">直前の呼び出し元に、インフラストラクチャ アクセス許可がありません。</exception>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure"/>
        /// </PermissionSet>
        public override Object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// 派生クラスで実装された場合、バックエンドのデータソースとの接続を初期化します。
        /// </summary>
        /// <param name="connectionString">接続に使用する文字列。</param>
        public abstract void Initialize(String connectionString);

        /// <summary>
        /// <see cref="Storage"/> によって使用されているすべてのリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <see cref="Storage"/> によって使用されているアンマネージ リソースを解放し、オプションでマネージ リソースも解放します。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 <c>true</c>、破棄されない場合は <c>false</c>。</param>
        protected virtual void Dispose(Boolean disposing)
        {
            this.UnderlyingDataSet.Dispose();
            this._disposed = true;
        }

        /// <summary>
        /// オブジェクトが破棄されているかどうかを確認し、破棄されている場合は例外を送出します。
        /// </summary>
        protected void CheckIfDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        #region Accounts
        /// <summary>
        /// 選択を行う文に後続するクエリ節を指定してバックエンドのデータソースからアカウントのデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        public virtual StorageDataSet.AccountsDataTable LoadAccountsDataTable(String clauses)
        {
            this.CheckIfDisposed();
            this.UnderlyingDataSet.Accounts.BeginLoadData();
            StorageDataSet.AccountsDataTable table = this.LoadAccountsDataTableImpl(clauses);
            this.UnderlyingDataSet.Accounts.EndLoadData();
            return table;
        }

        /// <summary>
        /// 派生クラスで実装された場合、<see cref="LoadAccountsDataTable(String)"/> メソッドを実装し、実際にデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        protected abstract StorageDataSet.AccountsDataTable LoadAccountsDataTableImpl(String clauses);

        /// <summary>
        /// 列の値を指定してバックエンドのデータソースからアカウントのデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="accountId">アカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="realm">アカウントに関連付けられているサービスを表す文字列。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        public virtual StorageDataSet.AccountsDataTable LoadAccountsDataTable(
            Nullable<Guid> accountId,
            String realm
        )
        {
            List<String> whereClauses = new List<String>();
            if (accountId.HasValue)
            {
                whereClauses.Add(String.Format("[AccountId] == '{0}'", accountId.Value.ToString("d")));
            }
            if (realm != null)
            {
                whereClauses.Add(String.Format("[Realm] == '{0}'", realm));
            }
            return this.LoadAccountsDataTable(whereClauses.Count > 0
                ? "WHERE " + whereClauses.Single()
                : String.Empty
            );
        }

        /// <summary>
        /// 主キーを指定してバックエンドのデータソースからアカウントのデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="accountId">アカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        public StorageDataSet.AccountsDataTable LoadAccountsDataTable(
            Nullable<Guid> accountId
        )
        {
            return this.LoadAccountsDataTable(accountId, null);
        }

        /// <summary>
        /// バックエンドのデータソースからアカウントのデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <returns>データソースから読み出したデータ表。</returns>
        public StorageDataSet.AccountsDataTable LoadAccountsDataTable()
        {
            return this.LoadAccountsDataTable(null, null);
        }

        /// <summary>
        /// 選択を行う文に後続するクエリ節を指定してバックエンドのデータソースからアカウントのデータ表を読み出し、データセット上のデータ表とマージした上で、アカウントを生成します。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表から生成されたアカウントのシーケンス。</returns>
        public IEnumerable<Account> LoadAccounts(String clauses)
        {
            return this.GetAccounts(this.LoadAccountsDataTable(clauses));
        }

        /// <summary>
        /// 列の値を指定してバックエンドのデータソースからアカウントのデータ表を読み出し、データセット上のデータ表とマージした上で、アカウントを生成します。
        /// </summary>
        /// <param name="accountId">アカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="realm">アカウントに関連付けられているサービスを表す文字列。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表から生成されたアカウントのシーケンス。</returns>
        public IEnumerable<Account> LoadAccounts(
            Nullable<Guid> accountId,
            String realm
        )
        {
            return this.GetAccounts(this.LoadAccountsDataTable(accountId, realm));
        }

        /// <summary>
        /// 主キーを指定してバックエンドのデータソースからアカウントのデータ表を読み出し、データセット上のデータ表とマージした上で、アカウントを生成します。
        /// </summary>
        /// <param name="accountId">アカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表から生成されたアカウントのシーケンス。</returns>
        public IEnumerable<Account> LoadAccounts(
            Nullable<Guid> accountId
        )
        {
            return this.GetAccounts(this.LoadAccountsDataTable(accountId, null));
        }

        /// <summary>
        /// バックエンドのデータソースからアカウントのデータ表を読み出し、データセット上のデータ表とマージした上で、アカウントを生成します。
        /// </summary>
        /// <returns>データソースから読み出したデータ表から生成されたアカウントのシーケンス。</returns>
        public IEnumerable<Account> LoadAccounts()
        {
            return this.GetAccounts(this.LoadAccountsDataTable(null, null));
        }

        /// <summary>
        /// データセット内で該当するデータ表の全てのデータ行を用いてアカウントを生成します。
        /// </summary>
        /// <returns>
        /// データセット内で該当するデータ表の全てのデータ行を用いて生成されたアカウントのシーケンス。
        /// </returns>
        public IEnumerable<Account> GetAccounts()
        {
            return this.GetAccounts(row => true);
        }

        /// <summary>
        /// 指定された条件に合致するデータ行を用いてアカウントを生成します。
        /// </summary>
        /// <param name="predicate">各データ行が条件に当てはまるかどうかをテストする関数。</param>
        /// <returns>条件に合致したデータ行を用いて生成されたアカウントのシーケンス。</returns>
        public IEnumerable<Account> GetAccounts(Func<StorageDataSet.AccountsRow, Boolean> predicate)
        {
            return this.GetAccounts(this.UnderlyingDataSet.Accounts.Where(predicate));
        }

        /// <summary>
        /// 列の値を指定して、合致するデータ行を用いてアカウントを生成します。
        /// </summary>
        /// <param name="accountId">アカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="realm">アカウントに関連付けられているサービスを表す文字列。指定しない場合は <c>null</c>。</param>
        /// <returns>条件に合致したデータ行を用いて生成されたアカウントのシーケンス。</returns>
        public IEnumerable<Account> GetAccounts(
            Nullable<Guid> accountId,
            String realm
        )
        {
            IEnumerable<Account> accounts = this.GetAccounts();
            if (accountId.HasValue)
            {
                accounts = accounts.Where(a => a.UnderlyingDataRow.AccountId == accountId.Value);
            }
            if (realm != null)
            {
                accounts = accounts.Where(a => a.UnderlyingDataRow.Realm == realm);
            }
            return accounts;
        }

        /// <summary>
        /// 主キーを指定して、合致するデータ行を用いてアカウントを生成します。
        /// </summary>
        /// <param name="accountId">アカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <returns>条件に合致したデータ行を用いて生成されたアカウントのシーケンス。</returns>
        public IEnumerable<Account> GetAccounts(
            Nullable<Guid> accountId
        )
        {
            return this.GetAccounts(accountId, null);
        }

        /// <summary>
        /// データ行のシーケンスを用いてアカウントを生成します。
        /// </summary>
        /// <param name="rows">生成に用いるデータ行のシーケンス。</param>
        /// <returns>生成されたアカウントのシーケンス。</returns>
        public IEnumerable<Account> GetAccounts(IEnumerable<StorageDataSet.AccountsRow> rows)
        {
            return rows.Select(row => this.GetAccount(row)).ToList();
        }

        /// <summary>
        /// データ行からアカウントを生成します。
        /// </summary>
        /// <param name="row">生成に用いるデータ行。</param>
        /// <returns>生成されたアカウント。</returns>
        public virtual Account GetAccount(StorageDataSet.AccountsRow row)
        {
            this.CheckIfDisposed();
            if (row == null)
            {
                return null;
            }
            Account account = new Account()
            {
                Storage = this,
                UnderlyingDataRow = row,
            };
            account.Connect();
            return account;
        }

        /// <summary>
        /// 値を指定して、このストレージを使用するアカウントを初期化します。既にバックエンドのデータソースに対応するデータ行が存在する場合は、データセットにロードされ、そこから生成されたアカウントを返します。
        /// </summary>
        /// <param name="accountId">アカウントを一意に識別するグローバル一意識別子 (GUID) 値。</param>
        /// <param name="realm">アカウントに関連付けられるサービスを表す文字列。</param>
        /// <returns>新しいアカウント。既にバックエンドのデータソースに存在する場合は、生成されたアカウント。</returns>
        public virtual Account NewAccount(Guid accountId, String realm)
        {
            this.CheckIfDisposed();
            StorageDataSet.AccountsRow row;
            if ((row = this.LoadAccountsDataTable(
                accountId
            ).SingleOrDefault()) != null)
            {
                return this.GetAccount(row);
            }
            Account account = new Account()
            {
                Storage = this,
            };
            account.Row.AccountId = accountId;
            account.Row.Realm = realm;
            account.Connect();
            return account;
        }
        #endregion

        #region Activities
        /// <summary>
        /// 選択を行う文に後続するクエリ節を指定してバックエンドのデータソースからアクティビティのデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        public virtual StorageDataSet.ActivitiesDataTable LoadActivitiesDataTable(String clauses)
        {
            this.CheckIfDisposed();
            this.UnderlyingDataSet.Activities.BeginLoadData();
            StorageDataSet.ActivitiesDataTable table = this.LoadActivitiesDataTableImpl(clauses);
            foreach (Guid accountId in table.Select(r => r.AccountId).Distinct())
            {
                this.LoadAccountsDataTable(accountId);
            }
            this.UnderlyingDataSet.Activities.EndLoadData();
            return table;
        }

        /// <summary>
        /// 派生クラスで実装された場合、<see cref="LoadActivitiesDataTable(String)"/> メソッドを実装し、実際にデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        protected abstract StorageDataSet.ActivitiesDataTable LoadActivitiesDataTableImpl(String clauses);

        /// <summary>
        /// 列の値を指定してバックエンドのデータソースからアクティビティのデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="accountId">アクティビティの主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="timestamp">アクティビティの行われた日時。指定しない場合は <c>null</c>。</param>
        /// <param name="category">アクティビティの種別を表す文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="subindex">アクティビティのサブインデックス。指定しない場合は <c>null</c>。</param>
        /// <param name="value">アクティビティに関連付けられている文字列の値。値は <see cref="String"/> として扱われます。値が存在しない状態を指定するには <see cref="DBNull"/> 値を指定してください。指定しない場合は <c>null</c>。</param>
        /// <param name="data">アクティビティに関連付けられているバイト列の値。値は <see cref="Byte"/> 配列として扱われます。値が存在しない状態を指定するには <see cref="DBNull"/> 値を指定してください。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        public virtual StorageDataSet.ActivitiesDataTable LoadActivitiesDataTable(
            Nullable<Guid> accountId,
            Nullable<DateTime> timestamp,
            String category,
            Nullable<Int32> subindex,
            Object value,
            Object data
        )
        {
            List<String> whereClauses = new List<String>();
            if (accountId.HasValue)
            {
                whereClauses.Add(String.Format("[AccountId] == '{0}'", accountId.Value.ToString("d")));
            }
            if (timestamp.HasValue)
            {
                whereClauses.Add(String.Format("[Timestamp] == datetime('{0}')", timestamp.Value.ToString("s")));
            }
            if (category != null)
            {
                whereClauses.Add(String.Format("[Category] == '{0}'", category));
            }
            if (subindex.HasValue)
            {
                whereClauses.Add(String.Format("[Subindex] == {0}", subindex.Value));
            }
            if (value != null)
            {
                whereClauses.Add(String.Format("[Value] == {0}", Convert.IsDBNull(value)
                    ? "NULL"
                    : String.Format("'{0}'", value)
                ));
            }
            if (data != null)
            {
                whereClauses.Add(String.Format("[Value] == {0}", Convert.IsDBNull(data)
                    ? "NULL"
                    : String.Format("x'{0}'", String.Join(
                        String.Empty,
                        (data as Byte[]).Select(b => b.ToString("x")).ToArray()
                    ))
                ));
            }
            return this.LoadActivitiesDataTable(whereClauses.Any()
                ? "WHERE " + String.Join(" AND ", whereClauses.ToArray())
                : String.Empty
            );
        }

        /// <summary>
        /// 主キーを指定してバックエンドのデータソースからアクティビティのデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="accountId">アクティビティの主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="timestamp">アクティビティの行われた日時。指定しない場合は <c>null</c>。</param>
        /// <param name="category">アクティビティの種別を表す文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="subindex">アクティビティのサブインデックス。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        public StorageDataSet.ActivitiesDataTable LoadActivitiesDataTable(
            Nullable<Guid> accountId,
            Nullable<DateTime> timestamp,
            String category,
            Nullable<Int32> subindex
        )
        {
            return this.LoadActivitiesDataTable(accountId, timestamp, category, subindex, null, null);
        }

        /// <summary>
        /// バックエンドのデータソースからアクティビティのデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <returns>データソースから読み出したデータ表。</returns>
        public StorageDataSet.ActivitiesDataTable LoadActivitiesDataTable()
        {
            return this.LoadActivitiesDataTable(null, null, null, null);
        }

        /// <summary>
        /// 選択を行う文に後続するクエリ節を指定してバックエンドのデータソースからアクティビティのデータ表を読み出し、データセット上のデータ表とマージした上で、アクティビティを生成します。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表から生成されたアクティビティのシーケンス。</returns>
        public IEnumerable<Activity> LoadActivities(String clauses)
        {
            return this.GetActivities(this.LoadActivitiesDataTable(clauses));
        }

        /// <summary>
        /// 列の値を指定してバックエンドのデータソースからアクティビティのデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="accountId">アクティビティの主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="timestamp">アクティビティの行われた日時。指定しない場合は <c>null</c>。</param>
        /// <param name="category">アクティビティの種別を表す文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="subindex">アクティビティのサブインデックス。指定しない場合は <c>null</c>。</param>
        /// <param name="value">アクティビティに関連付けられている文字列の値。値は <see cref="String"/> として扱われます。値が存在しない状態を指定するには <see cref="DBNull"/> 値を指定してください。指定しない場合は <c>null</c>。</param>
        /// <param name="data">アクティビティに関連付けられているバイト列の値。値は <see cref="Byte"/> 配列として扱われます。値が存在しない状態を指定するには <see cref="DBNull"/> 値を指定してください。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表から生成されたアクティビティのシーケンス。</returns>
        public IEnumerable<Activity> LoadActivities(
            Nullable<Guid> accountId,
            Nullable<DateTime> timestamp,
            String category,
            Nullable<Int32> subindex,
            Object value,
            Object data
        )
        {
            return this.GetActivities(this.LoadActivitiesDataTable(accountId, timestamp, category, subindex, value, data));
        }

        /// <summary>
        /// 主キーを指定してバックエンドのデータソースからアクティビティのデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="accountId">アクティビティの主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="timestamp">アクティビティの行われた日時。指定しない場合は <c>null</c>。</param>
        /// <param name="category">アクティビティの種別を表す文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="subindex">アクティビティのサブインデックス。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表から生成されたアクティビティのシーケンス。</returns>
        public IEnumerable<Activity> LoadActivities(
            Nullable<Guid> accountId,
            Nullable<DateTime> timestamp,
            String category,
            Nullable<Int32> subindex
        )
        {
            return this.GetActivities(this.LoadActivitiesDataTable(accountId, timestamp, category, subindex));
        }

        /// <summary>
        /// バックエンドのデータソースからアカウントのデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <returns>データソースから読み出したデータ表から生成されたアクティビティのシーケンス。</returns>
        public IEnumerable<Activity> LoadActivities()
        {
            return this.GetActivities(this.LoadActivitiesDataTable());
        }

        /// <summary>
        /// データセット内で該当するデータ表の全てのデータ行を用いてアクティビティを生成します。
        /// </summary>
        /// <returns>データセット内で該当するデータ表の全てのデータ行を用いて生成されたアクティビティのシーケンス。</returns>
        public IEnumerable<Activity> GetActivities()
        {
            return this.GetActivities(row => true);
        }

        /// <summary>
        /// 指定された条件に合致するデータ行を用いてアクティビティを生成します。
        /// </summary>
        /// <param name="predicate">各データ行が条件に当てはまるかどうかをテストする関数。</param>
        /// <returns>条件に合致したデータ行を用いて生成されたアクティビティのシーケンス。</returns>
        public IEnumerable<Activity> GetActivities(Func<StorageDataSet.ActivitiesRow, Boolean> predicate)
        {
            return this.GetActivities(this.UnderlyingDataSet.Activities.Where(predicate));
        }

        /// <summary>
        /// 列の値を指定して、合致するデータ行を用いてアクティビティを生成します。
        /// </summary>
        /// <param name="accountId">アクティビティの主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="timestamp">アクティビティの行われた日時。指定しない場合は <c>null</c>。</param>
        /// <param name="category">アクティビティの種別を表す文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="subindex">アクティビティのサブインデックス。指定しない場合は <c>null</c>。</param>
        /// <param name="value">アクティビティに関連付けられている文字列の値。値は <see cref="String"/> として扱われます。値が存在しない状態を指定するには <see cref="DBNull"/> 値を指定してください。指定しない場合は <c>null</c>。</param>
        /// <param name="data">アクティビティに関連付けられているバイト列の値。値は <see cref="Byte"/> 配列として扱われます。値が存在しない状態を指定するには <see cref="DBNull"/> 値を指定してください。指定しない場合は <c>null</c>。</param>
        /// <returns>条件に合致したデータ行を用いて生成されたアクティビティのシーケンス。</returns>
        public IEnumerable<Activity> GetActivities(
            Nullable<Guid> accountId,
            Nullable<DateTime> timestamp,
            String category,
            Nullable<Int32> subindex,
            Object value,
            Object data
        )
        {
            IEnumerable<Activity> activities = this.GetActivities();
            if (accountId.HasValue)
            {
                activities = activities.Where(a => a.UnderlyingDataRow.AccountId == accountId.Value);
            }
            if (timestamp.HasValue)
            {
                activities = activities.Where(a => a.UnderlyingDataRow.Timestamp == timestamp.Value);
            }
            if (category != null)
            {
                activities = activities.Where(a => a.UnderlyingDataRow.Category == category);
            }
            if (subindex.HasValue)
            {
                activities = activities.Where(a => a.UnderlyingDataRow.Subindex == subindex.Value);
            }
            if (value != null)
            {
                activities = Convert.IsDBNull(value)
                    ? activities.Where(a => a.UnderlyingDataRow.IsValueNull())
                                 : activities.Where(a => a.UnderlyingDataRow.Value == (String) value);
            }
            if (data != null)
            {
                activities = Convert.IsDBNull(data)
                    ? activities.Where(a => a.UnderlyingDataRow.IsDataNull())
                    : activities.Where(a => a.UnderlyingDataRow.Data == data);
            }
            return activities;
        }

        /// <summary>
        /// 主キーを指定して、合致するデータ行を用いてアクティビティを生成します。
        /// </summary>
        /// <param name="accountId">アクティビティの主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="timestamp">アクティビティの行われた日時。指定しない場合は <c>null</c>。</param>
        /// <param name="category">アクティビティの種別を表す文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="subindex">アクティビティのサブインデックス。指定しない場合は <c>null</c>。</param>
        /// <returns>条件に合致したデータ行を用いて生成されたアクティビティのシーケンス。</returns>
        public IEnumerable<Activity> GetActivities(
            Nullable<Guid> accountId,
            Nullable<DateTime> timestamp,
            String category,
            Nullable<Int32> subindex
        )
        {
            return this.GetActivities(accountId, timestamp, category, subindex, null, null);
        }

        /// <summary>
        /// データ行のシーケンスを用いてアクティビティを生成します。
        /// </summary>
        /// <param name="rows">生成に用いるデータ行のシーケンス。</param>
        /// <returns>生成されたアクティビティのシーケンス。</returns>
        public IEnumerable<Activity> GetActivities(IEnumerable<StorageDataSet.ActivitiesRow> rows)
        {
            return rows.Select(row => this.GetActivity(row)).ToList();
        }

        /// <summary>
        /// データ行からアクティビティを生成します。
        /// </summary>
        /// <param name="row">生成に用いるデータ行。</param>
        /// <returns>生成されたアクティビティ。</returns>
        public virtual Activity GetActivity(StorageDataSet.ActivitiesRow row)
        {
            this.CheckIfDisposed();
            if (row == null)
            {
                return null;
            }
            Activity activity = new Activity()
            {
                Storage = this,
                UnderlyingDataRow = row,
            };
            activity.Connect();
            return activity;
        }

        /// <summary>
        /// 値を指定して、このストレージを使用するアクティビティを初期化します。既にバックエンドのデータソースに対応するデータ行が存在する場合は、データセットにロードされ、そこから生成されたアクティビティを返します。
        /// </summary>
        /// <param name="account">アクティビティの主体となるアカウント。</param>
        /// <param name="timestamp">アクティビティの行われた日時。</param>
        /// <param name="category">アクティビティの種別を表す文字列。</param>
        /// <returns>新しいアクティビティ。既にバックエンドのデータソースに存在する場合は、生成されたアクティビティ。</returns>
        /// <remarks>
        /// サブインデックスは自動的に設定されます。タイムスタンプがより遅い同一のアクティビティが存在する場合は、提示されたタイムスタンプに置き換えられ、サブインデックスが修正された上で返されます。
        /// </remarks>
        public Activity NewActivity(
            Account account,
            DateTime timestamp,
            String category
        )
        {
            this.LoadActivitiesDataTable(String.Format(
                "WHERE [AccountId] == '{0}' AND [Timestamp] >= datetime('{1}') AND [Category] == '{2}' ORDER BY [Timestamp] DESC, [Subindex] DESC LIMIT 1",
                account.AccountId.ToString("d"),
                timestamp.ToString("s"),
                category
            ));

            Activity activity = this.GetActivities(r =>
                r.AccountId == account.AccountId &&
                r.Timestamp >= timestamp &&
                r.Category == category
            )
                .OrderByDescending(a => a.Timestamp)
                .ThenBy(a => a.Subindex)
                .FirstOrDefault();

            if (activity == null)
            {
                return this.NewActivity(
                    account,
                    timestamp,
                    category,
                    this.GetActivities(
                        account.AccountId, timestamp, category, null
                    ).Count()
                );
            }
            else
            {
                if (activity.Timestamp > timestamp)
                {
                    // Fix timestamp to more earlier.
                    activity.Timestamp = timestamp;
                    activity.FixSubindex();
                }
                return activity;
            }
        }

        /// <summary>
        /// 値を指定して、このストレージを使用するアクティビティを初期化します。既にバックエンドのデータソースに対応するデータ行が存在する場合は、データセットにロードされ、そこから生成されたアクティビティを返します。
        /// </summary>
        /// <param name="account">アクティビティの主体となるアカウント。</param>
        /// <param name="timestamp">アクティビティの行われた日時。</param>
        /// <param name="category">アクティビティの種別を表す文字列。</param>
        /// <param name="subindex">アクティビティのサブインデックス。</param>
        /// <returns>新しいアクティビティ。既にバックエンドのデータソースに存在する場合は、生成されたアクティビティ。</returns>
        public virtual Activity NewActivity(
            Account account,
            DateTime timestamp,
            String category,
            Int32 subindex
        )
        {
            this.CheckIfDisposed();
            StorageDataSet.ActivitiesRow row;
            if ((row = this.LoadActivitiesDataTable(
                account.AccountId,
                timestamp,
                category,
                subindex
            ).SingleOrDefault()) != null)
            {
                return this.GetActivity(row);
            }
            Activity activity = new Activity()
            {
                Storage = this,
            };
            activity.Row.AccountId = account.AccountId;
            activity.Row.Timestamp = timestamp;
            activity.Row.Category = category;
            activity.Row.Subindex = subindex;
            activity.Connect();
            return activity;
        }
        #endregion

        #region FavorMap
        /// <summary>
        /// 選択を行う文に後続するクエリ節を指定してバックエンドのデータソースからお気に入りの関係のデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        public virtual StorageDataSet.FavorMapDataTable LoadFavorMapDataTable(String clauses)
        {
            this.CheckIfDisposed();
            this.UnderlyingDataSet.FavorMap.BeginLoadData();
            StorageDataSet.FavorMapDataTable table = this.LoadFavorMapDataTableImpl(clauses);
            foreach (Guid accountId in table.Select(r => r.AccountId).Distinct())
            {
                this.LoadAccountsDataTable(accountId);
            }
            foreach (var activityColumns in table.Select(r => new
            {
                r.FavoringAccountId,
                r.FavoringTimestamp,
                r.FavoringCategory,
                r.FavoringSubindex,
            }).Distinct())
            {
                this.LoadActivitiesDataTable(
                    activityColumns.FavoringAccountId,
                    activityColumns.FavoringTimestamp,
                    activityColumns.FavoringCategory,
                    activityColumns.FavoringSubindex
                );
            }
            this.UnderlyingDataSet.FavorMap.EndLoadData();
            return table;
        }

        /// <summary>
        /// 派生クラスで実装された場合、<see cref="LoadFavorMapDataTable(String)"/> メソッドを実装し、実際にデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        protected abstract StorageDataSet.FavorMapDataTable LoadFavorMapDataTableImpl(String clauses);

        /// <summary>
        /// 主キーを指定してバックエンドのデータソースからお気に入りの関係のデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="accountId">お気に入りとしてマークしている主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="favoringAccountId">お気に入りとしてマークしているアクティビティの主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="favoringTimestamp">お気に入りとしてマークしているアクティビティの行われた日時。指定しない場合は <c>null</c>。</param>
        /// <param name="favoringCategory">お気に入りとしてマークしているアクティビティの種別を表す文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="favoringSubindex">お気に入りとしてマークしているアクティビティのサブインデックス。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        public virtual StorageDataSet.FavorMapDataTable LoadFavorMapDataTable(
            Nullable<Guid> accountId,
            Nullable<Guid> favoringAccountId,
            Nullable<DateTime> favoringTimestamp,
            String favoringCategory,
            Nullable<Int32> favoringSubindex
        )
        {
            List<String> whereClauses = new List<String>();
            if (accountId.HasValue)
            {
                whereClauses.Add(String.Format("[AccountId] == '{0}'", accountId.Value.ToString("d")));
            }
            if (favoringAccountId.HasValue)
            {
                whereClauses.Add(String.Format("[FavoringAccountId] == '{0}'", favoringAccountId.Value.ToString("d")));
            }
            if (favoringTimestamp.HasValue)
            {
                whereClauses.Add(String.Format("[FavoringTimestamp] == datetime('{0}')", favoringTimestamp.Value.ToString("s")));
            }
            if (favoringCategory != null)
            {
                whereClauses.Add(String.Format("[FavoringCategory] == '{0}'", favoringCategory));
            }
            if (favoringSubindex.HasValue)
            {
                whereClauses.Add(String.Format("[FavoringSubindex] == {0}", favoringSubindex.Value));
            }
            return this.LoadFavorMapDataTable(whereClauses.Any()
                ? "WHERE " + String.Join(" AND ", whereClauses.ToArray())
                : String.Empty
            );
        }

        /// <summary>
        /// バックエンドのデータソースからお気に入りの関係のデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <returns>データソースから読み出したデータ表。</returns>
        public StorageDataSet.FavorMapDataTable LoadFavorMapDataTable()
        {
            return this.LoadFavorMapDataTable(null, null, null, null, null);
        }

        /// <summary>
        /// 選択を行う文に後続するクエリ節を指定してバックエンドのデータソースからお気に入りの関係のデータ表を読み出し、データセット上のデータ表とマージした上で、お気に入りの関係を生成します。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表から生成されたお気に入りの関係のシーケンス。</returns>
        public IEnumerable<FavorElement> LoadFavorElements(String clauses)
        {
            return this.GetFavorElements(this.LoadFavorMapDataTable(clauses));
        }

        /// <summary>
        /// 主キーを指定してバックエンドのデータソースからお気に入りの関係のデータ表を読み出し、データセット上のデータ表とマージした上で、お気に入りの関係を生成します。
        /// </summary>
        /// <param name="accountId">お気に入りとしてマークしている主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="favoringAccountId">お気に入りとしてマークしているアクティビティの主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="favoringTimestamp">お気に入りとしてマークしているアクティビティの行われた日時。指定しない場合は <c>null</c>。</param>
        /// <param name="favoringCategory">お気に入りとしてマークしているアクティビティの種別を表す文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="favoringSubindex">お気に入りとしてマークしているアクティビティのサブインデックス。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表から生成されたお気に入りの関係のシーケンス。</returns>
        public IEnumerable<FavorElement> LoadFavorElements(
            Nullable<Guid> accountId,
            Nullable<Guid> favoringAccountId,
            Nullable<DateTime> favoringTimestamp,
            String favoringCategory,
            Nullable<Int32> favoringSubindex
        )
        {
            return this.GetFavorElements(this.LoadFavorMapDataTable(accountId, favoringAccountId, favoringTimestamp, favoringCategory, favoringSubindex));
        }

        /// <summary>
        /// バックエンドのデータソースからお気に入りの関係のデータ表を読み出し、データセット上のデータ表とマージした上で、お気に入りの関係を生成します。
        /// </summary>
        /// <returns>データソースから読み出したデータ表から生成されたお気に入りの関係のシーケンス。</returns>
        public IEnumerable<FavorElement> LoadFavorElements()
        {
            return this.GetFavorElements(this.LoadFavorMapDataTable());
        }

        /// <summary>
        /// データセット内で該当するデータ表の全てのデータ行を用いてお気に入りの関係を生成します。
        /// </summary>
        /// <returns>データセット内で該当するデータ表の全てのデータ行を用いて生成されたお気に入りの関係のシーケンス。</returns>
        public IEnumerable<FavorElement> GetFavorElements()
        {
            return this.GetFavorElements(row => true);
        }

        /// <summary>
        /// 指定された条件に合致するデータ行を用いてお気に入りの関係を生成します。
        /// </summary>
        /// <param name="predicate">各データ行が条件に当てはまるかどうかをテストする関数。</param>
        /// <returns>条件に合致したデータ行を用いて生成されたお気に入りの関係のシーケンス。</returns>
        public IEnumerable<FavorElement> GetFavorElements(Func<StorageDataSet.FavorMapRow, Boolean> predicate)
        {
            return this.GetFavorElements(this.UnderlyingDataSet.FavorMap.Where(predicate));
        }

        /// <summary>
        /// 主キーを指定して、合致するデータ行を用いてお気に入りの関係を生成します。
        /// </summary>
        /// <param name="accountId">お気に入りとしてマークしている主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="favoringAccountId">お気に入りとしてマークしているアクティビティの主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="favoringTimestamp">お気に入りとしてマークしているアクティビティの行われた日時。指定しない場合は <c>null</c>。</param>
        /// <param name="favoringCategory">お気に入りとしてマークしているアクティビティの種別を表す文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="favoringSubindex">お気に入りとしてマークしているアクティビティのサブインデックス。指定しない場合は <c>null</c>。</param>
        /// <returns>条件に合致したデータ行を用いて生成されたお気に入りの関係のシーケンス。</returns>
        public IEnumerable<FavorElement> GetFavorElements(
            Nullable<Guid> accountId,
            Nullable<Guid> favoringAccountId,
            Nullable<DateTime> favoringTimestamp,
            String favoringCategory,
            Nullable<Int32> favoringSubindex
        )
        {
            IEnumerable<FavorElement> elements = this.GetFavorElements();
            if (accountId.HasValue)
            {
                elements = elements.Where(e => e.UnderlyingDataRow.AccountId == accountId.Value);
            }
            if (favoringAccountId.HasValue)
            {
                elements = elements.Where(e => e.UnderlyingDataRow.FavoringAccountId == favoringAccountId);
            }
            if (favoringTimestamp.HasValue)
            {
                elements = elements.Where(e => e.UnderlyingDataRow.FavoringTimestamp == favoringTimestamp.Value);
            }
            if (favoringCategory != null)
            {
                elements = elements.Where(e => e.UnderlyingDataRow.FavoringCategory == favoringCategory);
            }
            if (favoringSubindex.HasValue)
            {
                elements = elements.Where(e => e.UnderlyingDataRow.FavoringSubindex == favoringSubindex);
            }
            return elements;
        }

        /// <summary>
        /// データ行のシーケンスを用いてお気に入りの関係を生成します。
        /// </summary>
        /// <param name="rows">生成に用いるデータ行のシーケンス。</param>
        /// <returns>生成されたお気に入りの関係のシーケンス。</returns>
        public IEnumerable<FavorElement> GetFavorElements(IEnumerable<StorageDataSet.FavorMapRow> rows)
        {
            return rows.Select(row => this.GetFavorElement(row)).ToList();
        }

        /// <summary>
        /// データ行からお気に入りの関係を生成します。
        /// </summary>
        /// <param name="row">生成に用いるデータ行。</param>
        /// <returns>生成されたお気に入りの関係。</returns>
        public virtual FavorElement GetFavorElement(StorageDataSet.FavorMapRow row)
        {
            this.CheckIfDisposed();
            if (row == null)
            {
                return null;
            }
            FavorElement element = new FavorElement()
            {
                Storage = this,
                UnderlyingDataRow = row,
            };
            element.Connect();
            return element;
        }

        /// <summary>
        /// 値を指定して、このストレージを使用するお気に入りの関係を初期化します。既にバックエンドのデータソースに対応するデータ行が存在する場合は、データセットにロードされ、そこから生成されたお気に入りの関係を返します。
        /// </summary>
        /// <param name="account">お気に入りとしてマークする主体となるアカウント。</param>
        /// <param name="favoringActivity">お気に入りとしてマークするアクティビティ。</param>
        /// <returns>新しいお気に入りの関係。既にバックエンドのデータソースに存在する場合は、生成されたお気に入りの関係。</returns>
        public virtual FavorElement NewFavorElement(
            Account account,
            Activity favoringActivity
        )
        {
            this.CheckIfDisposed();
            StorageDataSet.FavorMapRow row;
            if ((row = this.LoadFavorMapDataTable(
                account.AccountId,
                favoringActivity.PrimaryKeys.AccountId,
                favoringActivity.Timestamp,
                favoringActivity.Category,
                favoringActivity.Subindex
            ).SingleOrDefault()) != null)
            {
                return this.GetFavorElement(row);
            }
            FavorElement element = new FavorElement()
            {
                Storage = this,
            };
            element.Row.AccountId = account.AccountId;
            element.Row.FavoringAccountId = favoringActivity.PrimaryKeys.AccountId;
            element.Row.FavoringTimestamp = favoringActivity.Timestamp;
            element.Row.FavoringCategory = favoringActivity.Category;
            element.Row.FavoringSubindex = favoringActivity.Subindex;
            element.Connect();
            return element;
        }
        #endregion

        #region FollowMap
        /// <summary>
        /// 選択を行う文に後続するクエリ節を指定してバックエンドのデータソースからフォローの関係のデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        public virtual StorageDataSet.FollowMapDataTable LoadFollowMapDataTable(String clauses)
        {
            this.CheckIfDisposed();
            this.UnderlyingDataSet.FollowMap.BeginLoadData();
            StorageDataSet.FollowMapDataTable table = this.LoadFollowMapDataTableImpl(clauses);
            foreach (Guid accountId in table.Select(r => r.AccountId).Union(table.Select(r => r.FollowingAccountId)))
            {
                this.LoadAccountsDataTable(accountId);
            }
            this.UnderlyingDataSet.FollowMap.EndLoadData();
            return table;
        }

        /// <summary>
        /// 派生クラスで実装された場合、<see cref="LoadFollowMapDataTable(String)"/> メソッドを実装し、実際にデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        protected abstract StorageDataSet.FollowMapDataTable LoadFollowMapDataTableImpl(String clauses);

        /// <summary>
        /// 主キーを指定してバックエンドのデータソースからフォローの関係のデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="accountId">フォローしている主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="followingAccountId">アカウントがフォローしているアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        public virtual StorageDataSet.FollowMapDataTable LoadFollowMapDataTable(
            Nullable<Guid> accountId,
            Nullable<Guid> followingAccountId
        )
        {
            List<String> whereClauses = new List<String>();
            if (accountId.HasValue)
            {
                whereClauses.Add(String.Format("[AccountId] == '{0}'", accountId.Value.ToString("d")));
            }
            if (followingAccountId.HasValue)
            {
                whereClauses.Add(String.Format("[FollowingAccountId] == '{0}'", followingAccountId.Value.ToString("d")));
            }
            return this.LoadFollowMapDataTable(whereClauses.Any()
                ? "WHERE " + String.Join(" AND ", whereClauses.ToArray())
                : String.Empty
            );
        }

        /// <summary>
        /// バックエンドのデータソースからフォローの関係のデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <returns>データソースから読み出したデータ表。</returns>
        public StorageDataSet.FollowMapDataTable LoadFollowMapDataTable()
        {
            return this.LoadFollowMapDataTable(default(Nullable<Guid>), default(Nullable<Guid>));
        }

        /// <summary>
        /// 選択を行う文に後続するクエリ節を指定してバックエンドのデータソースからフォローの関係のデータ表を読み出し、データセット上のデータ表とマージした上で、フォローの関係を生成します。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表から生成されたフォローの関係のシーケンス。</returns>
        public IEnumerable<FollowElement> LoadFollowElements(String clauses)
        {
            return this.GetFollowElements(this.LoadFollowMapDataTable(clauses));
        }

        /// <summary>
        /// 主キーを指定してバックエンドのデータソースからフォローの関係のデータ表を読み出し、データセット上のデータ表とマージした上で、フォローの関係を生成します。
        /// </summary>
        /// <param name="accountId">フォローしている主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="followingAccountId">アカウントがフォローしているアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表から生成されたフォローの関係のシーケンス。</returns>
        public IEnumerable<FollowElement> LoadFollowElements(
            Nullable<Guid> accountId,
            Nullable<Guid> followingAccountId
        )
        {
            return this.GetFollowElements(this.LoadFollowMapDataTable(accountId, followingAccountId));
        }

        /// <summary>
        /// バックエンドのデータソースからフォローの関係のデータ表を読み出し、データセット上のデータ表とマージした上で、フォローの関係を生成します。
        /// </summary>
        /// <returns>データソースから読み出したデータ表から生成されたフォローの関係のシーケンス。</returns>
        public IEnumerable<FollowElement> LoadFollowElements()
        {
            return this.GetFollowElements(this.LoadFollowMapDataTable());
        }

        /// <summary>
        /// データセット内で該当するデータ表の全てのデータ行を用いてフォローの関係を生成します。
        /// </summary>
        /// <returns>データセット内で該当するデータ表の全てのデータ行を用いて生成されたフォローの関係のシーケンス。</returns>
        public IEnumerable<FollowElement> GetFollowElements()
        {
            return this.GetFollowElements(row => true);
        }

        /// <summary>
        /// 指定された条件に合致するデータ行を用いてフォローの関係を生成します。
        /// </summary>
        /// <param name="predicate">各データ行が条件に当てはまるかどうかをテストする関数。</param>
        /// <returns>条件に合致したデータ行を用いて生成されたフォローの関係のシーケンス。</returns>
        public IEnumerable<FollowElement> GetFollowElements(Func<StorageDataSet.FollowMapRow, Boolean> predicate)
        {
            return this.GetFollowElements(this.UnderlyingDataSet.FollowMap.Where(predicate));
        }

        /// <summary>
        /// 主キーを指定して、合致するデータ行を用いてフォローの関係を生成します。
        /// </summary>
        /// <param name="accountId">フォローしている主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="followingAccountId">アカウントがフォローしているアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <returns>条件に合致したデータ行を用いて生成されたフォローの関係のシーケンス。</returns>
        public IEnumerable<FollowElement> GetFollowElements(
            Nullable<Guid> accountId,
            Nullable<Guid> followingAccountId
        )
        {
            IEnumerable<FollowElement> elements = this.GetFollowElements();
            if (accountId.HasValue)
            {
                elements = elements.Where(e => e.UnderlyingDataRow.AccountId == accountId.Value);
            }
            if (followingAccountId.HasValue)
            {
                elements = elements.Where(e => e.UnderlyingDataRow.FollowingAccountId == followingAccountId.Value);
            }
            return elements;
        }

        /// <summary>
        /// データ行のシーケンスを用いてフォローの関係を生成します。
        /// </summary>
        /// <param name="rows">生成に用いるデータ行のシーケンス。</param>
        /// <returns>生成されたフォローの関係のシーケンス。</returns>
        public IEnumerable<FollowElement> GetFollowElements(IEnumerable<StorageDataSet.FollowMapRow> rows)
        {
            return rows.Select(row => this.GetFollowElement(row)).ToList();
        }

        /// <summary>
        /// データ行からフォローの関係を生成します。
        /// </summary>
        /// <param name="row">生成に用いるデータ行。</param>
        /// <returns>生成されたフォローの関係。</returns>
        public virtual FollowElement GetFollowElement(StorageDataSet.FollowMapRow row)
        {
            this.CheckIfDisposed();
            if (row == null)
            {
                return null;
            }
            FollowElement element = new FollowElement()
            {
                Storage = this,
                UnderlyingDataRow = row,
            };
            element.Connect();
            return element;
        }

        /// <summary>
        /// 値を指定して、このストレージを使用するフォローの関係を初期化します。既にバックエンドのデータソースに対応するデータ行が存在する場合は、データセットにロードされ、そこから生成されたフォローの関係を返します。
        /// </summary>
        /// <param name="account">フォローする主体となるアカウント。</param>
        /// <param name="followingAccount">フォローするアカウント。</param>
        /// <returns>新しいフォローの関係。既にバックエンドのデータソースに存在する場合は、生成されたフォローの関係。</returns>
        public virtual FollowElement NewFollowElement(
            Account account,
            Account followingAccount
        )
        {
            this.CheckIfDisposed();
            StorageDataSet.FollowMapRow row;
            if ((row = this.LoadFollowMapDataTable(
                account.AccountId,
                followingAccount.AccountId
            ).SingleOrDefault()) != null)
            {
                return this.GetFollowElement(row);
            }
            FollowElement element = new FollowElement()
            {
                Storage = this,
            };
            element.Row.AccountId = account.AccountId;
            element.Row.FollowingAccountId = followingAccount.AccountId;
            element.Connect();
            return element;
        }
        #endregion

        #region Posts
        /// <summary>
        /// 選択を行う文に後続するクエリ節を指定してバックエンドのデータソースからポストのデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        public virtual StorageDataSet.PostsDataTable LoadPostsDataTable(String clauses)
        {
            this.CheckIfDisposed();
            this.UnderlyingDataSet.Posts.BeginLoadData();
            StorageDataSet.PostsDataTable table = this.LoadPostsDataTableImpl(clauses);
            foreach (var activityColumn in table.Select(r => new
            {
                r.AccountId,
                r.PostId,
            }))
            {
                this.LoadActivitiesDataTable(activityColumn.AccountId, null, "Post", null, activityColumn.PostId, null);
            }
            this.UnderlyingDataSet.Posts.EndLoadData();
            return table;
        }

        /// <summary>
        /// 派生クラスで実装された場合、<see cref="LoadPostsDataTable(String)"/> メソッドを実装し、実際にデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        protected abstract StorageDataSet.PostsDataTable LoadPostsDataTableImpl(String clauses);

        /// <summary>
        /// 列の値を指定してバックエンドのデータソースからポストのデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="accountId">ポストを投稿した主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="postId">任意のサービス内においてポストを一意に識別する文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="text">ポストの本文。値は <see cref="String"/> として扱われます。値が存在しない状態を指定するには <see cref="DBNull"/> 値を指定してください。指定しない場合は <c>null</c>。</param>
        /// <param name="source">ポストの投稿に使用されたクライアントを表す文字列。値は <see cref="String"/> として扱われます。値が存在しない状態を指定するには <see cref="DBNull"/> 値を指定してください。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        public virtual StorageDataSet.PostsDataTable LoadPostsDataTable(
            Nullable<Guid> accountId,
            String postId,
            Object text,
            Object source
        )
        {
            List<String> whereClauses = new List<String>();
            if (accountId.HasValue)
            {
                whereClauses.Add(String.Format("[AccountId] == '{0}'", accountId.Value.ToString("d")));
            }
            if (postId != null)
            {
                whereClauses.Add(String.Format("[PostId] == '{0}'", postId));
            }
            if (text != null)
            {
                whereClauses.Add(String.Format("[Value] == {0}", Convert.IsDBNull(text)
                    ? "NULL"
                    : String.Format("'{0}'", text)
                ));
            }
            if (source != null)
            {
                whereClauses.Add(String.Format("[Value] == {0}", Convert.IsDBNull(source)
                    ? "NULL"
                    : String.Format("'{0}'", source)
                ));
            }
            return this.LoadPostsDataTable(whereClauses.Any()
                ? "WHERE " + String.Join(" AND ", whereClauses.ToArray())
                : String.Empty
            );
        }

        /// <summary>
        /// 主キーを指定してバックエンドのデータソースからポストのデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="accountId">ポストを投稿した主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="postId">任意のサービス内においてポストを一意に識別する文字列。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        public StorageDataSet.PostsDataTable LoadPostsDataTable(
            Nullable<Guid> accountId,
            String postId
        )
        {
            return this.LoadPostsDataTable(accountId, postId, null, null);
        }

        /// <summary>
        /// バックエンドのデータソースからポストのデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <returns>データソースから読み出したデータ表。</returns>
        public StorageDataSet.PostsDataTable LoadPostsDataTable()
        {
            return this.LoadPostsDataTable(null, null);
        }

        /// <summary>
        /// 選択を行う文に後続するクエリ節を指定してバックエンドのデータソースからポストのデータ表を読み出し、データセット上のデータ表とマージした上で、ポストを生成します。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表から生成されたポストのシーケンス。</returns>
        public IEnumerable<Post> LoadPosts(String clauses)
        {
            return this.GetPosts(this.LoadPostsDataTable(clauses));
        }

        /// <summary>
        /// 列の値を指定してバックエンドのデータソースからポストのデータ表を読み出し、データセット上のデータ表とマージした上で、ポストを生成します。
        /// </summary>
        /// <param name="accountId">ポストを投稿した主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="postId">任意のサービス内においてポストを一意に識別する文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="text">ポストの本文。値は <see cref="String"/> として扱われます。値が存在しない状態を指定するには <see cref="DBNull"/> 値を指定してください。指定しない場合は <c>null</c>。</param>
        /// <param name="source">ポストの投稿に使用されたクライアントを表す文字列。値は <see cref="String"/> として扱われます。値が存在しない状態を指定するには <see cref="DBNull"/> 値を指定してください。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表から生成されたポストのシーケンス。</returns>
        public IEnumerable<Post> LoadPosts(
            Nullable<Guid> accountId,
            String postId,
            Object text,
            Object source
        )
        {
            return this.GetPosts(this.LoadPostsDataTable(accountId, postId, text, source));
        }

        /// <summary>
        /// 主キーを指定してバックエンドのデータソースからポストのデータ表を読み出し、データセット上のデータ表とマージした上で、ポストを生成します。
        /// </summary>
        /// <param name="accountId">ポストを投稿した主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="postId">任意のサービス内においてポストを一意に識別する文字列。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表から生成されたポストのシーケンス。</returns>
        public IEnumerable<Post> LoadPosts(
            Nullable<Guid> accountId,
            String postId
        )
        {
            return this.GetPosts(this.LoadPostsDataTable(accountId, postId));
        }

        /// <summary>
        /// バックエンドのデータソースからポストのデータ表を読み出し、データセット上のデータ表とマージした上で、ポストを生成します。
        /// </summary>
        /// <returns>データソースから読み出したデータ表から生成されたポストのシーケンス。</returns>
        public IEnumerable<Post> LoadPosts()
        {
            return this.GetPosts(this.LoadPostsDataTable());
        }

        /// <summary>
        /// データセット内で該当するデータ表の全てのデータ行を用いてポストを生成します。
        /// </summary>
        /// <returns>データセット内で該当するデータ表の全てのデータ行を用いて生成されたポストのシーケンス。</returns>
        public IEnumerable<Post> GetPosts()
        {
            return this.GetPosts(row => true);
        }

        /// <summary>
        /// 指定された条件に合致するデータ行を用いてポストを生成します。
        /// </summary>
        /// <param name="predicate">各データ行が条件に当てはまるかどうかをテストする関数。</param>
        /// <returns>条件に合致したデータ行を用いて生成されたポストのシーケンス。</returns>
        public IEnumerable<Post> GetPosts(Func<StorageDataSet.PostsRow, Boolean> predicate)
        {
            return this.GetPosts(this.UnderlyingDataSet.Posts.Where(predicate));
        }

        /// <summary>
        /// 列の値を指定して、合致するデータ行を用いてポストを生成します。
        /// </summary>
        /// <param name="accountId">ポストを投稿した主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="postId">任意のサービス内においてポストを一意に識別する文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="text">ポストの本文。値は <see cref="String"/> として扱われます。値が存在しない状態を指定するには <see cref="DBNull"/> 値を指定してください。指定しない場合は <c>null</c>。</param>
        /// <param name="source">ポストの投稿に使用されたクライアントを表す文字列。値は <see cref="String"/> として扱われます。値が存在しない状態を指定するには <see cref="DBNull"/> 値を指定してください。指定しない場合は <c>null</c>。</param>
        /// <returns>条件に合致したデータ行を用いて生成されたポストのシーケンス。</returns>
        public IEnumerable<Post> GetPosts(
            Nullable<Guid> accountId,
            String postId,
            Object text,
            Object source
        )
        {
            IEnumerable<Post> posts = this.GetPosts();
            if (accountId.HasValue)
            {
                posts = posts.Where(p => p.UnderlyingDataRow.AccountId == accountId.Value);
            }
            if (postId != null)
            {
                posts = posts.Where(p => p.UnderlyingDataRow.PostId == postId);
            }
            if (text != null)
            {
                posts = Convert.IsDBNull(text)
                    ? posts.Where(p => p.UnderlyingDataRow.IsTextNull())
                    : posts.Where(p => p.UnderlyingDataRow.Text == (String) text);
            }
            if (source != null)
            {
                posts = Convert.IsDBNull(source)
                    ? posts.Where(p => p.UnderlyingDataRow.IsSourceNull())
                    : posts.Where(p => p.UnderlyingDataRow.Source == (String) source);
            }
            return posts;
        }

        /// <summary>
        /// 主キーを指定して、合致するデータ行を用いてポストを生成します。
        /// </summary>
        /// <param name="accountId">ポストを投稿した主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="postId">任意のサービス内においてポストを一意に識別する文字列。指定しない場合は <c>null</c>。</param>
        /// <returns>条件に合致したデータ行を用いて生成されたポストのシーケンス。</returns>
        public IEnumerable<Post> GetPosts(
            Nullable<Guid> accountId,
            String postId
        )
        {
            return this.GetPosts(accountId, postId, null, null);
        }

        /// <summary>
        /// データ行のシーケンスを用いてポストを生成します。
        /// </summary>
        /// <param name="rows">生成に用いるデータ行のシーケンス。</param>
        /// <returns>生成されたポストのシーケンス。</returns>
        public IEnumerable<Post> GetPosts(IEnumerable<StorageDataSet.PostsRow> rows)
        {
            return rows.Select(row => this.GetPost(row)).ToList();
        }

        /// <summary>
        /// データ行からポストを生成します。
        /// </summary>
        /// <param name="row">生成に用いるデータ行。</param>
        /// <returns>生成されたポスト。</returns>
        public virtual Post GetPost(StorageDataSet.PostsRow row)
        {
            this.CheckIfDisposed();
            if (row == null)
            {
                return null;
            }
            Post post = new Post()
            {
                Storage = this,
                UnderlyingDataRow = row,
            };
            post.Connect();
            return post;
        }

        /// <summary>
        /// 値を指定して、このストレージを使用するポストを初期化します。既にバックエンドのデータソースに対応するデータ行が存在する場合は、データセットにロードされ、そこから生成されたポストを返します。
        /// </summary>
        /// <param name="activity">ポストと一対一で対応するアクティビティ。</param>
        /// <returns>新しいポスト。既にバックエンドのデータソースに存在する場合は、生成されたポスト。</returns>
        public virtual Post NewPost(
            Activity activity
        )
        {
            this.CheckIfDisposed();
            StorageDataSet.PostsRow row;
            if ((row = this.LoadPostsDataTable(
                activity.PrimaryKeys.AccountId,
                activity.Value
            ).SingleOrDefault()) != null)
            {
                return this.GetPost(row);
            }
            Post post = new Post()
            {
                Storage = this,
            };
            post.Row.AccountId = activity.PrimaryKeys.AccountId;
            post.Row.PostId = activity.Value;
            post.Connect();
            return post;
        }
        #endregion

        #region ReplyMap
        /// <summary>
        /// 選択を行う文に後続するクエリ節を指定してバックエンドのデータソースから返信の関係のデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        public virtual StorageDataSet.ReplyMapDataTable LoadReplyMapDataTable(String clauses)
        {
            this.CheckIfDisposed();
            this.UnderlyingDataSet.ReplyMap.BeginLoadData();
            StorageDataSet.ReplyMapDataTable table = this.LoadReplyMapDataTableImpl(clauses);
            foreach (var postColumns in table
                .Select(r => new
                {
                    r.AccountId,
                    r.PostId,
                })
                .Union(table.Select(r => new
                {
                    AccountId = r.InReplyToAccountId,
                    PostId = r.InReplyToPostId,
                }))
            )
            {
                this.LoadPostsDataTable(postColumns.AccountId, postColumns.PostId);
            }
            this.UnderlyingDataSet.ReplyMap.EndLoadData();
            return table;
        }

        /// <summary>
        /// 派生クラスで実装された場合、<see cref="LoadReplyMapDataTable(String)"/> メソッドを実装し、実際にデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        protected abstract StorageDataSet.ReplyMapDataTable LoadReplyMapDataTableImpl(String clauses);

        /// <summary>
        /// 主キーを指定してバックエンドのデータソースから返信の関係のデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="accountId">返信している主体であるポストを投稿した主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="postId">任意のサービス内において返信している主体であるポストを一意に識別する文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="inReplyToAccountId">ポストの返信元のポストを投稿した主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="inReplyToPostId">任意のサービス内においてポストを一意に識別する文字列。ポストの返信元の指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        public virtual StorageDataSet.ReplyMapDataTable LoadReplyMapDataTable(
            Nullable<Guid> accountId,
            String postId,
            Nullable<Guid> inReplyToAccountId,
            String inReplyToPostId
        )
        {
            List<String> whereClauses = new List<String>();
            if (accountId.HasValue)
            {
                whereClauses.Add(String.Format("[AccountId] == '{0}'", accountId.Value.ToString("d")));
            }
            if (postId != null)
            {
                whereClauses.Add(String.Format("[PostId] == '{0}'", postId));
            }
            if (inReplyToAccountId.HasValue)
            {
                whereClauses.Add(String.Format("[InReplyToAccountId] == '{0}'", inReplyToAccountId.Value.ToString("d")));
            }
            if (inReplyToPostId != null)
            {
                whereClauses.Add(String.Format("[InReplyToPostId] == '{0}'", inReplyToPostId));
            }
            return this.LoadReplyMapDataTable(whereClauses.Any()
                ? "WHERE " + String.Join(" AND ", whereClauses.ToArray())
                : String.Empty
            );

        }

        /// <summary>
        /// バックエンドのデータソースから返信の関係のデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <returns>データソースから読み出したデータ表。</returns>
        public StorageDataSet.ReplyMapDataTable LoadReplyMapDataTable()
        {
            return this.LoadReplyMapDataTable(null, null, null, null);
        }

        /// <summary>
        /// 選択を行う文に後続するクエリ節を指定してバックエンドのデータソースから返信の関係のデータ表を読み出し、データセット上のデータ表とマージした上で、返信の関係を生成します。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表から生成された返信の関係のシーケンス。</returns>
        public IEnumerable<ReplyElement> LoadReplyElements(String clauses)
        {
            return this.GetReplyElements(this.LoadReplyMapDataTable(clauses));
        }

        /// <summary>
        /// 主キーを指定してバックエンドのデータソースから返信の関係のデータ表を読み出し、データセット上のデータ表とマージした上で、返信の関係を生成します。
        /// </summary>
        /// <param name="accountId">返信している主体であるポストを投稿した主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="postId">任意のサービス内において返信している主体であるポストを一意に識別する文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="inReplyToAccountId">ポストの返信元のポストを投稿した主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="inReplyToPostId">任意のサービス内においてポストを一意に識別する文字列。ポストの返信元の指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表から生成された返信の関係のシーケンス。</returns>
        public IEnumerable<ReplyElement> LoadReplyElements(
            Nullable<Guid> accountId,
            String postId,
            Nullable<Guid> inReplyToAccountId,
            String inReplyToPostId
        )
        {
            return this.GetReplyElements(this.LoadReplyMapDataTable(accountId, postId, inReplyToAccountId, inReplyToPostId));
        }

        /// <summary>
        /// バックエンドのデータソースから返信の関係のデータ表を読み出し、データセット上のデータ表とマージした上で、返信の関係を生成します。
        /// </summary>
        /// <returns>データソースから読み出したデータ表から生成された返信の関係のシーケンス。</returns>
        public IEnumerable<ReplyElement> LoadReplyElements()
        {
            return this.GetReplyElements(this.LoadReplyMapDataTable());
        }

        /// <summary>
        /// データセット内で該当するデータ表の全てのデータ行を用いて返信の関係を生成します。
        /// </summary>
        /// <returns>データセット内で該当するデータ表の全てのデータ行を用いて生成された返信の関係のシーケンス。</returns>
        public IEnumerable<ReplyElement> GetReplyElements()
        {
            return this.GetReplyElements(row => true);
        }

        /// <summary>
        /// 指定された条件に合致するデータ行を用いて返信の関係を生成します。
        /// </summary>
        /// <param name="predicate">各データ行が条件に当てはまるかどうかをテストする関数。</param>
        /// <returns>条件に合致したデータ行を用いて生成された返信の関係のシーケンス。</returns>
        public IEnumerable<ReplyElement> GetReplyElements(Func<StorageDataSet.ReplyMapRow, Boolean> predicate)
        {
            return this.GetReplyElements(this.UnderlyingDataSet.ReplyMap.Where(predicate));
        }

        /// <summary>
        /// 主キーを指定して、合致するデータ行を用いて返信の関係を生成します。
        /// </summary>
        /// <param name="accountId">返信している主体であるポストを投稿した主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="postId">任意のサービス内において返信している主体であるポストを一意に識別する文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="inReplyToAccountId">ポストの返信元のポストを投稿した主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="inReplyToPostId">任意のサービス内においてポストを一意に識別する文字列。ポストの返信元の指定しない場合は <c>null</c>。</param>
        /// <returns>条件に合致したデータ行を用いて生成された返信の関係のシーケンス。</returns>
        public IEnumerable<ReplyElement> GetReplyElements(
            Nullable<Guid> accountId,
            String postId,
            Nullable<Guid> inReplyToAccountId,
            String inReplyToPostId
        )
        {
            IEnumerable<ReplyElement> elements = this.GetReplyElements();
            if (accountId.HasValue)
            {
                elements = elements.Where(e => e.UnderlyingDataRow.AccountId == accountId.Value);
            }
            if (postId != null)
            {
                elements = elements.Where(e => e.UnderlyingDataRow.PostId == postId);
            }
            if (inReplyToAccountId.HasValue)
            {
                elements = elements.Where(e => e.UnderlyingDataRow.InReplyToAccountId == inReplyToAccountId.Value);
            }
            if (inReplyToPostId != null)
            {
                elements = elements.Where(e => e.UnderlyingDataRow.InReplyToPostId == inReplyToPostId);
            }
            return elements;
        }

        /// <summary>
        /// データ行のシーケンスを用いて返信の関係を生成します。
        /// </summary>
        /// <param name="rows">生成に用いるデータ行のシーケンス。</param>
        /// <returns>生成された返信の関係のシーケンス。</returns>
        public IEnumerable<ReplyElement> GetReplyElements(IEnumerable<StorageDataSet.ReplyMapRow> rows)
        {
            return rows.Select(row => this.GetReplyElement(row)).ToList();
        }

        /// <summary>
        /// データ行から返信の関係を生成します。
        /// </summary>
        /// <param name="row">生成に用いるデータ行。</param>
        /// <returns>生成された返信の関係。</returns>
        public virtual ReplyElement GetReplyElement(StorageDataSet.ReplyMapRow row)
        {
            this.CheckIfDisposed();
            if (row == null)
            {
                return null;
            }
            ReplyElement element = new ReplyElement()
            {
                Storage = this,
                UnderlyingDataRow = row,
            };
            element.Connect();
            return element;
        }

        /// <summary>
        /// 値を指定して、このストレージを使用する返信の関係を初期化します。既にバックエンドのデータソースに対応するデータ行が存在する場合は、データセットにロードされ、そこから生成された返信の関係を返します。
        /// </summary>
        /// <param name="post">返信する主体となるポスト</param>
        /// <param name="inReplyToPost">返信元のポスト。</param>
        /// <returns>新しい返信の関係。既にバックエンドのデータソースに存在する場合は、生成された返信の関係。</returns>
        public virtual ReplyElement NewReplyElement(
            Post post,
            Post inReplyToPost
        )
        {
            this.CheckIfDisposed();
            StorageDataSet.ReplyMapRow row;
            if ((row = this.LoadReplyMapDataTable(
                post.Activity.PrimaryKeys.AccountId,
                post.PostId,
                inReplyToPost.PrimaryKeys.AccountId,
                inReplyToPost.PostId
            ).SingleOrDefault()) != null)
            {
                return this.GetReplyElement(row);
            }
            ReplyElement element = new ReplyElement()
            {
                Storage = this,
            };
            element.Row.AccountId = post.PrimaryKeys.AccountId;
            element.Row.PostId = post.PostId;
            element.Row.InReplyToAccountId = inReplyToPost.PrimaryKeys.AccountId;
            element.Row.InReplyToPostId = inReplyToPost.PostId;
            element.Connect();
            return element;
        }
        #endregion

        #region TagMap
        /// <summary>
        /// 選択を行う文に後続するクエリ節を指定してバックエンドのデータソースからタグの関係のデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        public virtual StorageDataSet.TagMapDataTable LoadTagMapDataTable(String clauses)
        {
            this.CheckIfDisposed();
            this.UnderlyingDataSet.TagMap.BeginLoadData();
            StorageDataSet.TagMapDataTable table = this.LoadTagMapDataTableImpl(clauses);
            foreach (var activityColumns in table.Select(r => new
            {
                r.AccountId,
                r.Timestamp,
                r.Category,
                r.Subindex,
            }))
            {
                this.LoadActivitiesDataTable(
                    activityColumns.AccountId,
                    activityColumns.Timestamp,
                    activityColumns.Category,
                    activityColumns.Subindex
                );
            }
            this.UnderlyingDataSet.TagMap.EndLoadData();
            return table;
        }

        /// <summary>
        /// 派生クラスで実装された場合、<see cref="LoadTagMapDataTable(String)"/> メソッドを実装し、実際にデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        protected abstract StorageDataSet.TagMapDataTable LoadTagMapDataTableImpl(String clauses);

        /// <summary>
        /// 主キーを指定してバックエンドのデータソースからタグの関係のデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <param name="accountId">タグを付与されている主体であるアクティビティの主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="timestamp">タグを付与されている主体であるアクティビティの行われた日時。指定しない場合は <c>null</c>。</param>
        /// <param name="category">タグを付与されている主体であるアクティビティの種別を表す文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="subindex">タグを付与されている主体であるアクティビティのサブインデックス。指定しない場合は <c>null</c>。</param>
        /// <param name="tag">タグの文字列。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表。</returns>
        public virtual StorageDataSet.TagMapDataTable LoadTagMapDataTable(
            Nullable<Guid> accountId,
            Nullable<DateTime> timestamp,
            String category,
            Nullable<Int32> subindex,
            String tag
        )
        {
            List<String> whereClauses = new List<String>();
            if (accountId.HasValue)
            {
                whereClauses.Add(String.Format("[AccountId] == '{0}'", accountId.Value.ToString("d")));
            }
            if (timestamp.HasValue)
            {
                whereClauses.Add(String.Format("[Timestamp] == datetime('{0}')", timestamp.Value.ToString("s")));
            }
            if (category != null)
            {
                whereClauses.Add(String.Format("[Category] == '{0}'", category));
            }
            if (subindex.HasValue)
            {
                whereClauses.Add(String.Format("[Subindex] == {0}", subindex.Value));
            }
            if (tag != null)
            {
                whereClauses.Add(String.Format("[Tag] == '{0}'", tag));
            }
            return this.LoadTagMapDataTable(whereClauses.Any()
                ? "WHERE " + String.Join(" AND ", whereClauses.ToArray())
                : String.Empty
            );
        }

        /// <summary>
        /// バックエンドのデータソースからタグの関係のデータ表を読み出し、データセット上のデータ表とマージします。
        /// </summary>
        /// <returns>データソースから読み出したデータ表。</returns>
        public StorageDataSet.TagMapDataTable LoadTagMapDataTable()
        {
            return this.LoadTagMapDataTable(null, null, null, null, null);
        }

        /// <summary>
        /// 選択を行う文に後続するクエリ節を指定してバックエンドのデータソースからタグの関係のデータ表を読み出し、データセット上のデータ表とマージした上で、タグの関係を生成します。
        /// </summary>
        /// <param name="clauses">読み出しに使用する、データ表内に存在する全てのデータを取得する文に続くクエリ節文字列。</param>
        /// <returns>データソースから読み出したデータ表から生成されたタグの関係のシーケンス。</returns>
        public IEnumerable<TagElement> LoadTagElements(String clauses)
        {
            return this.GetTagElements(this.LoadTagMapDataTable(clauses));
        }

        /// <summary>
        /// 主キーを指定してバックエンドのデータソースからタグの関係のデータ表を読み出し、データセット上のデータ表とマージした上で、タグの関係を生成します。
        /// </summary>
        /// <param name="accountId">タグを付与されている主体であるアクティビティの主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="timestamp">タグを付与されている主体であるアクティビティの行われた日時。指定しない場合は <c>null</c>。</param>
        /// <param name="category">タグを付与されている主体であるアクティビティの種別を表す文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="subindex">タグを付与されている主体であるアクティビティのサブインデックス。指定しない場合は <c>null</c>。</param>
        /// <param name="tag">タグの文字列。指定しない場合は <c>null</c>。</param>
        /// <returns>データソースから読み出したデータ表から生成されたタグの関係のシーケンス。</returns>
        public IEnumerable<TagElement> LoadTagElements(
            Nullable<Guid> accountId,
            Nullable<DateTime> timestamp,
            String category,
            Nullable<Int32> subindex,
            String tag
        )
        {
            return this.GetTagElements(this.LoadTagMapDataTable(accountId, timestamp, category, subindex, tag));
        }

        /// <summary>
        /// バックエンドのデータソースからタグの関係のデータ表を読み出し、データセット上のデータ表とマージした上で、タグの関係を生成します。
        /// </summary>
        /// <returns>データソースから読み出したデータ表から生成されたタグの関係のシーケンス。</returns>
        public IEnumerable<TagElement> LoadTagElements()
        {
            return this.GetTagElements(this.LoadTagMapDataTable());
        }

        /// <summary>
        /// データセット内で該当するデータ表の全てのデータ行を用いてタグの関係を生成します。
        /// </summary>
        /// <returns>データセット内で該当するデータ表の全てのデータ行を用いて生成されたタグの関係のシーケンス。</returns>
        public IEnumerable<TagElement> GetTagElements()
        {
            return this.GetTagElements(row => true);
        }

        /// <summary>
        /// 指定された条件に合致するデータ行を用いてタグの関係を生成します。
        /// </summary>
        /// <param name="predicate">各データ行が条件に当てはまるかどうかをテストする関数。</param>
        /// <returns>条件に合致したデータ行を用いて生成されたタグの関係のシーケンス。</returns>
        public IEnumerable<TagElement> GetTagElements(Func<StorageDataSet.TagMapRow, Boolean> predicate)
        {
            return this.GetTagElements(this.UnderlyingDataSet.TagMap.Where(predicate));
        }

        /// <summary>
        /// 主キーを指定して、合致するデータ行を用いてタグの関係を生成します。
        /// </summary>
        /// <param name="accountId">タグを付与されている主体であるアクティビティの主体であるアカウントを一意に識別するグローバル一意識別子 (GUID) 値。指定しない場合は <c>null</c>。</param>
        /// <param name="timestamp">タグを付与されている主体であるアクティビティの行われた日時。指定しない場合は <c>null</c>。</param>
        /// <param name="category">タグを付与されている主体であるアクティビティの種別を表す文字列。指定しない場合は <c>null</c>。</param>
        /// <param name="subindex">タグを付与されている主体であるアクティビティのサブインデックス。指定しない場合は <c>null</c>。</param>
        /// <param name="tag">タグの文字列。指定しない場合は <c>null</c>。</param>
        /// <returns>条件に合致したデータ行を用いて生成されたタグの関係のシーケンス。</returns>
        public IEnumerable<TagElement> GetTagElements(
            Nullable<Guid> accountId,
            Nullable<DateTime> timestamp,
            String category,
            Nullable<Int32> subindex,
            String tag
        )
        {
            IEnumerable<TagElement> elements = this.GetTagElements();
            if (accountId.HasValue)
            {
                elements = elements.Where(e => e.UnderlyingDataRow.AccountId == accountId.Value);
            }
            if (timestamp.HasValue)
            {
                elements = elements.Where(e => e.UnderlyingDataRow.Timestamp == timestamp.Value);
            }
            if (category != null)
            {
                elements = elements.Where(e => e.UnderlyingDataRow.Category == category);
            }
            if (subindex.HasValue)
            {
                elements = elements.Where(e => e.UnderlyingDataRow.Subindex == subindex.Value);
            }
            if (tag != null)
            {
                elements = elements.Where(e => e.Tag == tag);
            }
            return elements;
        }

        /// <summary>
        /// データ行のシーケンスを用いてタグの関係を生成します。
        /// </summary>
        /// <param name="rows">生成に用いるデータ行のシーケンス。</param>
        /// <returns>生成されたタグの関係のシーケンス。</returns>
        public IEnumerable<TagElement> GetTagElements(IEnumerable<StorageDataSet.TagMapRow> rows)
        {
            return rows.Select(row => this.GetTagElement(row)).ToList();
        }

        /// <summary>
        /// データ行からタグの関係を生成します。
        /// </summary>
        /// <param name="row">生成に用いるデータ行。</param>
        /// <returns>生成されたタグの関係。</returns>
        public virtual TagElement GetTagElement(StorageDataSet.TagMapRow row)
        {
            this.CheckIfDisposed();
            if (row == null)
            {
                return null;
            }
            TagElement element = new TagElement()
            {
                Storage = this,
                UnderlyingDataRow = row,
            };
            element.Connect();
            return element;
        }

        /// <summary>
        /// 値を指定して、このストレージを使用するタグの関係を初期化します。既にバックエンドのデータソースに対応するデータ行が存在する場合は、データセットにロードされ、そこから生成されたタグの関係を返します。
        /// </summary>
        /// <param name="activity">タグを付与される主体となるアクティビティ。</param>
        /// <param name="tag">付与されるタグの文字列。</param>
        /// <returns>新しいタグの関係。既にバックエンドのデータソースに存在する場合は、生成されたタグの関係。</returns>
        public virtual TagElement NewTagElement(
            Activity activity,
            String tag
        )
        {
            this.CheckIfDisposed();
            StorageDataSet.TagMapRow row;
            if ((row = this.LoadTagMapDataTable(
                activity.PrimaryKeys.AccountId,
                activity.Timestamp,
                activity.Category,
                activity.Subindex,
                tag
            ).SingleOrDefault()) != null)
            {
                return this.GetTagElement(row);
            }
            TagElement element = new TagElement()
            {
                Storage = this,
            };
            element.Row.AccountId = activity.PrimaryKeys.AccountId;
            element.Row.Timestamp = activity.Timestamp;
            element.Row.Category = activity.Category;
            element.Row.Subindex = activity.Subindex;
            element.Row.Tag = tag;
            element.Connect();
            return element;
        }
        #endregion

        /// <summary>
        /// 派生クラスで実装された場合、データセットにおける変更点および関連するその他のデータ行をバックエンドのデータソースに対し追加、更新、削除します。
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// 指定されたストレージとデータセットのデータをマージします。
        /// </summary>
        /// <param name="destination"></param>
        public virtual void Merge(Storage destination)
        {
            this.CheckIfDisposed();
            this.UnderlyingDataSet.Merge(destination.UnderlyingDataSet);
        }
    }
}