// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Tests.Common;
using System.Text;
using Xunit;

public static class BinaryEncodingTests 
{
    // Client and Server bindings setup exactly the same using Binary Message encoder
    // and exchanging a basic message
    [Fact]
    [OuterLoop]
    public static void SameBinding_Binary_EchoBasicString()
    {
        string variationDetails = "Client:: CustomBinding/BinaryEncoder/Http\nServer:: CustomBinding/BinaryEncoder/Http";
        string testString = "Hello";
        StringBuilder errorBuilder = new StringBuilder();
        bool success = false;

        try
        {
            CustomBinding binding = new CustomBinding(new BinaryMessageEncodingBindingElement(), new HttpTransportBindingElement());
            ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBinary_Address));
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

        Assert.True(errorBuilder.Length == 0, errorBuilder.ToString());
    }

    // Client and Server bindings setup exactly the same using Binary Message encoder
    // and exchanging a complicated message
    [Fact]
    [OuterLoop]
    public static void SameBinding_Binary_EchoComplexString()
    {
        string variationDetails = "Client:: CustomBinding/BinaryEncoder/Http\nServer:: CustomBinding/BinaryEncoder/Http";
        StringBuilder errorBuilder = new StringBuilder();
        bool success = false;

        try
        {
            CustomBinding binding = new CustomBinding(new BinaryMessageEncodingBindingElement(), new HttpTransportBindingElement());
            ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBinary_Address));
            IWcfService serviceProxy = factory.CreateChannel();

            ComplexCompositeType compositeObject = ScenarioTestHelpers.GetInitializedComplexCompositeType();

            ComplexCompositeType result = serviceProxy.EchoComplex(compositeObject);
            success = compositeObject.Equals(result);

            if (!success)
            {
                errorBuilder.AppendLine(String.Format("    Error: expected response from service: '{0}' Actual was: '{1}'", compositeObject, result));
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

        Assert.True(errorBuilder.Length == 0, errorBuilder.ToString());
    }
}
