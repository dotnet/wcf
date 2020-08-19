﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    interface IReliableFactorySettings
    {
        TimeSpan AcknowledgementInterval { get; }

        bool FlowControlEnabled { get; }

        TimeSpan InactivityTimeout { get; }

        int MaxPendingChannels { get; }

        int MaxRetryCount { get; }

        int MaxTransferWindowSize { get; }

        MessageVersion MessageVersion { get; }

        bool Ordered { get; }

        ReliableMessagingVersion ReliableMessagingVersion { get; }

        TimeSpan SendTimeout { get; }
    }
}
