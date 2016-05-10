// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal class HttpWindowsResource : HttpResource
    {
        protected override string Address { get { return "http-windows"; } }

        protected override Binding GetBinding()
        {
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            return binding;
        }

        protected override string GetHost(ResourceRequestContext context)
        {
            return Environment.MachineName;
        }
    }
}

