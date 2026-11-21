// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using Xunit;

public static class UriTemplateTableTest
{
    private static readonly Uri s_baseAddress = new Uri("http://localhost/svc/");

    private static UriTemplateTable CreateTable(params KeyValuePair<UriTemplate, object>[] pairs)
    {
        return new UriTemplateTable(s_baseAddress, pairs);
    }

    private static KeyValuePair<UriTemplate, object> Pair(string template, object data)
    {
        return new KeyValuePair<UriTemplate, object>(new UriTemplate(template), data);
    }

    [WcfFact]
    public static void Default_Ctor_Has_No_BaseAddress_And_No_Entries()
    {
        UriTemplateTable table = new UriTemplateTable();

        Assert.Null(table.BaseAddress);
        Assert.Null(table.OriginalBaseAddress);
        Assert.Empty(table.KeyValuePairs);
        Assert.False(table.IsReadOnly);
    }

    [WcfFact]
    public static void Ctor_With_KeyValuePairs_Populates_Table()
    {
        UriTemplateTable table = new UriTemplateTable(new[] { Pair("a/{x}", "A"), Pair("b/{y}", "B") });

        Assert.Equal(2, table.KeyValuePairs.Count);
        Assert.Null(table.BaseAddress);
    }

    [WcfFact]
    public static void Ctor_Normalizes_BaseAddress_But_Preserves_Original()
    {
        // BaseAddress is normalized for comparison purposes: a trailing slash is added, the host
        // is forced to localhost, the port/credentials are dropped, the scheme becomes http and
        // the path is uppercased. OriginalBaseAddress keeps what the caller passed in.
        Uri original = new Uri("https://example.com:8443/Svc");
        UriTemplateTable table = new UriTemplateTable(original);

        Assert.Equal(new Uri("http://localhost/SVC/"), table.BaseAddress);
        Assert.Equal(original, table.OriginalBaseAddress);
    }

    [WcfFact]
    public static void BaseAddress_Setter_Rejects_Null()
    {
        UriTemplateTable table = new UriTemplateTable();

        Assert.Throws<ArgumentNullException>(() => table.BaseAddress = null);
    }

    [WcfFact]
    public static void BaseAddress_Setter_Rejects_Relative_Uri()
    {
        UriTemplateTable table = new UriTemplateTable();

        Assert.Throws<ArgumentException>(() => table.BaseAddress = new Uri("/relative", UriKind.Relative));
    }

    [WcfFact]
    public static void Ctor_Relative_BaseAddress_Throws()
    {
        Assert.Throws<ArgumentException>(() => new UriTemplateTable(new Uri("/relative", UriKind.Relative)));
    }

    [WcfFact]
    public static void MakeReadOnly_Without_BaseAddress_Throws()
    {
        UriTemplateTable table = new UriTemplateTable(new[] { Pair("a/{x}", "A") });

        Assert.Throws<InvalidOperationException>(() => table.MakeReadOnly(false));
    }

    [WcfFact]
    public static void MakeReadOnly_Rejects_Equivalent_Templates_When_Duplicates_Disallowed()
    {
        UriTemplateTable table = CreateTable(Pair("a/{x}", "A"), Pair("a/{y}", "B"));

        Assert.Throws<InvalidOperationException>(() => table.MakeReadOnly(false));
    }

    [WcfFact]
    public static void MakeReadOnly_Allows_Equivalent_Templates_When_Duplicates_Allowed()
    {
        UriTemplateTable table = CreateTable(Pair("a/{x}", "A"), Pair("a/{y}", "B"));

        table.MakeReadOnly(true);

        Assert.True(table.IsReadOnly);
    }

    [WcfFact]
    public static void MakeReadOnly_Freezes_KeyValuePairs()
    {
        UriTemplateTable table = CreateTable(Pair("a/{x}", "A"));

        table.MakeReadOnly(false);

        Assert.True(table.KeyValuePairs.IsReadOnly);
        Assert.Throws<InvalidOperationException>(() => table.KeyValuePairs.Add(Pair("b/{y}", "B")));
    }

    [WcfFact]
    public static void MakeReadOnly_Freezes_BaseAddress()
    {
        UriTemplateTable table = CreateTable(Pair("a/{x}", "A"));

        table.MakeReadOnly(false);

        Assert.Throws<InvalidOperationException>(() => table.BaseAddress = new Uri("http://localhost/other/"));
    }

    [WcfFact]
    public static void Match_Implicitly_Freezes_Table()
    {
        UriTemplateTable table = CreateTable(Pair("a/{x}", "A"));

        Assert.False(table.IsReadOnly);
        table.Match(new Uri("http://localhost/svc/a/1"));

        Assert.True(table.IsReadOnly);
    }

    [WcfFact]
    public static void MatchSingle_Implicitly_Freezes_Table()
    {
        UriTemplateTable table = CreateTable(Pair("a/{x}", "A"));

        Assert.False(table.IsReadOnly);
        table.MatchSingle(new Uri("http://localhost/svc/a/1"));

        Assert.True(table.IsReadOnly);
    }

    [WcfFact]
    public static void Match_Null_Uri_Throws()
    {
        UriTemplateTable table = CreateTable(Pair("a/{x}", "A"));

        Assert.Throws<ArgumentNullException>(() => table.Match(null));
        Assert.Throws<ArgumentNullException>(() => table.MatchSingle(null));
    }

    [WcfFact]
    public static void Match_Relative_Uri_Returns_No_Matches()
    {
        UriTemplateTable table = CreateTable(Pair("a/{x}", "A"));

        Assert.Empty(table.Match(new Uri("/a/1", UriKind.Relative)));
        Assert.Null(table.MatchSingle(new Uri("/a/1", UriKind.Relative)));
    }

    [WcfFact]
    public static void Match_Returns_Associated_Data()
    {
        UriTemplateTable table = CreateTable(Pair("a/{x}", "A"), Pair("b/{y}", "B"));

        UriTemplateMatch match = table.MatchSingle(new Uri("http://localhost/svc/b/2"));

        Assert.NotNull(match);
        Assert.Equal("B", match.Data);
        Assert.Equal("2", match.BoundVariables["Y"]);
    }

    [WcfFact]
    public static void Match_Prefers_Literal_Segment_Over_Variable()
    {
        UriTemplateTable table = CreateTable(Pair("a/{x}", "VAR"), Pair("a/lit", "LIT"));

        Collection<UriTemplateMatch> matches = table.Match(new Uri("http://localhost/svc/a/lit"));

        Assert.Single(matches);
        Assert.Equal("LIT", matches[0].Data);
    }

    [WcfFact]
    public static void Match_Returns_All_Equivalent_Candidates()
    {
        UriTemplateTable table = CreateTable(Pair("a/{x}", "A"), Pair("a/{y}", "B"));
        table.MakeReadOnly(true);

        Collection<UriTemplateMatch> matches = table.Match(new Uri("http://localhost/svc/a/1"));

        Assert.Equal(2, matches.Count);
    }

    [WcfFact]
    public static void MatchSingle_Throws_When_Multiple_Templates_Match()
    {
        UriTemplateTable table = CreateTable(Pair("a/{x}", "A"), Pair("a/{y}", "B"));
        table.MakeReadOnly(true);

        Assert.Throws<UriTemplateMatchException>(() => table.MatchSingle(new Uri("http://localhost/svc/a/1")));
    }

    [WcfFact]
    public static void Match_Returns_Empty_When_Nothing_Matches()
    {
        UriTemplateTable table = CreateTable(Pair("a/{x}", "A"));

        Assert.Empty(table.Match(new Uri("http://localhost/svc/zzz/1")));
        Assert.Null(table.MatchSingle(new Uri("http://localhost/svc/zzz/1")));
    }

    [WcfFact]
    public static void Match_Uri_Outside_BaseAddress_Returns_No_Matches()
    {
        UriTemplateTable table = CreateTable(Pair("a/{x}", "A"));

        Assert.Empty(table.Match(new Uri("http://localhost/other/a/1")));
    }

    [WcfFact]
    public static void Match_Falls_Back_To_Wildcard_Template()
    {
        UriTemplateTable table = CreateTable(Pair("a/lit", "LIT"), Pair("a/{*rest}", "REST"));

        Assert.Equal("LIT", table.MatchSingle(new Uri("http://localhost/svc/a/lit")).Data);

        UriTemplateMatch wildcardMatch = table.MatchSingle(new Uri("http://localhost/svc/a/x/y"));
        Assert.NotNull(wildcardMatch);
        Assert.Equal("REST", wildcardMatch.Data);
        Assert.Equal("x/y", wildcardMatch.BoundVariables["REST"]);
    }

    [WcfFact]
    public static void Match_Distinguishes_Templates_By_Query_Literal()
    {
        UriTemplateTable table = CreateTable(Pair("a?mode=fast", "FAST"), Pair("a?mode=slow", "SLOW"));

        Assert.Equal("FAST", table.MatchSingle(new Uri("http://localhost/svc/a?mode=fast")).Data);
        Assert.Equal("SLOW", table.MatchSingle(new Uri("http://localhost/svc/a?mode=slow")).Data);
        Assert.Null(table.MatchSingle(new Uri("http://localhost/svc/a?mode=other")));
    }

}
