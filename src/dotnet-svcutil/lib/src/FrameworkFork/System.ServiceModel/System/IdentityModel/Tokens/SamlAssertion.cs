// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
        string assertionId = /*TODO: SamlConstants.AssertionIdPrefix*/ "SamlSecurityToken-" + Guid.NewGuid().ToString();
        bool isReadOnly = false;
        ReadOnlyCollection<SecurityKey> cryptoList;

        public string AssertionId
        {
            get { return this.assertionId; }
            set
            {
                if (isReadOnly)
                    throw /*System.ServiceModel.*/DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));

                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value"); // TODO: DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.SAMLAssertionIdRequired);

                this.assertionId = value;
            }
        }

        public bool IsReadOnly
        {
            get { return this.isReadOnly; }
        }

        internal ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                return this.cryptoList;
            }
        }
    }
}
