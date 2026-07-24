// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.MsmqIntegration;
using Infrastructure.Common;
using Xunit;

public static class MsmqLeafTypesTest
{
    [WcfTheory]
    [InlineData(MsmqAuthenticationMode.None, true)]
    [InlineData(MsmqAuthenticationMode.WindowsDomain, true)]
    [InlineData(MsmqAuthenticationMode.Certificate, true)]
    [InlineData((MsmqAuthenticationMode)42, false)]
    public static void MsmqAuthenticationModeHelper_IsDefined(MsmqAuthenticationMode mode, bool expected)
    {
        Type t = typeof(MsmqAuthenticationMode).Assembly.GetType("System.ServiceModel.MsmqAuthenticationModeHelper", throwOnError: true);
        bool actual = (bool)t.GetMethod("IsDefined").Invoke(null, new object[] { mode });
        Assert.Equal(expected, actual);
    }

    [WcfTheory]
    [InlineData(MsmqEncryptionAlgorithm.RC4Stream, true)]
    [InlineData(MsmqEncryptionAlgorithm.Aes, true)]
    [InlineData((MsmqEncryptionAlgorithm)42, false)]
    public static void MsmqEncryptionAlgorithmHelper_IsDefined(MsmqEncryptionAlgorithm value, bool expected)
    {
        Type t = typeof(MsmqEncryptionAlgorithm).Assembly.GetType("System.ServiceModel.MsmqEncryptionAlgorithmHelper", throwOnError: true);
        bool actual = (bool)t.GetMethod("IsDefined").Invoke(null, new object[] { value });
        Assert.Equal(expected, actual);
    }

    [WcfTheory]
    [InlineData(MsmqSecureHashAlgorithm.MD5, true)]
    [InlineData(MsmqSecureHashAlgorithm.Sha1, true)]
    [InlineData(MsmqSecureHashAlgorithm.Sha256, true)]
    [InlineData(MsmqSecureHashAlgorithm.Sha512, true)]
    [InlineData((MsmqSecureHashAlgorithm)42, false)]
    public static void MsmqSecureHashAlgorithmHelper_IsDefined(MsmqSecureHashAlgorithm value, bool expected)
    {
        Type t = typeof(MsmqSecureHashAlgorithm).Assembly.GetType("System.ServiceModel.MsmqSecureHashAlgorithmHelper", throwOnError: true);
        bool actual = (bool)t.GetMethod("IsDefined").Invoke(null, new object[] { value });
        Assert.Equal(expected, actual);
    }

    [WcfTheory]
    [InlineData(NetMsmqSecurityMode.None, true)]
    [InlineData(NetMsmqSecurityMode.Transport, true)]
    [InlineData(NetMsmqSecurityMode.Message, true)]
    [InlineData(NetMsmqSecurityMode.Both, true)]
    [InlineData((NetMsmqSecurityMode)42, false)]
    public static void NetMsmqSecurityModeHelper_IsDefined(NetMsmqSecurityMode value, bool expected)
    {
        Type t = typeof(NetMsmqSecurityMode).Assembly.GetType("System.ServiceModel.NetMsmqSecurityModeHelper", throwOnError: true);
        bool actual = (bool)t.GetMethod("IsDefined", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(null, new object[] { value });
        Assert.Equal(expected, actual);
    }

    [WcfTheory]
    [InlineData(MsmqIntegrationSecurityMode.None, true)]
    [InlineData(MsmqIntegrationSecurityMode.Transport, true)]
    [InlineData((MsmqIntegrationSecurityMode)42, false)]
    public static void MsmqIntegrationSecurityModeHelper_IsDefined(MsmqIntegrationSecurityMode value, bool expected)
    {
        Type t = typeof(MsmqIntegrationSecurityMode).Assembly.GetType("System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityModeHelper", throwOnError: true);
        bool actual = (bool)t.GetMethod("IsDefined", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(null, new object[] { value });
        Assert.Equal(expected, actual);
    }

    [WcfTheory]
    [InlineData(MsmqMessageSerializationFormat.Xml, true)]
    [InlineData(MsmqMessageSerializationFormat.Binary, true)]
    [InlineData(MsmqMessageSerializationFormat.ActiveX, true)]
    [InlineData(MsmqMessageSerializationFormat.ByteArray, true)]
    [InlineData(MsmqMessageSerializationFormat.Stream, true)]
    [InlineData((MsmqMessageSerializationFormat)42, false)]
    public static void MsmqMessageSerializationFormatHelper_IsDefined(MsmqMessageSerializationFormat value, bool expected)
    {
        Type t = typeof(MsmqMessageSerializationFormat).Assembly.GetType("System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormatHelper", throwOnError: true);
        bool actual = (bool)t.GetMethod("IsDefined", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(null, new object[] { value });
        Assert.Equal(expected, actual);
    }

    [WcfFact]
    public static void PoisonMessageException_IsCommunicationException()
    {
        var ex = new PoisonMessageException("msg");
        Assert.IsAssignableFrom<CommunicationException>(ex);
        Assert.Equal("msg", ex.Message);
    }

    [WcfFact]
    public static void MsmqPoisonMessageException_DefaultLookupId_IsZero()
    {
        var ex = new MsmqPoisonMessageException();
        Assert.Equal(0L, ex.MessageLookupId);
    }

    [WcfFact]
    public static void MsmqPoisonMessageException_LookupIdCtor_RoundTrips()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new MsmqPoisonMessageException(123456789L, inner);
        Assert.Equal(123456789L, ex.MessageLookupId);
        Assert.Same(inner, ex.InnerException);
        Assert.IsAssignableFrom<PoisonMessageException>(ex);
    }
}
