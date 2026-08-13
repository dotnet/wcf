// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using Infrastructure.Common;
using Xunit;

public static class WebAttributeTest
{

    [WcfFact]
    public static void WebGet_Defaults_Are_Unset()
    {
        WebGetAttribute attribute = new WebGetAttribute();

        Assert.Null(attribute.UriTemplate);
        Assert.Equal(WebMessageBodyStyle.Bare, attribute.BodyStyle);
        Assert.Equal(WebMessageFormat.Xml, attribute.RequestFormat);
        Assert.Equal(WebMessageFormat.Xml, attribute.ResponseFormat);
        Assert.False(attribute.IsBodyStyleSetExplicitly);
        Assert.False(attribute.IsRequestFormatSetExplicitly);
        Assert.False(attribute.IsResponseFormatSetExplicitly);
    }

    [WcfFact]
    public static void WebGet_Setting_A_Property_Flips_Only_Its_Explicit_Flag()
    {
        WebGetAttribute attribute = new WebGetAttribute { BodyStyle = WebMessageBodyStyle.Wrapped };

        Assert.True(attribute.IsBodyStyleSetExplicitly);
        Assert.False(attribute.IsRequestFormatSetExplicitly);
        Assert.False(attribute.IsResponseFormatSetExplicitly);
    }

    [WcfFact]
    public static void WebGet_Explicit_Flags_Are_Set_Even_When_Assigned_The_Default_Value()
    {
        WebGetAttribute attribute = new WebGetAttribute
        {
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml
        };

        Assert.True(attribute.IsBodyStyleSetExplicitly);
        Assert.True(attribute.IsRequestFormatSetExplicitly);
        Assert.True(attribute.IsResponseFormatSetExplicitly);
    }

    [WcfFact]
    public static void WebGet_Properties_RoundTrip()
    {
        WebGetAttribute attribute = new WebGetAttribute
        {
            UriTemplate = "items/{id}",
            BodyStyle = WebMessageBodyStyle.WrappedResponse,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json
        };

        Assert.Equal("items/{id}", attribute.UriTemplate);
        Assert.Equal(WebMessageBodyStyle.WrappedResponse, attribute.BodyStyle);
        Assert.Equal(WebMessageFormat.Json, attribute.RequestFormat);
        Assert.Equal(WebMessageFormat.Json, attribute.ResponseFormat);
    }

    [WcfFact]
    public static void WebGet_Rejects_Undefined_Enum_Values()
    {
        WebGetAttribute attribute = new WebGetAttribute();

        Assert.Throws<ArgumentOutOfRangeException>(() => attribute.BodyStyle = (WebMessageBodyStyle)99);
        Assert.Throws<ArgumentOutOfRangeException>(() => attribute.RequestFormat = (WebMessageFormat)99);
        Assert.Throws<ArgumentOutOfRangeException>(() => attribute.ResponseFormat = (WebMessageFormat)99);
    }

    [WcfFact]
    public static void WebGet_Provides_An_OperationContractAttribute()
    {
        Assert.NotNull(InvokeGetOperationContractAttribute(new WebGetAttribute()));
    }

    [WcfFact]
    public static void WebGet_Operation_Behavior_Methods_Are_No_Ops()
    {
        IOperationBehavior behavior = new WebGetAttribute();

        behavior.AddBindingParameters(null, null);
        behavior.ApplyClientBehavior(null, null);
        behavior.ApplyDispatchBehavior(null, null);
        behavior.Validate(null);
    }

    [WcfFact]
    public static void WebGet_Is_Method_Only_And_Not_Inherited_Multiple_Times()
    {
        AttributeUsageAttribute usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(
            typeof(WebGetAttribute), typeof(AttributeUsageAttribute));

        Assert.NotNull(usage);
        Assert.Equal(AttributeTargets.Method, usage.ValidOn);
        Assert.False(usage.AllowMultiple);
    }

    [WcfFact]
    public static void WebInvoke_Defaults_Are_Unset()
    {
        WebInvokeAttribute attribute = new WebInvokeAttribute();

        Assert.Null(attribute.Method);
        Assert.Null(attribute.UriTemplate);
        Assert.Equal(WebMessageBodyStyle.Bare, attribute.BodyStyle);
        Assert.Equal(WebMessageFormat.Xml, attribute.RequestFormat);
        Assert.Equal(WebMessageFormat.Xml, attribute.ResponseFormat);
        Assert.False(attribute.IsBodyStyleSetExplicitly);
        Assert.False(attribute.IsRequestFormatSetExplicitly);
        Assert.False(attribute.IsResponseFormatSetExplicitly);
    }

    [WcfFact]
    public static void WebInvoke_Properties_RoundTrip()
    {
        WebInvokeAttribute attribute = new WebInvokeAttribute
        {
            Method = "PUT",
            UriTemplate = "items/{id}",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Xml
        };

        Assert.Equal("PUT", attribute.Method);
        Assert.Equal("items/{id}", attribute.UriTemplate);
        Assert.Equal(WebMessageBodyStyle.WrappedRequest, attribute.BodyStyle);
        Assert.Equal(WebMessageFormat.Json, attribute.RequestFormat);
        Assert.Equal(WebMessageFormat.Xml, attribute.ResponseFormat);
        Assert.True(attribute.IsBodyStyleSetExplicitly);
        Assert.True(attribute.IsRequestFormatSetExplicitly);
        Assert.True(attribute.IsResponseFormatSetExplicitly);
    }

    [WcfFact]
    public static void WebInvoke_Rejects_Undefined_Enum_Values()
    {
        WebInvokeAttribute attribute = new WebInvokeAttribute();

        Assert.Throws<ArgumentOutOfRangeException>(() => attribute.BodyStyle = (WebMessageBodyStyle)99);
        Assert.Throws<ArgumentOutOfRangeException>(() => attribute.RequestFormat = (WebMessageFormat)(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => attribute.ResponseFormat = (WebMessageFormat)(-1));
    }

    [WcfFact]
    public static void WebInvoke_Provides_An_OperationContractAttribute()
    {
        Assert.NotNull(InvokeGetOperationContractAttribute(new WebInvokeAttribute()));
    }

    [WcfFact]
    public static void WebInvoke_Operation_Behavior_Methods_Are_No_Ops()
    {
        IOperationBehavior behavior = new WebInvokeAttribute();

        behavior.AddBindingParameters(null, null);
        behavior.ApplyClientBehavior(null, null);
        behavior.ApplyDispatchBehavior(null, null);
        behavior.Validate(null);
    }

    [WcfFact]
    public static void WebInvoke_Is_Method_Only_And_Not_Inherited_Multiple_Times()
    {
        AttributeUsageAttribute usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(
            typeof(WebInvokeAttribute), typeof(AttributeUsageAttribute));

        Assert.NotNull(usage);
        Assert.Equal(AttributeTargets.Method, usage.ValidOn);
        Assert.False(usage.AllowMultiple);
    }

    // IOperationContractAttributeProvider is internal to System.ServiceModel.Primitives, so the
    // explicit interface implementation can only be reached reflectively from this assembly.
    private static object InvokeGetOperationContractAttribute(object attribute)
    {
        Type providerType = Array.Find(
            attribute.GetType().GetInterfaces(),
            i => i.Name == "IOperationContractAttributeProvider");

        Assert.NotNull(providerType);

        InterfaceMapping mapping = attribute.GetType().GetInterfaceMap(providerType);
        int index = Array.FindIndex(mapping.InterfaceMethods, m => m.Name == "GetOperationContractAttribute");

        Assert.True(index >= 0, "GetOperationContractAttribute was not found on the interface");

        return mapping.TargetMethods[index].Invoke(attribute, null);
    }
}
