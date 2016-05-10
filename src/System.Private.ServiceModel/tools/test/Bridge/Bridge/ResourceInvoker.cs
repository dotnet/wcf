// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using WcfTestBridgeCommon;

namespace Bridge
{
    public class ResourceInvoker
    {
        public static ResourceResponse DynamicInvokePut(string resourceName, Dictionary<string, string> properties)
        {
            if (String.IsNullOrEmpty(resourceName))
            {
                throw new ArgumentNullException("resourceName");
            }

            // Disallow concurrent resource instantation or configuration changes
            lock (ConfigController.ConfigLock)
            {
                AppDomain appDomain;
                if (String.IsNullOrWhiteSpace(ConfigController.CurrentAppDomainName))
                {
                    throw new InvalidOperationException("The Bridge resource folder has not been configured.");
                }
                if (!TypeCache.AppDomains.TryGetValue(ConfigController.CurrentAppDomainName, out appDomain))
                {
                    throw new ArgumentException("Resource not found", "resource");
                }

                Type loaderType = typeof(AssemblyLoader);
                var loader =
                    (AssemblyLoader)appDomain.CreateInstanceFromAndUnwrap(
                        loaderType.Assembly.Location,
                        loaderType.FullName);

                ResourceRequestContext context = new ResourceRequestContext
                {
                    BridgeConfiguration = ConfigController.BridgeConfiguration,
                    ResourceName = resourceName,
                    Properties = properties
                };
                object result = loader.IResourceCall(resourceName, "Put", new object[] { context });
                return (ResourceResponse)result;
            }
        }

        public static ResourceResponse DynamicInvokeGet(string resourceName, Dictionary<string, string> properties)
        {
            if (String.IsNullOrWhiteSpace(resourceName))
            {
                throw new ArgumentNullException("resourceName");
            }

            // Disallow concurrent resource instantation or configuration changes
            lock (ConfigController.ConfigLock)
            {
                AssemblyLoader loader = GetLoaderFromAppDomain();

                ResourceRequestContext context = new ResourceRequestContext
                {
                    BridgeConfiguration = ConfigController.BridgeConfiguration,
                    ResourceName = resourceName,
                    Properties = properties
                };

                object result = loader.IResourceCall(resourceName, "Get", new object[] { context });
                return (ResourceResponse)result;
            }
        }

        public static IList<string> DynamicInvokeGetAllResources()
        {
            // Disallow concurrent resource instantation or configuration changes  
            lock (ConfigController.ConfigLock)
            {
                AssemblyLoader loader = GetLoaderFromAppDomain();
                return loader.GetTypes();
            }
        }

        private static AssemblyLoader GetLoaderFromAppDomain()
        {
            AppDomain appDomain;
            if (!TypeCache.AppDomains.TryGetValue(ConfigController.CurrentAppDomainName, out appDomain))
            {
                throw new ArgumentException("Resource not found");
            }

            Type loaderType = typeof(AssemblyLoader);
            var loader =
                (AssemblyLoader)appDomain.CreateInstanceFromAndUnwrap(
                    loaderType.Assembly.Location,
                    loaderType.FullName);
            return loader;
        }
    }
}
