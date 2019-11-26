// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public static class HttpRequestMessagePropertyTest
{
    [WcfFact]
    public static void Default_Ctor_Initializes_Properties()
    {
        HttpRequestMessageProperty requestMsgProperty = new HttpRequestMessageProperty();

        Assert.NotNull(requestMsgProperty.Headers);
        Assert.Equal("POST", requestMsgProperty.Method);
        Assert.Equal(string.Empty, requestMsgProperty.QueryString);
        Assert.False(requestMsgProperty.SuppressEntityBody);
    }

    [WcfFact]
    public static void CreateCopy_Copies_Properties()
    {
        const string testKeyName = "testKey";

        HttpRequestMessageProperty original = new HttpRequestMessageProperty();

        // Set properties to other than default ctor to verify default ctor is not used
        original.QueryString = "name=Fred";
        original.Method = "GET";
        original.SuppressEntityBody = true;
        original.Headers[testKeyName] = "testValue";

        IMessageProperty copyMessageProperty = ((IMessageProperty)original).CreateCopy();
        Assert.IsType<HttpRequestMessageProperty>(copyMessageProperty);
        HttpRequestMessageProperty copy = (HttpRequestMessageProperty)copyMessageProperty;

        Assert.Equal(original.QueryString, copy.QueryString);
        Assert.Equal(original.Method, copy.Method);
        Assert.Equal<bool>(original.SuppressEntityBody, copy.SuppressEntityBody);
        Assert.Equal<int>(original.Headers.Count, copy.Headers.Count);
        Assert.Equal(original.Headers[testKeyName], copy.Headers[testKeyName]);
    }

    [WcfFact]
    public static void Name_Property()
    {
        Assert.Equal("httpRequest", HttpRequestMessageProperty.Name);
    }

    [WcfFact]
    public static void Method_Property_Sets()
    {
        const string newMethod = "PUT";
        HttpRequestMessageProperty requestMsgProperty = new HttpRequestMessageProperty();
        requestMsgProperty.Method = newMethod;
        Assert.Equal(newMethod, requestMsgProperty.Method);
    }

    [WcfFact]
    public static void Method_Property_Set_Null_Throws()
    {
        HttpRequestMessageProperty requestMsgProperty = new HttpRequestMessageProperty();
        Assert.Throws<ArgumentNullException>(() => requestMsgProperty.Method = null);
    }

    [WcfFact]
    public static void QueryString_Property_Sets()
    {
        const string newQueryString = "name=Mary";
        HttpRequestMessageProperty requestMsgProperty = new HttpRequestMessageProperty();
        requestMsgProperty.QueryString = newQueryString;
        Assert.Equal(newQueryString, requestMsgProperty.QueryString);
    }

    [WcfFact]
    public static void QueryString_Property_Set_Null_Throws()
    {
        HttpRequestMessageProperty requestMsgProperty = new HttpRequestMessageProperty();
        Assert.Throws<ArgumentNullException>(() => requestMsgProperty.QueryString = null);
    }
}
