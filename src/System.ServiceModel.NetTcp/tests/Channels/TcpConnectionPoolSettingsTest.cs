// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Tests.Common;
using Infrastructure.Common;
using Xunit;

public static class TcpConnectionPoolSettingsTest
{
#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#endif
    [WcfFact]
    public static void GroupName_Property_Set_Null_Value_Throws()
    {
        // TcpConnectionPoolSettings has no public constructor but we can access it from the TcpTransportBindingElement
        TcpTransportBindingElement element = new TcpTransportBindingElement();
        TcpConnectionPoolSettings settings = element.ConnectionPoolSettings;
        Assert.Throws<ArgumentNullException>(() => settings.GroupName = null);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [MemberData("ValidTimeOuts", MemberType = typeof(TestData))]
    public static void IdleTimeout_Property_Sets(TimeSpan timeSpan)
    {
        // TcpConnectionPoolSettings has no public constructor but we can access it from the TcpTransportBindingElement
        TcpTransportBindingElement element = new TcpTransportBindingElement();
        TcpConnectionPoolSettings settings = element.ConnectionPoolSettings;
        settings.IdleTimeout = timeSpan;
        Assert.Equal<TimeSpan>(timeSpan, settings.IdleTimeout);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [MemberData("InvalidTimeOuts", MemberType = typeof(TestData))]
    public static void IdleTimeout_Property_Set_Invalid_Value_Throws(TimeSpan timeSpan)
    {
        // TcpConnectionPoolSettings has no public constructor but we can access it from the TcpTransportBindingElement
        TcpTransportBindingElement element = new TcpTransportBindingElement();
        TcpConnectionPoolSettings settings = element.ConnectionPoolSettings;
        Assert.Throws<ArgumentOutOfRangeException>(() => settings.IdleTimeout = timeSpan);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [MemberData("ValidTimeOuts", MemberType = typeof(TestData))]
    public static void LeaseTimeout_Property_Sets(TimeSpan timeSpan)
    {
        // TcpConnectionPoolSettings has no public constructor but we can access it from the TcpTransportBindingElement
        TcpTransportBindingElement element = new TcpTransportBindingElement();
        TcpConnectionPoolSettings settings = element.ConnectionPoolSettings;
        settings.LeaseTimeout = timeSpan;
        Assert.Equal<TimeSpan>(timeSpan, settings.LeaseTimeout);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [MemberData("InvalidTimeOuts", MemberType = typeof(TestData))]
    public static void LeaseTimeout_Property_Set_Invalid_Value_Throws(TimeSpan timeSpan)
    {
        // TcpConnectionPoolSettings has no public constructor but we can access it from the TcpTransportBindingElement
        TcpTransportBindingElement element = new TcpTransportBindingElement();
        TcpConnectionPoolSettings settings = element.ConnectionPoolSettings;
        Assert.Throws<ArgumentOutOfRangeException>(() => settings.LeaseTimeout = timeSpan);
    }


#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
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

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [InlineData(-1)]
    public static void MaxOutboundConnectionsPerEndpoint_Property_Set_Invalid_Value_Throws(int value)
    {
        // TcpConnectionPoolSettings has no public constructor but we can access it from the TcpTransportBindingElement
        TcpTransportBindingElement element = new TcpTransportBindingElement();
        TcpConnectionPoolSettings settings = element.ConnectionPoolSettings;
        Assert.Throws<ArgumentOutOfRangeException>(() => settings.MaxOutboundConnectionsPerEndpoint = value);
    }
}
