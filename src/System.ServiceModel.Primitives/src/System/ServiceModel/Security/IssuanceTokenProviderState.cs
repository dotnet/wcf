// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Tokens;

namespace System.ServiceModel.Security
{
    internal class IssuanceTokenProviderState : IDisposable
    {
        private GenericXmlSecurityToken _serviceToken;

        public IssuanceTokenProviderState() { }

        public bool IsNegotiationCompleted { get; private set; } = false;

        public GenericXmlSecurityToken ServiceToken
        {
            get
            {
                CheckCompleted();
                return _serviceToken;
            }
        }

        public EndpointAddress TargetAddress { get; set; }

        public EndpointAddress RemoteAddress { get; set; }

        public string Context { get; set; }

        public virtual void Dispose() { }

        public void SetServiceToken(GenericXmlSecurityToken serviceToken)
        {
            if (IsNegotiationCompleted)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.NegotiationIsCompleted));
            }
            _serviceToken = serviceToken;
            IsNegotiationCompleted = true;
        }

        private void CheckCompleted()
        {
            if (!IsNegotiationCompleted)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.NegotiationIsNotCompleted));
            }
        }
    }
}
