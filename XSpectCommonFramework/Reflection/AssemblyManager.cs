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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Achiral;
using System.Collections.ObjectModel;
using System.Security.Policy;
using Achiral.Extension;
using System.IO;
using XSpect.Extension;

namespace XSpect.Reflection
{
    public partial class AssemblyManager
        : MarshalByRefObject,
          ICollection<AssemblyName>,
          IDisposable
    {
        protected IDictionary<String, KeyValuePair<AppDomain, Assembly>> Domains
        {
            get;
            set;
        }

        public Assembly this[String key]
        {
            get
            {
                return this.Domains[key].Value;
            }
        }

        public Assembly this[AssemblyName assemblyRef]
        {
            get
            {
                return this[assemblyRef.FullName];
            }
        }

        public Int32 Count
        {
            get
            {
                return this.Domains.Count;
            }
        }

        public Boolean IsReadOnly
        {
            get
            {
                return false;
            }
        }
        
        public AppDomainSetup DefaultAppDomainSetup
        {
            get;
            set;
        }

        public Evidence DefaultEvidence
        {
            get;
            set;
        }

        public IDictionary<String, String> DefaultOptions
        {
            get;
            set;
        }

        public CompilerParameters DefaultParameters
        {
            get;
            set;
        }

        public AssemblyManager()
        {
            this.Domains = new Dictionary<String, KeyValuePair<AppDomain, Assembly>>();
            this.DefaultAppDomainSetup = new AppDomainSetup();
            this.DefaultEvidence = null;
            this.DefaultOptions = new Dictionary<String, String>();
            this.DefaultParameters = new CompilerParameters();
        }

        public virtual void Add(AssemblyName assemblyRef)
        {
            this.Load(assemblyRef.FullName, assemblyRef);
        }

        public virtual void Clear()
        {
            foreach (String key in this.Domains.Keys.ToArray())
            {
                this.Unload(key);
            }
        }

        public virtual Boolean Contains(AssemblyName assemblyRef)
        {
            return this.FindKey(assemblyRef) != null;
        }

        public virtual void CopyTo(AssemblyName[] array, Int32 arrayIndex)
        {
            this.Domains.Values.Select(v => v.Value.GetName()).ToArray().CopyTo(array, arrayIndex);
        }

        public virtual Boolean Remove(AssemblyName assemblyRef)
        {
            return this.Unload(this.FindKey(assemblyRef));
        }

        public virtual IEnumerator<AssemblyName> GetEnumerator()
        {
            return this.Domains.Values.Select(v => v.Value.GetName()).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public virtual void Dispose()
        {
            this.Clear();
        }

        public virtual Assembly Load(
            String key,
            AssemblyName assemblyRef,
            Evidence securityInfo,
            AppDomainSetup info
        )
        {
            AppDomain domain = this.CreateDomain(key, securityInfo, info);
            Assembly assembly = new LoadHelper(domain, assemblyRef).Load();
            return this.RegisterAssembly(key, domain, assembly);
        }
        
        public Assembly Load(
            String key,
            AssemblyName assemblyRef
        )
        {
            return this.Load(key, assemblyRef, this.DefaultEvidence, this.DefaultAppDomainSetup);
        }

        public virtual Assembly Load(
            String key,
            String assemblyString,
            Evidence securityInfo,
            AppDomainSetup info
        )
        {
            AppDomain domain = this.CreateDomain(key, securityInfo, info);
            Assembly assembly = new LoadHelper(domain, assemblyString).Load();
            return this.RegisterAssembly(key, domain, assembly);
        }

        public Assembly Load(
            String key,
            String assemblyString
        )
        {
            return this.Load(key, assemblyString, this.DefaultEvidence, this.DefaultAppDomainSetup);
        }

        public virtual Assembly Load(
            String key,
            Byte[] rawAssembly,
            Evidence securityInfo,
            AppDomainSetup info
        )
        {
            AppDomain domain = this.CreateDomain(key, securityInfo, info);
            Assembly assembly = new LoadHelper(domain, rawAssembly).Load();
            return this.RegisterAssembly(key, domain, assembly);
        }

        public Assembly Load(
            String key,
            Byte[] rawAssembly
        )
        {
            return this.Load(key, rawAssembly, this.DefaultEvidence, this.DefaultAppDomainSetup);
        }

        public virtual Assembly Load(
            String key,
            Byte[] rawAssembly,
            Byte[] rawSymbolStore,
            Evidence securityInfo,
            AppDomainSetup info
        )
        {
            AppDomain domain = this.CreateDomain(key, securityInfo, info);
            Assembly assembly = new LoadHelper(domain, rawAssembly, rawSymbolStore).Load();
            return this.RegisterAssembly(key, domain, assembly);
        }

        public Assembly Load(
            String key,
            Byte[] rawAssembly,
            Byte[] rawSymbolStore
        )
        {
            return this.Load(key, rawAssembly, rawSymbolStore, this.DefaultEvidence, this.DefaultAppDomainSetup);
        }

        public virtual Assembly Load(
            String key,
            FileInfo assemblyFile,
            FileInfo symbolStoreFile,
            Evidence securityInfo,
            AppDomainSetup info
        )
        {
            return this.Load(
                key,
                assemblyFile.OpenRead().Dispose(s => s.ReadAll()),
                symbolStoreFile != null
                    ? symbolStoreFile.OpenRead().Dispose(s => s.ReadAll())
                    : null,
                securityInfo,
                info
            );
        }

        public Assembly Load(
            String key,
            FileInfo assemblyFile,
            FileInfo symbolStoreFile
        )
        {
            return this.Load(key, assemblyFile, symbolStoreFile, this.DefaultEvidence, this.DefaultAppDomainSetup);
        }

        public virtual Assembly Load(
            String key,
            FileInfo assemblyFile,
            Evidence securityInfo,
            AppDomainSetup info
        )
        {
            return this.Load(key, assemblyFile, null, securityInfo, info);
        }

        public Assembly Load(
            String key,
            FileInfo assemblyFile
        )
        {
            return this.Load(key, assemblyFile, null, this.DefaultEvidence, this.DefaultAppDomainSetup);
        }

        public Assembly LoadFile(
            String key,
            String assemblyPath,
            Evidence securityInfo,
            AppDomainSetup info
        )
        {
            AppDomain domain = this.CreateDomain(key, securityInfo, info);
            Assembly assembly = new LoadHelper(domain, assemblyPath).LoadFile();
            return this.RegisterAssembly(key, domain, assembly);
        }

        public Assembly LoadFile(
            String key,
            String assemblyPath
        )
        {
            return this.LoadFile(key, assemblyPath, this.DefaultEvidence, this.DefaultAppDomainSetup);
        }

        public Assembly LoadFile(
            String key,
            FileInfo assemblyFile,
            Evidence securityInfo,
            AppDomainSetup info
        )
        {
            AppDomain domain = this.CreateDomain(key, securityInfo, info);
            Assembly assembly = new LoadHelper(domain, assemblyFile.FullName).LoadFile();
            return this.RegisterAssembly(key, domain, assembly);
        }

        public Assembly LoadFile(
            String key,
            FileInfo assemblyFile
        )
        {
            return this.LoadFile(key, assemblyFile.FullName);
        }

        public Assembly LoadFrom(
            String key,
            String assemblyFile,
            Evidence securityInfo,
            AppDomainSetup info
        )
        {
            AppDomain domain = this.CreateDomain(key, securityInfo, info);
            Assembly assembly = new LoadHelper(domain, assemblyFile).LoadFrom();
            return this.RegisterAssembly(key, domain, assembly);
        }

        public Assembly LoadFrom(
            String key,
            String assemblyFile
        )
        {
            return this.LoadFrom(key, assemblyFile, this.DefaultEvidence, this.DefaultAppDomainSetup);
        }

        public Assembly LoadFrom(
            String key,
            FileInfo assemblyFile,
            Evidence securityInfo,
            AppDomainSetup info
        )
        {
            AppDomain domain = this.CreateDomain(key, securityInfo, info);
            Assembly assembly = new LoadHelper(domain, assemblyFile.FullName).LoadFrom();
            return this.RegisterAssembly(key, domain, assembly);
        }

        public Assembly LoadFrom(
            String key,
            FileInfo assemblyFile
        )
        {
            return this.LoadFrom(key, assemblyFile.FullName);
        }

        public virtual Assembly Compile(
            String key,
            String language,
            IDictionary<String, String> options,
            CompilerParameters parameters,
            Evidence securityInfo,
            AppDomainSetup info,
            params String[] sources
        )
        {
            AppDomain domain = this.CreateDomain(key, securityInfo, info);
            parameters.OutputAssembly = Guid.NewGuid().ToString("n") + ".dll";
            Assembly assembly = new CompileHelper(
                domain,
                this.GetCodeDomProvider(language, options),
                parameters,
                sources
            ).Compile();
            if (this.Contains(assembly.GetName()))
            {
                this.Unload(key);
                throw new ArgumentException("Specified assembly is already being loaded.", "rawAssembly");
            }
            this.Domains.Add(key, new KeyValuePair<AppDomain, Assembly>(domain, assembly));
            return assembly;
        }

        public Assembly Compile(
            String key,
            String language,
            params String[] sources
        )
        {
            return this.Compile(
                key,
                language,
                this.DefaultOptions,
                this.DefaultParameters,
                this.DefaultEvidence,
                this.DefaultAppDomainSetup,
                sources
            );
        }

        public virtual Assembly Compile(
            String key,
            IDictionary<String, String> options,
            CompilerParameters parameters,
            Evidence securityInfo,
            AppDomainSetup info,
            params FileInfo[] files
        )
        {
            return this.Compile(
                key,
                files.First().Extension,
                options,
                parameters,
                securityInfo,
                info,
                files
                    .Select(f => f.OpenText().Dispose(r => r.ReadToEnd()))
                    .ToArray()
            );
        }

        public Assembly Compile(
            String key,
            params FileInfo[] files
        )
        {
            return this.Compile(
                key,
                this.DefaultOptions,
                this.DefaultParameters,
                this.DefaultEvidence,
                this.DefaultAppDomainSetup,
                files
            );
        }

        public virtual Assembly Compile(
            String key,
            IDictionary<String, String> options,
            CompilerParameters parameters,
            Evidence securityInfo,
            AppDomainSetup info,
            params String[] files
        )
        {
            return this.Compile(
                key,
                options,
                parameters,
                securityInfo,
                info,
                files.Select(s => new FileInfo(s)).ToArray()
            );
        }

        public Assembly Compile(
            String key,
            params String[] files
        )
        {
            return this.Compile(
                key,
                this.DefaultOptions,
                this.DefaultParameters,
                this.DefaultEvidence,
                this.DefaultAppDomainSetup,
                files
            );
        }

        public virtual String FindKey(AssemblyName assemblyRef)
        {
            return this.Domains.SingleOrDefault(p => p.Value.Value.GetName() == assemblyRef).Key;
        }

        public virtual Boolean Unload(String key)
        {
            AppDomain domain = this.Domains[key].Key;
            if (domain != null)
            {
                this.Domains.Remove(key);
                this.UnloadDomain(domain);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual AppDomain CreateDomain(String key, Evidence securityInfo, AppDomainSetup info)
        {
            AppDomain domain = AppDomain.CreateDomain("_XcfAsmMgr_" + key, securityInfo, info);
            return domain;
        }

        protected virtual void UnloadDomain(AppDomain domain)
        {
            AppDomain.Unload(domain);
        }

        protected virtual CodeDomProvider GetCodeDomProvider(String language, IDictionary<String, String> options)
        {
            Type providerType = CodeDomProvider.GetCompilerInfo(
                language.StartsWith(".")
                    ? CodeDomProvider.GetLanguageFromExtension(language)
                    : language
            ).CodeDomProviderType;
            ConstructorInfo constructor;

            if ((constructor = providerType.GetConstructors().SingleOrDefault(c =>
                c.GetParameters().Any(p => p.ParameterType == typeof(IDictionary<String, String>))
            )) != null)
            {
                return constructor.Invoke(new Object[]
                {
                    options,
                }) as CodeDomProvider;
            }
            else
            {
                return providerType.GetConstructor(Type.EmptyTypes).Invoke(new Object[]
                {
                }) as CodeDomProvider;
            }
        }

        protected virtual Assembly RegisterAssembly(String key, AppDomain domain, Assembly assembly)
        {
            if (this.Contains(assembly.GetName()))
            {
                this.Unload(key);
                throw new ArgumentException("Specified assembly is already being loaded.", "rawAssembly");
            }
            this.Domains.Add(key, new KeyValuePair<AppDomain, Assembly>(domain, assembly));
            return assembly;
        }
    }
}