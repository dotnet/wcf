// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using WcfTestBridgeCommon;

namespace Bridge
{
    public class ResourceInvoker
    {
        public static object DynamicInvokePut(resource resource)
        {
            if (String.IsNullOrEmpty(resource.name))
            {
                throw new ArgumentNullException("resource.name");
            }

            // Disallow concurrent resource instantation or configuration changes
            lock (ConfigController.BridgeLock)
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

                ResourceRequestContext context = new ResourceRequestContext
                {
                    BridgeConfiguration = ConfigController.BridgeConfiguration
                };
                return loader.IResourceCall(resource.name, "Put", new object[] { context });
            }
        }

        public static object DynamicInvokeGet(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            // Disallow concurrent resource instantation or configuration changes
            lock (ConfigController.BridgeLock)
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

                return loader.IResourceCall(name, "Get", null);
            }
        }
    }
}
