// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Tests.Common;
using Xunit;

public static class TcpConnectionPoolSettingsTest
{
    [Theory]
    [InlineData("")]
    [InlineData("testValue")]
    public static void GroupName_Property_Sets(string groupName)
    {
        // TcpConnectionPoolSettings has no public constructor but we can access it from the TcpTransportBindingElement
        TcpTransportBindingElement element = new TcpTransportBindingElement();
        TcpConnectionPoolSettings settings = element.ConnectionPoolSettings;
        settings.GroupName = groupName;
        Assert.Equal<string>(groupName, settings.GroupName);
    }

    [Fact]
    public static void GroupName_Property_Set_Null_Value_Throws()
    {
        // TcpConnectionPoolSettings has no public constructor but we can access it from the TcpTransportBindingElement
        TcpTransportBindingElement element = new TcpTransportBindingElement();
        TcpConnectionPoolSettings settings = element.ConnectionPoolSettings;
        Assert.Throws<ArgumentNullException>(() => settings.GroupName = null);
    }

    [Theory]
    [MemberData("ValidTimeOuts", MemberType = typeof(TestData))]
    public static void IdleTimeout_Property_Sets(TimeSpan timeSpan)
    {
        // TcpConnectionPoolSettings has no public constructor but we can access it from the TcpTransportBindingElement
        TcpTransportBindingElement element = new TcpTransportBindingElement();
        TcpConnectionPoolSettings settings = element.ConnectionPoolSettings;
        settings.IdleTimeout = timeSpan;
        Assert.Equal<TimeSpan>(timeSpan, settings.IdleTimeout);
    }

    [Theory]
    [MemberData("InvalidTimeOuts", MemberType = typeof(TestData))]
    public static void IdleTimeout_Property_Set_Invalid_Value_Throws(TimeSpan timeSpan)
    {
        // TcpConnectionPoolSettings has no public constructor but we can access it from the TcpTransportBindingElement
        TcpTransportBindingElement element = new TcpTransportBindingElement();
        TcpConnectionPoolSettings settings = element.ConnectionPoolSettings;
        Assert.Throws<ArgumentOutOfRangeException>(() => settings.IdleTimeout = timeSpan);
    }

    [Theory]
    [MemberData("ValidTimeOuts", MemberType = typeof(TestData))]
    public static void LeaseTimeout_Property_Sets(TimeSpan timeSpan)
    {
        // TcpConnectionPoolSettings has no public constructor but we can access it from the TcpTransportBindingElement
        TcpTransportBindingElement element = new TcpTransportBindingElement();
        TcpConnectionPoolSettings settings = element.ConnectionPoolSettings;
        settings.LeaseTimeout = timeSpan;
        Assert.Equal<TimeSpan>(timeSpan, settings.LeaseTimeout);
    }

    [Theory]
    [MemberData("InvalidTimeOuts", MemberType = typeof(TestData))]
    public static void LeaseTimeout_Property_Set_Invalid_Value_Throws(TimeSpan timeSpan)
    {
        // TcpConnectionPoolSettings has no public constructor but we can access it from the TcpTransportBindingElement
        TcpTransportBindingElement element = new TcpTransportBindingElement();
        TcpConnectionPoolSettings settings = element.ConnectionPoolSettings;
        Assert.Throws<ArgumentOutOfRangeException>(() => settings.LeaseTimeout = timeSpan);
    }


    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public static void MaxOutboundConnectionsPerEndpoint_Property_Sets(int value)
    {
        // TcpConnectionPoolSettings has no public constructor but we can access it from the TcpTransportBindingElement
        TcpTransportBindingElement element = new TcpTransportBindingElement();
        TcpConnectionPoolSettings settings = element.ConnectionPoolSettings;
        settings.MaxOutboundConnectionsPerEndpoint = value;
        Assert.Equal<int>(value, settings.MaxOutboundConnectionsPerEndpoint);
    }

    [Theory]
    [InlineData(-1)]
    public static void MaxOutboundConnectionsPerEndpoint_Property_Set_Invalid_Value_Throws(int value)
    {
        // TcpConnectionPoolSettings has no public constructor but we can access it from the TcpTransportBindingElement
        TcpTransportBindingElement element = new TcpTransportBindingElement();
        TcpConnectionPoolSettings settings = element.ConnectionPoolSettings;
        Assert.Throws<ArgumentOutOfRangeException>(() => settings.MaxOutboundConnectionsPerEndpoint = value);
    }
}
