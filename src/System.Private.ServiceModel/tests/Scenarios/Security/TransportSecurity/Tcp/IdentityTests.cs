// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using Xunit;
using Infrastructure.Common;

public class IdentityTests : ConditionalWcfTest
{
#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(833)] // Not supported in NET Native
#else
    [ConditionalFact(nameof(Root_Certificate_Installed), nameof(Client_Certificate_Installed))]
#endif
    [OuterLoop]
    // The product code will check the Dns identity from the server and throw if it does not match what is specified in DnsEndpointIdentity
    public static void VerifyServiceIdentityMatchDnsEndpointIdentity()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        bool client_Certificate_Installed = Client_Certificate_Installed();
        if (!root_Certificate_Installed || !client_Certificate_Installed)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            Console.WriteLine("Client_Certificate_Installed evaluated as {0}", client_Certificate_Installed);
            return;
        }
#endif
        string testString = "Hello";

        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.Transport;
        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

        EndpointAddress endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_VerifyDNS_Address), new DnsEndpointIdentity(Endpoints.Tcp_VerifyDNS_HostName));
        ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(833)] // Not supported in NET Native
#else
    [ConditionalFact(nameof(Root_Certificate_Installed), nameof(Client_Certificate_Installed))]
#endif
    [OuterLoop]
    // Verify product throws MessageSecurityException when the Dns identity from the server does not match the expectation
    public static void ServiceIdentityNotMatch_Throw_MessageSecurityException()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        bool client_Certificate_Installed = Client_Certificate_Installed();
        if (!root_Certificate_Installed || !client_Certificate_Installed)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            Console.WriteLine("Client_Certificate_Installed evaluated as {0}", client_Certificate_Installed);
            return;
        }
#endif
        string testString = "Hello";

        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.Transport;
        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

        EndpointAddress endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_VerifyDNS_Address), new DnsEndpointIdentity("wrongone"));
        ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
        IWcfService serviceProxy = factory.CreateChannel();

        try
        {
            var exception = Assert.Throws<MessageSecurityException>(() =>
            {
                var result = serviceProxy.Echo(testString);
                Assert.Equal(testString, result);
            });

            Assert.True(exception.Message.Contains(Endpoints.Tcp_VerifyDNS_HostName), string.Format("Expected exception message contains: '{0}', actual: '{1}')", Endpoints.Tcp_VerifyDNS_HostName, exception.Message));
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects(factory);
        }
    }
}
