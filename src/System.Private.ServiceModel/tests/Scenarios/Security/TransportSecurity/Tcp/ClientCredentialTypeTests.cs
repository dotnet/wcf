// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Xunit;

public static class Tcp_ClientCredentialTypeTests
{
    // Simple echo of a string using NetTcpBinding on both client and server with all default settings.
    // Default settings means SecurityMode is set to Transport.
    [Fact]
    [ActiveIssue(300)]
    [OuterLoop]
    public static void SameBinding_DefaultSettings_EchoString()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;

        try
        {
            NetTcpBinding binding = new NetTcpBinding();

            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_DefaultBinding_Address));
            IWcfService serviceProxy = factory.CreateChannel();

            string result = serviceProxy.Echo(testString);
            Assert.Equal(testString, result);

            factory.Close(); 
        }
        finally 
        {
            ScenarioTestHelpers.CloseCommunicationObjects(factory); 
        }
    }

    // Simple echo of a string using NetTcpBinding on both client and server with all default settings.
    [Fact]
    [OuterLoop]
    public static void SameBinding_SecurityModeNone_EchoString()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;

        try
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_Address));
            IWcfService serviceProxy = factory.CreateChannel();

            string result = serviceProxy.Echo(testString);
            Assert.Equal(testString, result);

            factory.Close();
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects(factory);
        }
    }

    // Simple echo of a string using NetTcpBinding on both client and server with SecurityMode=Transport
    [Fact]
    [ActiveIssue(300)]
    [OuterLoop]
    public static void SameBinding_SecurityModeTransport_EchoString()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;

        try
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_Address));
            IWcfService serviceProxy = factory.CreateChannel();

            string result = serviceProxy.Echo(testString);
            Assert.Equal(testString, result);

            factory.Close(); 
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects(factory);
        }
    }

    // Simple echo of a string using NetTcpBinding on both client and server with SecurityMode=Transport
    [Fact]
    [ActiveIssue(310)]
    [OuterLoop]
    public static void SameBinding_SecurityModeTransport_ClientCredentialTypeCertificate_EchoString()
    {
        string testString = "Hello";
        ChannelFactory<IWcfService> factory = null;

        try
        {
            CustomBinding binding = new CustomBinding(
                new SslStreamSecurityBindingElement(),
                new BinaryMessageEncodingBindingElement(),
                new TcpTransportBindingElement());
            
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_CustomBinding_SslStreamSecurity_Address));
            
            IWcfService serviceProxy = factory.CreateChannel();

            string result = serviceProxy.Echo(testString);
            Assert.Equal(testString, result);

            factory.Close();
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects(factory);
        }
    }
}
