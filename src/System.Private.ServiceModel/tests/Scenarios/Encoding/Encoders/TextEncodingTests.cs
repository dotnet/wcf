// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Tests.Common;
using System.Text;
using Xunit;

public static class TextEncodingTests 
{
    // Simple echo of a string. Same binding on both client and server. CustomBinding with TextMessageEncoding and no WindowsStreamSecurityBindingElement
    [Fact]
    [OuterLoop]
    public static void SameBinding_SecurityModeNone_Text_EchoString_Roundtrip()
    {
        string variationDetails = "Client:: CustomBinding/TcpTransport/NoWindowsStreamSecurity/TextMessageEncoding = None\nServer:: CustomBinding/TcpTransport/NoWindowsStreamSecurity/TextMessageEncoding = None";
        string testString = "Hello";
        StringBuilder errorBuilder = new StringBuilder();
        bool success = false;

        try
        {
            BindingElement[] bindingElements = new BindingElement[2];
            bindingElements[0] = new TextMessageEncodingBindingElement();
            bindingElements[1] = new TcpTransportBindingElement();
            CustomBinding binding = new CustomBinding(bindingElements);

            ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_CustomBinding_NoSecurity_Text_Address));
            IWcfService serviceProxy = factory.CreateChannel();

            string result = serviceProxy.Echo(testString);
            success = string.Equals(result, testString);

            if (!success)
            {
                errorBuilder.AppendLine(String.Format("    Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));
            }
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("    Error: Unexpected exception was caught while doing the basic echo test for variation...\n'{0}'\nException: {1}", variationDetails, ex.ToString()));
            for (Exception innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
            {
                errorBuilder.AppendLine(String.Format("Inner exception: {0}", innerException.ToString()));
            }
        }

        Assert.True(errorBuilder.Length == 0, "Test case FAILED with errors: " + errorBuilder.ToString());
    }
}
