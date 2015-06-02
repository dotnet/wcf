// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel.Channels;
using System.Text;
using Xunit;

public static class Encoding_Encoders_TextEncodingTests
{
    // Simple echo of a string. Same binding on both client and server. CustomBinding with TextMessageEncoding and no WindowsStreamSecurityBindingElement
    [Fact]
    [OuterLoop]
    public static void SameBinding_SecurityModeNone_Text_EchoString_Roundtrip()
    {
        string variationDetails = "Client:: CustomBinding/TcpTransport/NoWindowsStreamSecurity/TextMessageEncoding = None\nServer:: CustomBinding/TcpTransport/NoWindowsStreamSecurity/TextMessageEncoding = None";
        StringBuilder errorBuilder = new StringBuilder();

        BindingElement[] bindingElements = new BindingElement[2];
        bindingElements[0] = new TextMessageEncodingBindingElement();
        bindingElements[1] = new TcpTransportBindingElement();
        CustomBinding binding = new CustomBinding(bindingElements);

        ScenarioTestHelpers.RunBasicEchoTest(binding, Endpoints.Tcp_CustomBinding_NoSecurity_Text_Address, variationDetails, errorBuilder);

        Assert.True(errorBuilder.Length == 0, "Test case FAILED with errors: " + errorBuilder.ToString());
    }
}
