// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Infrastructure.Common;
using Xunit;

public static class UriTemplateMatchTest
{
    private static readonly Uri s_baseAddress = new Uri("http://localhost/svc/");

    [WcfFact]
    public static void Default_Ctor_Initializes_Empty_Collections()
    {
        UriTemplateMatch match = new UriTemplateMatch();

        Assert.Empty(match.BoundVariables);
        Assert.Empty(match.QueryParameters);
        Assert.Empty(match.RelativePathSegments);
        Assert.Empty(match.WildcardPathSegments);
        Assert.Null(match.Data);
        Assert.Null(match.Template);
        Assert.Null(match.BaseUri);
        Assert.Null(match.RequestUri);
    }

    [WcfFact]
    public static void Properties_RoundTrip()
    {
        UriTemplate template = new UriTemplate("a/{b}");
        Uri baseUri = new Uri("http://localhost/base/");
        Uri requestUri = new Uri("http://localhost/base/a/1");
        UriTemplateMatch match = new UriTemplateMatch
        {
            Template = template,
            BaseUri = baseUri,
            RequestUri = requestUri,
            Data = 42
        };

        Assert.Same(template, match.Template);
        Assert.Equal(baseUri, match.BaseUri);
        Assert.Equal(requestUri, match.RequestUri);
        Assert.Equal(42, match.Data);
    }

    [WcfFact]
    public static void Collections_Are_Mutable()
    {
        UriTemplateMatch match = new UriTemplateMatch();

        match.BoundVariables.Add("A", "1");
        match.QueryParameters.Add("q", "2");
        match.RelativePathSegments.Add("seg");
        match.WildcardPathSegments.Add("tail");

        Assert.Equal("1", match.BoundVariables["A"]);
        Assert.Equal("2", match.QueryParameters["q"]);
        Assert.Equal("seg", match.RelativePathSegments[0]);
        Assert.Equal("tail", match.WildcardPathSegments[0]);
    }

    [WcfFact]
    public static void BoundVariables_Are_Decoded_And_Uppercase_Keyed()
    {
        UriTemplate template = new UriTemplate("a/{b}?q={c}");

        UriTemplateMatch match = template.Match(s_baseAddress, new Uri("http://localhost/svc/a/x%20y?q=z%20w"));

        Assert.NotNull(match);
        Assert.Equal(new[] { "B", "C" }, match.BoundVariables.AllKeys);
        Assert.Equal("x y", match.BoundVariables["B"]);
        Assert.Equal("z w", match.BoundVariables["C"]);
    }

    [WcfFact]
    public static void QueryParameters_Contain_All_Query_Values_Decoded()
    {
        UriTemplate template = new UriTemplate("a?q={c}");

        UriTemplateMatch match = template.Match(s_baseAddress, new Uri("http://localhost/svc/a?q=z%20w&other=p%2Bq"));

        Assert.NotNull(match);
        Assert.Equal("z w", match.QueryParameters["q"]);
        Assert.Equal("p+q", match.QueryParameters["other"]);
    }

    [WcfFact]
    public static void RelativePathSegments_Exclude_BaseAddress_And_Are_Decoded()
    {
        UriTemplate template = new UriTemplate("a/{b}/{c}");

        UriTemplateMatch match = template.Match(s_baseAddress, new Uri("http://localhost/svc/a/x%20y/z"));

        Assert.NotNull(match);
        Assert.Equal(new[] { "a", "x y", "z" }, match.RelativePathSegments);
    }

    [WcfFact]
    public static void WildcardPathSegments_Contain_Only_The_Wildcard_Tail()
    {
        UriTemplate template = new UriTemplate("a/{*rest}");

        UriTemplateMatch match = template.Match(s_baseAddress, new Uri("http://localhost/svc/a/b/c"));

        Assert.NotNull(match);
        Assert.Equal(new[] { "a", "b", "c" }, match.RelativePathSegments);
        Assert.Equal(new[] { "b", "c" }, match.WildcardPathSegments);
    }
}
