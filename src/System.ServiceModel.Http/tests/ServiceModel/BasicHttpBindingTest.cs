// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Tests.Common;
using System.Text;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public static class BasicHttpBindingTest
{
#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#endif
    [WcfFact]
    public static void Default_Ctor_Initializes_Properties()
    {
        var binding = new BasicHttpBinding();
        Assert.Equal<string>("BasicHttpBinding", binding.Name);
        Assert.Equal<string>("http://tempuri.org/", binding.Namespace);
        Assert.Equal<string>("http", binding.Scheme);
        Assert.Equal<Encoding>(Encoding.GetEncoding("utf-8"), binding.TextEncoding);
        Assert.Equal<string>(Encoding.GetEncoding("utf-8").WebName, binding.TextEncoding.WebName);
        Assert.False(binding.AllowCookies);
        Assert.Equal<TimeSpan>(TimeSpan.FromMinutes(1), binding.CloseTimeout);
        Assert.Equal<TimeSpan>(TimeSpan.FromMinutes(1), binding.OpenTimeout);
        Assert.Equal<TimeSpan>(TimeSpan.FromMinutes(10), binding.ReceiveTimeout);
        Assert.Equal<TimeSpan>(TimeSpan.FromMinutes(1), binding.SendTimeout);
        Assert.Equal<EnvelopeVersion>(EnvelopeVersion.Soap11, binding.EnvelopeVersion);
        Assert.Equal<MessageVersion>(MessageVersion.Soap11, binding.MessageVersion);
        Assert.Equal<long>(524288, binding.MaxBufferPoolSize);
        Assert.Equal<long>(65536, binding.MaxBufferSize);
        Assert.Equal<long>(65536, binding.MaxReceivedMessageSize);
        Assert.Equal<TransferMode>(TransferMode.Buffered, binding.TransferMode);

        Assert.True(TestHelpers.XmlDictionaryReaderQuotasAreEqual(binding.ReaderQuotas, new XmlDictionaryReaderQuotas()), "XmlDictionaryReaderQuotas");
    }

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#endif
    [WcfFact]
    public static void Ctor_With_BasicHttpSecurityMode_Transport_Initializes_Properties()
    {
        var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);

        Assert.Equal<string>("BasicHttpBinding", binding.Name);
        Assert.Equal<string>("http://tempuri.org/", binding.Namespace);
        Assert.Equal<string>("https", binding.Scheme);
        Assert.Equal<Encoding>(Encoding.GetEncoding("utf-8"), binding.TextEncoding);
        Assert.Equal<string>(Encoding.GetEncoding("utf-8").WebName, binding.TextEncoding.WebName);
        Assert.False(binding.AllowCookies);
        Assert.Equal<TimeSpan>(TimeSpan.FromMinutes(1), binding.CloseTimeout);
        Assert.Equal<TimeSpan>(TimeSpan.FromMinutes(1), binding.OpenTimeout);
        Assert.Equal<TimeSpan>(TimeSpan.FromMinutes(10), binding.ReceiveTimeout);
        Assert.Equal<TimeSpan>(TimeSpan.FromMinutes(1), binding.SendTimeout);
        Assert.Equal<EnvelopeVersion>(EnvelopeVersion.Soap11, binding.EnvelopeVersion);
        Assert.Equal<MessageVersion>(MessageVersion.Soap11, binding.MessageVersion);
        Assert.Equal<long>(524288, binding.MaxBufferPoolSize);
        Assert.Equal<long>(65536, binding.MaxBufferSize);
        Assert.Equal<long>(65536, binding.MaxReceivedMessageSize);
        Assert.Equal<TransferMode>(TransferMode.Buffered, binding.TransferMode);

        Assert.True(TestHelpers.XmlDictionaryReaderQuotasAreEqual(binding.ReaderQuotas, new XmlDictionaryReaderQuotas()), "XmlDictionaryReaderQuotas");
    }

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#endif
    [WcfFact]
    public static void Ctor_With_BasicHttpSecurityMode_TransportCredentialOnly_Initializes_Properties()
    {
        var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
        Assert.Equal<string>("BasicHttpBinding", binding.Name);
        Assert.Equal<string>("http://tempuri.org/", binding.Namespace);
        Assert.Equal<string>("http", binding.Scheme);
        Assert.Equal<Encoding>(Encoding.GetEncoding("utf-8"), binding.TextEncoding);
        Assert.Equal<string>(Encoding.GetEncoding("utf-8").WebName, binding.TextEncoding.WebName);
        Assert.False(binding.AllowCookies);
        Assert.Equal<TimeSpan>(TimeSpan.FromMinutes(1), binding.CloseTimeout);
        Assert.Equal<TimeSpan>(TimeSpan.FromMinutes(1), binding.OpenTimeout);
        Assert.Equal<TimeSpan>(TimeSpan.FromMinutes(10), binding.ReceiveTimeout);
        Assert.Equal<TimeSpan>(TimeSpan.FromMinutes(1), binding.SendTimeout);
        Assert.Equal<EnvelopeVersion>(EnvelopeVersion.Soap11, binding.EnvelopeVersion);
        Assert.Equal<MessageVersion>(MessageVersion.Soap11, binding.MessageVersion);
        Assert.Equal<long>(524288, binding.MaxBufferPoolSize);
        Assert.Equal<long>(65536, binding.MaxBufferSize);
        Assert.Equal<long>(65536, binding.MaxReceivedMessageSize);
        Assert.Equal<TransferMode>(TransferMode.Buffered, binding.TransferMode);

        Assert.True(TestHelpers.XmlDictionaryReaderQuotasAreEqual(binding.ReaderQuotas, new XmlDictionaryReaderQuotas()), "XmlDictionaryReaderQuotas");
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [InlineData(true)]
    [InlineData(false)]
    public static void AllowCookies_Property_Sets(bool value)
    {
        var binding = new BasicHttpBinding();
        binding.AllowCookies = value;
        Assert.Equal<bool>(value, binding.AllowCookies);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public static void MaxBufferPoolSize_Property_Sets(long value)
    {
        var binding = new BasicHttpBinding();
        binding.MaxBufferPoolSize = value;
        Assert.Equal<long>(value, binding.MaxBufferPoolSize);
    }


#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public static void MaxBufferPoolSize_Property_Set_Invalid_Value_Throws(long value)
    {
        var binding = new BasicHttpBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.MaxBufferPoolSize = value);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public static void MaxBufferSize_Property_Sets(int value)
    {
        var binding = new BasicHttpBinding();
        binding.MaxBufferSize = value;
        Assert.Equal<long>(value, binding.MaxBufferSize);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public static void MaxBufferSize_Property_Set_Invalid_Value_Throws(int value)
    {
        var binding = new BasicHttpBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.MaxBufferSize = value);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public static void MaxReceivedMessageSize_Property_Sets(long value)
    {
        var binding = new BasicHttpBinding();
        binding.MaxReceivedMessageSize = value;
        Assert.Equal<long>(value, binding.MaxReceivedMessageSize);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public static void MaxReceivedMessageSize_Property_Set_Invalid_Value_Throws(int value)
    {
        var binding = new BasicHttpBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.MaxReceivedMessageSize = value);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [InlineData("testName")]
    public static void Name_Property_Sets(string value)
    {
        var binding = new BasicHttpBinding();
        binding.Name = value;
        Assert.Equal<string>(value, binding.Name);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [InlineData(new object[] { null } )]  // Work-around issue #1449 with this syntax
    [InlineData("")]
    public static void Name_Property_Set_Invalid_Value_Throws(string value)
    {
        var binding = new BasicHttpBinding();
        Assert.Throws<ArgumentException>(() => binding.Name = value);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [InlineData("")]
    [InlineData("http://hello")]
    [InlineData("testNamespace")]
    public static void Namespace_Property_Sets(string value)
    {
        var binding = new BasicHttpBinding();
        binding.Namespace = value;
        Assert.Equal<string>(value, binding.Namespace);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [InlineData(new object[] { null } )]  // Work-around issue #1449 with this syntax
    public static void Namespace_Property_Set_Invalid_Value_Throws(string value)
    {
        var binding = new BasicHttpBinding();
        Assert.Throws<ArgumentNullException>(() => binding.Namespace = value);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#endif
    [WcfFact]
    public static void ReaderQuotas_Property_Sets()
    {
        var binding = new BasicHttpBinding();

        XmlDictionaryReaderQuotas maxQuota = XmlDictionaryReaderQuotas.Max;
        XmlDictionaryReaderQuotas defaultQuota = new XmlDictionaryReaderQuotas();

        Assert.True(TestHelpers.XmlDictionaryReaderQuotasAreEqual(binding.ReaderQuotas, defaultQuota));

        maxQuota.CopyTo(binding.ReaderQuotas);
        Assert.True(TestHelpers.XmlDictionaryReaderQuotasAreEqual(binding.ReaderQuotas, maxQuota), "Setting Max ReaderQuota failed");
    }

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#endif
    [WcfFact]
    public static void ReaderQuotas_Property_Set_Null_Throws()
    {
        var binding = new BasicHttpBinding();
        Assert.Throws<ArgumentNullException>(() => binding.ReaderQuotas = null);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [MemberData("ValidTimeOuts", MemberType = typeof(TestData))]
    public static void CloseTimeout_Property_Sets(TimeSpan timeSpan)
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        binding.CloseTimeout = timeSpan;
        Assert.Equal<TimeSpan>(timeSpan, binding.CloseTimeout);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [MemberData("InvalidTimeOuts", MemberType = typeof(TestData))]
    public static void CloseTimeout_Property_Set_Invalid_Value_Throws(TimeSpan timeSpan)
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.CloseTimeout = timeSpan);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [MemberData("ValidTimeOuts", MemberType = typeof(TestData))]
    public static void OpenTimeout_Property_Sets(TimeSpan timeSpan)
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        binding.OpenTimeout = timeSpan;
        Assert.Equal<TimeSpan>(timeSpan, binding.OpenTimeout);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [MemberData("InvalidTimeOuts", MemberType = typeof(TestData))]
    public static void OpenTimeout_Property_Set_Invalid_Value_Throws(TimeSpan timeSpan)
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.OpenTimeout = timeSpan);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [MemberData("ValidTimeOuts", MemberType = typeof(TestData))]
    public static void SendTimeout_Property_Sets(TimeSpan timeSpan)
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        binding.SendTimeout = timeSpan;
        Assert.Equal<TimeSpan>(timeSpan, binding.SendTimeout);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [MemberData("InvalidTimeOuts", MemberType = typeof(TestData))]
    public static void SendTimeout_Property_Set_Invalid_Value_Throws(TimeSpan timeSpan)
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.SendTimeout = timeSpan);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [MemberData("ValidTimeOuts", MemberType = typeof(TestData))]
    public static void ReceiveTimeout_Property_Sets(TimeSpan timeSpan)
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        binding.ReceiveTimeout = timeSpan;
        Assert.Equal<TimeSpan>(timeSpan, binding.ReceiveTimeout);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [MemberData("InvalidTimeOuts", MemberType = typeof(TestData))]
    public static void ReceiveTimeout_Property_Set_Invalid_Value_Throws(TimeSpan timeSpan)
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.SendTimeout = timeSpan);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [MemberData("ValidEncodings", MemberType = typeof(TestData))]
    public static void TextEncoding_Property_Sets(Encoding encoding)
    {
        var binding = new BasicHttpBinding();
        binding.TextEncoding = encoding;
        Assert.Equal<Encoding>(encoding, binding.TextEncoding);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [MemberData("InvalidEncodings", MemberType = typeof(TestData))]
    public static void TextEncoding_Property_Set_Invalid_Value_Throws(Encoding encoding)
    {
        var binding = new BasicHttpBinding();
        Assert.Throws<ArgumentException>(() => binding.TextEncoding = encoding);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [InlineData(TransferMode.Buffered)]
    [InlineData(TransferMode.Streamed)]
    [InlineData(TransferMode.StreamedRequest)]
    [InlineData(TransferMode.StreamedResponse)]
    public static void TransferMode_Property_Sets(TransferMode transferMode)
    {
        var binding = new BasicHttpBinding();
        binding.TransferMode = transferMode;
        Assert.Equal<TransferMode>(transferMode, binding.TransferMode);
    }
}
