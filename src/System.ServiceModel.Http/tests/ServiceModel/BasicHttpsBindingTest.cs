// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Tests.Common;
using System.Text;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public class BasicHttpsBindingTest : ConditionalWcfTest
{
    [WcfFact]
    public static void Default_Ctor_Initializes_Properties()
    {
        var binding = new BasicHttpsBinding();
        Assert.Equal("BasicHttpsBinding", binding.Name);
        Assert.Equal("http://tempuri.org/", binding.Namespace);
        Assert.Equal("https", binding.Scheme);
        Assert.Equal<Encoding>(Encoding.GetEncoding("utf-8"), binding.TextEncoding);
        Assert.Equal(Encoding.GetEncoding("utf-8").WebName, binding.TextEncoding.WebName);
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

    [WcfFact]
    public static void Ctor_With_BasicHttpSecurityMode_Transport_Initializes_Properties()
    {
        var binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);

        Assert.Equal("BasicHttpsBinding", binding.Name);
        Assert.Equal("http://tempuri.org/", binding.Namespace);
        Assert.Equal("https", binding.Scheme);
        Assert.Equal<Encoding>(Encoding.GetEncoding("utf-8"), binding.TextEncoding);
        Assert.Equal(Encoding.GetEncoding("utf-8").WebName, binding.TextEncoding.WebName);
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

    [WcfFact]
    [Condition(nameof(FrameworkIsNetNative))]
    public static void DirectlySettingCertificateCredential()
    {
        var binding = new BasicHttpsBinding();
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
        var factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(FakeAddress.HttpsAddress));
        byte[] certificateBytes = Convert.FromBase64String(BasicHttpsBindingTest.AUserCertificate);
        var certificate = new X509Certificate2(certificateBytes);
        factory.Credentials.ClientCertificate.Certificate = certificate;
        var channel = factory.CreateChannel();
        Assert.Throws<PlatformNotSupportedException>(() => channel.Echo("hello"));
    }

    [WcfTheory]
    [InlineData(true)]
    [InlineData(false)]
    public static void AllowCookies_Property_Sets(bool value)
    {
        var binding = new BasicHttpsBinding();
        binding.AllowCookies = value;
        Assert.Equal<bool>(value, binding.AllowCookies);
    }

    [WcfTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public static void MaxBufferPoolSize_Property_Sets(long value)
    {
        var binding = new BasicHttpsBinding();
        binding.MaxBufferPoolSize = value;
        Assert.Equal<long>(value, binding.MaxBufferPoolSize);
    }

    [WcfTheory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public static void MaxBufferPoolSize_Property_Set_Invalid_Value_Throws(long value)
    {
        var binding = new BasicHttpsBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.MaxBufferPoolSize = value);
    }

    [WcfTheory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public static void MaxBufferSize_Property_Sets(int value)
    {
        var binding = new BasicHttpsBinding();
        binding.MaxBufferSize = value;
        Assert.Equal<long>(value, binding.MaxBufferSize);
    }

    [WcfTheory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public static void MaxBufferSize_Property_Set_Invalid_Value_Throws(int value)
    {
        var binding = new BasicHttpsBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.MaxBufferSize = value);
    }

    [WcfTheory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public static void MaxReceivedMessageSize_Property_Sets(long value)
    {
        var binding = new BasicHttpsBinding();
        binding.MaxReceivedMessageSize = value;
        Assert.Equal<long>(value, binding.MaxReceivedMessageSize);
    }

    [WcfTheory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public static void MaxReceivedMessageSize_Property_Set_Invalid_Value_Throws(int value)
    {
        var binding = new BasicHttpsBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.MaxReceivedMessageSize = value);
    }

    [WcfTheory]
    [InlineData("testName")]
    public static void Name_Property_Sets(string value)
    {
        var binding = new BasicHttpsBinding();
        binding.Name = value;
        Assert.Equal(value, binding.Name);
    }

    [WcfTheory]
    [InlineData(new object[] { null })]
    [InlineData("")]
    public static void Name_Property_Set_Invalid_Value_Throws(string value)
    {
        var binding = new BasicHttpsBinding();
        Assert.Throws<ArgumentException>(() => binding.Name = value);
    }

    [WcfTheory]
    [InlineData("")]
    [InlineData("http://hello")]
    [InlineData("testNamespace")]
    public static void Namespace_Property_Sets(string value)
    {
        var binding = new BasicHttpsBinding();
        binding.Namespace = value;
        Assert.Equal(value, binding.Namespace);
    }

    [WcfTheory]
    [InlineData(new object[] { null })]
    public static void Namespace_Property_Set_Invalid_Value_Throws(string value)
    {
        var binding = new BasicHttpsBinding();
        Assert.Throws<ArgumentNullException>(() => binding.Namespace = value);
    }

    [WcfFact]
    public static void ReaderQuotas_Property_Sets()
    {
        var binding = new BasicHttpsBinding();

        XmlDictionaryReaderQuotas maxQuota = XmlDictionaryReaderQuotas.Max;
        XmlDictionaryReaderQuotas defaultQuota = new XmlDictionaryReaderQuotas();

        Assert.True(TestHelpers.XmlDictionaryReaderQuotasAreEqual(binding.ReaderQuotas, defaultQuota));

        maxQuota.CopyTo(binding.ReaderQuotas);
        Assert.True(TestHelpers.XmlDictionaryReaderQuotasAreEqual(binding.ReaderQuotas, maxQuota), "Setting Max ReaderQuota failed");
    }

    [WcfFact]
    public static void ReaderQuotas_Property_Set_Null_Throws()
    {
        var binding = new BasicHttpsBinding();
        Assert.Throws<ArgumentNullException>(() => binding.ReaderQuotas = null);
    }

    [WcfTheory]
    [MemberData(nameof(TestData.ValidTimeOuts), MemberType = typeof(TestData))]
    public static void CloseTimeout_Property_Sets(TimeSpan timeSpan)
    {
        BasicHttpsBinding binding = new BasicHttpsBinding();
        binding.CloseTimeout = timeSpan;
        Assert.Equal<TimeSpan>(timeSpan, binding.CloseTimeout);
    }

    [WcfTheory]
    [MemberData(nameof(TestData.InvalidTimeOuts), MemberType = typeof(TestData))]
    public static void CloseTimeout_Property_Set_Invalid_Value_Throws(TimeSpan timeSpan)
    {
        BasicHttpsBinding binding = new BasicHttpsBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.CloseTimeout = timeSpan);
    }

    [WcfTheory]
    [MemberData(nameof(TestData.ValidTimeOuts), MemberType = typeof(TestData))]
    public static void OpenTimeout_Property_Sets(TimeSpan timeSpan)
    {
        BasicHttpsBinding binding = new BasicHttpsBinding();
        binding.OpenTimeout = timeSpan;
        Assert.Equal<TimeSpan>(timeSpan, binding.OpenTimeout);
    }

    [WcfTheory]
    [MemberData(nameof(TestData.InvalidTimeOuts), MemberType = typeof(TestData))]
    public static void OpenTimeout_Property_Set_Invalid_Value_Throws(TimeSpan timeSpan)
    {
        BasicHttpsBinding binding = new BasicHttpsBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.OpenTimeout = timeSpan);
    }

    [WcfTheory]
    [MemberData(nameof(TestData.ValidTimeOuts), MemberType = typeof(TestData))]
    public static void SendTimeout_Property_Sets(TimeSpan timeSpan)
    {
        BasicHttpsBinding binding = new BasicHttpsBinding();
        binding.SendTimeout = timeSpan;
        Assert.Equal<TimeSpan>(timeSpan, binding.SendTimeout);
    }

    [WcfTheory]
    [MemberData(nameof(TestData.InvalidTimeOuts), MemberType = typeof(TestData))]
    public static void SendTimeout_Property_Set_Invalid_Value_Throws(TimeSpan timeSpan)
    {
        BasicHttpsBinding binding = new BasicHttpsBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.SendTimeout = timeSpan);
    }

    [WcfTheory]
    [MemberData(nameof(TestData.ValidTimeOuts), MemberType = typeof(TestData))]
    public static void ReceiveTimeout_Property_Sets(TimeSpan timeSpan)
    {
        BasicHttpsBinding binding = new BasicHttpsBinding();
        binding.ReceiveTimeout = timeSpan;
        Assert.Equal<TimeSpan>(timeSpan, binding.ReceiveTimeout);
    }

    [WcfTheory]
    [MemberData(nameof(TestData.InvalidTimeOuts), MemberType = typeof(TestData))]
    public static void ReceiveTimeout_Property_Set_Invalid_Value_Throws(TimeSpan timeSpan)
    {
        BasicHttpsBinding binding = new BasicHttpsBinding();
        Assert.Throws<ArgumentOutOfRangeException>(() => binding.SendTimeout = timeSpan);
    }

    [WcfTheory]
    [MemberData(nameof(TestData.ValidEncodings), MemberType = typeof(TestData))]
    public static void TextEncoding_Property_Sets(Encoding encoding)
    {
        var binding = new BasicHttpsBinding();
        binding.TextEncoding = encoding;
        Assert.Equal<Encoding>(encoding, binding.TextEncoding);
    }

    [WcfTheory]
    [MemberData(nameof(TestData.InvalidEncodings), MemberType = typeof(TestData))]
    public static void TextEncoding_Property_Set_Invalid_Value_Throws(Encoding encoding)
    {
        var binding = new BasicHttpsBinding();
        Assert.Throws<ArgumentException>(() => binding.TextEncoding = encoding);
    }

    [WcfTheory]
    [InlineData(TransferMode.Buffered)]
    [InlineData(TransferMode.Streamed)]
    [InlineData(TransferMode.StreamedRequest)]
    [InlineData(TransferMode.StreamedResponse)]
    public static void TransferMode_Property_Sets(TransferMode transferMode)
    {
        var binding = new BasicHttpsBinding();
        binding.TransferMode = transferMode;
        Assert.Equal<TransferMode>(transferMode, binding.TransferMode);
    }

    // Dummy certificate for CN=A User, OU=UserAccounts, DC=corp, DC=contoso, DC=com
    public static string AUserCertificate = @"MIID1jCCAr6gAwIBAgIQFL5feogWTrVAD4sKd0pO8TANBgkqhkiG9w0BAQsFADBs
MRMwEQYKCZImiZPyLGQBGRYDY29tMRcwFQYKCZImiZPyLGQBGRYHY29udG9zbzEU
MBIGCgmSJomT8ixkARkWBGNvcnAxFTATBgNVBAsMDFVzZXJBY2NvdW50czEPMA0G
A1UEAwwGQSBVc2VyMB4XDTE3MDcwNzIyMjU1MVoXDTE4MDcwNzIyNDU1MVowbDET
MBEGCgmSJomT8ixkARkWA2NvbTEXMBUGCgmSJomT8ixkARkWB2NvbnRvc28xFDAS
BgoJkiaJk/IsZAEZFgRjb3JwMRUwEwYDVQQLDAxVc2VyQWNjb3VudHMxDzANBgNV
BAMMBkEgVXNlcjCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBALqhutWy
wt7iTJKW3ok1sJRrLVi5CTyszp+ClsjlQmyEJAT8qgY+20yg1ZazOg6yvpnmAC7K
hIaCbVNUvPed8XndwDgOUIVTW6J1uRr4Fjo5s3eh5/rqKjTLnn45vlgFWBI1R2xE
ywNnLNKW+2LlB5UAOZT3O6i3OCx4UQgPJN2PYMel1GFY74qGwDCICgRrWMQUFUCM
8CTr41Ai8uTco74Wih6VlHlp69jhimExrCuOlznMJbIioPxUydpcCcZb5oNU1WGM
MpBtplA1Mhm+j79nAj1XADVH9Unuyd0xqPpWCGRQ9/CjMz7uVCWB87PEj6tH+N/N
1Q7cH909onx7pmECAwEAAaN0MHIwDgYDVR0PAQH/BAQDAgWgMBMGA1UdJQQMMAoG
CCsGAQUFBwMCMCwGA1UdEQQlMCOgIQYKKwYBBAGCNxQCA6ATDBFhdXNlckBjb250
b3NvLmNvbTAdBgNVHQ4EFgQUb+Ejoslugks+rf0MQJrhpxVEb0AwDQYJKoZIhvcN
AQELBQADggEBAFdLXY381+PUpEYoSSYuGGkD7gojHNSBOy7DqW9PzaVf9myhSbtc
8GlHgX+bjTrknCGxe+1UP6+Ca/80N/nch1Fp78TI2BPuM086bypFRrESTO2kxu01
5P0yAVWfLzbOLyqZMnto7iMtxLoeNlT1XDLkZCi0p8yIwveNY4Lu6aFVM1eCCrCH
TCNUExh67YaLp5r4EHYbQnQGBeUHt9+Ak+6FMKUOKc5JHCkkKKyXW4lQ/IS3UKgR
nkudWiURY2U2yqYO3JblJr/vnArxXpIKz4nZ6ICuPf7z0PovZaTmfjORUdtajSLl
xi2noRHYcFaPJY/OyqWKxKvfGt+ysDoTMCI=";

    public static bool FrameworkIsNetNative()
    {
        return FrameworkHelper.Current == FrameworkID.NetNative;
    }
}
