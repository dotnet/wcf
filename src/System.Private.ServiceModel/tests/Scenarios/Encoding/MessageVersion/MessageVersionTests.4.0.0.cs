// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Infrastructure.Common;
using Xunit;

public partial class MessageVersionTests : ConditionalWcfTest
{
    // Client and Server bindings setup exactly the same using Soap12WSA10
    [WcfFact]
    [OuterLoop]
    public static void SameBinding_Soap12WSA10_EchoString()
    {
        CustomBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        string result = null;
        string testString = "Hello";

        try
        {
            // *** SETUP *** \\
            binding = new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8), new HttpTransportBindingElement());
            endpointAddress = new EndpointAddress(Endpoints.HttpSoap12_Address);
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

    [WcfFact]
    [OuterLoop]
    public static void SameBinding_Soap11_EchoString()
    {
        CustomBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        string testString = "Hello";
        string result = null;

        try
        {
            // *** SETUP *** \\
            binding = new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8), new HttpTransportBindingElement());
            endpointAddress = new EndpointAddress(Endpoints.HttpSoap11_Address);
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
