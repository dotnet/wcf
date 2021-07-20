// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Description;
using Infrastructure.Common;
using Xunit;

public static class TypedMessageConverterTest
{
    [WcfFact]
    public static void Default_Create()
    {
        var typedMessageConverter = TypedMessageConverter.Create(typeof(TestMessage), "http://TestMessage/Action");
        Assert.NotNull(typedMessageConverter);
    }

    [System.ServiceModel.MessageContract]
    private class TestMessage
    {
    }
}
