// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

