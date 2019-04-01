// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Security.Tokens;
using System.Threading.Tasks;

namespace System.ServiceModel.Security
{
    internal class SessionSymmetricTransportSecurityProtocolFactory : TransportSecurityProtocolFactory
    {
        private SecurityTokenParameters _securityTokenParameters;

        public SessionSymmetricTransportSecurityProtocolFactory() : base()
        {
        }

        public override bool SupportsReplayDetection
        {
            get
            {
                return true;
            }
        }

        public SecurityTokenParameters SecurityTokenParameters
        {
            get
            {
                return _securityTokenParameters;
            }
            set
            {
                ThrowIfImmutable();
                _securityTokenParameters = value;
            }
        }

        protected override SecurityProtocol OnCreateSecurityProtocol(EndpointAddress target, Uri via, object listenerSecurityState, TimeSpan timeout)
        {
            if (ActAsInitiator)
            {
                return new InitiatorSessionSymmetricTransportSecurityProtocol(this, target, via);
            }
            else
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
        }

        public override async Task OnOpenAsync(TimeSpan timeout)
        {
            await base.OnOpenAsync(timeout);
            if (SecurityTokenParameters == null)
            {
                OnPropertySettingsError(nameof(SecurityTokenParameters), true);
            }
            if (SecurityTokenParameters.RequireDerivedKeys)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
        }

        internal SecurityTokenParameters GetTokenParameters()
        {
            return _securityTokenParameters;
        }
    }
}
