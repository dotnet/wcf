// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public static class SpnEndpointIdentityTest
{
    [WcfTheory]
    [InlineData("")]
    [InlineData("host/wcf")]
    [InlineData("host/wcf.example.com")]
    public static void Ctor_SpnName(string spn)
    {
        SpnEndpointIdentity spnEndpointEntity = new SpnEndpointIdentity(spn);
    }

    [WcfFact]
    public static void Ctor_NullSpn()
    {
        string spnName = null;

        Assert.Throws<ArgumentNullException>("spnName", () =>
        {
            SpnEndpointIdentity spnEndpointEntity = new SpnEndpointIdentity(spnName);
        });
    }

    [WcfTheory]
    [MemberData("ValidTimeSpans", MemberType = typeof(TestData))]
    public static void Set_SpnLookupTime_ValidTimes(TimeSpan timeSpan)
    {
        SpnEndpointIdentity.SpnLookupTime = timeSpan;
    }

    [WcfTheory]
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
