// -*- mode: csharp; encoding: utf-8; tab-width: 4; c-basic-offset: 4; indent-tabs-mode: nil; -*-
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using log4net;
using XSpect.Hooking;
using XSpect.MetaTweet.Modules;
using XSpect.Configuration;
using XSpect.MetaTweet.Objects;
using XSpect.MetaTweet.Properties;
using XSpect.Extension;
using Achiral;
using Achiral.Extension;
using System.Reflection;

namespace XSpect.MetaTweet
{
    /// <summary>
    /// MetaTweet サーバの既定の初期化動作を提供します。
    /// </summary>
    /// <remarks>
    /// MetaTweet サーバは、自身と同じディレクトリの <c>init.*</c> コード ファイルを検索します。ファイルが存在しない場合、<see cref="Initialize"/> メソッドが呼び出されます。存在する場合、コードはコンパイルされ、このクラスと置き換わる形で実行されます。
    /// </remarks>
    public class Initializer
        : Object
    {
        private static ServerCore _host;

        /// <summary>
        /// MetaTweet サーバを初期化します。
        /// </summary>
        /// <param name="host">初期化されるサーバ オブジェクト。</param>
        /// <param name="args">サーバ オブジェクトに渡された引数。</param>
        public static void Initialize(ServerCore host, IDictionary<String, String> args)
        {
            _host = host;
            InitializeHooksInObject(_host);
            InitializeHooksInObject(_host.ModuleManager);
            _host.ModuleManager.LoadHook.Succeeded.Add((manager, domainKey, ret) =>
            {
                ModuleDomain dom = ret as ModuleDomain;
                InitializeHooksInObject(dom);
                manager.Log.InfoFormat(Resources.ModuleAssemblyLoaded, domainKey);
                dom.AddHook.Succeeded.AddRange(
                    (domain, moduleKey, typeName, configFile, module) =>
                        manager.Log.InfoFormat(
                            Resources.ModuleObjectAdded, domainKey, moduleKey, typeName, configFile.Null(f => f.Name)
                        ),
                    (domain, moduleKey, typeName, configFile, module) =>
                        RegisterModuleHook(module),
                    (domain, moduleKey, typeName, configFile, module) =>
                        module.Initialize()
                );
                dom.RemoveHook.Succeeded.Add((domain, moduleKey, type) =>
                    manager.Log.InfoFormat(Resources.ModuleObjectRemoved, domainKey, type.FullName, moduleKey)
                );
            });
            _host.ModuleManager.UnloadHook.Succeeded.Add((self, domain) =>
                self.Log.InfoFormat(Resources.ModuleAssemblyUnloaded, domain)
            );
            host.ModuleManager.Configuration.ResolveChild("init").ForEach(entry =>
            {
                host.ModuleManager.Load(entry.Key);
                entry.Get<IList<Struct<String, String>>>()
                    .ForEach(e => host.ModuleManager[entry.Key].Add(e.Item1, e.Item2));
            });
        }

        private static void RegisterModuleHook(IModule module)
        {
            InitializeHooksInObject(module);
            module.InitializeHook.Before.Add((self) =>
                self.Log.InfoFormat(
                    Resources.ModuleObjectInitializing,
                    self.Name
                )
            );
            module.InitializeHook.Succeeded.Add((self) =>
                self.Log.InfoFormat(
                    Resources.ModuleObjectInitialized,
                    self.Name,
                    self.Configuration.ConfigurationFile.Name
                )
            );

            if (module is InputFlowModule)
            {
                var input = module as InputFlowModule;
                input.InputHook.Before.Add((self, selector, storage, args) =>
                    self.Log.InfoFormat(
                        Resources.InputFlowPerforming,
                        self.Name,
                        selector,
                        storage.Name,
                        args.Inspect().Indent(4)
                    )
                );
                input.InputHook.Succeeded.Add((self, selector, storage, args, ret) =>
                    self.Log.InfoFormat(Resources.InputFlowPerformed, self.Name, ret.Count()
                        .If(i => i > 1, i => i + " objects", i => i + " object")
                    )
                );
            }
            else if (module is FilterFlowModule)
            {
                var filter = module as FilterFlowModule;
                filter.FilterHook.Before.Add((self, selector, source, storage, args) =>
                    self.Log.InfoFormat(
                        Resources.FilterFlowPerforming,
                        self.Name,
                        selector,
                        source.Count(),
                        storage.Name,
                        args.Inspect().Indent(4)
                    )
                );
                filter.FilterHook.Succeeded.Add((self, selector, source, storage, args ,ret) =>
                    self.Log.InfoFormat(Resources.FilterFlowPerformed, self.Name, ret.Count()
                        .If(i => i > 1, i => i + " objects", i => i +  " object")
                    )
                );
            }
            else if (module is OutputFlowModule)
            {
                var output = module as OutputFlowModule;
                output.OutputHook.Before.Add((self, selector, source, storage, args, type) =>
                    self.Log.InfoFormat(
                        Resources.OutputFlowPerforming,
                        self.Name,
                        selector,
                        source.Count(),
                        storage.Name,
                        args.Inspect().Indent(4),
                        type.FullName
                    )
                );
                output.OutputHook.Succeeded.Add((self, selector, source, storage, args, type, ret) =>
                    self.Log.InfoFormat(Resources.OutputFlowPerformed, self.Name)
                );
            }
            else if (module is ServantModule)
            {
                var servant = module as ServantModule;
                servant.StartHook.Before.Add(self => self.Log.InfoFormat(Resources.ServantStarting, self.Name));
                servant.StartHook.Succeeded.Add(self => self.Log.InfoFormat(Resources.ServantStarted, self.Name));
                servant.StopHook.Before.Add(self => self.Log.InfoFormat(Resources.ServantStopping, self.Name));
                servant.StopHook.Succeeded.Add(self => self.Log.InfoFormat(Resources.ServantStopped, self.Name));
                servant.AbortHook.Before.Add(self => self.Log.InfoFormat(Resources.ServantAborting, self.Name));
                servant.AbortHook.Succeeded.Add(self => self.Log.InfoFormat(Resources.ServantAborted, self.Name));
            }
            else if (module is StorageModule)
            {
                var storage = module as StorageModule;
                storage.GetAccountsHook.Succeeded.Add((self, accountId, realm, ret) =>
                    self.Log.DebugFormat(
                        Resources.StorageGotAccounts,
                        self.Name,
                        accountId != null ? accountId.Value.ToString("D") : "(null)",
                        realm ?? "(null)",
                        ret.Count().If(i => i > 1, i => i + " objects", i => i + " object")
                    )
                );
                storage.GetActivitiesHook.Succeeded.Add((self, accountId, timestamp, category, subId, userAgent, value, data, ret) =>
                    self.Log.DebugFormat(
                        Resources.StorageGotActivities,
                        self.Name,
                        accountId != null ? accountId.Value.ToString("D") : "(null)",
                        timestamp != null ? timestamp.Value.ToString("R") : "(null)",
                        category ?? "(null)",
                        subId ?? "(null)",
                        userAgent ?? "(null)",
                        value != null ? (value is DBNull ? "(DBNull)" : value) : "(null)",
                        data != null ? (data is DBNull ? "(DBNull)" : "Byte[" + (value as Byte[]).Length + "]") : "(null)",
                        ret.Count().If(i => i > 1, i => i + " objects", i => i + " object")
                    )
                );
                storage.GetAnnotationsHook.Succeeded.Add((self, accountId, name, ret) =>
                    self.Log.DebugFormat(
                        Resources.StorageGotAnnotations,
                        self.Name,
                        accountId != null ? accountId.Value.ToString("D") : "(null)",
                        name ?? "(null)",
                        ret.Count().If(i => i > 1, i => i + " objects", i => i + " object")
                    )
                );
                storage.GetRelationsHook.Succeeded.Add((self, accountId, name, relatingAccountId, ret) =>
                    self.Log.DebugFormat(
                        Resources.StorageGotRelations,
                        self.Name,
                        accountId != null ? accountId.Value.ToString("D") : "(null)",
                        name ?? "(null)",
                        relatingAccountId != null ? relatingAccountId.Value.ToString("D") : "(null)",
                        ret.Count().If(i => i > 1, i => i + " objects", i => i + " object")
                    )
                );
                storage.GetMarksHook.Succeeded.Add((self, accountId, name, markingAccountId, markingTimestamp, markingCategory, markingSubId, ret) =>
                    self.Log.DebugFormat(
                        Resources.StorageGotMarks,
                        self.Name,
                        accountId != null ? accountId.Value.ToString("D") : "(null)",
                        name ?? "(null)",
                        markingAccountId != null ? markingAccountId.Value.ToString("D") : "(null)",
                        markingTimestamp != null ? markingTimestamp.Value.ToString("R") : "(null)",
                        markingCategory ?? "(null)",
                        markingSubId ?? "(null)",
                        ret.Count().If(i => i > 1, i => i + " objects", i => i + " object")
                    )
                );
                storage.GetReferencesHook.Succeeded.Add((self, accountId, timestamp, category, subId, name, referringAccountId, referringTimestamp, referringCategory, referringSubId, ret) =>
                    self.Log.DebugFormat(
                        Resources.StorageGotReferences,
                        self.Name,
                        accountId != null ? accountId.Value.ToString("D") : "(null)",
                        timestamp != null ? timestamp.Value.ToString("R") : "(null)",
                        category ?? "(null)",
                        subId ?? "(null)",
                        name ?? "(null)",
                        referringAccountId != null ? referringAccountId.Value.ToString("D") : "(null)",
                        referringTimestamp != null ? referringTimestamp.Value.ToString("R") : "(null)",
                        referringCategory ?? "(null)",
                        referringSubId ?? "(null)",
                        ret.Count().If(i => i > 1, i => i + " objects", i => i + " object")
                    )
                );
                storage.GetTagsHook.Succeeded.Add((self, accountId, timestamp, category, subId, name, ret) =>
                    self.Log.DebugFormat(
                        Resources.StorageGotTags,
                        self.Name,
                        accountId != null ? accountId.Value.ToString("D") : "(null)",
                        timestamp != null ? timestamp.Value.ToString("R") : "(null)",
                        category ?? "(null)",
                        subId ?? "(null)",
                        name ?? "(null)",
                        ret.Count().If(i => i > 1, i => i + " objects", i => i + " object")
                    )
                );
                storage.NewAccountHook.Succeeded.Add((self, accountId, realm, ret) =>
                    self.Log.DebugFormat(
                        ret.Item2 ? Resources.StorageAddedAccount : Resources.StorageAddedExistingAccount,
                        self.Name,
                        ret.Item1.ToString()
                    )
                );
                storage.NewActivityHook.Succeeded.Add((self, account, timestamp, category, subid, userAgent, value, data, ret) =>
                    self.Log.DebugFormat(
                        ret.Item2 ? Resources.StorageAddedActivity : Resources.StorageAddedExistingActivity,
                        self.Name,
                        ret.Item1.ToString()
                    )
                );
                storage.NewAnnotationHook.Succeeded.Add((self, account, name, ret) =>
                    self.Log.DebugFormat(
                        ret.Item2 ? Resources.StorageAddedAnnotation : Resources.StorageAddedExistingAnnotation,
                        self.Name,
                        ret.Item1.ToString()
                    )
                );
                storage.NewRelationHook.Succeeded.Add((self, account, name, relatingAccount, ret) =>
                    self.Log.DebugFormat(
                        ret.Item2 ? Resources.StorageAddedRelation : Resources.StorageAddedExistingRelation,
                        self.Name,
                        ret.Item1.ToString()
                    )
                );
                storage.NewMarkHook.Succeeded.Add((self, account, name, markingActivity, ret) =>
                    self.Log.DebugFormat(
                        ret.Item2 ? Resources.StorageAddedMark : Resources.StorageAddedExistingMark,
                        self.Name,
                        ret.Item1.ToString()
                    )
                );
                storage.NewReferenceHook.Succeeded.Add((self, activity, name, referringActivity, ret) =>
                    self.Log.DebugFormat(
                        ret.Item2 ? Resources.StorageAddedReference : Resources.StorageAddedExistingReference,
                        self.Name,
                        ret.Item1.ToString()
                    )
                );
                storage.NewTagHook.Succeeded.Add((self, activity, name, ret) =>
                    self.Log.DebugFormat(
                        ret.Item2 ? Resources.StorageAddedTag : Resources.StorageAddedExistingTag,
                        self.Name,
                        ret.Item1.ToString()
                    )
                );
                storage.UpdateHook.Succeeded.Add((self, ret) =>
                    self.Log.InfoFormat(
                        Resources.StorageUpdated,
                        self.Name,
                        ret.If(i => i > 1, i => i + " objects", i => i + " object")
                    )
                );
#if DEBUG
                List<String> newObjects = new List<String>();
                Action<Tuple<StorageObject, Boolean>> check = _ =>
                {
                    if (_.Item2)
                    {
                        String str = _.Item1.ToString();
                        Debug.Assert(!newObjects.Contains(str), "Duplicate object was created!", str);
                        newObjects.Add(str);
                        if (_host.Parameters.Contains("debug", "true"))
                        {
                            File.AppendAllText(
                                _host.Directories.LogDirectory.File("dbg_newobj.log.tsv").FullName,
                                String.Format("{0}\t{1}\t{2}\n",
                                    newObjects.Count - 1,
                                    DateTime.UtcNow.ToString("s"),
                                    str
                                )
                            );
                        }
                    }
                };
                storage.NewAccountHook.Succeeded.Add((self, accountId, realm, ret) => check(ret.Select((_, c) => new Tuple<StorageObject, Boolean>(_, c))));
                storage.NewActivityHook.Succeeded.Add((self, account, timestamp, category, subid, userAgent, value, data, ret) => check(ret.Select((_, c) => new Tuple<StorageObject, Boolean>(_, c))));
                storage.NewAnnotationHook.Succeeded.Add((self, account, name, ret) => check(ret.Select((_, c) => new Tuple<StorageObject, Boolean>(_, c))));
                storage.NewRelationHook.Succeeded.Add((self, account, name, relatingAccount, ret) => check(ret.Select((_, c) => new Tuple<StorageObject, Boolean>(_, c))));
                storage.NewMarkHook.Succeeded.Add((self, account, name, markingActivity, ret) => check(ret.Select((_, c) => new Tuple<StorageObject, Boolean>(_, c))));
                storage.NewReferenceHook.Succeeded.Add((self, activity, name, referringActivity, ret) => check(ret.Select((_, c) => new Tuple<StorageObject, Boolean>(_, c))));
                storage.NewTagHook.Succeeded.Add((self, activity, name, ret) => check(ret.Select((_, c) => new Tuple<StorageObject, Boolean>(_, c))));
#endif
            }
        }

        private static void InitializeHooksInObject(Object obj)
        {
            obj.GetType().GetProperties()
                .Where(p => p.PropertyType.BaseType.Do(
                    t => t != null
                        && t.IsGenericType
                        && t.GetGenericTypeDefinition() == typeof(Hook<,,,,>)
                ))
                .Select(p => p.GetValue(obj, null))
                .Select(o => o.GetType().GetProperty("Failed").GetValue(o, null) as IList)
                .ForEach(l => l.Add(l.GetType()
                    .GetGenericArguments()
                    .Single()
                    .Do(d => d.GetGenericArguments()
                        .Select((t, i) => Expression.Parameter(t, "_" + i))
                        .ToArray()
                        .Do(p =>
                            // (self, ..., ex) => (self as ILoggable).Log.Fatal("Unhandled exception occured.", ex);
                            Expression.Lambda(
                                d,
                                Expression.Call(
                                    Expression.Property(
                                        Expression.TypeAs(p.First(), typeof(ILoggable)),
                                        typeof(ILoggable).GetProperty("Log")
                                    ),
                                    typeof(ILog).GetMethod("Fatal", Create.TypeArray<Object, Exception>()),
                                    Expression.Constant("Unhandled exception occured."),
                                    p.Last()
                                ),
                                p
                            )
                            .Compile()
                        )
                    )
                ));
        }
    }
}
