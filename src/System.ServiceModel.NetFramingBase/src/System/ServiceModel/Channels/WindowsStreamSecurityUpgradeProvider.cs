// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime;
using System.Security.Authentication;
using System.Security.Principal;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class WindowsStreamSecurityUpgradeProvider : StreamSecurityUpgradeProvider
    {
        internal const string NegotiateUpgradeString = "application/negotiate";
        private bool _extractGroupsForWindowsAccounts;
        private SecurityTokenManager _securityTokenManager;

        public WindowsStreamSecurityUpgradeProvider(WindowsStreamSecurityBindingElement bindingElement, BindingContext context)
            : base(context.Binding)
        {
            _extractGroupsForWindowsAccounts = NFTransportDefaults.ExtractGroupsForWindowsAccounts;
            ProtectionLevel = bindingElement.ProtectionLevel;
            Scheme = context.Binding.Scheme;
            SecurityCredentialsManager credentialProvider = context.BindingParameters.Find<SecurityCredentialsManager>();
            if (credentialProvider == null)
            {
                credentialProvider = new ClientCredentials();
            }

            _securityTokenManager = credentialProvider.CreateSecurityTokenManager();
        }

        public string Scheme { get; }

        internal bool ExtractGroupsForWindowsAccounts
        {
            get
            {
                return _extractGroupsForWindowsAccounts;
            }
        }

        internal IdentityVerifier IdentityVerifier { get; private set; }

        public ProtectionLevel ProtectionLevel { get; }

        public override StreamUpgradeInitiator CreateUpgradeInitiator(EndpointAddress remoteAddress, Uri via)
        {
            this.ThrowIfDisposedOrNotOpen();
            return new WindowsStreamSecurityUpgradeInitiator(this, remoteAddress, via);
        }

        protected override void OnAbort()
        {
        }

        protected override void OnClose(TimeSpan timeout)
        {
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return Task.CompletedTask.ToApm(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            return Task.CompletedTask;
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return Task.CompletedTask.ToApm(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected internal override Task OnOpenAsync(TimeSpan timeout)
        {
            return Task.CompletedTask;
        }

        protected override void OnOpened()
        {
            base.OnOpened();

            if (IdentityVerifier == null)
            {
                IdentityVerifier = IdentityVerifier.CreateDefault();
            }
        }

        private class WindowsStreamSecurityUpgradeInitiator : StreamSecurityUpgradeInitiatorBase
        {
            private WindowsStreamSecurityUpgradeProvider _parent;
            private NetworkCredential _credential;
            private TokenImpersonationLevel _impersonationLevel;
            private SecurityTokenProvider _clientTokenProvider;
            private bool _allowNtlm;

            public WindowsStreamSecurityUpgradeInitiator(
                WindowsStreamSecurityUpgradeProvider parent, EndpointAddress remoteAddress, Uri via)
                : base(NegotiateUpgradeString, remoteAddress, via)
            {
                _parent = parent;
                _clientTokenProvider = WindowsStreamTransportSecurityHelpers.GetSspiTokenProvider(
                    parent._securityTokenManager, remoteAddress, via, parent.Scheme);
            }

            internal override async ValueTask OpenAsync(TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                await base.OpenAsync(timeoutHelper.RemainingTime());
                await SecurityUtils.OpenTokenProviderIfRequiredAsync(_clientTokenProvider, timeoutHelper.RemainingTime());
                (_credential, _impersonationLevel, _allowNtlm) = await WindowsStreamTransportSecurityHelpers.GetSspiCredentialAsync(_clientTokenProvider, timeoutHelper.RemainingTime());
                return;
            }

            internal override async ValueTask CloseAsync(TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                await base.CloseAsync(timeoutHelper.RemainingTime());
                await SecurityUtils.CloseTokenProviderIfRequiredAsync(_clientTokenProvider, timeoutHelper.RemainingTime());
            }

            private static SecurityMessageProperty CreateServerSecurity(NegotiateStream negotiateStream)
            {
                GenericIdentity remoteIdentity = (GenericIdentity)negotiateStream.RemoteIdentity;
                string principalName = remoteIdentity.Name;
                if ((principalName != null) && (principalName.Length > 0))
                {
                    ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies = SecurityUtils.CreatePrincipalNameAuthorizationPolicies(principalName);
                    SecurityMessageProperty result = new SecurityMessageProperty();
                    result.TransportToken = new SecurityTokenSpecification(null, authorizationPolicies);
                    result.ServiceSecurityContext = new ServiceSecurityContext(authorizationPolicies);
                    return result;
                }
                else
                {
                    return null;
                }
            }

            protected override async Task<(Stream upgradedStream, SecurityMessageProperty remoteSecurity)> OnInitiateUpgradeAsync(Stream stream)
            {
                NegotiateStream negotiateStream;
                SecurityMessageProperty remoteSecurity;
                string targetName;
                EndpointIdentity identity;

                if (WcfEventSource.Instance.WindowsStreamSecurityOnInitiateUpgradeIsEnabled())
                {
                    WcfEventSource.Instance.WindowsStreamSecurityOnInitiateUpgrade();
                }

                // prepare
                InitiateUpgradePrepare(stream, out negotiateStream, out targetName, out identity);

                // authenticate
                try
                {
                    await negotiateStream.AuthenticateAsClientAsync(_credential, targetName, _parent.ProtectionLevel, _impersonationLevel);
                }
                catch (AuthenticationException exception)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(exception.Message,
                        exception));
                }
                catch (IOException ioException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(
                        SR.Format(SR.NegotiationFailedIO, ioException.Message), ioException));
                }

                remoteSecurity = CreateServerSecurity(negotiateStream);
                ValidateMutualAuth(identity, negotiateStream, remoteSecurity, _allowNtlm);

                return (negotiateStream, remoteSecurity);
            }

            private void InitiateUpgradePrepare(
                Stream stream,
                out NegotiateStream negotiateStream,
                out string targetName,
                out EndpointIdentity identity)
            {
                negotiateStream = new NegotiateStream(stream);
                var referenceAddress = RemoteAddress;
                AdjustAddress(ref referenceAddress, Via);
                if (_parent.IdentityVerifier.TryGetIdentity(referenceAddress, out identity))
                {
                    targetName = SecurityUtils.GetSpnFromIdentity(identity, RemoteAddress);
                }
                else
                {
                    targetName = SecurityUtils.GetSpnFromTarget(RemoteAddress);
                }
            }

            // Copied from IdentityVerifier as used by the internal method TryGetIdentity(EndpointAddress reference, Uri via, out EndpointIdentity identity)
            private static void AdjustAddress(ref EndpointAddress reference, Uri via)
            {
                // if we don't have an identity and we have differing Uris, we should use the Via
                if (reference.Identity == null && reference.Uri != via)
                {
                    reference = new EndpointAddress(via);
                }
            }

            private void ValidateMutualAuth(EndpointIdentity expectedIdentity, NegotiateStream negotiateStream,
                SecurityMessageProperty remoteSecurity, bool allowNtlm)
            {
                if (negotiateStream.IsMutuallyAuthenticated)
                {
                    if (expectedIdentity != null)
                    {
                        if (!_parent.IdentityVerifier.CheckAccess(expectedIdentity,
                            remoteSecurity.ServiceSecurityContext.AuthorizationContext))
                        {
                            string primaryIdentity = SecurityUtilsEx.GetIdentityNamesFromContext(remoteSecurity.ServiceSecurityContext.AuthorizationContext);
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(SR.Format(
                                SR.RemoteIdentityFailedVerification, primaryIdentity)));
                        }
                    }
                }
                else if (!allowNtlm)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityNegotiationException(SR.Format(SR.StreamMutualAuthNotSatisfied)));
                }
            }
        }
    }
}
