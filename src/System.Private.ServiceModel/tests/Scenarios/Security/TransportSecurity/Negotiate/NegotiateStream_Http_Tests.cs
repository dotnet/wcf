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
    //     Windows: Edit the NegotiateStreamTestConfiguration.Instance.UserName and .Password variables to a user valid on your Kerberos realm
    //       Linux: Edit the NegotiateStreamTestConfiguration.Instance.UserName and .Password variables to a user valid on your Kerberos realm
    //              If previous tests were run, it may be necessary to run 'kdestroy -A' to remove all
    //              prior Kerberos tickets
    // 
    // NegotiateStream_*_With_ExplicitSpn
    //     Windows: Edit the NegotiateStreamTestConfiguration.Instance.Spn variable to match a valid SPN for the server 
    //       Linux: Edit the NegotiateStreamTestConfiguration.Instance.Spn variable to match a valid SPN for the server 
    //   
    //     By default, the SPN is the same as the host's fully qualified domain name, for example, 
    //     'host.domain.com'
    //     On a Windows host, one has to register the SPN using 'setspn', or run the process as LOCAL SYSTEM
    //     by using a tool like psexec and running 'psexec -s -h <WcfBridge.exe>' 
    // 
    // NegotiateStream_*_With_Upn
    //     Windows: Edit the NegotiateStreamTestConfiguration.Instance.Upn field to match a valid UPN for the server in the form of 
    //              'user@DOMAIN.COM'
    //       Linux: This scenario is not yet supported - dotnet/corefx#6606
    //
    // NegotiateStream_*_With_ExplicitUserNameAndPassword_With_Spn
    //     Windows: Edit the NegotiateStreamTestConfiguration.Instance.Spn variable to match a valid SPN for the server
    //              Edit the NegotiateStreamTestConfiguration.Instance.UserName and .Password variables to a user valid on your Kerberos realm
    //       Linux: Edit the NegotiateStreamTestConfiguration.Instance.Spn variable to match a valid SPN for the server
    //              Edit the NegotiateStreamTestConfiguration.Instance.UserName and .Password variables to a user valid on your Kerberos realm
    // 
    // NegotiateStream_*_With_ExplicitUserNameAndPassword_With_Upn
    //     Windows: Edit the NegotiateStreamTestConfiguration.Instance.Upn variable to match a valid UPN for the server
    //              Edit the NegotiateStreamTestConfiguration.Instance.UserName and .Password variables to a user valid on your Kerberos realm
    //       Linux: This scenario is not yet supported - dotnet/corefx#6606

    // These tests are used for testing NegotiateStream (SecurityMode.Transport) 

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#else
    [ConditionalFact(nameof(Windows_Authentication_Available), nameof(Root_Certificate_Installed))]
#endif
    [OuterLoop]
    public static void NegotiateStream_Http_AmbientCredentials()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool root_Certificate_Installed = Root_Certificate_Installed();
        if (!windows_Authentication_Available || !root_Certificate_Installed)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);    
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            return;
        }
#endif
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
#else
    [ConditionalFact(nameof(Windows_Authentication_Available), nameof(Root_Certificate_Installed))]
#endif
    [OuterLoop]
    [ActiveIssue(1265)]
    // Test Requirements \\
    // The following environment variables must be set...
    //          "NegotiateTestRealm"
    //          "NegotiateTestDomain"
    //          "NegotiateTestUserName"
    //          "NegotiateTestPassword"
    //          "ServiceUri" (server running as machine context)
    public static void NegotiateStream_Http_With_ExplicitUserNameAndPassword()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool root_Certificate_Installed = Root_Certificate_Installed();
        if (!windows_Authentication_Available || !root_Certificate_Installed)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);    
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            return;
        }
#endif
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#else
    [ConditionalFact(nameof(Windows_Authentication_Available), nameof(Root_Certificate_Installed))]
#endif
    [OuterLoop]
    [ActiveIssue(1265)]
    // Test Requirements \\
    // The following environment variables must be set...
    //          "NegotiateTestRealm"
    //          "NegotiateTestDomain"
    //          "NegotiateTestUserName"
    //          "NegotiateTestPassword"
    //          "NegotiateTestSpn" (host/<servername>)
    //          "ServiceUri" (server running as machine context)
    public static void NegotiateStream_Http_With_ExplicitSpn()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool root_Certificate_Installed = Root_Certificate_Installed();
        if (!windows_Authentication_Available || !root_Certificate_Installed)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);    
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            return;
        }
#endif
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(10)]
#else
    [ConditionalFact(nameof(Windows_Authentication_Available), nameof(Root_Certificate_Installed))]
#endif
    [OuterLoop]
    public static void NegotiateStream_Http_With_Upn()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool root_Certificate_Installed = Root_Certificate_Installed();
        if (!windows_Authentication_Available || !root_Certificate_Installed)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);    
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            return;
        }
#endif
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
                    new UpnEndpointIdentity(NegotiateStreamTestConfiguration.Instance.NegotiateTestUpn)
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
#else
    [ConditionalFact(nameof(Windows_Authentication_Available), nameof(Root_Certificate_Installed))]
#endif
    [OuterLoop]
    [ActiveIssue(1265)]
    // Test Requirements \\
    // The following environment variables must be set...
    //          "NegotiateTestRealm"
    //          "NegotiateTestDomain"
    //          "NegotiateTestUserName"
    //          "NegotiateTestPassword"
    //          "NegotiateTestSpn" (host/<servername>)
    //          "ServiceUri" (server running as machine context)
    public static void NegotiateStream_Http_With_ExplicitUserNameAndPassword_With_Spn()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool root_Certificate_Installed = Root_Certificate_Installed();
        if (!windows_Authentication_Available || !root_Certificate_Installed)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);    
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            return;
        }
#endif
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(10)]
#else
    [ConditionalFact(nameof(Windows_Authentication_Available), nameof(Root_Certificate_Installed))]
#endif
    [OuterLoop]
    public static void NegotiateStream_Http_With_ExplicitUserNameAndPassword_With_Upn()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool root_Certificate_Installed = Root_Certificate_Installed();
        if (!windows_Authentication_Available || !root_Certificate_Installed)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);    
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            return;
        }
#endif
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
