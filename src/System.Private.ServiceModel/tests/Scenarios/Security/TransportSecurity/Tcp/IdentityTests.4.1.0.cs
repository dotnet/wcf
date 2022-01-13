// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using Xunit;
using Infrastructure.Common;

public partial class IdentityTests : ConditionalWcfTest
{
    [WcfFact]
    [Issue(3572, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed), nameof(Client_Certificate_Installed))]
    [OuterLoop]
    // The product code will check the Dns identity from the server and throw if it does not match what is specified in DnsEndpointIdentity
    public static void VerifyServiceIdentityMatchDnsEndpointIdentity()
    {
        string testString = "Hello";

        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.Transport;
        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

        EndpointAddress endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_VerifyDNS_Address), new DnsEndpointIdentity(Endpoints.Tcp_VerifyDNS_HostName));
        ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
        factory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
        IWcfService serviceProxy = factory.CreateChannel();

        try
        {
            var result = serviceProxy.Echo(testString);
            Assert.Equal(testString, result);

            factory.Close();
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects(factory);
        }
    }
}
