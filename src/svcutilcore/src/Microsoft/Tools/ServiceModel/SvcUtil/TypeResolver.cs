// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.SvcUtil.XmlSerializer
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class TypeResolver
    {
        private Assembly _assembly;
        private Options _toolOptions;

        internal TypeResolver(Options toolOptions)
        {
            _toolOptions = toolOptions;
        }

        public Assembly ResolveType(object sender, ResolveEventArgs args)
        {
            if (_assembly != null)
            {
                return _assembly;
            }

            return null;
        }

        public Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);

            if (_assembly != null && AssemblyName.ReferenceMatchesDefinition(_assembly.GetName(), assemblyName))
            {
                return _assembly;
            }

            Assembly refAssembly = FindAssembly(assemblyName);
            if (refAssembly != null)
                return refAssembly;

            return null;
        }

        internal Assembly Assembly
        {
            get
            {
                return _assembly;
            }
            set
            {
                _assembly = value;
            }
        }

        private Assembly FindAssembly(AssemblyName assemblyName)
        {
            Assembly match = null;
            foreach (Assembly refAssembly in _toolOptions.ReferencedAssemblies)
            {
                if (AssemblyName.ReferenceMatchesDefinition(refAssembly.GetName(), assemblyName))
                {
                    if (match == null)
                    {
                        match = refAssembly;
                    }
                    else
                    {
                        throw new TypeResolver.Exception(SR.Format(SR.ErrAmbiguityInAssemblyNames, assemblyName.FullName, match.FullName, refAssembly.FullName));
                    }
                }
            }
            return match;
        }

        internal class Exception : InvalidOperationException
        {
            internal Exception(string message) : base(message) { }
        }
    }
}
