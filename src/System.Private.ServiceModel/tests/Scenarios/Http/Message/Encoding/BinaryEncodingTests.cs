// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel.Channels;
using System.Text;
using Xunit;

public static class Http_Message_Encoding_BinaryEncodingTests
{
    // Client and Server bindings setup exactly the same using Binary Message encoder
    // and exchanging a basic message
    [Fact]
    [OuterLoop]
    public static void SameBinding_Binary_EchoBasicString()
    {
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            CustomBinding binding = new CustomBinding(new BinaryMessageEncodingBindingElement(), new HttpTransportBindingElement());
            ScenarioTestHelpers.RunBasicEchoTest(binding, Endpoints.HttpBinary_Address, "CustomBinding with binary, http", errorBuilder);
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
            for (Exception innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
            {
                errorBuilder.AppendLine(String.Format("Inner exception: {0}", innerException.ToString()));
            }
        }

        Assert.True(errorBuilder.Length == 0, errorBuilder.ToString());
    }

    // Client and Server bindings setup exactly the same using Binary Message encoder
    // and exchanging a complicated message
    [Fact]
    [OuterLoop]
    public static void SameBinding_Binary_EchoComplexString()
    {
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            CustomBinding binding = new CustomBinding(new BinaryMessageEncodingBindingElement(), new HttpTransportBindingElement());
            ScenarioTestHelpers.RunComplexEchoTest(binding, Endpoints.HttpBinary_Address, "CustomBinding with binary, http", errorBuilder);
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, errorBuilder.ToString());
    }
}
