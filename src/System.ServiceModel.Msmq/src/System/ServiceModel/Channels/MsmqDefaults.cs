// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Net.Security;
using System.ServiceModel.MsmqIntegration;
using System.ServiceModel.Security;

namespace System.ServiceModel.Channels
{
    internal static class MsmqDefaults
    {
        internal const MessageCredentialType DefaultClientCredentialType = MessageCredentialType.Windows;
        internal const DeadLetterQueue DeadLetterQueue = System.ServiceModel.DeadLetterQueue.System;
        internal const bool Durable = true;
        internal const bool ExactlyOnce = true;
        internal const bool ReceiveContextEnabled = true;
        internal const int MaxRetryCycles = 2;
        internal const int MaxPoolSize = 8;
        internal const MsmqAuthenticationMode MsmqAuthenticationMode = System.ServiceModel.MsmqAuthenticationMode.WindowsDomain;
        internal const MsmqEncryptionAlgorithm MsmqEncryptionAlgorithm = System.ServiceModel.MsmqEncryptionAlgorithm.RC4Stream;
        internal const MsmqSecureHashAlgorithm DefaultMsmqSecureHashAlgorithm = System.ServiceModel.MsmqSecureHashAlgorithm.Sha256;
        internal static MsmqSecureHashAlgorithm MsmqSecureHashAlgorithm => DefaultMsmqSecureHashAlgorithm;
        internal const ProtectionLevel MsmqProtectionLevel = ProtectionLevel.Sign;
        internal const ReceiveErrorHandling ReceiveErrorHandling = System.ServiceModel.ReceiveErrorHandling.Fault;
        internal const int ReceiveRetryCount = 5;
        internal const QueueTransferProtocol QueueTransferProtocol = System.ServiceModel.QueueTransferProtocol.Native;
        internal const MsmqMessageSerializationFormat MsmqMessageSerializationFormat = MsmqIntegration.MsmqMessageSerializationFormat.Xml;
        internal const string RetryCycleDelayString = "00:30:00";
        internal static TimeSpan RetryCycleDelay => TimeSpan.Parse(RetryCycleDelayString);
        internal const string TimeToLiveString = "1.00:00:00";
        internal static TimeSpan TimeToLive => TimeSpan.Parse(TimeToLiveString);
        internal const bool UseActiveDirectory = false;
        internal const bool UseSourceJournal = false;
        internal const bool UseMsmqTracing = false;
        internal const string ValidityDurationString = "00:05:00";
        internal static TimeSpan ValidityDuration => TimeSpan.Parse(ValidityDurationString);
        internal static SecurityAlgorithmSuite MessageSecurityAlgorithmSuite => SecurityAlgorithmSuite.Default;
    }
}
