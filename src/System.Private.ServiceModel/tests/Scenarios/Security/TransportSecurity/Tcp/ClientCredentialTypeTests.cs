// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

public static class Security_TransportSecurity_Tcp_ClientCredentialTypeTests
{
    // Simple echo of a string using NetTcpBinding on both client and server with all default settings.
    [Fact(Skip = "SecurityMode.Transport not yet supported.")]
    [OuterLoop]
    public static void SameBinding_DefaultSettings_EchoString()
    {
        string testCaseName = "NetTcpBinding_ScenarioTests.SameBinding_DefaultSettings_EchoString";
        string variationDetails = "Client:: NetTcpBinding/DefaultValues\nServer:: NetTcpBinding/DefaultValues";

        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            NetTcpBinding binding = new NetTcpBinding();
            ScenarioTestHelpers.RunBasicEchoTest(binding, Endpoints.Tcp_DefaultBinding_Address, variationDetails, errorBuilder);
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, String.Format("Test Case: {0} FAILED with the following errors: {1}", testCaseName, errorBuilder));
    }

    // Simple echo of a string using NetTcpBinding on both client and server with all default settings.
    [Fact]
    [OuterLoop]
    public static void SameBinding_SecurityModeNone_EchoString()
    {
        string testCaseName = "NetTcpBinding_ScenarioTests.SameBinding_SecurityModeNone_EchoString";
        string variationDetails = "Client:: NetTcpBinding/SecurityMode = None\nServer:: NetTcpBinding/SecurityMode = None";
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            ScenarioTestHelpers.RunBasicEchoTest(binding, Endpoints.Tcp_NoSecurity_Address, variationDetails, errorBuilder);
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, String.Format("Test Case: {0} FAILED with the following errors: {1}", testCaseName, errorBuilder));
    }

    // Simple echo of a string using NetTcpBinding on both client and server with SecurityMode=Transport
    [Fact(Skip = "Net.Tcp with SecurityMode=Transport is not currently supported.")]
    [OuterLoop]
    public static void SameBinding_SecurityModeTransport_EchoString()
    {
        string testCaseName = "NetTcpBinding_ScenarioTests.SameBinding_SecurityModeTransport_EchoString";
        string variationDetails = "Client:: NetTcpBinding/SecurityMode = Transport\nServer:: NetTcpBinding/SecurityMode = Transport";
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            ScenarioTestHelpers.RunBasicEchoTest(binding, Endpoints.Tcp_NoSecurity_Address, variationDetails, errorBuilder);
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, String.Format("Test Case: {0} FAILED with the following errors: {1}", testCaseName, errorBuilder));
    }
}
