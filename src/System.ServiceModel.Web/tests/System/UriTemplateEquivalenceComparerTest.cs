// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Infrastructure.Common;
using Xunit;

public static class UriTemplateEquivalenceComparerTest
{
    [WcfFact]
    public static void Equals_Matches_IsEquivalentTo()
    {
        UriTemplateEquivalenceComparer comparer = new UriTemplateEquivalenceComparer();

        Assert.True(comparer.Equals(new UriTemplate("{a}/{b}"), new UriTemplate("{x}/{y}")));
        Assert.True(comparer.Equals(new UriTemplate("a/{b}"), new UriTemplate("a/{c}")));
        Assert.False(comparer.Equals(new UriTemplate("a/{b}"), new UriTemplate("b/{c}")));
    }

    [WcfFact]
    public static void Equals_Handles_Nulls()
    {
        UriTemplateEquivalenceComparer comparer = new UriTemplateEquivalenceComparer();

        Assert.True(comparer.Equals(null, null));
        Assert.False(comparer.Equals(new UriTemplate("a/{b}"), null));
        Assert.False(comparer.Equals(null, new UriTemplate("a/{b}")));
    }

    [WcfFact]
    public static void GetHashCode_Is_Equal_For_Equivalent_Templates()
    {
        UriTemplateEquivalenceComparer comparer = new UriTemplateEquivalenceComparer();

        Assert.Equal(comparer.GetHashCode(new UriTemplate("a/{b}")), comparer.GetHashCode(new UriTemplate("a/{c}")));
    }

    [WcfFact]
    public static void GetHashCode_Null_Throws()
    {
        UriTemplateEquivalenceComparer comparer = new UriTemplateEquivalenceComparer();

        Assert.Throws<ArgumentNullException>(() => comparer.GetHashCode(null));
    }

    [WcfFact]
    public static void Comparer_Works_As_Dictionary_Key_Comparer()
    {
        Dictionary<UriTemplate, string> map =
            new Dictionary<UriTemplate, string>(new UriTemplateEquivalenceComparer())
            {
                { new UriTemplate("a/{b}"), "first" }
            };

        Assert.True(map.ContainsKey(new UriTemplate("a/{different}")));
        Assert.False(map.ContainsKey(new UriTemplate("z/{b}")));
    }
}
