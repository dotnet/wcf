// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public static partial class ClientBaseTests_4_4_0
{
    [WcfFact]
    [OuterLoop]
    public static void ClientBaseOfT_ClientCredentials()
    {
        MyClientBase client = null;
        ClientCredentials clientCredentials = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            string endpoint = Endpoints.HttpSoap12_Address;
            client = new MyClientBase(customBinding, new EndpointAddress(endpoint));

            // *** EXECUTE *** \\
            clientCredentials = client.ClientCredentials;

            // *** VALIDATE *** \\
            Assert.True(clientCredentials != null, "ClientCredentials should not be null");
            Assert.True(clientCredentials.ClientCertificate != null, "ClientCredentials.ClientCertificate should not be null");
            Assert.True(clientCredentials.ServiceCertificate != null, "ClientCredentials.ServiceCertificate should not be null");
            Assert.True(clientCredentials.HttpDigest != null, "ClientCredentials.HttpDigest should not be null");

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();

        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects(client);
        }
    }
}
