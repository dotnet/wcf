// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Reflection;
using Infrastructure.Common;
using Xunit;

public static class MsmqUriTest
{
    private static readonly Assembly s_msmqAsm = typeof(System.ServiceModel.NetMsmqBinding).Assembly;
    private static readonly Type s_msmqUriType = s_msmqAsm.GetType("System.ServiceModel.Channels.MsmqUri", throwOnError: true);

    private static object GetTranslator(string name)
    {
        return s_msmqUriType.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
    }

    private static string Translate(object translator, Uri uri)
    {
        return (string)translator.GetType().GetMethod("UriToFormatName").Invoke(translator, new object[] { uri });
    }

    private static string TranslateByScheme(Uri uri)
    {
        MethodInfo m = s_msmqUriType.GetMethod("UriToFormatNameByScheme", BindingFlags.NonPublic | BindingFlags.Static);
        return (string)m.Invoke(null, new object[] { uri });
    }

    [WcfTheory]
    [InlineData("net.msmq://localhost/private/q1", "DIRECT=OS:.\\private$\\q1")]
    [InlineData("net.msmq://localhost/q1", "DIRECT=OS:.\\q1")]
    [InlineData("net.msmq://machine01/private/q1", "DIRECT=OS:machine01\\private$\\q1")]
    [InlineData("net.msmq://machine01/q1", "DIRECT=OS:machine01\\q1")]
    [InlineData("net.msmq://10.0.0.1/q1", "DIRECT=TCP:10.0.0.1\\q1")]
    [InlineData("net.msmq://10.0.0.1/private/q1", "DIRECT=TCP:10.0.0.1\\private$\\q1")]
    public static void NetMsmqAddressTranslator_FormatName(string uriString, string expected)
    {
        object translator = GetTranslator("NetMsmqAddressTranslator");
        Assert.Equal(expected, Translate(translator, new Uri(uriString)));
    }

    [WcfFact]
    public static void NetMsmqAddressTranslator_Scheme()
    {
        object translator = GetTranslator("NetMsmqAddressTranslator");
        Assert.Equal("net.msmq", translator.GetType().GetProperty("Scheme").GetValue(translator));
    }

    [WcfFact]
    public static void NetMsmqAddressTranslator_RejectsWrongScheme()
    {
        object translator = GetTranslator("NetMsmqAddressTranslator");
        var ex = Assert.Throws<TargetInvocationException>(() => Translate(translator, new Uri("http://localhost/q1")));
        Assert.IsType<ArgumentException>(ex.InnerException);
    }

    [WcfFact]
    public static void NetMsmqAddressTranslator_RejectsLegacyPrivateDollar()
    {
        object translator = GetTranslator("NetMsmqAddressTranslator");
        var ex = Assert.Throws<TargetInvocationException>(() => Translate(translator, new Uri("net.msmq://localhost/private$/q1")));
        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }

    [WcfTheory]
    [InlineData("net.msmq://server:8080/q1", "DIRECT=http://server:8080/msmq/q1")]
    [InlineData("net.msmq://server/q1", "DIRECT=http://server/msmq/q1")]
    [InlineData("net.msmq://server/private/q1", "DIRECT=http://server/msmq/private$/q1")]
    public static void SrmpAddressTranslator_FormatName(string uriString, string expected)
    {
        object translator = GetTranslator("SrmpAddressTranslator");
        Assert.Equal(expected, Translate(translator, new Uri(uriString)));
    }

    [WcfTheory]
    [InlineData("net.msmq://server/q1", "DIRECT=https://server/msmq/q1")]
    public static void SrmpsAddressTranslator_FormatName(string uriString, string expected)
    {
        object translator = GetTranslator("SrmpsAddressTranslator");
        Assert.Equal(expected, Translate(translator, new Uri(uriString)));
    }

    [WcfTheory]
    [InlineData("msmq.formatname:DIRECT=OS:.\\private$\\q1", "DIRECT=OS:.\\private$\\q1")]
    [InlineData("msmq.formatname:DIRECT=TCP:10.0.0.1\\q1", "DIRECT=TCP:10.0.0.1\\q1")]
    public static void FormatNameAddressTranslator_FormatName(string uriString, string expected)
    {
        object translator = GetTranslator("FormatNameAddressTranslator");
        Assert.Equal(expected, Translate(translator, new Uri(uriString)));
    }

    [WcfFact]
    public static void UriToFormatNameByScheme_DispatchesOnScheme()
    {
        Assert.Equal("DIRECT=OS:.\\q1", TranslateByScheme(new Uri("net.msmq://localhost/q1")));
        Assert.Equal("DIRECT=OS:.\\q1", TranslateByScheme(new Uri("msmq.formatname:DIRECT=OS:.\\q1")));
    }

    [WcfFact]
    public static void UriToFormatNameByScheme_RejectsUnknownScheme()
    {
        var ex = Assert.Throws<TargetInvocationException>(() => TranslateByScheme(new Uri("ftp://localhost/q1")));
        Assert.IsType<ArgumentException>(ex.InnerException);
    }
}
