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

public class NegotiateStream_Http_Tests : ConditionalWcfTest
{
    // The tests are as follows:
    //
    // NegotiateStream_*_AmbientCredentials
    //     Windows: This should pass by default without any code changes
    //       Linux: This should not pass by default 
    //              Run 'kinit user@DC.DOMAIN.COM' before running this test to use ambient credentials
    //              ('DC.DOMAIN.COM' must be in capital letters) 
    //              If previous tests were run, it may be necessary to run 'kdestroy -A' to remove all
    //              prior Kerberos tickets
    // 
    // NegotiateStream_*_With_ExplicitUserNameAndPassword
    //     Windows: Set the ExplicitUserName, ExplicitPassword, and NegotiateTestDomain TestProperties to a user valid on your Kerberos realm
    //       Linux: Set the ExplicitUserName, ExplicitPassword, and NegotiateTestDomain TestProperties to a user valid on your Kerberos realm
    //              If previous tests were run, it may be necessary to run 'kdestroy -A' to remove all
    //              prior Kerberos tickets
    // 
    // NegotiateStream_*_With_ExplicitSpn
    //     Windows: Set the NegotiateTestSPN TestProperties to match a valid SPN for the server 
    //       Linux: Set the NegotiateTestSPN TestProperties to match a valid SPN for the server 
    //   
    //     By default, the SPN is the same as the host's fully qualified domain name, for example, 
    //     'host.domain.com'
    //     On a Windows host, one has to register the SPN using 'setspn', or run the process as LOCAL SYSTEM.
    //     This can be done by setting the PSEXEC_PATH environment variable to point to the folder containing
    //     psexec.exe prior to starting the WCF self-host service. 
    // 
    // NegotiateStream_*_With_Upn
    //     Windows: Set the NegotiateTestUPN TestProperties to match a valid UPN for the server in the form of 
    //              'user@DOMAIN.COM'
    //       Linux: This scenario is not yet supported - dotnet/corefx#6606
    //
    // NegotiateStream_*_With_ExplicitUserNameAndPassword_With_Spn
    //     Windows: Set the NegotiateTestUPN TestProperties to match a valid UPN for the server
    //              Set the ExplicitUserName, ExplicitPassword, and NegotiateTestDomain TestProperties to a user valid on your Kerberos realm
    //       Linux: Set the NegotiateTestUPN TestProperties to match a valid UPN for the server
    //              Set the ExplicitUserName, ExplicitPassword, and NegotiateTestDomain TestProperties to a user valid on your Kerberos realm
    // 
    // NegotiateStream_*_With_ExplicitUserNameAndPassword_With_Upn
    //     Windows: Set the NegotiateTestUPN TestProperties to match a valid UPN for the server
    //              Set the ExplicitUserName, ExplicitPassword, and NegotiateTestDomain TestProperties to a user valid on your Kerberos realm
    //       Linux: This scenario is not yet supported - dotnet/corefx#6606

    // These tests are used for testing NegotiateStream (SecurityMode.Transport) 

    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available),
               nameof(Root_Certificate_Installed),
               nameof(Ambient_Credentials_Available),
               nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void NegotiateStream_Http_AmbientCredentials()
    {
        string testString = "Hello";
        string result = "";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;

            factory = new ChannelFactory<IWcfService>(
                binding,
                new EndpointAddress(Endpoints.Https_WindowsAuth_Address));
            serviceProxy = factory.CreateChannel();

            if (Environment.Version.Major == 5 && !OSID.AnyWindows.MatchesCurrent() && !TestProperties.GetProperty(TestProperties.ServiceUri_PropertyName).Contains("/"))
            {
                Assert.Throws<System.ServiceModel.ProtocolException>(() => { result = serviceProxy.Echo(testString); });
            }
            else
            {
                // *** EXECUTE *** \\
                result = serviceProxy.Echo(testString);

                // *** VALIDATE *** \\
                Assert.Equal(testString, result);
            }

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available),
               nameof(Root_Certificate_Installed),
               nameof(Explicit_Credentials_Available),
               nameof(Domain_Available),
               nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    // Test Requirements \\
    // The following environment variables must be set...
    //          "NegotiateTestRealm"
    //          "NegotiateTestDomain"
    //          "ExplicitUserName"
    //          "ExplicitPassword"
    //          "ServiceUri" (server running as machine context)
    public static void NegotiateStream_Http_With_ExplicitUserNameAndPassword()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            factory = new ChannelFactory<IWcfService>(
                binding,
                new EndpointAddress(Endpoints.Https_WindowsAuth_Address));

            factory.Credentials.Windows.ClientCredential.Domain = GetDomain();
            factory.Credentials.Windows.ClientCredential.UserName = GetExplicitUserName();
            factory.Credentials.Windows.ClientCredential.Password = GetExplicitPassword();

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available),
              nameof(Root_Certificate_Installed),
              nameof(Explicit_Credentials_Available),
              nameof(Domain_Available),
              nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    // Test Requirements \\
    // The following environment variables must be set...
    //          "NegotiateTestRealm"
    //          "NegotiateTestDomain"
    //          "ExplicitUserName"
    //          "ExplicitPassword"
    //          "ServiceUri" (server running as machine context)
    public static void NegotiateStream_Http_With_ExplicitUserNameAndPasswordForNet50()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            string spn = GetSPN().ToLowerInvariant().Replace("host", "HTTP");
            factory = new ChannelFactory<IWcfService>(
                binding,
                new EndpointAddress(new Uri(Endpoints.Https_WindowsAuth_Address), new SpnEndpointIdentity(spn)));

            factory.Credentials.Windows.ClientCredential.Domain = GetDomain();
            factory.Credentials.Windows.ClientCredential.UserName = GetExplicitUserName();
            factory.Credentials.Windows.ClientCredential.Password = GetExplicitPassword();

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available),
               nameof(Root_Certificate_Installed),
               nameof(SPN_Available),
               nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    // Test Requirements \\
    // The following environment variables must be set...
    //          "NegotiateTestRealm"
    //          "NegotiateTestDomain"
    //          "NegotiateTestSpn" (host/<servername>)
    //          "ServiceUri" (server running as machine context)
    public static void NegotiateStream_Http_With_ExplicitSpn()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            factory = new ChannelFactory<IWcfService>(
                binding,
                new EndpointAddress(
                    new Uri(Endpoints.Https_WindowsAuth_Address),
                    new SpnEndpointIdentity(GetSPN())
            ));

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Issue(2805)]
    [Issue(25320, Repository = "dotnet/runtime")]
    [Condition(nameof(Windows_Authentication_Available),
               nameof(Root_Certificate_Installed),
               nameof(UPN_Available))]
    [OuterLoop]
    public static void NegotiateStream_Http_With_Upn()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            factory = new ChannelFactory<IWcfService>(
                binding,
                new EndpointAddress(
                    new Uri(Endpoints.Https_WindowsAuth_Address),
                    new UpnEndpointIdentity(GetUPN())
            ));

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available),
               nameof(Root_Certificate_Installed),
               nameof(Explicit_Credentials_Available),
               nameof(Domain_Available),
               nameof(SPN_Available),
               nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    // Test Requirements \\
    // The following environment variables must be set...
    //          "NegotiateTestRealm"
    //          "NegotiateTestDomain"
    //          "ExplicitUserName"
    //          "ExplicitPassword"
    //          "NegotiateTestSpn" (host/<servername>)
    //          "ServiceUri" (server running as machine context)
    public static void NegotiateStream_Http_With_ExplicitUserNameAndPassword_With_Spn()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            factory = new ChannelFactory<IWcfService>(
                binding,
                new EndpointAddress(
                    new Uri(Endpoints.Https_WindowsAuth_Address),
                    new SpnEndpointIdentity(GetSPN())
            ));

            factory.Credentials.Windows.ClientCredential.Domain = GetDomain();
            factory.Credentials.Windows.ClientCredential.UserName = GetExplicitUserName();
            factory.Credentials.Windows.ClientCredential.Password = GetExplicitPassword();

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Issue(2805)]
    [Issue(25320, Repository = "dotnet/runtime")]
    [Condition(nameof(Windows_Authentication_Available),
               nameof(Root_Certificate_Installed),
               nameof(Explicit_Credentials_Available),
               nameof(Domain_Available),
               nameof(UPN_Available))]
    [OuterLoop]
    public static void NegotiateStream_Http_With_ExplicitUserNameAndPassword_With_Upn()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            factory = new ChannelFactory<IWcfService>(
                binding,
                new EndpointAddress(
                    new Uri(Endpoints.Https_WindowsAuth_Address),
                    new UpnEndpointIdentity(GetUPN())
            ));

            factory.Credentials.Windows.ClientCredential.Domain = GetDomain();
            factory.Credentials.Windows.ClientCredential.UserName = GetExplicitUserName();
            factory.Credentials.Windows.ClientCredential.Password = GetExplicitPassword();

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}
