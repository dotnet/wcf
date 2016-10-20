// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public static class MessageContractTest
{
    [WcfFact]
    public static void Default_Ctor_Initializes_Properties()
    {
        // Verify new MessageContractAttribute() initializes correct defaults.
        MessageContractAttribute messageCA = new MessageContractAttribute();

        Assert.True(messageCA.IsWrapped);

        // Assert.True for more informative message
        Assert.True(messageCA.WrapperName == null, "WrapperName should be null");
        Assert.True(messageCA.WrapperNamespace == null, "WrapperNamespace should be null");
    }

    [WcfTheory]
    [InlineData(true)]
    [InlineData(false)]
    public static void IsWrapped_Property_Sets(bool isWrapped)
    {
        MessageContractAttribute messageCA = new MessageContractAttribute();

        messageCA.IsWrapped = isWrapped;
        Assert.Equal(isWrapped, messageCA.IsWrapped);
    }

    [WcfTheory]
    [InlineData("testWrapperName")]
    public static void WrapperName_Property_Sets(string wrapperName)
    {
        MessageContractAttribute messageCA = new MessageContractAttribute();

        messageCA.WrapperName = wrapperName;
        Assert.Equal(wrapperName, messageCA.WrapperName);
    }

    [WcfTheory]
    [InlineData("")]
    public static void WrapperName_Property_Sets_Throws_Argument(string wrapperName)
    {
        MessageContractAttribute messageCA = new MessageContractAttribute();
        Assert.Throws<ArgumentOutOfRangeException>(() => messageCA.WrapperName = wrapperName);
    }

    [WcfTheory]
    [InlineData(new object[] { null } )]
    public static void WrapperName_Property_Sets_Throws_ArgumentNull(string wrapperName)
    {
        MessageContractAttribute messageCA = new MessageContractAttribute();
        Assert.Throws<ArgumentNullException>(() => messageCA.WrapperName = wrapperName);
    }

    [WcfTheory]
    [InlineData("http://www.contoso.com")]
    [InlineData("testNamespace")]
    [InlineData("")]
    [InlineData(new object[] { null } )]
    public static void WrapperNamespace_Property_Sets(string wrapperNamespace)
    {
        MessageContractAttribute messageCA = new MessageContractAttribute();

        messageCA.WrapperNamespace = wrapperNamespace;
        Assert.Equal(wrapperNamespace, messageCA.WrapperNamespace);
    }

    [WcfTheory]
    [InlineData(true)]
    [InlineData(false)]
    public static void MessageHeader_MustUnderStand_Sets(bool mustUnderstand)
    {
        MessageHeaderAttribute attribute = new MessageHeaderAttribute();
        attribute.MustUnderstand = mustUnderstand;
        Assert.Equal(mustUnderstand, attribute.MustUnderstand);
    }
}
