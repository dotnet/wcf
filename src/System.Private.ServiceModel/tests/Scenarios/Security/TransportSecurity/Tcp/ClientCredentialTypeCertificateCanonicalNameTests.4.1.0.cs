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
    // We set up three endpoints on the WCF service (server) side, each with a different certificate: 
    // Tcp_ClientCredentialType_Certificate_With_CanonicalName_Localhost_Address - is bound to a cert where CN=localhost
    // Tcp_ClientCredentialType_Certificate_With_CanonicalName_DomainName_Address - is bound to a cert where CN=domainname
    // Tcp_ClientCredentialType_Certificate_With_CanonicalName_Fqdn_Address - is bound to a cert with a fqdn, e.g., CN=domainname.example.com
    //
    // When tests are run, a /p:ServiceHost=<name> is specified; if none is specified, then "localhost" is used
    // Hence, we are only able to determine at runtime whether a particular endpoint presented by the WCF Service is going 
    // to pass a variation or fail a variation. 

    [WcfFact]
    [Issue(3572, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void Certificate_With_CanonicalName_Localhost_Address_EchoString()
    {
        var localhostEndpointUri = new Uri(Endpoints.Tcp_ClientCredentialType_Certificate_With_CanonicalName_Localhost_Address);
        var endpointAddress = new EndpointAddress(localhostEndpointUri);
        bool shouldCallSucceed = string.Compare(localhostEndpointUri.Host, "localhost", StringComparison.OrdinalIgnoreCase) == 0;

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
            errorBuilder.AppendFormat("The call to '{0}' should have failed but succeeded. ", localhostEndpointUri.Host);
            errorBuilder.AppendFormat("This means that the certificate validation passed when it should have failed. ");
            errorBuilder.AppendFormat("Check the certificate returned by the endpoint at '{0}' ", localhostEndpointUri);
            errorBuilder.AppendFormat("to see that it is correct; if it is, there is likely an issue with the identity checking logic.");

            Assert.True(shouldCallSucceed, errorBuilder.ToString());
        }
        catch (Exception exception) when (exception is CommunicationException || exception is MessageSecurityException)
        {
            if ((exception is MessageSecurityException) || (exception is CommunicationException) && !string.Equals(exception.InnerException.GetType().ToString(), "System.ServiceModel.Security.MessageSecurityException"))
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
                errorBuilder.AppendFormat("The call to '{0}' should have been successful but failed with a MessageSecurityException. ", localhostEndpointUri.Host);
                errorBuilder.AppendFormat("This usually means that the certificate validation failed when it should have passed. ");
                errorBuilder.AppendFormat("When connecting to host '{0}', the expectation is that the DNSClaim will be for the same hostname. ", localhostEndpointUri.Host);
                errorBuilder.AppendFormat("Exception message: {0}{1}", Environment.NewLine, exception.Message);

                Assert.True(!shouldCallSucceed, errorBuilder.ToString());
            }
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Issue(3572, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed), nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void Certificate_With_CanonicalName_DomainName_Address_EchoString()
    {
        var domainNameEndpointUri = new Uri(Endpoints.Tcp_ClientCredentialType_Certificate_With_CanonicalName_DomainName_Address);
        var endpointAddress = new EndpointAddress(domainNameEndpointUri);

        // We check that: 
        // 1. The WCF service's reported hostname does not contain a '.' (which means we're hitting the FQDN)
        // 2. The WCF service's reported hostname is not "localhost" (which means we're hitting localhost)
        // If both these conditions are true, expect the test to pass. Otherwise, it should fail
        bool shouldCallSucceed = domainNameEndpointUri.Host.IndexOf('.') == -1 && string.Compare(domainNameEndpointUri.Host, "localhost", StringComparison.OrdinalIgnoreCase) != 0;

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
            errorBuilder.AppendFormat("The call to '{0}' should have failed but succeeded. ", domainNameEndpointUri.Host);
            errorBuilder.AppendFormat("This means that the certificate validation passed when it should have failed. ");
            errorBuilder.AppendFormat("Check the certificate returned by the endpoint at '{0}' ", domainNameEndpointUri);
            errorBuilder.AppendFormat("to see that it is correct; if it is, there is likely an issue with the identity checking logic.");

            Assert.True(shouldCallSucceed, errorBuilder.ToString());
        }
        catch (Exception exception) when (exception is CommunicationException || exception is MessageSecurityException)
        {
            if ((exception is MessageSecurityException) || (exception is CommunicationException) && !string.Equals(exception.InnerException.GetType().ToString(), "System.ServiceModel.Security.MessageSecurityException"))
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
                errorBuilder.AppendFormat("The call to '{0}' should have been successful but failed with a MessageSecurityException. ", domainNameEndpointUri.Host);
                errorBuilder.AppendFormat("This usually means that the certificate validation failed when it should have passed. ");
                errorBuilder.AppendFormat("When connecting to host '{0}', the expectation is that the DNSClaim will be for the same hostname. ", domainNameEndpointUri.Host);
                errorBuilder.AppendFormat("Exception message: {0}{1}", Environment.NewLine, exception.Message);

                Assert.True(!shouldCallSucceed, errorBuilder.ToString());
            }
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Issue(3572, OS = OSID.OSX)]
    [Condition(nameof(Root_Certificate_Installed), nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void Certificate_With_CanonicalName_Fqdn_Address_EchoString()
    {
        bool shouldCallSucceed = false;
        var domainNameEndpointUri = new Uri(Endpoints.Tcp_ClientCredentialType_Certificate_With_CanonicalName_DomainName_Address);
        // Get just the hostname part of the domainName Uri
        var domainNameHost = domainNameEndpointUri.Host.Split('.')[0];

        var fqdnEndpointUri = new Uri(Endpoints.Tcp_ClientCredentialType_Certificate_With_CanonicalName_Fqdn_Address);
        var endpointAddress = new EndpointAddress(fqdnEndpointUri);

        // If the WCF service's reported FQDN is the same as the services's reported hostname, 
        // it means that there the WCF service is set up on a network where FQDNs aren't used, only hostnames.
        // Since our pass/fail detection logic on whether or not this is an FQDN depends on whether the host name has a '.', we don't test this case
        if (string.Compare(domainNameHost, fqdnEndpointUri.Host, StringComparison.OrdinalIgnoreCase) != 0)
        {
            shouldCallSucceed = fqdnEndpointUri.Host.IndexOf('.') > -1;
        }


        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            factory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
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
            errorBuilder.AppendFormat("The call to '{0}' should have failed but succeeded. ", fqdnEndpointUri.Host);
            errorBuilder.AppendFormat("This means that the certificate validation passed when it should have failed. ");
            errorBuilder.AppendFormat("Check the certificate returned by the endpoint at '{0}' ", fqdnEndpointUri);
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
            errorBuilder.AppendFormat("The call to '{0}' should have been successful but failed with a MessageSecurityException. ", fqdnEndpointUri.Host);
            errorBuilder.AppendFormat("This usually means that the certificate validation failed when it should have passed. ");
            errorBuilder.AppendFormat("When connecting to host '{0}', the expectation is that the DNSClaim will be for the same hostname. ", fqdnEndpointUri.Host);
            errorBuilder.AppendFormat("Exception message: {0}{1}", Environment.NewLine, exception.Message);

            Assert.True(!shouldCallSucceed, errorBuilder.ToString());
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}
