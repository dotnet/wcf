// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IdentityModel.Tokens
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.IdentityModel;
    using System.IdentityModel.Claims;
    using System.IdentityModel.Policy;
    using System.IdentityModel.Selectors;
    using System.Runtime;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;
    using Microsoft.Xml;

    public class SamlAssertion // TODO: ICanonicalWriterEndRootElementCallback
    {
        private string _assertionId = /*TODO: SamlConstants.AssertionIdPrefix*/ "SamlSecurityToken-" + Guid.NewGuid().ToString();
        private bool _isReadOnly = false;
        private ReadOnlyCollection<SecurityKey> _cryptoList;

        public string AssertionId
        {
            get { return _assertionId; }
            set
            {
                if (_isReadOnly)
                    throw /*System.ServiceModel.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));

                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value"); // TODO: DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.SAMLAssertionIdRequired);

                _assertionId = value;
            }
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
        }

        internal ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                return _cryptoList;
            }
        }
    }
}
