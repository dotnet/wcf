// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public static class WebBodyFormatMessagePropertyTest
{
    [WcfFact]
    public static void Name_Is_Stable_Contract()
    {
        Assert.Equal("WebBodyFormatMessageProperty", WebBodyFormatMessageProperty.Name);
    }

    [WcfFact]
    public static void Ctor_Stores_Format()
    {
        Assert.Equal(WebContentFormat.Xml, new WebBodyFormatMessageProperty(WebContentFormat.Xml).Format);
        Assert.Equal(WebContentFormat.Json, new WebBodyFormatMessageProperty(WebContentFormat.Json).Format);
        Assert.Equal(WebContentFormat.Raw, new WebBodyFormatMessageProperty(WebContentFormat.Raw).Format);
    }

    [WcfFact]
    public static void Ctor_Rejects_Default_Format()
    {
        Assert.Throws<ArgumentException>(() => new WebBodyFormatMessageProperty(WebContentFormat.Default));
    }

    [WcfFact]
    public static void CreateCopy_Returns_Same_Instance_Because_Property_Is_Immutable()
    {
        WebBodyFormatMessageProperty property = new WebBodyFormatMessageProperty(WebContentFormat.Json);

        Assert.Same(property, property.CreateCopy());
    }

    [WcfFact]
    public static void ToString_Includes_Format()
    {
        Assert.Equal("WebBodyFormatMessageProperty(Format=Json)", new WebBodyFormatMessageProperty(WebContentFormat.Json).ToString());
        Assert.Equal("WebBodyFormatMessageProperty(Format=Raw)", new WebBodyFormatMessageProperty(WebContentFormat.Raw).ToString());
        Assert.Equal("WebBodyFormatMessageProperty(Format=Xml)", new WebBodyFormatMessageProperty(WebContentFormat.Xml).ToString());
    }

    [WcfFact]
    public static void Property_RoundTrips_Through_Message_Properties()
    {
        using (Message message = Message.CreateMessage(MessageVersion.None, string.Empty))
        {
            message.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));

            Assert.True(message.Properties.TryGetValue(WebBodyFormatMessageProperty.Name, out object value));
            WebBodyFormatMessageProperty property = Assert.IsType<WebBodyFormatMessageProperty>(value);
            Assert.Equal(WebContentFormat.Raw, property.Format);
        }
    }
}
