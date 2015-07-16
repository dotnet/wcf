// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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

            AppDomain appDomain;
            if (!TypeCache.AppDomains.TryGetValue(ConfigController.CurrentAppDomain, out appDomain))
            {
                throw new ArgumentException("Resource not found");
            }

            Type loaderType = typeof(AssemblyLoader);
            var loader =
                (AssemblyLoader)appDomain.CreateInstanceFromAndUnwrap(
                    loaderType.Assembly.Location,
                    loaderType.FullName);

            return loader.IResourceCall(resource.name, "Put");
        }

        public static object DynamicInvokeGet(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            AppDomain appDomain;
            if (!TypeCache.AppDomains.TryGetValue(ConfigController.CurrentAppDomain, out appDomain))
            {
                throw new ArgumentException("Resource not found");
            }

            Type loaderType = typeof(AssemblyLoader);
            var loader =
                (AssemblyLoader)appDomain.CreateInstanceFromAndUnwrap(
                    loaderType.Assembly.Location,
                    loaderType.FullName);

            return loader.IResourceCall(name, "Get");
        }
    }
}
