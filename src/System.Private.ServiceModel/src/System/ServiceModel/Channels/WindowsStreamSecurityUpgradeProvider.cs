// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IdentityModel.Selectors;
using System.Net;
using System.Net.Security;
using System.Runtime;
using System.ServiceModel.Description;
using System.ServiceModel.Security;

namespace System.ServiceModel.Channels
{
    internal class WindowsStreamSecurityUpgradeProvider : StreamSecurityUpgradeProvider
    {
        private bool _extractGroupsForWindowsAccounts;
        private EndpointIdentity _identity;
        private ProtectionLevel _protectionLevel;
        private SecurityTokenManager _securityTokenManager;
        private NetworkCredential _serverCredential = null;
        private string _scheme;
        private bool _isClient;
        private Uri _listenUri;

        public WindowsStreamSecurityUpgradeProvider(WindowsStreamSecurityBindingElement bindingElement,
            BindingContext context, bool isClient)
            : base(context.Binding)
        {
            _extractGroupsForWindowsAccounts = TransportDefaults.ExtractGroupsForWindowsAccounts;
            _protectionLevel = bindingElement.ProtectionLevel;
            _scheme = context.Binding.Scheme;
            _isClient = isClient;
            _listenUri = TransportSecurityHelpers.GetListenUri(context.ListenUriBaseAddress, context.ListenUriRelativeAddress);

            SecurityCredentialsManager credentialProvider = context.BindingParameters.Find<SecurityCredentialsManager>();

            if (credentialProvider == null)
            {
                if (isClient)
                {
                    credentialProvider = ClientCredentials.CreateDefaultCredentials();
                }
                else
                {
                    throw ExceptionHelper.PlatformNotSupported("WindowsStreamSecurityUpgradeProvider for server is not supported.");
                }
            }

            _securityTokenManager = credentialProvider.CreateSecurityTokenManager();
        }

        public string Scheme
        {
            get { return _scheme; }
        }

        internal bool ExtractGroupsForWindowsAccounts
        {
            get
            {
                return _extractGroupsForWindowsAccounts;
            }
        }

        public override EndpointIdentity Identity
        {
            get
            {
                // If the server credential is null, then we have not been opened yet and have no identity to expose.
                if (_serverCredential != null)
                {
                    if (_identity == null)
                    {
                        lock (ThisLock)
                        {
                            if (_identity == null)
                            {
                                _identity = SecurityUtils.CreateWindowsIdentity(_serverCredential);
                            }
                        }
                    }
                }
                return _identity;
            }
        }

        public override StreamUpgradeAcceptor CreateUpgradeAcceptor()
        {
            throw ExceptionHelper.PlatformNotSupported("WindowsStreamSecurityUpgradeProvider.CreateUpgradeAcceptor is not supported.");
        }

        public override StreamUpgradeInitiator CreateUpgradeInitiator(EndpointAddress remoteAddress, Uri via)
        {
            throw ExceptionHelper.PlatformNotSupported("WindowsStreamSecurityUpgradeProvider.CreateUpgradeInitiator is not supported.");
        }

        protected override void OnAbort()
        {
        }

        protected override void OnClose(TimeSpan timeout)
        {
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            if (!_isClient)
            {
                throw ExceptionHelper.PlatformNotSupported("WindowsStreamSecurityUpgradeProvider.OnOpen for server is not supported.");
            }
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            OnOpen(timeout);
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }
    }
}
