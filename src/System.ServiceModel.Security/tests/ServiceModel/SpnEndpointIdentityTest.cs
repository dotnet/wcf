// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using Xunit;

public static class SpnEndpointIdentityTest
{
    [Theory]
    [InlineData("")]
    [InlineData("host/wcf")]
    [InlineData("host/wcf.example.com")]
    public static void Ctor_SpnName(string spn)
    {
        SpnEndpointIdentity spnEndpointEntity = new SpnEndpointIdentity(spn);
    }

    [Fact]
    public static void Ctor_NullSpn()
    {
        string spnName = null;

        Assert.Throws<ArgumentNullException>("spnName", () =>
        {
            SpnEndpointIdentity spnEndpointEntity = new SpnEndpointIdentity(spnName);
        });
    }

    [Theory]
    [MemberData("ValidTimeSpans", MemberType = typeof(TestData))]
    public static void Set_SpnLookupTime_ValidTimes(TimeSpan timeSpan)
    {
        SpnEndpointIdentity.SpnLookupTime = timeSpan; 
    }

    [Theory]
    [MemberData("InvalidTimeSpans", MemberType = typeof(TestData))]
    public static void Set_SpnLookupTime_InvalidTimes_Throws(TimeSpan timeSpan)
    {
        Assert.Throws<ArgumentOutOfRangeException>("value", () =>
        {
            SpnEndpointIdentity.SpnLookupTime = timeSpan;
        });
    }

    private class TestData
    {
        public static IEnumerable<object> ValidTimeSpans()
        {
            TimeSpan[] validTimeSpans = new TimeSpan[] { TimeSpan.Zero, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60), TimeSpan.MaxValue };
            foreach (var ts in validTimeSpans)
            {
                yield return new object[] { ts };
            }
        }

        public static IEnumerable<object> InvalidTimeSpans()
        {
            TimeSpan[] validTimeSpans = new TimeSpan[] { TimeSpan.FromSeconds(-1), TimeSpan.MinValue };
            foreach (var ts in validTimeSpans)
            {
                yield return new object[] { ts };
            }
        }
    }
}
