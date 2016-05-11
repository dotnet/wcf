// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Infrastructure.Common;
using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using Xunit;
using System.Collections.Generic;
using System.Text;

public class Tcp_ClientCredentialTypeCertificateCanonicalNameTests : ConditionalWcfTest
{
    // We set up three endpoints on the Bridge (server) side, each with a different certificate: 
    // Tcp_ClientCredentialType_Certificate_With_CanonicalName_Localhost_Address - is bound to a cert where CN=localhost
    // Tcp_ClientCredentialType_Certificate_With_CanonicalName_DomainName_Address - is bound to a cert where CN=domainname
    // Tcp_ClientCredentialType_Certificate_With_CanonicalName_Fqdn_Address - is bound to a cert with a fqdn, e.g., CN=domainname.example.com
    //
    // When tests are run, a /p:BridgeHost=<name> is specified; if none is specified, then "localhost" is used
    // Hence, we are only able to determine at runtime whether a particular endpoint presented by the Bridge is going 
    // to pass a variation or fail a variation. 

    [ConditionalTheory(nameof(Root_Certificate_Installed))]
    [OuterLoop]
    [MemberData("TcpClientCredentialType_Certificate_With_Only_CanonicalName_MemberData")]
    public static void TcpClientCredentialType_Certificate_With_Only_CanonicalName_EchoString(Uri endpointUri, bool shouldCallSucceed)
    {
        var endpointAddress = new EndpointAddress(endpointUri);

        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();

            var errorBuilder = new StringBuilder();
            errorBuilder.AppendFormat("The call to '{0}' should have failed but succeeded. ", endpointUri.Host);
            errorBuilder.AppendFormat("This means that the certificate validation passed when it should have failed. ");
            errorBuilder.AppendFormat("Check the certificate returned by the endpoint at '{0}' ", endpointUri);
            errorBuilder.AppendFormat("to see that it is correct; if it is, there is likely an issue with the identity checking logic.");

            Assert.True(shouldCallSucceed, errorBuilder.ToString());
        }
        catch (MessageSecurityException exception)
        {
            // If there's a MessageSecurityException, we assume that the cert validation failed. Unfortunately checking for the 
            // message is really brittle and we can't account for loc and how .NET Native will display the exceptions
            // The exception message should look like:
            //
            // System.ServiceModel.Security.MessageSecurityException : Identity check failed for outgoing message. The expected
            // DNS identity of the remote endpoint was 'localhost' but the remote endpoint provided DNS claim 'example.com'.If this
            // is a legitimate remote endpoint, you can fix the problem by explicitly specifying DNS identity 'example.com' as
            // the Identity property of EndpointAddress when creating channel proxy.

            var errorBuilder = new StringBuilder();
            errorBuilder.AppendFormat("The call to '{0}' should have been successful but failed with a MessageSecurityException. ", endpointUri.Host);
            errorBuilder.AppendFormat("This usually means that the certificate validation failed when it should have passed. ");
            errorBuilder.AppendFormat("When connecting to host '{0}', the expectation is that the DNSClaim will be for the same hostname. ", endpointUri.Host);
            errorBuilder.AppendFormat("Exception message: {0}{1}", Environment.NewLine, exception.Message);

            Assert.True(!shouldCallSucceed, errorBuilder.ToString());
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    public static List<object[]> TcpClientCredentialType_Certificate_With_Only_CanonicalName_MemberData
    {
        get
        {
            List<object[]> list = new List<object[]>();

            var localhostEndpointUri = new Uri(Endpoints.Tcp_ClientCredentialType_Certificate_With_CanonicalName_Localhost_Address);
            list.Add(new object[] { localhostEndpointUri, string.Compare(localhostEndpointUri.Host, "localhost", StringComparison.OrdinalIgnoreCase) == 0 });

            // We check that: 
            // 1. The Bridge's reported hostname does not contain a '.' (which means we're hitting the FQDN)
            // 2. The Bridge's reported hostname is not "localhost" (which means we're hitting localhost)
            // If both these conditions are true, expect the test to pass. Otherwise, it should fail
            var domainNameEndpointUri = new Uri(Endpoints.Tcp_ClientCredentialType_Certificate_With_CanonicalName_DomainName_Address);
            list.Add(new object[] {
                domainNameEndpointUri,
                domainNameEndpointUri.Host.IndexOf('.') == -1 && string.Compare(domainNameEndpointUri.Host, "localhost", StringComparison.OrdinalIgnoreCase) != 0
            });

            // Get just the hostname part of the domainName Uri
            var domainNameHost = domainNameEndpointUri.Host.Split('.')[0];

            // If the Bridge's reported FQDN is the same as the Bridge's reported hostname, 
            // it means that there the bridge is set up on a network where FQDNs aren't used, only hostnames.
            // Since our pass/fail detection logic on whether or not this is an FQDN depends on whether the host name has a '.', we don't test this case

            var fqdnEndpointUri = new Uri(Endpoints.Tcp_ClientCredentialType_Certificate_With_CanonicalName_Fqdn_Address);
            if (string.Compare(domainNameHost, fqdnEndpointUri.Host, StringComparison.OrdinalIgnoreCase) != 0)
            {
                list.Add(new object[] { fqdnEndpointUri, fqdnEndpointUri.Host.IndexOf('.') > -1 });
            }

            return list;
        }
    }
}
