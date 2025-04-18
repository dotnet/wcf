// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using Xunit;
using Infrastructure.Common;

public class Https_ClientCredentialTypeTests : ConditionalWcfTest
{
    private static string s_username;
    private static string s_password;
    private const string BasicUsernameHeaderName = "BasicUsername";
    private const string BasicPasswordHeaderName = "BasicPassword";

    static Https_ClientCredentialTypeTests()
    {
        s_username = TestProperties.GetProperty(TestProperties.ExplicitUserName_PropertyName);
        s_password = TestProperties.GetProperty(TestProperties.ExplicitPassword_PropertyName);
    }


    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed))]
    [Issue(3572, OS = OSID.OSX)]
    [Issue(2561, OS = OSID.SLES)] // Active Issue - needs investigation
    [OuterLoop]
    public static void BasicAuthentication_RoundTrips_Echo()
    {
        BasicHttpBinding basicHttpBinding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IWcfCustomUserNameService> factory = null;
        string username = null;
        string password = null;
        IWcfCustomUserNameService serviceProxy = null;
        string testString = null;
        string result = null;

        try
        {
            // *** SETUP *** \\
            basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            endpointAddress = new EndpointAddress(Endpoints.Https_BasicAuth_Address);
            factory = new ChannelFactory<IWcfCustomUserNameService>(basicHttpBinding, endpointAddress);
            username = Guid.NewGuid().ToString("n").Substring(0, 8);
            password = Guid.NewGuid().ToString("n").Substring(0, 16);
            factory.Credentials.UserName.UserName = username;
            factory.Credentials.UserName.Password = password;
            serviceProxy = factory.CreateChannel();
            testString = "I am a test";

            // *** EXECUTE *** \\
            using (var scope = new OperationContextScope((IContextChannel)serviceProxy))
            {
                HttpRequestMessageProperty requestMessageProperty;
                if (!OperationContext.Current.OutgoingMessageProperties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    requestMessageProperty = new HttpRequestMessageProperty();
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessageProperty;
                }
                else
                {
                    requestMessageProperty = (HttpRequestMessageProperty)OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name];
                }

                requestMessageProperty.Headers[BasicUsernameHeaderName] = username;
                requestMessageProperty.Headers[BasicPasswordHeaderName] = password;

                result = serviceProxy.Echo(testString);
            }

            // *** VALIDATE *** \\
            Assert.True(String.Equals(result, testString),
                        String.Format("Basic echo test.\nTest variation:...\n{0}\nUsing address: '{1}'\nError: expected response from service: '{2}' Actual was: '{3}'",
                        "BasicAuthentication_RoundTrips_Echo",
                        Endpoints.Https_BasicAuth_Address,
                        testString,
                        result));

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }


    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed))]
    [Issue(3572, OS = OSID.OSX)]
    [Issue(2561, OS = OSID.SLES)] // Active Issue - needs investigation
    [OuterLoop]
    public static void BasicAuthenticationInvalidPwd_throw_MessageSecurityException()
    {
        BasicHttpBinding basicHttpBinding = null;
        ChannelFactory<IWcfCustomUserNameService> factory = null;
        EndpointAddress endpointAddress = null;
        string username = null;
        string password = null;
        IWcfCustomUserNameService serviceProxy = null;
        string testString = null;
        // Will need to use localized string once it is available
        // On Native retail, the message is stripped to 'HttpAuthorizationForbidden, Basic'
        // On Debug or .Net Core, the entire message is "The HTTP request was forbidden with client authentication scheme 'Basic'."
        // Thus we will only check message contains "forbidden"
        string message = "forbidden";

        // *** VALIDATE *** \\
        MessageSecurityException exception = Assert.Throws<MessageSecurityException>(() =>
        {
            // *** SETUP *** \\
            basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            endpointAddress = new EndpointAddress(Endpoints.Https_BasicAuth_Address);
            factory = new ChannelFactory<IWcfCustomUserNameService>(basicHttpBinding, endpointAddress);
            username = Guid.NewGuid().ToString("n").Substring(0, 8);
            password = Guid.NewGuid().ToString("n").Substring(0, 16);
            factory.Credentials.UserName.UserName = username;
            factory.Credentials.UserName.Password = password + "Invalid";
            serviceProxy = factory.CreateChannel();
            testString = "I am a test";

            // *** EXECUTE *** \\
            using (var scope = new OperationContextScope((IContextChannel)serviceProxy))
            {
                HttpRequestMessageProperty requestMessageProperty;
                if (!OperationContext.Current.OutgoingMessageProperties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    requestMessageProperty = new HttpRequestMessageProperty();
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessageProperty;
                }
                else
                {
                    requestMessageProperty = (HttpRequestMessageProperty)OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name];
                }

                requestMessageProperty.Headers[BasicUsernameHeaderName] = username;
                requestMessageProperty.Headers[BasicPasswordHeaderName] = password;

                try
                {
                    string result = serviceProxy.Echo(testString);
                }
                finally
                {
                    // *** ENSURE CLEANUP *** \\
                    ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
                }

            }
        });

        // *** ADDITIONAL VALIDATION *** \\
        Assert.True(exception.Message.ToLower().Contains(message), string.Format("Expected exception message to contain: '{0}', actual message is: '{1}'", message, exception.Message));
    }

    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed))]
    [OuterLoop]
    public static void BasicAuthenticationEmptyUser_throw_ArgumentException()
    {
        BasicHttpBinding basicHttpBinding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IWcfCustomUserNameService> factory = null;
        IWcfCustomUserNameService serviceProxy = null;
        string testString = null;
        string paraMessage = "username";

        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
        {
            // *** SETUP *** \\
            basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            endpointAddress = new EndpointAddress(Endpoints.Https_BasicAuth_Address);
            factory = new ChannelFactory<IWcfCustomUserNameService>(basicHttpBinding, endpointAddress);
            factory.Credentials.UserName.UserName = "";
            factory.Credentials.UserName.Password = "NoUserName";
            serviceProxy = factory.CreateChannel();
            testString = "I am a test";

            // *** EXECUTE *** \\
            try
            {
                string result = serviceProxy.Echo(testString);
            }
            finally
            {
                // *** ENSURE CLEANUP *** \\
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }
        });

        Assert.True(exception.Message.ToLower().Contains(paraMessage), string.Format("Expected exception message to contain: '{0}', actual: '{1}'", paraMessage, exception.Message));
    }

    [WcfFact]
    [Condition(nameof(Server_Domain_Joined),
               nameof(Root_Certificate_Installed),
               nameof(Digest_Authentication_Available),
               nameof(Explicit_Credentials_Available),
               nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    // Test Requirements \\
    // The following environment variables must be set...
    //          "ExplicitUserName"
    //          "ExplicitPassword"
    public static void DigestAuthentication_RoundTrips_Echo()
    {
        BasicHttpBinding basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
        basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Digest;

        Action<ChannelFactory> credentials = (factory) =>
        {
            factory.Credentials.HttpDigest.ClientCredential.UserName = s_username;
            factory.Credentials.HttpDigest.ClientCredential.Password = s_password;
        };

        ScenarioTestHelpers.RunBasicEchoTest(basicHttpBinding, Endpoints.Https_DigestAuth_Address, "BasicHttpBinding - Digest auth ", credentials);
    }

    [WcfFact]
    [Condition(nameof(NTLM_Available), nameof(Root_Certificate_Installed), nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void NtlmAuthentication_RoundTrips_Echo()
    {
        BasicHttpBinding basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
        basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;

        ScenarioTestHelpers.RunBasicEchoTest(basicHttpBinding, Endpoints.Https_NtlmAuth_Address, "BasicHttpBinding with NTLM authentication", null);
    }

    [WcfFact]
    [Condition(nameof(NTLM_Available), nameof(Root_Certificate_Installed), nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void IntegratedWindowsAuthentication_Ntlm_RoundTrips_Echo()
    {
        BasicHttpBinding basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
        basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
        var binding = new CustomBinding(basicHttpBinding);
        var htbe = binding.Elements.Find<HttpsTransportBindingElement>();
        htbe.AuthenticationScheme = System.Net.AuthenticationSchemes.IntegratedWindowsAuthentication;

        ScenarioTestHelpers.RunBasicEchoTest(binding, Endpoints.Https_NtlmAuth_Address, "BasicHttpBinding with IntegratedWindowsAuthentication authentication and Ntlm endpoint", null);
    }
}
