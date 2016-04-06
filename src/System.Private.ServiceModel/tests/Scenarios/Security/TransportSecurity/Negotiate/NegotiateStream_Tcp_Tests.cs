// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Infrastructure.Common;
using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using Xunit;

public static class NegotiateStream_Tcp_Tests
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
    //     Windows: Edit the s_UserName and s_Password variables to a user valid on your Kerberos realm
    //       Linux: Edit the s_UserName and s_Password variables to a user valid on your Kerberos realm
    //              If previous tests were run, it may be necessary to run 'kdestroy -A' to remove all
    //              prior Kerberos tickets
    // 
    // NegotiateStream_*_With_ExplicitSpn
    //     Windows: Edit the s_Spn variable to match a valid SPN for the server 
    //       Linux: Edit the s_Spn variable to match a valid SPN for the server 
    //   
    //     By default, the SPN is the same as the host's fully qualified domain name, for example, 
    //     'host.domain.com'
    //     On a Windows host, one has to register the SPN using 'setspn', or run the process as LOCAL SYSTEM
    //     by using a tool like psexec and running 'psexec -s -h <WcfBridge.exe>' 
    // 
    // NegotiateStream_*_With_Upn
    //     Windows: Edit the s_Upn field to match a valid UPN for the server in the form of 
    //              'user@DOMAIN.COM'
    //       Linux: This scenario is not yet supported - dotnet/corefx#6606
    //
    // NegotiateStream_*_With_ExplicitUserNameAndPassword_With_Spn
    //     Windows: Edit the s_Spn variable to match a valid SPN for the server
    //              Edit the s_UserName and s_Password variables to a user valid on your Kerberos realm
    //       Linux: Edit the s_Spn variable to match a valid SPN for the server
    //              Edit the s_UserName and s_Password variables to a user valid on your Kerberos realm
    // 
    // NegotiateStream_*_With_ExplicitUserNameAndPassword_With_Upn
    //     Windows: Edit the s_Upn variable to match a valid UPN for the server
    //              Edit the s_UserName and s_Password variables to a user valid on your Kerberos realm
    //       Linux: This scenario is not yet supported - dotnet/corefx#6606

    // These tests are used for testing NegotiateStream (SecurityMode.Transport) 

    [Fact]
    [ActiveIssue(851, PlatformID.AnyUnix)]
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

    [Fact]
    [ActiveIssue(851)]
    [OuterLoop]
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

            factory.Credentials.Windows.ClientCredential.Domain = NegotiateStreamTestConfiguration.Instance.NegotiateTestDomain;
            factory.Credentials.Windows.ClientCredential.UserName = NegotiateStreamTestConfiguration.Instance.NegotiateTestUserName;
            factory.Credentials.Windows.ClientCredential.Password = NegotiateStreamTestConfiguration.Instance.NegotiateTestPassword;

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

    [Fact]
    [ActiveIssue(851)]
    [OuterLoop]
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
                    new SpnEndpointIdentity(NegotiateStreamTestConfiguration.Instance.NegotiateTestSpn)
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

    [Fact]
    [ActiveIssue(851)]
    [OuterLoop]
    public static void NegotiateStream_Tcp_With_Upn()
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
                    new SpnEndpointIdentity(NegotiateStreamTestConfiguration.Instance.NegotiateTestSpn)
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

    [Fact]
    [ActiveIssue(851)]
    [OuterLoop]
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
                    new SpnEndpointIdentity(NegotiateStreamTestConfiguration.Instance.NegotiateTestSpn)
            ));

            factory.Credentials.Windows.ClientCredential.Domain = NegotiateStreamTestConfiguration.Instance.NegotiateTestDomain;
            factory.Credentials.Windows.ClientCredential.UserName = NegotiateStreamTestConfiguration.Instance.NegotiateTestUserName;
            factory.Credentials.Windows.ClientCredential.Password = NegotiateStreamTestConfiguration.Instance.NegotiateTestPassword;

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

    [Fact]
    [ActiveIssue(851)]
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
                    new UpnEndpointIdentity(NegotiateStreamTestConfiguration.Instance.NegotiateTestUpn)
            ));

            factory.Credentials.Windows.ClientCredential.Domain = NegotiateStreamTestConfiguration.Instance.NegotiateTestDomain;
            factory.Credentials.Windows.ClientCredential.UserName = NegotiateStreamTestConfiguration.Instance.NegotiateTestUserName;
            factory.Credentials.Windows.ClientCredential.Password = NegotiateStreamTestConfiguration.Instance.NegotiateTestPassword;

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
