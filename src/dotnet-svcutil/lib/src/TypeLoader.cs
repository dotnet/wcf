// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal static class TypeLoader
    {
        private static List<string> s_loadTypesWarnings = new List<string>();

        static public Assembly LoadAssembly(string path)
        {
            string[] runtimeAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
            var paths = new List<string>(runtimeAssemblies);

            var resolver = new PathAssemblyResolver(paths);
            var mlc = new MetadataLoadContext(resolver);
            
            try
            {
                return mlc.LoadFromAssemblyPath(path);
            }
            catch (Exception ex)
            {
                ToolConsole.WriteWarning(string.Format(SR.ErrUnableToLoadReferenceFormat, path, ex.Message));
                return null;
            }
        }

        static public Type[] LoadTypes(Assembly assembly, Verbosity verbosity)
        {
            List<Type> listType = new List<Type>();

            try
            {
                listType.AddRange(assembly.GetTypes());
            }
            catch (ReflectionTypeLoadException rtle)
            {
                string warning;
                listType.AddRange(Array.FindAll<Type>(rtle.Types, delegate (Type t)
                {
                    return t != null;
                }));               

                if (verbosity > Verbosity.Normal)
                {
                    foreach (var ex in rtle.LoaderExceptions)
                    {
                        warning = ex.Message;
                        if (!s_loadTypesWarnings.Contains(warning))
                        {
                            s_loadTypesWarnings.Add(warning);
                            ToolConsole.WriteWarning(warning);
                        }
                    }

                    if (listType.Count == 0)
                    {
                        warning = string.Format(SR.ErrCouldNotLoadTypesFromAssemblyAtFormat, assembly.Location);
                        if (!s_loadTypesWarnings.Contains(warning))
                        {
                            s_loadTypesWarnings.Add(warning);
                            ToolConsole.WriteWarning(warning);
                        }
                    }
                    else
                    {
                        warning = string.Format(SR.WrnCouldNotLoadTypesFromReferenceAssemblyAtFormat, assembly.Location);

                        if (!s_loadTypesWarnings.Contains(warning))
                        {
                            s_loadTypesWarnings.Add(warning);
                            ToolConsole.WriteWarning(warning);
                        }
                    }
                }
            }

            return listType.ToArray();
        }

        static private List<Type> GetUnAvailableTypes(List<Type> types)
        {
            List<Type> unavailableTypes = new List<Type>();
            foreach (Type type in types)
            {
                try
                {
                    type.Assembly.GetCustomAttributes(typeof(ContractNamespaceAttribute));
                    type.Module.GetCustomAttributes(typeof(ContractNamespaceAttribute));
                }
                catch (FileNotFoundException)
                {
                    unavailableTypes.Add(type);
                }
            }

            return unavailableTypes;
        }
    }
}
