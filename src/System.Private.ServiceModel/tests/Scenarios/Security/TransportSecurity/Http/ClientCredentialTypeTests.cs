// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

using System.ServiceModel;

using Infrastructure.Common;

using Xunit;

public static class Http_ClientCredentialTypeTests
{
    [Fact]
    [OuterLoop]
    public static void DigestAuthentication_Echo_RoundTrips_String_No_Domain()
    {
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        string testString = "Hello";
        BasicHttpBinding binding;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Digest;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Http_DigestAuth_NoDomain_Address));
            factory.Credentials.HttpDigest.ClientCredential = BridgeClientAuthenticationManager.NetworkCredential;
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.True(result == testString, string.Format("Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));

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
}
