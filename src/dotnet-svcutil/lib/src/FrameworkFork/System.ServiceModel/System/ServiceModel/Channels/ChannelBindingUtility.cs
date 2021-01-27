// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Net.Security;
using System.Runtime;
using System.Security.Authentication.ExtendedProtection;

namespace System.ServiceModel.Channels
{
    internal static class ChannelBindingUtility
    {
        private static ExtendedProtectionPolicy s_disabledPolicy = new ExtendedProtectionPolicy(PolicyEnforcement.Never);
        private static ExtendedProtectionPolicy s_defaultPolicy = s_disabledPolicy;

        public static ExtendedProtectionPolicy DisabledPolicy
        {
            get
            {
                return s_disabledPolicy;
            }
        }

        public static ExtendedProtectionPolicy DefaultPolicy
        {
            get
            {
                return s_defaultPolicy;
            }
        }

#if !FEATURE_NETNATIVE
        public static ChannelBinding GetToken(SslStream stream)
        {
            return GetToken(stream.TransportContext);
        }
#endif // !FEATURE_NETNATIVE

        public static ChannelBinding GetToken(TransportContext context)
        {
            ChannelBinding token = null;
            if (context != null)
            {
                token = context.GetChannelBinding(ChannelBindingKind.Endpoint);
            }
            return token;
        }

        public static void TryAddToMessage(ChannelBinding channelBindingToken, Message message, bool messagePropertyOwnsCleanup)
        {
            if (channelBindingToken != null)
            {
                ChannelBindingMessageProperty property = new ChannelBindingMessageProperty(channelBindingToken, messagePropertyOwnsCleanup);
                property.AddTo(message);
                property.Dispose(); //message.Properties.Add() creates a copy...
            }
        }

        public static void Dispose(ref ChannelBinding channelBinding)
        {
            // Explicitly cast to IDisposable to avoid the SecurityException.
            IDisposable disposable = (IDisposable)channelBinding;
            channelBinding = null;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
