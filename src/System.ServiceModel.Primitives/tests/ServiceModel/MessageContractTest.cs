// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using Xunit;

public static class MessageContractTest
{
    [Fact]
    public static void Default_Ctor_Initializes_Properties()
    {
        // Verify new MessageContractAttribute() initializes correct defaults.
        MessageContractAttribute messageCA = new MessageContractAttribute();

        Assert.True(messageCA.IsWrapped);

        // Assert.True for more informative message
        Assert.True(messageCA.WrapperName == null, "WrapperName should be null");
        Assert.True(messageCA.WrapperNamespace == null, "WrapperNamespace should be null");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public static void IsWrapped_Property_Sets(bool isWrapped)
    {
        MessageContractAttribute messageCA = new MessageContractAttribute();

        messageCA.IsWrapped = isWrapped;
        Assert.Equal(isWrapped, messageCA.IsWrapped);
    }

    [Theory]
    [InlineData("testWrapperName")]
    public static void WrapperName_Property_Sets(string wrapperName)
    {
        MessageContractAttribute messageCA = new MessageContractAttribute();

        messageCA.WrapperName = wrapperName;
        Assert.Equal(wrapperName, messageCA.WrapperName);
    }

    [Theory]
    [InlineData("")]
    public static void WrapperName_Property_Sets_Throws_Argument(string wrapperName)
    {
        MessageContractAttribute messageCA = new MessageContractAttribute();
        Assert.Throws<ArgumentOutOfRangeException>(() => messageCA.WrapperName = wrapperName);
    }

    [Theory]
    [InlineData(null)]
    public static void WrapperName_Property_Sets_Throws_ArgumentNull(string wrapperName)
    {
        MessageContractAttribute messageCA = new MessageContractAttribute();
        Assert.Throws<ArgumentNullException>(() => messageCA.WrapperName = wrapperName);
    }

    [Theory]
    [InlineData("http://www.contoso.com")]
    [InlineData("testNamespace")]
    [InlineData("")]
    [InlineData(null)]
    public static void WrapperNamespace_Property_Sets(string wrapperNamespace)
    {
        MessageContractAttribute messageCA = new MessageContractAttribute();

        messageCA.WrapperNamespace = wrapperNamespace;
        Assert.Equal(wrapperNamespace, messageCA.WrapperNamespace);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public static void MessageHeader_MustUnderStand_Sets(bool mustUnderstand)
    {
        MessageHeaderAttribute attribute = new MessageHeaderAttribute();
        attribute.MustUnderstand = mustUnderstand;
        Assert.Equal(mustUnderstand, attribute.MustUnderstand);
    }
}
