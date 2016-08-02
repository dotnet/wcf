// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Security;
using Infrastructure.Common;
using Xunit;

public partial class ExpectedExceptionTests : ConditionalWcfTest
{
#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#endif
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed), nameof(Client_Certificate_Installed))]
    [OuterLoop]
    // Confirm that the Validate method of the custom X509CertificateValidator is called and that an exception thrown there is handled correctly.
    public static void TCP_ServiceCertFailedCustomValidate_Throw_Exception()
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
        NetTcpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        // *** VALIDATE *** \\
        var exception = Assert.Throws<Exception>(() =>
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_VerifyDNS_Address), new DnsEndpointIdentity(Endpoints.Tcp_VerifyDNS_HostName));
            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            factory.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new MyCertificateValidator();
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            try
            {
                var result = serviceProxy.Echo(testString);

                // *** CLEANUP *** \\
                ((ICommunicationObject)serviceProxy).Close();
                factory.Close();
            }
            finally
            {
                // *** ENSURE CLEANUP *** \\
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }
        });

        // *** ADDITIONAL VALIDATION *** \\
        Assert.Equal(MyCertificateValidator.exceptionMsg, exception.Message);
    }
}