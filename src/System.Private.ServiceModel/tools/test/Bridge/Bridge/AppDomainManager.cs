// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using WcfTestBridgeCommon;

namespace Bridge
{
    public static class AppDomainManager
    {
        public static string UpdateApp(BridgeConfiguration oldConfig, BridgeConfiguration newConfig)
        {
            if (!String.Equals(oldConfig.BridgeResourceFolder, newConfig.BridgeResourceFolder, StringComparison.OrdinalIgnoreCase))
            {
                var newPath = Path.GetFullPath(newConfig.BridgeResourceFolder);
                Trace.WriteLine("Adding assemblies in the directory");
                string friendlyName = CreateAppDomain(newPath);
                return friendlyName;
            }

            return "BridgeAppDomain" + (TypeCache.AppDomains.Count - 1);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static string CreateAppDomain(string path)
        {
            string friendlyName = "BridgeAppDomain" + TypeCache.AppDomains.Count;
            var appDomainSetup = new AppDomainSetup();
            appDomainSetup.ApplicationBase = path;
            var newAppDomain = AppDomain.CreateDomain(friendlyName, AppDomain.CurrentDomain.Evidence, appDomainSetup);

            Type loaderType = typeof(AssemblyLoader);
            var loader =
                (AssemblyLoader)newAppDomain.CreateInstanceFromAndUnwrap(
                    Path.Combine(path, "WcfTestBridgeCommon.dll"),
                    loaderType.FullName);
            loader.LoadAssemblies();

            TypeCache.AppDomains.Add(friendlyName, newAppDomain);
            TypeCache.Cache.Add(friendlyName, loader.GetTypes());

            return friendlyName;
        }
    }

    internal static class TypeCache
    {
        private static Dictionary<string, AppDomain> s_appDomains = new Dictionary<string, AppDomain>();
        private static IDictionary<string, List<string>> s_cache = new Dictionary<string, List<string>>();

        public static Dictionary<string, AppDomain> AppDomains { get { return s_appDomains; } }

        public static IDictionary<string, List<string>> Cache { get { return s_cache; } }
    }
}
