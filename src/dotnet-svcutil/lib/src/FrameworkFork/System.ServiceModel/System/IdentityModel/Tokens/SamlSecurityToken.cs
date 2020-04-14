// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.IdentityModel.Tokens
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using Microsoft.Xml.Serialization;
    using Microsoft.Xml;
    using Microsoft.Xml.Schema;
    using Microsoft.CodeDom;
    using System.Runtime.Serialization;
    using System.Globalization;
    using System.Threading;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Policy;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.IO;

    public class SamlSecurityToken : SecurityToken
    {
        private SamlAssertion _assertion;

        protected SamlSecurityToken()
        {
        }

        public SamlSecurityToken(SamlAssertion assertion)
        {
            Initialize(assertion);
        }

        protected void Initialize(SamlAssertion assertion)
        {
            if (assertion == null)
                throw /*System.ServiceModel.*/DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("assertion");

            _assertion = assertion;
        }

        public override string Id
        {
            get { return _assertion.AssertionId; }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                return _assertion.SecurityKeys;
            }
        }

        public SamlAssertion Assertion
        {
            get { return _assertion; }
        }

        public override DateTime ValidFrom
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override DateTime ValidTo
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
