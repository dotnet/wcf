// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// 

using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;

using Xunit;

public static class Http_ClientCredentialTypeTests
{
    [WcfFact]
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

            string DigestUsernameHeaderName = "DigestUsername";
            string DigestPasswordHeaderName = "DigestPassword";
            string DigestRealmHeaderName = "DigestRealm";
            string username = Guid.NewGuid().ToString("n").Substring(0, 8);
            string password = Guid.NewGuid().ToString("n").Substring(0, 16);
            string realm = Guid.NewGuid().ToString("n").Substring(0, 5);
            factory.Credentials.HttpDigest.ClientCredential = new NetworkCredential(username, password, realm);

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = null;
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

                requestMessageProperty.Headers[DigestUsernameHeaderName] = username;
                requestMessageProperty.Headers[DigestPasswordHeaderName] = password;
                requestMessageProperty.Headers[DigestRealmHeaderName] = realm;

                result = serviceProxy.Echo(testString);
            }

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
