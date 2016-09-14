// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Tokens;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Security
{
    class SecurityProtocolCorrelationState
    {
        private SecurityToken _token;
        private SignatureConfirmations _signatureConfirmations;
        private ServiceModelActivity _activity;

        public SecurityProtocolCorrelationState(SecurityToken token)
        {
            _token = token;
            _activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.Current : null;
        }

        public SecurityToken Token
        {
            get { return _token; }
        }

        internal SignatureConfirmations SignatureConfirmations
        {
            get { return _signatureConfirmations; }
            set { _signatureConfirmations = value; }
        }

        internal ServiceModelActivity Activity
        {
            get { return _activity; }
        }
    }
}
