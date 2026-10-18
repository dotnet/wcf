// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Infrastructure.Common;
using Xunit;

public static class UriTemplateTest
{
    private static readonly Uri s_baseAddress = new Uri("http://localhost/svc/");

    [WcfFact]
    public static void Ctor_Null_Template_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new UriTemplate(null));
    }

    [WcfFact]
    public static void Ctor_Accepts_Valid_Templates()
    {
        string[] valid =
        {
            string.Empty,
            "/",
            "literal",
            "/leading",
            "a/b/c",
            "{a}",
            "{a}/{b}",
            "a/*",
            "a/{*rest}",
            "a?x",
            "a?x={y}",
            "{a=1}/{b}",
            "{a}/{b=1}",
            "{a=null}",
            "a#frag",
            "pre{a}post"
        };

        foreach (string template in valid)
        {
            UriTemplate uriTemplate = new UriTemplate(template);
            Assert.Equal(template, uriTemplate.ToString());
        }
    }

    [WcfFact]
    public static void Ctor_Empty_Variable_Name_Throws_FormatException()
    {
        Assert.Throws<FormatException>(() => new UriTemplate("{}"));
    }

    [WcfFact]
    public static void Ctor_Duplicate_Variable_Names_Throw()
    {
        // Same name twice in the path, and once in the path plus once in the query.
        Assert.Throws<InvalidOperationException>(() => new UriTemplate("{a}/{a}"));
        Assert.Throws<InvalidOperationException>(() => new UriTemplate("{a}?x={a}"));
    }

    [WcfFact]
    public static void Ctor_Duplicate_Query_Names_Throw()
    {
        Assert.Throws<InvalidOperationException>(() => new UriTemplate("a?x=1&x=2"));
    }

    [WcfFact]
    public static void Ctor_Wildcard_Must_Be_Last_Segment()
    {
        Assert.Throws<FormatException>(() => new UriTemplate("a/*/b"));
        Assert.Throws<FormatException>(() => new UriTemplate("{*a}/b"));
        Assert.Throws<FormatException>(() => new UriTemplate("?x={*a}"));
    }

    [WcfFact]
    public static void Ctor_Wildcard_Cannot_Have_Default_Value()
    {
        Assert.Throws<InvalidOperationException>(() => new UriTemplate("{*a=1}"));
    }

    [WcfFact]
    public static void Ctor_Variable_As_Query_Name_Throws()
    {
        // Only query *values* may be variables; the name must be a literal.
        Assert.Throws<ArgumentException>(() => new UriTemplate("a?{x}"));
    }

    [WcfFact]
    public static void Ctor_Query_Variable_Cannot_Have_Default_Value()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => new UriTemplate("a?x={v=5}"));

        Assert.Contains("'a?x={v=5}'", exception.Message);
        Assert.Contains("variable 'v'", exception.Message);
        Assert.Contains("default value '5'", exception.Message);
    }

    [WcfFact]
    public static void Ctor_AdditionalDefaults_On_Query_Variable_Throws()
    {
        Dictionary<string, string> defaults = new Dictionary<string, string> { { "v", "5" } };
        Assert.Throws<InvalidOperationException>(() => new UriTemplate("a?x={v}", false, defaults));
    }

    [WcfFact]
    public static void Ctor_AdditionalDefaults_Applies_To_Path_Variable()
    {
        Dictionary<string, string> defaults = new Dictionary<string, string> { { "p", "5" } };
        UriTemplate template = new UriTemplate("a/{p}", false, defaults);

        Assert.Equal("5", template.Defaults["p"]);
        Assert.Equal("http://localhost/svc/a/5",
            template.BindByName(s_baseAddress, new Dictionary<string, string>()).AbsoluteUri);
    }

    [WcfFact]
    public static void Ctor_AdditionalDefaults_For_Unknown_Variable_Is_Ignored()
    {
        Dictionary<string, string> defaults = new Dictionary<string, string> { { "zz", "5" } };
        UriTemplate template = new UriTemplate("a/{p}", false, defaults);

        Assert.Equal("a/{p}", template.ToString());
    }

    [WcfFact]
    public static void IgnoreTrailingSlash_Reflects_Ctor_Argument()
    {
        Assert.False(new UriTemplate("a/{b}").IgnoreTrailingSlash);
        Assert.False(new UriTemplate("a/{b}", false).IgnoreTrailingSlash);
        Assert.True(new UriTemplate("a/{b}", true).IgnoreTrailingSlash);
    }

    [WcfFact]
    public static void ToString_Returns_Original_Template_Verbatim()
    {
        Assert.Equal("/A/{b}?c=1", new UriTemplate("/A/{b}?c=1").ToString());
    }

    [WcfFact]
    public static void PathSegmentVariableNames_Are_Uppercased()
    {
        UriTemplate template = new UriTemplate("weather/{state}/{city}?day={day}");

        Assert.Equal(new[] { "STATE", "CITY" }, template.PathSegmentVariableNames);
    }

    [WcfFact]
    public static void QueryValueVariableNames_Are_Uppercased()
    {
        UriTemplate template = new UriTemplate("weather/{state}/{city}?day={day}");

        Assert.Equal(new[] { "DAY" }, template.QueryValueVariableNames);
    }

    [WcfFact]
    public static void Wildcard_Variable_Appears_In_PathSegmentVariableNames()
    {
        UriTemplate template = new UriTemplate("a/{*rest}");

        Assert.Equal(new[] { "REST" }, template.PathSegmentVariableNames);
    }

    [WcfFact]
    public static void Anonymous_Wildcard_Declares_No_Variable()
    {
        UriTemplate template = new UriTemplate("a/*");

        Assert.Empty(template.PathSegmentVariableNames);
    }

    [WcfFact]
    public static void Defaults_Is_ReadOnly()
    {
        UriTemplate template = new UriTemplate("weather/{state}/{city=Seattle}");

        Assert.True(template.Defaults.IsReadOnly);
        Assert.Throws<NotSupportedException>(() => template.Defaults.Add("x", "y"));
    }

    [WcfFact]
    public static void Defaults_Lookup_Is_Case_Insensitive()
    {
        UriTemplate template = new UriTemplate("weather/{state}/{city=Seattle}");

        Assert.Equal("Seattle", template.Defaults["city"]);
        Assert.Equal("Seattle", template.Defaults["CITY"]);
        Assert.Equal("Seattle", template.Defaults["CiTy"]);
    }

    [WcfFact]
    public static void Defaults_Contains_Only_Variables_With_Defaults()
    {
        UriTemplate template = new UriTemplate("weather/{state}/{city=Seattle}");

        Assert.Single(template.Defaults);
        Assert.False(template.Defaults.ContainsKey("STATE"));
    }

    [WcfFact]
    public static void Defaults_Null_Literal_Maps_To_Null_Value()
    {
        UriTemplate template = new UriTemplate("a/{b=null}");

        Assert.True(template.Defaults.ContainsKey("B"));
        Assert.Null(template.Defaults["B"]);
    }

    [WcfFact]
    public static void BindByName_Null_BaseAddress_Throws()
    {
        UriTemplate template = new UriTemplate("a/{b}");
        Dictionary<string, string> parameters = new Dictionary<string, string> { { "b", "1" } };

        Assert.Throws<ArgumentNullException>(() => template.BindByName(null, parameters));
    }

    [WcfFact]
    public static void BindByName_Relative_BaseAddress_Throws()
    {
        UriTemplate template = new UriTemplate("a/{b}");
        Dictionary<string, string> parameters = new Dictionary<string, string> { { "b", "1" } };

        Assert.Throws<ArgumentException>(
            () => template.BindByName(new Uri("/relative", UriKind.Relative), parameters));
    }

    [WcfFact]
    public static void BindByName_Substitutes_Path_And_Query_Variables()
    {
        UriTemplate template = new UriTemplate("weather/{state}/{city}?day={day}");
        Dictionary<string, string> parameters = new Dictionary<string, string>
        {
            { "state", "WA" }, { "city", "Seattle" }, { "day", "monday" }
        };

        Assert.Equal("http://localhost/svc/weather/WA/Seattle?day=monday",
            template.BindByName(s_baseAddress, parameters).AbsoluteUri);
    }

    [WcfFact]
    public static void BindByName_Parameter_Names_Are_Case_Insensitive()
    {
        UriTemplate template = new UriTemplate("weather/{state}/{city}");
        Dictionary<string, string> parameters = new Dictionary<string, string>
        {
            { "STATE", "WA" }, { "CiTy", "Seattle" }
        };

        Assert.Equal("http://localhost/svc/weather/WA/Seattle",
            template.BindByName(s_baseAddress, parameters).AbsoluteUri);
    }

    [WcfFact]
    public static void BindByName_Accepts_NameValueCollection()
    {
        UriTemplate template = new UriTemplate("weather/{state}/{city}");
        NameValueCollection parameters = new NameValueCollection { { "state", "WA" }, { "city", "Seattle" } };

        Assert.Equal("http://localhost/svc/weather/WA/Seattle",
            template.BindByName(s_baseAddress, parameters).AbsoluteUri);
    }

    [WcfFact]
    public static void BindByName_Missing_Value_Throws()
    {
        UriTemplate template = new UriTemplate("weather/{state}/{city}");
        Dictionary<string, string> parameters = new Dictionary<string, string> { { "state", "WA" } };

        Assert.Throws<ArgumentException>(() => template.BindByName(s_baseAddress, parameters));
    }

    [WcfFact]
    public static void BindByName_Null_Or_Empty_Value_Throws()
    {
        UriTemplate template = new UriTemplate("a/{b}");

        Assert.Throws<ArgumentException>(
            () => template.BindByName(s_baseAddress, new Dictionary<string, string> { { "b", null } }));
        Assert.Throws<ArgumentException>(
            () => template.BindByName(s_baseAddress, new Dictionary<string, string> { { "b", string.Empty } }));
    }

    [WcfFact]
    public static void BindByName_Uses_Default_When_Value_Not_Supplied()
    {
        UriTemplate template = new UriTemplate("weather/{state}/{city=Seattle}");
        Dictionary<string, string> parameters = new Dictionary<string, string> { { "state", "WA" } };

        Assert.Equal("http://localhost/svc/weather/WA/Seattle",
            template.BindByName(s_baseAddress, parameters).AbsoluteUri);
    }

    [WcfFact]
    public static void BindByName_OmitDefaults_Drops_Trailing_Default_Segment()
    {
        UriTemplate template = new UriTemplate("weather/{state}/{city=Seattle}");
        Dictionary<string, string> parameters = new Dictionary<string, string> { { "state", "WA" } };

        Assert.Equal("http://localhost/svc/weather/WA/",
            template.BindByName(s_baseAddress, parameters, true).AbsoluteUri);
    }

    [WcfFact]
    public static void BindByName_Null_Default_Drops_Segment()
    {
        UriTemplate template = new UriTemplate("a/{b=null}");

        Assert.Equal("http://localhost/svc/a/",
            template.BindByName(s_baseAddress, new Dictionary<string, string>()).AbsoluteUri);
    }

    [WcfFact]
    public static void BindByName_Escapes_Reserved_Characters()
    {
        UriTemplate template = new UriTemplate("a/{b}");

        Assert.Equal("http://localhost/svc/a/x%20y",
            template.BindByName(s_baseAddress, new Dictionary<string, string> { { "b", "x y" } }).AbsoluteUri);
        Assert.Equal("http://localhost/svc/a/x%3Fy",
            template.BindByName(s_baseAddress, new Dictionary<string, string> { { "b", "x?y" } }).AbsoluteUri);
        Assert.Equal("http://localhost/svc/a/%C3%A9",
            template.BindByName(s_baseAddress, new Dictionary<string, string> { { "b", "\u00e9" } }).AbsoluteUri);
    }

    [WcfFact]
    public static void BindByName_Preserves_Slash_As_Path_Separator_For_Compatibility()
    {
        UriTemplate template = new UriTemplate("a/{b}");

        Assert.Equal("http://localhost/svc/a/x/y",
            template.BindByName(s_baseAddress, new Dictionary<string, string> { { "b", "x/y" } }).AbsoluteUri);
    }

    [WcfFact]
    public static void BindByName_Escapes_Query_Values()
    {
        UriTemplate template = new UriTemplate("a?q={b}");

        Assert.Equal("http://localhost/svc/a?q=x%26y",
            template.BindByName(s_baseAddress, new Dictionary<string, string> { { "b", "x&y" } }).AbsoluteUri);
    }

    [WcfFact]
    public static void BindByName_BaseAddress_Without_Trailing_Slash_Is_Normalized()
    {
        UriTemplate template = new UriTemplate("a/{b}");
        Uri noTrailingSlash = new Uri("http://localhost/svc");

        Assert.Equal("http://localhost/svc/a/1",
            template.BindByName(noTrailingSlash, new Dictionary<string, string> { { "b", "1" } }).AbsoluteUri);
    }

    [WcfFact]
    public static void BindByPosition_Substitutes_In_Declaration_Order()
    {
        UriTemplate template = new UriTemplate("weather/{state}/{city}?day={day}");

        Assert.Equal("http://localhost/svc/weather/WA/Seattle?day=monday",
            template.BindByPosition(s_baseAddress, "WA", "Seattle", "monday").AbsoluteUri);
    }

    [WcfFact]
    public static void BindByPosition_Wrong_Argument_Count_Throws()
    {
        UriTemplate template = new UriTemplate("weather/{state}/{city}");

        Assert.Throws<FormatException>(() => template.BindByPosition(s_baseAddress, "WA"));
        Assert.Throws<FormatException>(() => template.BindByPosition(s_baseAddress, "WA", "Seattle", "extra"));
    }

    [WcfFact]
    public static void BindByPosition_Null_BaseAddress_Throws()
    {
        UriTemplate template = new UriTemplate("a/{b}");

        Assert.Throws<ArgumentNullException>(() => template.BindByPosition(null, "1"));
    }

    [WcfFact]
    public static void Match_Null_Candidate_Throws()
    {
        UriTemplate template = new UriTemplate("a/{b}");

        Assert.Throws<ArgumentNullException>(() => template.Match(s_baseAddress, null));
    }

    [WcfFact]
    public static void Match_Null_BaseAddress_Throws()
    {
        UriTemplate template = new UriTemplate("a/{b}");

        Assert.Throws<ArgumentNullException>(() => template.Match(null, new Uri("http://localhost/svc/a/1")));
    }

    [WcfFact]
    public static void Match_Relative_Candidate_Returns_Null()
    {
        UriTemplate template = new UriTemplate("a/{b}");

        Assert.Null(template.Match(s_baseAddress, new Uri("/a/1", UriKind.Relative)));
    }

    [WcfFact]
    public static void Match_Binds_Path_Variables()
    {
        UriTemplate template = new UriTemplate("weather/{state}/{city}");

        UriTemplateMatch match = template.Match(s_baseAddress, new Uri("http://localhost/svc/weather/WA/Seattle"));

        Assert.NotNull(match);
        Assert.Equal("WA", match.BoundVariables["STATE"]);
        Assert.Equal("Seattle", match.BoundVariables["CITY"]);
        Assert.Equal(new[] { "weather", "WA", "Seattle" }, match.RelativePathSegments);
    }

    [WcfFact]
    public static void Match_Populates_BaseUri_RequestUri_And_Template()
    {
        UriTemplate template = new UriTemplate("a/{b}");
        Uri request = new Uri("http://localhost/svc/a/1");

        UriTemplateMatch match = template.Match(s_baseAddress, request);

        Assert.NotNull(match);
        Assert.Equal(s_baseAddress, match.BaseUri);
        Assert.Equal(request, match.RequestUri);
        Assert.Same(template, match.Template);
    }

    [WcfFact]
    public static void Match_Literals_Are_Case_Insensitive()
    {
        UriTemplate template = new UriTemplate("Weather/{state}");

        Assert.NotNull(template.Match(s_baseAddress, new Uri("http://localhost/svc/WEATHER/WA")));
        Assert.NotNull(template.Match(s_baseAddress, new Uri("http://localhost/svc/weather/WA")));
    }

    [WcfFact]
    public static void Match_Fails_When_Segment_Count_Differs()
    {
        UriTemplate template = new UriTemplate("a/{b}");

        Assert.Null(template.Match(s_baseAddress, new Uri("http://localhost/svc/a")));
        Assert.Null(template.Match(s_baseAddress, new Uri("http://localhost/svc/a/1/2")));
    }

    [WcfFact]
    public static void Match_Trailing_Slash_Fails_Unless_Ignored()
    {
        Assert.Null(new UriTemplate("a/{b}").Match(s_baseAddress, new Uri("http://localhost/svc/a/1/")));
        Assert.NotNull(new UriTemplate("a/{b}", true).Match(s_baseAddress, new Uri("http://localhost/svc/a/1/")));
    }

    [WcfFact]
    public static void Match_Default_Makes_Trailing_Segment_Optional_Only_With_Trailing_Slash()
    {
        UriTemplate template = new UriTemplate("weather/{state}/{city=Seattle}");

        // Without a trailing slash there is nothing to indicate the omitted segment.
        Assert.Null(template.Match(s_baseAddress, new Uri("http://localhost/svc/weather/WA")));

        UriTemplateMatch match = template.Match(s_baseAddress, new Uri("http://localhost/svc/weather/WA/"));
        Assert.NotNull(match);
        Assert.Equal("Seattle", match.BoundVariables["CITY"]);

        // ignoreTrailingSlash relaxes that requirement.
        UriTemplate ignoring = new UriTemplate("weather/{state}/{city=Seattle}", true);
        UriTemplateMatch ignoringMatch = ignoring.Match(s_baseAddress, new Uri("http://localhost/svc/weather/WA"));
        Assert.NotNull(ignoringMatch);
        Assert.Equal("Seattle", ignoringMatch.BoundVariables["CITY"]);
    }

    [WcfFact]
    public static void Match_Null_Default_Binds_Null()
    {
        UriTemplate template = new UriTemplate("a/{b=null}");

        UriTemplateMatch match = template.Match(s_baseAddress, new Uri("http://localhost/svc/a/"));

        Assert.NotNull(match);
        Assert.True(match.BoundVariables.AllKeys.Length == 1);
        Assert.Null(match.BoundVariables["B"]);
    }

    [WcfFact]
    public static void Match_Ignores_Host_And_Compares_Path_Relatively()
    {
        UriTemplate template = new UriTemplate("a/{b}");

        Assert.NotNull(template.Match(s_baseAddress, new Uri("http://otherhost/svc/a/1")));
    }

    [WcfFact]
    public static void Match_Decodes_Path_Variables()
    {
        UriTemplate template = new UriTemplate("a/{b}");

        Assert.Equal("x/y",
            template.Match(s_baseAddress, new Uri("http://localhost/svc/a/x%2Fy")).BoundVariables["B"]);
        Assert.Equal("\u00e9",
            template.Match(s_baseAddress, new Uri("http://localhost/svc/a/%C3%A9")).BoundVariables["B"]);
    }

    [WcfFact]
    public static void Match_Binds_Query_Variables()
    {
        UriTemplate template = new UriTemplate("a?q={v}");

        UriTemplateMatch match = template.Match(s_baseAddress, new Uri("http://localhost/svc/a?q=hello%20world"));

        Assert.NotNull(match);
        Assert.Equal("hello world", match.BoundVariables["V"]);
        Assert.Equal("hello world", match.QueryParameters["q"]);
    }

    [WcfFact]
    public static void Match_Query_Names_Are_Case_Insensitive()
    {
        UriTemplate template = new UriTemplate("a?X={v}");

        Assert.Equal("1", template.Match(s_baseAddress, new Uri("http://localhost/svc/a?x=1")).BoundVariables["V"]);
    }

    [WcfFact]
    public static void Match_Absent_Query_Variable_Binds_Null()
    {
        UriTemplate template = new UriTemplate("a?q={v}");

        UriTemplateMatch match = template.Match(s_baseAddress, new Uri("http://localhost/svc/a"));

        Assert.NotNull(match);
        Assert.Null(match.BoundVariables["V"]);
    }

    [WcfFact]
    public static void Match_Allows_Extra_Query_Parameters()
    {
        UriTemplate template = new UriTemplate("a?q={v}");

        UriTemplateMatch match = template.Match(s_baseAddress, new Uri("http://localhost/svc/a?q=1&extra=2"));

        Assert.NotNull(match);
        Assert.Equal("1", match.BoundVariables["V"]);
        Assert.Equal("2", match.QueryParameters["extra"]);
    }

    [WcfFact]
    public static void Match_Requires_Literal_Query_Values()
    {
        UriTemplate template = new UriTemplate("a?q=1");

        Assert.NotNull(template.Match(s_baseAddress, new Uri("http://localhost/svc/a?q=1")));
        Assert.Null(template.Match(s_baseAddress, new Uri("http://localhost/svc/a?q=2")));
    }

    [WcfFact]
    public static void Match_Anonymous_Wildcard_Fills_WildcardPathSegments()
    {
        UriTemplate template = new UriTemplate("a/*");

        UriTemplateMatch match = template.Match(s_baseAddress, new Uri("http://localhost/svc/a/b/c/d"));

        Assert.NotNull(match);
        Assert.Empty(match.BoundVariables);
        Assert.Equal(new[] { "b", "c", "d" }, match.WildcardPathSegments);
    }

    [WcfFact]
    public static void Match_Named_Wildcard_Binds_Remainder_And_Fills_WildcardPathSegments()
    {
        UriTemplate template = new UriTemplate("a/{*rest}");

        UriTemplateMatch match = template.Match(s_baseAddress, new Uri("http://localhost/svc/a/b/c/d"));

        Assert.NotNull(match);
        Assert.Equal("b/c/d", match.BoundVariables["REST"]);
        Assert.Equal(new[] { "b", "c", "d" }, match.WildcardPathSegments);
    }

    [WcfFact]
    public static void Match_WildcardPathSegments_Are_Decoded()
    {
        UriTemplate template = new UriTemplate("a/*");

        UriTemplateMatch match = template.Match(
            s_baseAddress,
            new Uri("http://localhost/svc/a/hello%20world/caf%C3%A9/x%2Fy"));

        Assert.NotNull(match);
        Assert.Equal(new[] { "hello world", "caf\u00e9", "x/y" }, match.WildcardPathSegments);
    }

    [WcfFact]
    public static void Match_Compound_Segment_Binds_Inner_Variable()
    {
        UriTemplate template = new UriTemplate("a/pre{b}post");

        UriTemplateMatch match = template.Match(s_baseAddress, new Uri("http://localhost/svc/a/preXYZpost"));

        Assert.NotNull(match);
        Assert.Equal("XYZ", match.BoundVariables["B"]);
        Assert.Null(template.Match(s_baseAddress, new Uri("http://localhost/svc/a/XYZ")));
    }

    [WcfFact]
    public static void Match_Empty_Template_Matches_Base_Address()
    {
        UriTemplateMatch match = new UriTemplate(string.Empty).Match(s_baseAddress, s_baseAddress);

        Assert.NotNull(match);
        Assert.Empty(match.RelativePathSegments);
    }

    [WcfFact]
    public static void Match_BaseAddress_Without_Trailing_Slash_Is_Normalized()
    {
        UriTemplate template = new UriTemplate("a/{b}");

        UriTemplateMatch match = template.Match(new Uri("http://localhost/svc"), new Uri("http://localhost/svc/a/1"));

        Assert.NotNull(match);
        Assert.Equal("1", match.BoundVariables["B"]);
    }

    [WcfFact]
    public static void IsEquivalentTo_Ignores_Variable_Names()
    {
        Assert.True(new UriTemplate("{a}/{b}").IsEquivalentTo(new UriTemplate("{x}/{y}")));
        Assert.True(new UriTemplate("a/{b}").IsEquivalentTo(new UriTemplate("a/{c}")));
    }

    [WcfFact]
    public static void IsEquivalentTo_Honors_Literals()
    {
        Assert.False(new UriTemplate("a/{b}").IsEquivalentTo(new UriTemplate("b/{c}")));
        Assert.False(new UriTemplate("a/{b}").IsEquivalentTo(new UriTemplate("a/b")));
    }

    [WcfFact]
    public static void IsEquivalentTo_Null_Returns_False()
    {
        Assert.False(new UriTemplate("a/{b}").IsEquivalentTo(null));
    }

}
