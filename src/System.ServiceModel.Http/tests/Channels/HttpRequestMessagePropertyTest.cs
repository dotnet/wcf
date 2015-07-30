// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel.Channels;
using Xunit;

public static class HttpRequestMessagePropertyTest
{
    [Fact]
    public static void Default_Ctor_Initializes_Properties()
    {
        HttpRequestMessageProperty requestMsgProperty = new HttpRequestMessageProperty();

        Assert.NotNull(requestMsgProperty.Headers);
        Assert.Equal<string>("POST", requestMsgProperty.Method);
        Assert.Equal<string>(string.Empty, requestMsgProperty.QueryString);
        Assert.False(requestMsgProperty.SuppressEntityBody);
    }

    [Fact]
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

        Assert.Equal<string>(original.QueryString, copy.QueryString);
        Assert.Equal<string>(original.Method, copy.Method);
        Assert.Equal<bool>(original.SuppressEntityBody, copy.SuppressEntityBody);
        Assert.Equal<int>(original.Headers.Count, copy.Headers.Count);
        Assert.Equal<string>(original.Headers[testKeyName], copy.Headers[testKeyName]);
    }

    [Fact]
    public static void Name_Property()
    {
        Assert.Equal<string>("httpRequest", HttpRequestMessageProperty.Name);
    }

    [Fact]
    public static void Method_Property_Sets()
    {
        const string newMethod = "PUT";
        HttpRequestMessageProperty requestMsgProperty = new HttpRequestMessageProperty();
        requestMsgProperty.Method = newMethod;
        Assert.Equal<string>(newMethod, requestMsgProperty.Method);
    }

    [Fact]
    public static void Method_Property_Set_Null_Throws()
    {
        HttpRequestMessageProperty requestMsgProperty = new HttpRequestMessageProperty();
        Assert.Throws<ArgumentNullException>(() => requestMsgProperty.Method = null);
    }

    [Fact]
    public static void QueryString_Property_Sets()
    {
        const string newQueryString = "name=Mary";
        HttpRequestMessageProperty requestMsgProperty = new HttpRequestMessageProperty();
        requestMsgProperty.QueryString = newQueryString;
        Assert.Equal<string>(newQueryString, requestMsgProperty.QueryString);
    }

    [Fact]
    public static void QueryString_Property_Set_Null_Throws()
    {
        HttpRequestMessageProperty requestMsgProperty = new HttpRequestMessageProperty();
        Assert.Throws<ArgumentNullException>(() => requestMsgProperty.QueryString = null);
    }
}
