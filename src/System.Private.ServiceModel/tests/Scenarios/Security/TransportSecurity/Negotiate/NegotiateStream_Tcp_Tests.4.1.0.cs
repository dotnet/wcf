// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public class NegotiateStream_Tcp_Tests : ConditionalWcfTest
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
    //     On a Windows host, one has to register the SPN using 'setspn', or run the process as LOCAL SYSTEM
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
               nameof(Ambient_Credentials_Available))]
    [OuterLoop]
    public static void NegotiateStream_Tcp_AmbientCredentials()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_DefaultBinding_Address));
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
    public static void NegotiateStream_Tcp_With_ExplicitUserNameAndPassword()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding();
            factory = new ChannelFactory<IWcfService>(binding,
                new EndpointAddress(Endpoints.Tcp_DefaultBinding_Address));

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
               nameof(SPN_Available))]
    [OuterLoop]
    // Test Requirements \\
    // The following environment variables must be set...
    //          "NegotiateTestRealm"
    //          "NegotiateTestSpn" (host/<servername>)
    //          "ServiceUri" (server running as machine context)
    public static void NegotiateStream_Tcp_With_ExplicitSpn()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding();
            factory = new ChannelFactory<IWcfService>(binding,
                new EndpointAddress(
                    new Uri(Endpoints.Tcp_DefaultBinding_Address),
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
    [Condition(nameof(Windows_Authentication_Available),
               nameof(SPN_Available))]
    [OuterLoop]
    public static void NegotiateStream_Tcp_With_SPN()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding();
            factory = new ChannelFactory<IWcfService>(
                binding,
                new EndpointAddress(
                    new Uri(Endpoints.Tcp_DefaultBinding_Address),
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
    [Condition(nameof(Windows_Authentication_Available),
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
    public static void NegotiateStream_Tcp_With_ExplicitUserNameAndPassword_With_Spn()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding();
            factory = new ChannelFactory<IWcfService>(
                binding,
                new EndpointAddress(
                    new Uri(Endpoints.Tcp_DefaultBinding_Address),
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
    [Issue(2147, OS = OSID.OSX | OSID.AnyUnix)]
    [Condition(nameof(Windows_Authentication_Available),
               nameof(Explicit_Credentials_Available),
               nameof(Domain_Available),
               nameof(UPN_Available),
               nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void NegotiateStream_Tcp_With_ExplicitUserNameAndPassword_With_Upn()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding();
            factory = new ChannelFactory<IWcfService>(
                binding,
                new EndpointAddress(
                    new Uri(Endpoints.Tcp_DefaultBinding_Address),
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
