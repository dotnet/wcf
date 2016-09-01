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
    //     by using a tool like psexec and running 'psexec -s -h <WcfBridge.exe>' 
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(1235)]
#endif
    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [Issue(1235, Framework = FrameworkID.NetNative)]
    [OuterLoop]
    public static void NegotiateStream_Tcp_AmbientCredentials()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool ambient_Credentials_Available = Ambient_Credentials_Available();

        if (!windows_Authentication_Available || 
            !ambient_Credentials_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);
            Console.WriteLine("Ambient_Credentials_Available evaluated as {0}", ambient_Credentials_Available);
            return;
        }
#endif
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(1235)]
#endif
    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available),
               nameof(Explicit_Credentials_Available),
               nameof(Domain_Available))]
    [Issue(1235, Framework = FrameworkID.NetNative)]
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
#if FULLXUNIT_NOTSUPPORTED
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool explicit_Credentials_Available = Explicit_Credentials_Available();
        bool domain_Available = Domain_Available();

        if (!windows_Authentication_Available ||
            !explicit_Credentials_Available ||
            !domain_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);
            Console.WriteLine("Explicit_Credentials_Available evaluated as {0}", explicit_Credentials_Available);
            Console.WriteLine("Domain_Available evaluated as {0}", domain_Available);
            return;
        }
#endif
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(1235)]
#endif
    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available),
               nameof(SPN_Available))]
    [Issue(1235, Framework = FrameworkID.NetNative)]
    [OuterLoop]
    // Test Requirements \\
    // The following environment variables must be set...
    //          "NegotiateTestRealm"
    //          "NegotiateTestSpn" (host/<servername>)
    //          "ServiceUri" (server running as machine context)
    public static void NegotiateStream_Tcp_With_ExplicitSpn()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool spn_Available = SPN_Available();

        if (!windows_Authentication_Available ||
            !spn_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);
            Console.WriteLine("SPN_Available evaluated as {0}", spn_Available);    
            return;
        }
#endif
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(1235)]
#endif
    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available),
               nameof(SPN_Available))]
    [Issue(1235, Framework = FrameworkID.NetNative)]
    [OuterLoop]
    public static void NegotiateStream_Tcp_With_SPN()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool spn_Available = SPN_Available();

        if (!windows_Authentication_Available ||
            !spn_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);
            Console.WriteLine("SPN_Available evaluated as {0}", spn_Available);    
            return;
        }
#endif
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(1235)]
#endif
    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available),
               nameof(Explicit_Credentials_Available),
               nameof(Domain_Available),
               nameof(SPN_Available))]
    [Issue(1235, Framework = FrameworkID.NetNative)]
    [Issue(1262)]
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
#if FULLXUNIT_NOTSUPPORTED
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool explicit_Credentials_Available = Explicit_Credentials_Available();
        bool domain_Available = Domain_Available();
        bool spn_Available = SPN_Available();

        if (!windows_Authentication_Available ||
            !explicit_Credentials_Available ||
            !domain_Available ||
            !spn_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);
            Console.WriteLine("Explicit_Credentials_Available evaluated as {0}", explicit_Credentials_Available);
            Console.WriteLine("Domain_Available evaluated as {0}", domain_Available);
            Console.WriteLine("SPN_Available evaluated as {0}", spn_Available);
            return;
        }
#endif
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(1235)]
#endif
    [WcfFact]
    [Condition(nameof(Windows_Authentication_Available),
               nameof(Explicit_Credentials_Available),
               nameof(Domain_Available),
               nameof(UPN_Available))]
    [Issue(1235, Framework = FrameworkID.NetNative)]
    [Issue(1262)]
    [OuterLoop]
    public static void NegotiateStream_Tcp_With_ExplicitUserNameAndPassword_With_Upn()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool explicit_Credentials_Available = Explicit_Credentials_Available();
        bool domain_Available = Domain_Available();
        bool upn_Available = UPN_Available();

        if (!windows_Authentication_Available ||
            !explicit_Credentials_Available ||
            !domain_Available ||
            !upn_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);
            Console.WriteLine("Explicit_Credentials_Available evaluated as {0}", explicit_Credentials_Available);
            Console.WriteLine("Domain_Available evaluated as {0}", domain_Available);
            Console.WriteLine("UPN_Available evaluated as {0}", upn_Available);
            return;
        }
#endif
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
