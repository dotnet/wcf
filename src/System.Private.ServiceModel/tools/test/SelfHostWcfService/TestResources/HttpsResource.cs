// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using WcfService.CertificateResources;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal abstract class HttpsResource : EndpointResource<WcfService, IWcfService>
    {
        protected override string Protocol { get { return BaseAddressResource.Https; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeHttpsPort;
        }

        protected override void ModifyHost(ServiceHost serviceHost, ResourceRequestContext context)
        {
            // Ensure the https certificate is installed before this endpoint resource is used
            CertificateResourceHelpers.EnsureSslPortCertificateInstalled(context.BridgeConfiguration);

            base.ModifyHost(serviceHost, context);
        }
    }
}
