// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.ServiceModel;
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
            EnsureHttpsCertificateInstalled(context.BridgeConfiguration);
            base.ModifyHost(serviceHost, context);
        }

        internal static string EnsureHttpsCertificateInstalled(BridgeConfiguration configuration)
        {
            // Ensure the https certificate is installed before this endpoint resource is used
            string certThumbPrint = CertificateManager.InstallMyCertificate(configuration,
                                                                            configuration.BridgeHttpsCertificate);

            // Ensure http.sys has been told to use this certificate on the https port
            CertificateManager.InstallSSLPortCertificate(certThumbPrint, configuration.BridgeHttpsPort);

            return certThumbPrint;
        }
    }
}
