// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using Xunit;

public static class Https_ClientCredentialTypeTests
{
    private static string s_username;
    private static string s_password;

    static Https_ClientCredentialTypeTests()
    {
        s_username = "wcf-test";
        s_password = "wcfSaysHell0World!";
    }

    [Fact]
    [OuterLoop]
    public static void BasicAuthentication_RoundTrips_Echo()
    {
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            ChannelFactory<IWcfCustomUserNameService> factory = new ChannelFactory<IWcfCustomUserNameService>(basicHttpBinding, new EndpointAddress(Endpoints.Https_BasicAuth_Address));
            factory.Credentials.UserName.UserName = "test1";
            factory.Credentials.UserName.Password = "test1pwd";

            IWcfCustomUserNameService serviceProxy = factory.CreateChannel();

            string testString = "I am a test";
            string result = serviceProxy.Echo(testString);
            bool success = string.Equals(result, testString);

            if (!success)
            {
                errorBuilder.AppendLine(string.Format("Basic echo test.\nTest variation:...\n{0}\nUsing address: '{1}'", "BasicAuthentication_RoundTrips_Echo", Endpoints.Https_BasicAuth_Address));
                errorBuilder.AppendLine(String.Format("    Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));
            }
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(string.Format("Basic echo test.\nTest variation:...\n{0}\nUsing address: '{1}'", "BasicAuthentication_RoundTrips_Echo", Endpoints.Https_BasicAuth_Address));
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, String.Format("Test Case: BasicAuthentication FAILED with the following errors: {0}", errorBuilder));
    }

    [Fact]
    [OuterLoop]
    public static void BasicAuthenticationInvalidPwd_throw_MessageSecurityException()
    {
        StringBuilder errorBuilder = new StringBuilder();
        // Will need to use localized string once it is available
        // On Native retail, the message is stripped to 'HttpAuthorizationForbidden, Basic'
        // On Debug or .Net Core, the entire message is "The HTTP request was forbidden with client authentication scheme 'Basic'."
        // Thus we will only check message contains "forbidden"
        string message = "forbidden";

        MessageSecurityException exception = Assert.Throws<MessageSecurityException>(() =>
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            ChannelFactory<IWcfCustomUserNameService> factory = new ChannelFactory<IWcfCustomUserNameService>(basicHttpBinding, new EndpointAddress(Endpoints.Https_BasicAuth_Address));
            factory.Credentials.UserName.UserName = "test1";
            factory.Credentials.UserName.Password = "test1";

            IWcfCustomUserNameService serviceProxy = factory.CreateChannel();

            string testString = "I am a test";
            string result = serviceProxy.Echo(testString);
        });
      
        Assert.True(exception.Message.ToLower().Contains(message), string.Format("Expected exception message to contain: '{0}', actual message is: '{1}'", message, exception.Message));
    }

    [Fact]
    [OuterLoop]
    public static void BasicAuthenticationEmptyUser_throw_ArgumentException()
    {
        StringBuilder errorBuilder = new StringBuilder();
        string paraMessage = "username";

        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            ChannelFactory<IWcfCustomUserNameService> factory = new ChannelFactory<IWcfCustomUserNameService>(basicHttpBinding, new EndpointAddress(Endpoints.Https_BasicAuth_Address));
            factory.Credentials.UserName.UserName = "";
            factory.Credentials.UserName.Password = "NoUserName";

            IWcfCustomUserNameService serviceProxy = factory.CreateChannel();

            string testString = "I am a test";
            string result = serviceProxy.Echo(testString);
        });

        Assert.True(exception.Message.ToLower().Contains(paraMessage), string.Format("Expected exception message to contain: '{0}', actual: '{1}'", paraMessage, exception.Message));
    }

    [Fact]
    [ActiveIssue(270)]
    [OuterLoop]
    public static void DigestAuthentication_RoundTrips_Echo()
    {
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Digest;
            Action<ChannelFactory> credentials = (factory) =>
            {
                factory.Credentials.HttpDigest.ClientCredential.UserName = s_username;
                factory.Credentials.HttpDigest.ClientCredential.Password = s_password;
            };

            ScenarioTestHelpers.RunBasicEchoTest(basicHttpBinding, Endpoints.Https_DigestAuth_Address, "BasicHttpBinding - Digest auth ", errorBuilder, credentials);
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, String.Format("Test Case: DigestAuthentication FAILED with the following errors: {0}", errorBuilder));
    }

    [Fact]
    [OuterLoop]
    public static void NtlmAuthentication_RoundTrips_Echo()
    {
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;

            ScenarioTestHelpers.RunBasicEchoTest(basicHttpBinding, Endpoints.Https_NtlmAuth_Address, "BasicHttpBinding with NTLM authentication", errorBuilder, null);
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, String.Format("Test Case: NtlmAuthentication FAILED with the following errors: {0}", errorBuilder));
    }

    [Fact]
    [OuterLoop]
    public static void WindowsAuthentication_RoundTrips_Echo()
    {
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
            basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;

            ScenarioTestHelpers.RunBasicEchoTest(basicHttpBinding, Endpoints.Http_WindowsAuth_Address, "BasicHttpBinding with Windows authentication", errorBuilder, null);
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, String.Format("Test Case: WindowsAuthentication FAILED with the following errors: {0}", errorBuilder));
    }
}
