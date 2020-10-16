// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.Net;
using System.Security.Principal;

namespace System.ServiceModel.Security.Tokens
{
    public class SspiSecurityToken : SecurityToken
    {
        private string _id;
        private readonly TokenImpersonationLevel _impersonationLevel;
        private readonly bool _allowNtlm;
        private readonly NetworkCredential _networkCredential;
        private readonly bool _extractGroupsForWindowsAccounts;
        private readonly bool _allowUnauthenticatedCallers = SspiSecurityTokenProvider.DefaultAllowUnauthenticatedCallers;
        private readonly DateTime _effectiveTime;
        private readonly DateTime _expirationTime;

        public SspiSecurityToken(TokenImpersonationLevel impersonationLevel, bool allowNtlm, NetworkCredential networkCredential)
        {
            _impersonationLevel = impersonationLevel;
            _allowNtlm = allowNtlm;
            _networkCredential = SecurityUtils.GetNetworkCredentialsCopy(networkCredential);
            _effectiveTime = DateTime.UtcNow;
            _expirationTime = _effectiveTime.AddHours(10);
        }

        public SspiSecurityToken(NetworkCredential networkCredential, bool extractGroupsForWindowsAccounts, bool allowUnauthenticatedCallers)
        {
            _networkCredential = SecurityUtils.GetNetworkCredentialsCopy(networkCredential);
            _extractGroupsForWindowsAccounts = extractGroupsForWindowsAccounts;
            _allowUnauthenticatedCallers = allowUnauthenticatedCallers;
            _effectiveTime = DateTime.UtcNow;
            _expirationTime = _effectiveTime.AddHours(10);
        }

        public override string Id
        {
            get
            {
                if (_id == null)
                    _id = SecurityUniqueId.Create().Value;
                return _id;
            }
        }

        public override DateTime ValidFrom
        {
            get { return _effectiveTime; }
        }

        public override DateTime ValidTo
        {
            get { return _expirationTime; }
        }

        public bool AllowUnauthenticatedCallers
        {
            get
            {
                return _allowUnauthenticatedCallers;
            }
        }

        public TokenImpersonationLevel ImpersonationLevel
        {
            get
            {
                return _impersonationLevel;
            }
        }

        public bool AllowNtlm
        {
            get
            {
                return _allowNtlm;
            }
        }

        public NetworkCredential NetworkCredential
        {
            get
            {
                return _networkCredential;
            }
        }

        public bool ExtractGroupsForWindowsAccounts
        {
            get
            {
                return _extractGroupsForWindowsAccounts;
            }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                return EmptyReadOnlyCollection<SecurityKey>.Instance;
            }
        }
    }
}
