// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Globalization;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Net;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Xml;
using static System.ServiceModel.Security.SecurityUtils;

namespace System.ServiceModel.Security
{
    internal class SpnegoTokenProvider : SspiNegotiationTokenProvider
    {
        private TokenImpersonationLevel _allowedImpersonationLevel = TokenImpersonationLevel.Identification;
        private ICredentials _clientCredential;
        private IdentityVerifier _identityVerifier = IdentityVerifier.CreateDefault();
        //private bool _allowNtlm = true;
        private bool _authenticateServer = true;
        private NetworkCredential _networkCredential;

        public SpnegoTokenProvider(NetworkCredential networkCredential, SecurityBindingElement securityBindingElement) : base(securityBindingElement)
        {
            _networkCredential = networkCredential;
        }

        // settings
        public IdentityVerifier IdentityVerifier
        {
            get
            {
                return _identityVerifier;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _identityVerifier = value;
            }
        }

        public TokenImpersonationLevel AllowedImpersonationLevel
        {
            get
            {
                return _allowedImpersonationLevel;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();

                TokenImpersonationLevelHelper.Validate(value);
                if (value == TokenImpersonationLevel.None)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value),
                        string.Format(CultureInfo.InvariantCulture, SRP.SpnegoImpersonationLevelCannotBeSetToNone)));
                }
                _allowedImpersonationLevel = value;
            }
        }

        public ICredentials ClientCredential
        {
            get
            {
                return _clientCredential;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _clientCredential = value;
            }
        }

        public bool AuthenticateServer
        {
            get
            {
                return _authenticateServer;
            }
            set
            {
                CommunicationObject.ThrowIfDisposedOrImmutable();
                _authenticateServer = value;
            }
        }

        // overrides
        public override XmlDictionaryString NegotiationValueType
        {
            get
            {
                return XD.TrustApr2004Dictionary.SpnegoValueTypeUri;
            }
        }

        public override void OnOpening()
        {
            base.OnOpening();
            if (_networkCredential == null)
            {
                string packageName = "Negotiate";

                NetworkCredential credential = null;
                if (_clientCredential != null)
                {
                    credential = _clientCredential.GetCredential(TargetAddress.Uri, packageName);
                }

                if (credential == null)
                {
                    credential = CredentialCache.DefaultNetworkCredentials;
                }
                else if (!NetworkCredentialHelper.IsDefault(credential))
                {
                    SecurityUtils.FixNetworkCredential(ref credential);
                }

                _networkCredential = credential;
            }
        }

        public override async Task OnCloseAsync(TimeSpan timeout)
        {
            await base.OnCloseAsync(timeout);
            FreeCredentialsHandle();
        }

        public override void OnAbort()
        {
            base.OnAbort();
            FreeCredentialsHandle();
        }

        void FreeCredentialsHandle()
        {
            _networkCredential = null;
        }

        protected override Task<SspiNegotiationTokenProviderState> CreateNegotiationStateAsync(EndpointAddress target, Uri via, TimeSpan timeout)
        {
            return Task.FromResult(CreateNegotiationState(target, via, timeout));
        }

        private SspiNegotiationTokenProviderState CreateNegotiationState(EndpointAddress target, Uri via, TimeSpan timeout)
        {
            EnsureEndpointAddressDoesNotRequireEncryption(target);

            EndpointIdentity identity = null;
            if (_identityVerifier == null)
            {
                identity = target.Identity;
            }
            else
            {
                _identityVerifier.TryGetIdentity(target, out identity);
            }

            string spn;
            if (AuthenticateServer)
            {
                spn = SecurityUtils.GetSpnFromIdentity(identity, target);
            }
            else
            {
                // if an SPN or UPN identity is configured (for example, in mixed mode SSPI), then
                // use that identity for Negotiate
                Claim identityClaim = identity?.IdentityClaim;
                if (identityClaim != null && (identityClaim.ClaimType == ClaimTypes.Spn || identityClaim.ClaimType == ClaimTypes.Upn))
                {
                    spn = identityClaim.Resource.ToString();
                }
                else
                {
                    spn = "host/" + target.Uri.DnsSafeHost;
                }
            }

            string packageName = "Negotiate";

            NetworkCredential credential = null;
            if (_clientCredential != null)
            {
                credential = _clientCredential.GetCredential(target.Uri, packageName);
            }

            WindowsSspiNegotiation sspiNegotiation = new WindowsSspiNegotiation(
                packageName,
                credential,
                AllowedImpersonationLevel,
                spn,
                true);

            return new SspiNegotiationTokenProviderState(sspiNegotiation);
        }

        protected override ReadOnlyCollection<IAuthorizationPolicy> ValidateSspiNegotiation(ISspiNegotiation sspiNegotiation)
        {
            WindowsSspiNegotiation windowsNegotiation = (WindowsSspiNegotiation)sspiNegotiation;
            if (windowsNegotiation.IsValidContext == false)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(SRP.InvalidSspiNegotiation));
            }
            if (AuthenticateServer && windowsNegotiation.IsMutualAuthFlag == false)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(SRP.CannotAuthenticateServer));
            }

            return SecurityUtils.CreatePrincipalNameAuthorizationPolicies(windowsNegotiation.ServicePrincipalName);
        }
    }
}
