// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Infrastructure.Common;
using Xunit;

public static class BinaryEncodingTests
{
    // Client and Server bindings setup exactly the same using Binary Message encoder
    // and exchanging a basic message
    [WcfFact]
    [OuterLoop]
    public static void SameBinding_Binary_EchoBasicString()
    {
        string testString = "Hello";
        CustomBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        EndpointAddress endpointAddress = null;
        IWcfService serviceProxy = null;
        string result = null;

        try
        {
            // *** SETUP *** \\
            binding = new CustomBinding(new BinaryMessageEncodingBindingElement(), new HttpTransportBindingElement());
            endpointAddress = new EndpointAddress(Endpoints.HttpBinary_Address);
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

    // Client and Server bindings setup exactly the same using Binary Message encoder
    // and exchanging a complicated message
    [WcfFact]
    [OuterLoop]
    public static void SameBinding_Binary_EchoComplexString()
    {
        CustomBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        EndpointAddress endpointAddress = null;
        IWcfService serviceProxy = null;
        ComplexCompositeType compositeObject = null;
        ComplexCompositeType result = null;

        try
        {
            // *** SETUP *** \\
            binding = new CustomBinding(new BinaryMessageEncodingBindingElement(), new HttpTransportBindingElement());
            endpointAddress = new EndpointAddress(Endpoints.HttpBinary_Address);
            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            serviceProxy = factory.CreateChannel();
            compositeObject = ScenarioTestHelpers.GetInitializedComplexCompositeType();

            // *** EXECUTE *** \\
            result = serviceProxy.EchoComplex(compositeObject);

            // *** VALIDATE *** \\
            Assert.True(compositeObject.Equals(result), String.Format("    Error: expected response from service: '{0}' Actual was: '{1}'", compositeObject, result));

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
