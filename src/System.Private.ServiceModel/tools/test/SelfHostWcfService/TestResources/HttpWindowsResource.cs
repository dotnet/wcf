// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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

