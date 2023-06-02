// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public static class UnixDomainSocketBindingTest
{
    [WcfTheory]
    [InlineData(UnixDomainSocketSecurityMode.None)]
    [InlineData(UnixDomainSocketSecurityMode.Transport)]
    [InlineData(UnixDomainSocketSecurityMode.TransportCredentialOnly)]
    public static void Ctor_Default_Initializes_Properties(UnixDomainSocketSecurityMode securityMode)
    {
        var binding = new UnixDomainSocketBinding(securityMode);

        Assert.Equal<EnvelopeVersion>(EnvelopeVersion.Soap12, binding.EnvelopeVersion);
        Assert.Equal<long>(512 * 1024, binding.MaxBufferPoolSize);
        Assert.Equal<long>(65536, binding.MaxBufferSize);
        Assert.True(TestHelpers.XmlDictionaryReaderQuotasAreEqual(binding.ReaderQuotas, new XmlDictionaryReaderQuotas()), "XmlDictionaryReaderQuotas");
        Assert.Equal("net.uds", binding.Scheme);
        Assert.Equal<TransferMode>(TransferMode.Buffered, binding.TransferMode);
        Assert.Equal<UnixDomainSocketSecurityMode>(securityMode, binding.Security.Mode);
    }

    [WcfTheory]
    [InlineData(TransferMode.Buffered)]
    [InlineData(TransferMode.Streamed)]
    [InlineData(TransferMode.StreamedRequest)]
    [InlineData(TransferMode.StreamedResponse)]
    public static void TransferMode_Property_Sets(TransferMode mode)
    {
        UnixDomainSocketBinding binding = new UnixDomainSocketBinding();
        binding.TransferMode = mode;
        Assert.Equal<TransferMode>(mode, binding.TransferMode);
    }

    [WcfTheory]
    [InlineData(0)]
    [InlineData(Int64.MaxValue)]
    public static void MaxBufferPoolSize_Property_Sets(long value)
    {
        UnixDomainSocketBinding binding = new UnixDomainSocketBinding();
        binding.MaxBufferPoolSize = value;
        Assert.Equal<long>(value, binding.MaxBufferPoolSize);
    }

    [WcfTheory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public static void MaxBufferSize_Property_Sets(int value)
    {
        UnixDomainSocketBinding binding = new UnixDomainSocketBinding();
        binding.MaxBufferSize = value;
        Assert.Equal<int>(value, binding.MaxBufferSize);
    }

    [WcfTheory]
    [InlineData(0)]
    [InlineData(-1)]
    public static void MaxBufferSize_Property_Set_With_Invalid_Value_Throws(int value)
    {
        UnixDomainSocketBinding binding = new UnixDomainSocketBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.MaxBufferSize = value);
    }

    [WcfTheory]
    [InlineData(1)]
    [InlineData(Int64.MaxValue)]
    public static void MaxReceivedMessageSize_Property_Sets(long value)
    {
        UnixDomainSocketBinding binding = new UnixDomainSocketBinding();
        binding.MaxReceivedMessageSize = value;
        Assert.Equal<long>(value, binding.MaxReceivedMessageSize);
    }

    [WcfTheory]
    [InlineData(0)]
    [InlineData(-1)]
    public static void MaxReceivedMessageSize_Property_Set_Invalid_Value_Throws(long value)
    {
        UnixDomainSocketBinding binding = new UnixDomainSocketBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.MaxReceivedMessageSize = value);
    }

    [WcfFact]
    public static void Security_Property_Sets()
    {
        UnixDomainSocketBinding binding = new UnixDomainSocketBinding();
        UnixDomainSocketSecurity security = new UnixDomainSocketSecurity();
        binding.Security = security;
        Assert.Equal<UnixDomainSocketSecurity>(security, binding.Security);
    }

    [WcfFact]
    public static void Security_Property_Set_Null_Throws()
    {
        UnixDomainSocketBinding binding = new UnixDomainSocketBinding();
        Assert.Throws<ArgumentNullException>(() => binding.Security = null);
    }
}
