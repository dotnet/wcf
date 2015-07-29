// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.Xml;
using Xunit;

public static class NetTcpBindingTest
{
    [Theory]
    [InlineData(SecurityMode.None)]
    [InlineData(SecurityMode.Transport)]
    public static void Ctor_Default_Initializes_Properties(SecurityMode securityMode)
    {
        var binding = new NetTcpBinding(securityMode);

        Assert.Equal<EnvelopeVersion>(EnvelopeVersion.Soap12, binding.EnvelopeVersion);
        Assert.Equal<long>(512 * 1024, binding.MaxBufferPoolSize);
        Assert.Equal<long>(65536, binding.MaxBufferSize);
        Assert.True(TestHelpers.XmlDictionaryReaderQuotasAreEqual(binding.ReaderQuotas, new XmlDictionaryReaderQuotas()), "XmlDictionaryReaderQuotas");
        Assert.Equal<string>("net.tcp", binding.Scheme);
        Assert.Equal<TransferMode>(TransferMode.Buffered, binding.TransferMode);
        Assert.Equal<SecurityMode>(securityMode, binding.Security.Mode);
    }

    [Theory]
    [InlineData(TransferMode.Buffered)]
    [InlineData(TransferMode.Streamed)]
    [InlineData(TransferMode.StreamedRequest)]
    [InlineData(TransferMode.StreamedResponse)]
    public static void TransferMode_Property_Sets(TransferMode mode)
    {
        NetTcpBinding binding = new NetTcpBinding();
        binding.TransferMode = mode;
        Assert.Equal<TransferMode>(mode, binding.TransferMode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(Int64.MaxValue)]
    public static void MaxBufferPoolSize_Property_Sets(long value)
    {
        NetTcpBinding binding = new NetTcpBinding();
        binding.MaxBufferPoolSize = value;
        Assert.Equal<long>(value, binding.MaxBufferPoolSize);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public static void MaxBufferSize_Property_Sets(int value)
    {
        NetTcpBinding binding = new NetTcpBinding();
        binding.MaxBufferSize = value;
        Assert.Equal<int>(value, binding.MaxBufferSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public static void MaxBufferSize_Property_Set_With_Invalid_Value_Throws(int value)
    {
        NetTcpBinding binding = new NetTcpBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.MaxBufferSize = value);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(Int64.MaxValue)]
    public static void MaxReceivedMessageSize_Property_Sets(long value)
    {
        NetTcpBinding binding = new NetTcpBinding();
        binding.MaxReceivedMessageSize = value;
        Assert.Equal<long>(value, binding.MaxReceivedMessageSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public static void MaxReceivedMessageSize_Property_Set_Invalid_Value_Throws(long value)
    {
        NetTcpBinding binding = new NetTcpBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.MaxReceivedMessageSize = value);
    }

    [Fact]
    public static void Security_Property_Sets()
    {
        NetTcpBinding binding = new NetTcpBinding();
        NetTcpSecurity security = new NetTcpSecurity();
        binding.Security = security;
        Assert.Equal<NetTcpSecurity>(security, binding.Security);
    }

    [Fact]
    public static void Security_Property_Set_Null_Throws()
    {
        NetTcpBinding binding = new NetTcpBinding();
        Assert.Throws<ArgumentNullException>(() => binding.Security = null);
    }
}
