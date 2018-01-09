//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace Microsoft.Tools.ServiceModel.SvcUtil
{

    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.ServiceModel;

    class TypeResolver
    {
        Assembly assembly;
        Options toolOptions;

        internal TypeResolver(Options toolOptions)
        {
            this.toolOptions = toolOptions;
        }

        public Assembly ResolveType(object sender, ResolveEventArgs args)
        {
            if (assembly != null)
            {
                return assembly;
            }

            return null;
        }

        public Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);

            if (assembly != null && AssemblyName.ReferenceMatchesDefinition(assembly.GetName(), assemblyName))
            {
                return assembly;
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
                return this.assembly;
            }
            set
            {
                this.assembly = value;
            }
        }

        Assembly FindAssembly(AssemblyName assemblyName)
        {
            Assembly match = null;
            foreach (Assembly refAssembly in toolOptions.ReferencedAssemblies)
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
