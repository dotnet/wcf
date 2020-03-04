// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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

