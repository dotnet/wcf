// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Security;
using Infrastructure.Common;
using Xunit;

public static class MsmqSecurityTypesTest
{
    [WcfFact]
    public static void MsmqTransportSecurity_Defaults_MatchNetFx()
    {
        var s = new MsmqTransportSecurity();
        Assert.Equal(MsmqAuthenticationMode.WindowsDomain, s.MsmqAuthenticationMode);
        Assert.Equal(MsmqEncryptionAlgorithm.RC4Stream, s.MsmqEncryptionAlgorithm);
        Assert.Equal(MsmqSecureHashAlgorithm.Sha256, s.MsmqSecureHashAlgorithm);
        Assert.Equal(ProtectionLevel.Sign, s.MsmqProtectionLevel);
    }

    [WcfFact]
    public static void MsmqTransportSecurity_CopyCtor_CopiesAllProperties()
    {
        var original = new MsmqTransportSecurity
        {
            MsmqAuthenticationMode = MsmqAuthenticationMode.Certificate,
            MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.Aes,
            MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.Sha512,
            MsmqProtectionLevel = ProtectionLevel.EncryptAndSign,
        };
        var copy = new MsmqTransportSecurity(original);
        Assert.Equal(original.MsmqAuthenticationMode, copy.MsmqAuthenticationMode);
        Assert.Equal(original.MsmqEncryptionAlgorithm, copy.MsmqEncryptionAlgorithm);
        Assert.Equal(original.MsmqSecureHashAlgorithm, copy.MsmqSecureHashAlgorithm);
        Assert.Equal(original.MsmqProtectionLevel, copy.MsmqProtectionLevel);
    }

    [WcfFact]
    public static void MsmqTransportSecurity_CopyCtor_NullThrows()
    {
        Assert.Throws<ArgumentNullException>(() => new MsmqTransportSecurity(null));
    }

    [WcfTheory]
    [InlineData((MsmqAuthenticationMode)42)]
    [InlineData((MsmqAuthenticationMode)(-1))]
    public static void MsmqTransportSecurity_InvalidAuthMode_Throws(MsmqAuthenticationMode bad)
    {
        var s = new MsmqTransportSecurity();
        Assert.Throws<ArgumentOutOfRangeException>(() => s.MsmqAuthenticationMode = bad);
    }

    [WcfTheory]
    [InlineData((ProtectionLevel)42)]
    public static void MsmqTransportSecurity_InvalidProtectionLevel_Throws(ProtectionLevel bad)
    {
        var s = new MsmqTransportSecurity();
        Assert.Throws<ArgumentOutOfRangeException>(() => s.MsmqProtectionLevel = bad);
    }

    [WcfFact]
    public static void MessageSecurityOverMsmq_Defaults()
    {
        var m = new MessageSecurityOverMsmq();
        Assert.Equal(MessageCredentialType.Windows, m.ClientCredentialType);
        Assert.Same(SecurityAlgorithmSuite.Default, m.AlgorithmSuite);
    }

    [WcfFact]
    public static void MessageSecurityOverMsmq_AlgorithmSuite_NullThrows()
    {
        var m = new MessageSecurityOverMsmq();
        Assert.Throws<ArgumentNullException>(() => m.AlgorithmSuite = null);
    }

    [WcfTheory]
    [InlineData((MessageCredentialType)42)]
    public static void MessageSecurityOverMsmq_InvalidCredential_Throws(MessageCredentialType bad)
    {
        var m = new MessageSecurityOverMsmq();
        Assert.Throws<ArgumentOutOfRangeException>(() => m.ClientCredentialType = bad);
    }

    [WcfTheory]
    [InlineData(DeadLetterQueue.None, true)]
    [InlineData(DeadLetterQueue.System, true)]
    [InlineData(DeadLetterQueue.Custom, true)]
    [InlineData((DeadLetterQueue)42, false)]
    public static void DeadLetterQueueHelper_IsDefined(DeadLetterQueue value, bool expected)
    {
        Type t = typeof(DeadLetterQueue).Assembly.GetType("System.ServiceModel.DeadLetterQueueHelper", throwOnError: true);
        bool actual = (bool)t.GetMethod("IsDefined", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(null, new object[] { value });
        Assert.Equal(expected, actual);
    }

    [WcfTheory]
    [InlineData(QueueTransferProtocol.Native, true)]
    [InlineData(QueueTransferProtocol.Srmp, true)]
    [InlineData(QueueTransferProtocol.SrmpSecure, true)]
    [InlineData((QueueTransferProtocol)42, false)]
    public static void QueueTransferProtocolHelper_IsDefined(QueueTransferProtocol value, bool expected)
    {
        Type t = typeof(QueueTransferProtocol).Assembly.GetType("System.ServiceModel.QueueTransferProtocolHelper", throwOnError: true);
        bool actual = (bool)t.GetMethod("IsDefined", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(null, new object[] { value });
        Assert.Equal(expected, actual);
    }
}
