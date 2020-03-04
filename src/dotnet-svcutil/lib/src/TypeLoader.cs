// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal static class TypeLoader
    {
        static List<string> LoadTypesWarnings = new List<string>();

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
                ToolConsole.WriteWarning(SR.GetString(SR.ErrUnableToLoadReferenceFormat, path, ex.Message));
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
                        if (!LoadTypesWarnings.Contains(warning))
                        {
                            LoadTypesWarnings.Add(warning);
                            ToolConsole.WriteWarning(warning);
                        }
                    }

                    if (types.Length == 0)
                    {
                        warning = SR.GetString(SR.ErrCouldNotLoadTypesFromAssemblyAtFormat, assembly.Location);
                        if (!LoadTypesWarnings.Contains(warning))
                        {
                            LoadTypesWarnings.Add(warning);
                            ToolConsole.WriteWarning(warning);
                        }
                    }
                    else
                    {
                        warning = SR.GetString(SR.WrnCouldNotLoadTypesFromReferenceAssemblyAtFormat, assembly.Location);

                        if (!LoadTypesWarnings.Contains(warning))
                        {
                            LoadTypesWarnings.Add(warning);
                            ToolConsole.WriteWarning(warning);
                        }
                    }
                }
            }
            return types;
        }
    }
}
