// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Security
{
    class SecurityTokenParametersEnumerable : IEnumerable<SecurityTokenParameters>
    {
        SecurityBindingElement _sbe;
        bool _clientTokensOnly;

        public SecurityTokenParametersEnumerable(SecurityBindingElement sbe) : this(sbe, false) { }

        public SecurityTokenParametersEnumerable(SecurityBindingElement sbe, bool clientTokensOnly)
        {
            if (sbe == null) throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(sbe));
            _sbe = sbe;
            _clientTokensOnly = clientTokensOnly;
        }

        public IEnumerator<SecurityTokenParameters> GetEnumerator()
        {
            foreach (SecurityTokenParameters stp in _sbe.EndpointSupportingTokenParameters.Endorsing)
                if (stp != null)
                    yield return stp;
            foreach (SecurityTokenParameters stp in _sbe.EndpointSupportingTokenParameters.SignedEncrypted)
                if (stp != null)
                    yield return stp;
            foreach (SecurityTokenParameters stp in _sbe.EndpointSupportingTokenParameters.SignedEndorsing)
                if (stp != null)
                    yield return stp;
            foreach (SecurityTokenParameters stp in _sbe.EndpointSupportingTokenParameters.Signed)
                if (stp != null)
                    yield return stp;
            foreach (SupportingTokenParameters str in _sbe.OperationSupportingTokenParameters.Values)
                if (str != null)
                {
                    foreach (SecurityTokenParameters stp in str.Endorsing)
                        if (stp != null)
                            yield return stp;
                    foreach (SecurityTokenParameters stp in str.SignedEncrypted)
                        if (stp != null)
                            yield return stp;
                    foreach (SecurityTokenParameters stp in str.SignedEndorsing)
                        if (stp != null)
                            yield return stp;
                    foreach (SecurityTokenParameters stp in str.Signed)
                        if (stp != null)
                            yield return stp;
                }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotImplementedException());
        }
    }
}
