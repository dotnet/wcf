// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal static class TypeLoader
    {
        private static List<string> s_loadTypesWarnings = new List<string>();

        static public Assembly LoadAssembly(string path)
        {
            string DotDll = ".dll";
            string DotExe = ".exe";

            if (path.EndsWith(DotDll, StringComparison.OrdinalIgnoreCase) || path.EndsWith(DotExe, StringComparison.OrdinalIgnoreCase))
            {
                path = path.Remove(path.Length - DotDll.Length, DotDll.Length);
            }
            try
            {
                return Assembly.Load(new AssemblyName(path));
            }
            catch (Exception ex)
            {
                ToolConsole.WriteWarning(string.Format(SR.ErrUnableToLoadReferenceFormat, path, ex.Message));
                return null;
            }
        }

        static public Type[] LoadTypes(Assembly assembly, Verbosity verbosity)
        {
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException rtle)
            {
                string warning;
                types = Array.FindAll<Type>(rtle.Types, delegate (Type t) { return t != null; });

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

                    if (types.Length == 0)
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
            return types;
        }
    }
}
