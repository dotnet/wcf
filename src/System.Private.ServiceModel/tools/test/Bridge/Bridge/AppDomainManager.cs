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
        // This method is called whenever the resource folder location changes.
        // Null is allowed and means there is no resource folder.
        public static string OnResourceFolderChanged(string oldFolder, string newFolder)
        {
            // Any change to the folder shuts down the prior AppDomain
            if (!String.Equals(oldFolder, newFolder))
            {
                if (!(String.IsNullOrEmpty(ConfigController.CurrentAppDomainName)))
                {
                    Trace.WriteLine(String.Format("{0:T} Shutting down the appDomain for {1}", DateTime.Now, ConfigController.CurrentAppDomainName),
                                    typeof(AppDomainManager).Name);
                    ShutdownAppDomain(ConfigController.CurrentAppDomainName);
                }
            }

            if (newFolder == null)
            {
                return null;
            }

            var newPath = Path.GetFullPath(newFolder);
            Trace.WriteLine(String.Format("{0:T} Adding assemblies from the resource folder {1}", DateTime.Now, newPath), 
                            typeof(AppDomainManager).Name);
            return CreateAppDomain(newPath);
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

        public static void ShutdownAppDomain(string appDomainName)
        {
            if (String.IsNullOrWhiteSpace(appDomainName))
            {
                throw new ArgumentNullException("appDomainName");
            }

            lock (ConfigController.BridgeLock)
            {
                AppDomain appDomain = null;
                if (TypeCache.AppDomains.TryGetValue(appDomainName, out appDomain))
                {
                    // If the AppDomain cannot unload, allow the exception to propagate
                    // back to the caller and leave the current cache state unaffected.
                    AppDomain.Unload(appDomain);
                    TypeCache.AppDomains.Remove(appDomainName);
                    TypeCache.Cache.Remove(appDomainName);
                }
            }
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
