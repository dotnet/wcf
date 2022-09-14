// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Tokens;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Security
{
    internal class SecurityProtocolCorrelationState
    {
        public SecurityProtocolCorrelationState(SecurityToken token)
        {
            Token = token;
            Activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.Current : null;
        }

        public SecurityToken Token { get; }

        internal SignatureConfirmations SignatureConfirmations { get; set; }

        internal ServiceModelActivity Activity { get; }
    }
}
