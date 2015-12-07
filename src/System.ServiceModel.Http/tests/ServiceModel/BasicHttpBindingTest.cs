// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Tests.Common;
using System.Text;
using System.Xml;
using Xunit;

public static class BasicHttpBindingTest
{
    [Fact]
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

    [Fact]
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

    [Fact]
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

    [Fact]
    public static void Ctor_With_BasicHttpSecurityMode_TransportWithMessageCredential_Initializes_Properties()
    {
        var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public static void AllowCookies_Property_Sets(bool value)
    {
        var binding = new BasicHttpBinding();
        binding.AllowCookies = value;
        Assert.Equal<bool>(value, binding.AllowCookies);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public static void MaxBufferPoolSize_Property_Sets(long value)
    {
        var binding = new BasicHttpBinding();
        binding.MaxBufferPoolSize = value;
        Assert.Equal<long>(value, binding.MaxBufferPoolSize);
    }


    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public static void MaxBufferPoolSize_Property_Set_Invalid_Value_Throws(long value)
    {
        var binding = new BasicHttpBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.MaxBufferPoolSize = value);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public static void MaxBufferSize_Property_Sets(int value)
    {
        var binding = new BasicHttpBinding();
        binding.MaxBufferSize = value;
        Assert.Equal<long>(value, binding.MaxBufferSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public static void MaxBufferSize_Property_Set_Invalid_Value_Throws(int value)
    {
        var binding = new BasicHttpBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.MaxBufferSize = value);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public static void MaxReceivedMessageSize_Property_Sets(long value)
    {
        var binding = new BasicHttpBinding();
        binding.MaxReceivedMessageSize = value;
        Assert.Equal<long>(value, binding.MaxReceivedMessageSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public static void MaxReceivedMessageSize_Property_Set_Invalid_Value_Throws(int value)
    {
        var binding = new BasicHttpBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.MaxReceivedMessageSize = value);
    }

    [Theory]
    [InlineData("testName")]
    public static void Name_Property_Sets(string value)
    {
        var binding = new BasicHttpBinding();
        binding.Name = value;
        Assert.Equal<string>(value, binding.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public static void Name_Property_Set_Invalid_Value_Throws(string value)
    {
        var binding = new BasicHttpBinding();
        Assert.Throws<ArgumentException>(() => binding.Name = value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("http://hello")]
    [InlineData("testNamespace")]
    public static void Namespace_Property_Sets(string value)
    {
        var binding = new BasicHttpBinding();
        binding.Namespace = value;
        Assert.Equal<string>(value, binding.Namespace);
    }

    [Theory]
    [InlineData(null)]
    public static void Namespace_Property_Set_Invalid_Value_Throws(string value)
    {
        var binding = new BasicHttpBinding();
        Assert.Throws<ArgumentNullException>(() => binding.Namespace = value);
    }
    public static MemberDataSet<Encoding> ValidEncodings
    {
        get
        {
            return new MemberDataSet<Encoding>
                {
                    { Encoding.BigEndianUnicode },
                    { Encoding.Unicode },
                    { Encoding.UTF8 },
                };
        }
    }

    public static MemberDataSet<Encoding> InvalidEncodings
    {
        get
        {
            MemberDataSet<Encoding> data = new MemberDataSet<Encoding>();
            foreach (string encodingName in new string[] { "utf-7", "Windows-1252", "us-ascii", "iso-8859-1", "x-Chinese-CNS", "IBM273" })
            {
                try
                {
                    Encoding encoding = Encoding.GetEncoding(encodingName);
                    data.Add(encoding);
                }
                catch
                {
                    // not all encodings are supported on all frameworks
                }
            }
            return data;
        }
    }

    [Fact]
    public static void ReaderQuotas_Property_Sets()
    {
        var binding = new BasicHttpBinding();

        XmlDictionaryReaderQuotas maxQuota = XmlDictionaryReaderQuotas.Max;
        XmlDictionaryReaderQuotas defaultQuota = new XmlDictionaryReaderQuotas();

        Assert.True(TestHelpers.XmlDictionaryReaderQuotasAreEqual(binding.ReaderQuotas, defaultQuota));

        binding.ReaderQuotas = maxQuota;
        Assert.True(TestHelpers.XmlDictionaryReaderQuotasAreEqual(binding.ReaderQuotas, maxQuota), "Setting Max ReaderQuota failed");
    }

    [Fact]
    public static void ReaderQuotas_Property_Set_Null_Throws()
    {
        var binding = new BasicHttpBinding();
        Assert.Throws<ArgumentNullException>(() => binding.ReaderQuotas = null);
    }

    [Theory]
    [MemberData("ValidTimeOuts", MemberType = typeof(TestData))]
    public static void CloseTimeout_Property_Sets(TimeSpan timeSpan)
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        binding.CloseTimeout = timeSpan;
        Assert.Equal<TimeSpan>(timeSpan, binding.CloseTimeout);
    }

    [Theory]
    [MemberData("InvalidTimeOuts", MemberType = typeof(TestData))]
    public static void CloseTimeout_Property_Set_Invalid_Value_Throws(TimeSpan timeSpan)
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.CloseTimeout = timeSpan);
    }

    [Theory]
    [MemberData("ValidTimeOuts", MemberType = typeof(TestData))]
    public static void OpenTimeout_Property_Sets(TimeSpan timeSpan)
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        binding.OpenTimeout = timeSpan;
        Assert.Equal<TimeSpan>(timeSpan, binding.OpenTimeout);
    }

    [Theory]
    [MemberData("InvalidTimeOuts", MemberType = typeof(TestData))]
    public static void OpenTimeout_Property_Set_Invalid_Value_Throws(TimeSpan timeSpan)
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.OpenTimeout = timeSpan);
    }

    [Theory]
    [MemberData("ValidTimeOuts", MemberType = typeof(TestData))]
    public static void SendTimeout_Property_Sets(TimeSpan timeSpan)
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        binding.SendTimeout = timeSpan;
        Assert.Equal<TimeSpan>(timeSpan, binding.SendTimeout);
    }

    [Theory]
    [MemberData("InvalidTimeOuts", MemberType = typeof(TestData))]
    public static void SendTimeout_Property_Set_Invalid_Value_Throws(TimeSpan timeSpan)
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.SendTimeout = timeSpan);
    }

    [Theory]
    [MemberData("ValidTimeOuts", MemberType = typeof(TestData))]
    public static void ReceiveTimeout_Property_Sets(TimeSpan timeSpan)
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        binding.ReceiveTimeout = timeSpan;
        Assert.Equal<TimeSpan>(timeSpan, binding.ReceiveTimeout);
    }

    [Theory]
    [MemberData("InvalidTimeOuts", MemberType = typeof(TestData))]
    public static void ReceiveTimeout_Property_Set_Invalid_Value_Throws(TimeSpan timeSpan)
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.SendTimeout = timeSpan);
    }

    [Theory]
    [MemberData("ValidEncodings", MemberType = typeof(TestData))]
    public static void TextEncoding_Property_Sets(Encoding encoding)
    {
        var binding = new BasicHttpBinding();
        binding.TextEncoding = encoding;
        Assert.Equal<Encoding>(encoding, binding.TextEncoding);
    }

    [Theory]
    [MemberData("InvalidEncodings", MemberType = typeof(TestData))]
    public static void TextEncoding_Property_Set_Invalid_Value_Throws(Encoding encoding)
    {
        var binding = new BasicHttpBinding();
        Assert.Throws<ArgumentException>(() => binding.TextEncoding = encoding);
    }

    [Theory]
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
