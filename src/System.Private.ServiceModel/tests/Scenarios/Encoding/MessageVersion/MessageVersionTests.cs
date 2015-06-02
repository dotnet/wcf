// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel.Channels;
using System.Text;
using Xunit;

public static class Encoding_MessageVersion_MessageVersionTests
{
    // Client and Server bindings setup exactly the same using Soap12WSA10
    [Fact]
    [OuterLoop]
    public static void SameBinding_Soap12WSA10_EchoString()
    {
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            CustomBinding binding = new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8), new HttpTransportBindingElement());
            ScenarioTestHelpers.RunBasicEchoTest(binding, Endpoints.HttpSoap12_Address, "CustomBinding with text, http", errorBuilder);
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, errorBuilder.ToString());
    }

    [Fact]
    [OuterLoop]
    public static void SameBinding_Soap11_EchoString()
    {
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            CustomBinding binding = new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8), new HttpTransportBindingElement());
            ScenarioTestHelpers.RunBasicEchoTest(binding, Endpoints.HttpSoap11_Address, "CustomBinding with text, http, explicit soap11", errorBuilder);
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, errorBuilder.ToString());
    }
}
