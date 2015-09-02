// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if FEATURE_CORECLR // ExtendedProtection
using System.Security.Authentication.ExtendedProtection;
#endif

namespace System.ServiceModel.Channels
{
    internal interface IStreamUpgradeChannelBindingProvider : IChannelBindingProvider
    {
#if FEATURE_CORECLR // ExtendedProtection

        ChannelBinding GetChannelBinding(StreamUpgradeInitiator upgradeInitiator, ChannelBindingKind kind);
        ChannelBinding GetChannelBinding(StreamUpgradeAcceptor upgradeAcceptor, ChannelBindingKind kind);
#endif
    }
}

