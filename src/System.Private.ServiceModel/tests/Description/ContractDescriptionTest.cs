// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net.Security;
using System.ServiceModel.Description;
using System.Text;
using TestTypes;
using Xunit;

public static class ContractDescriptionTest
{
    [Fact]
    public static void Ctor_Taking_Name_Initializes()
    {
        const string expectedName = "testName";
        ContractDescription cd = new ContractDescription(expectedName);
        Assert.Equal<string>(expectedName, cd.Name);
        Assert.Equal<string>("http://tempuri.org/", cd.Namespace);
        Assert.Null(cd.ContractType);
        Assert.Null(cd.CallbackContractType);
        Assert.NotNull(cd.Operations);
        Assert.Equal<int>(0, cd.Operations.Count);
        Assert.NotNull(cd.Behaviors);
        Assert.Equal<int>(0, cd.Behaviors.Count);
        Assert.NotNull(cd.ContractBehaviors);
        Assert.Equal<int>(0, cd.ContractBehaviors.Count);
    }

    [Fact]
    public static void Ctor_Taking_Name_With_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ContractDescription(null));
    }

    [Fact]
    public static void Ctor_Taking_Name_With_Empty_String_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ContractDescription(string.Empty));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("testNamespace")]
    [InlineData("http://localhost")]
    public static void Ctor_Taking_Name_And_Namespace_Initializes(string nameSpace)
    {
        const string expectedName = "testName";
        ContractDescription cd = new ContractDescription(expectedName, nameSpace);
        Assert.Equal<string>(expectedName, cd.Name);
        string expectedNamespace = nameSpace == null ? "http://tempuri.org/" : nameSpace;
        Assert.Equal<string>(expectedNamespace, cd.Namespace);
        Assert.Null(cd.ContractType);
        Assert.Null(cd.CallbackContractType);
        Assert.NotNull(cd.Operations);
        Assert.Equal<int>(0, cd.Operations.Count);
        Assert.NotNull(cd.Behaviors);
        Assert.Equal<int>(0, cd.Behaviors.Count);
        Assert.NotNull(cd.ContractBehaviors);
        Assert.Equal<int>(0, cd.ContractBehaviors.Count);
    }

    [Theory]
    [InlineData(typeof(IWtfService))]
    [InlineData(null)]
    public static void ContractType_Property_Sets(Type contractType)
    {
        ContractDescription cd = new ContractDescription("test");
        cd.ContractType = contractType;
        Assert.Equal(contractType, cd.ContractType);
    }

    [Theory]
    [InlineData(typeof(IWtfService))]
    [InlineData(null)]
    public static void CallbackContractType_Property_Sets(Type contractType)
    {
        ContractDescription cd = new ContractDescription("test");
        cd.CallbackContractType = contractType;
        Assert.Equal<Type>(contractType, cd.CallbackContractType);
    }

    [Theory]
    [InlineData("name1")]
    public static void Name_Property_Sets(string name)
    {
        ContractDescription cd = new ContractDescription("test");
        cd.Name = name;
        Assert.Equal<string>(name, cd.Name);
    }

    [Fact]
    public static void Name_Property_With_Null_Throws()
    {
        ContractDescription cd = new ContractDescription("test");
        Assert.Throws<ArgumentNullException>(() => cd.Name = null);
    }

    [Fact]
    public static void Name_Property_With_Empty_String_Throws()
    {
        ContractDescription cd = new ContractDescription("test");
        Assert.Throws<ArgumentOutOfRangeException>(() => cd.Name = String.Empty);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("testNamespace")]
    [InlineData("http://localhost")]
    public static void Namespace_Property_Sets(string nameSpace)
    {
        ContractDescription cd = new ContractDescription("test");
        cd.Namespace = nameSpace;
        Assert.Equal<string>(nameSpace, cd.Namespace);
    }

    [Theory]
    [InlineData("http://invalid<>")]
    [InlineData("http://badencode%")]
    public static void Namespace_Property_Set_With_Invalid_Value_Throws(string nameSpace)
    {
        ContractDescription cd = new ContractDescription("test");
        Assert.Throws<ArgumentException>(() => cd.Namespace = nameSpace);
    }

    [Theory]
    [InlineData(ProtectionLevel.None)]
    [InlineData(ProtectionLevel.Sign)]
    [InlineData(ProtectionLevel.EncryptAndSign)]
    public static void ProtectionLevel_Property_Sets(ProtectionLevel level)
    {
        ContractDescription cd = new ContractDescription("test");
        cd.ProtectionLevel = level;
        Assert.Equal<ProtectionLevel>(level, cd.ProtectionLevel);
    }

    [Fact]
    public static void ProtectionLevel_Property_Set_With_Invalid_Value_Throws()
    {
        ContractDescription cd = new ContractDescription("test");
        Assert.Throws<ArgumentOutOfRangeException>(() => cd.ProtectionLevel = (ProtectionLevel)99);
    }
}
