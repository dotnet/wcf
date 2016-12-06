// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public static partial class TextEncodingTests
{
    // Simple echo of a string. Same binding on both client and server. CustomBinding with TextMessageEncoding and no WindowsStreamSecurityBindingElement
    [WcfFact]
    [OuterLoop]
    public static void SameBinding_SecurityModeNone_Text_EchoString_Roundtrip()
    {
        BindingElement[] bindingElements = null;
        CustomBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        EndpointAddress endpointAddress = null;
        string result = null;
        string testString = "Hello";

        try
        {
            // *** SETUP *** \\
            bindingElements = new BindingElement[2];
            bindingElements[0] = new TextMessageEncodingBindingElement();
            bindingElements[1] = new TcpTransportBindingElement();
            binding = new CustomBinding(bindingElements);
            endpointAddress = new EndpointAddress(Endpoints.Tcp_CustomBinding_NoSecurity_Text_Address);
            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.True(String.Equals(result, testString), String.Format("    Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));

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
