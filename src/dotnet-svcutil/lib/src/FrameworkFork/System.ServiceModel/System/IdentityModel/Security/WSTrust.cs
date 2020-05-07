// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IdentityModel.Security
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.ServiceModel.Security;
    using System.ServiceModel.Security.Tokens;
    using Microsoft.Xml;
    using KeyIdentifierClauseEntry = System.IdentityModel.Selectors.SecurityTokenSerializer.KeyIdentifierClauseEntry;

    internal class WSTrust : SecurityTokenSerializer.SerializerEntries
    {
        private KeyInfoSerializer _securityTokenSerializer;
        private TrustDictionary _serializerDictionary;

        public WSTrust(KeyInfoSerializer securityTokenSerializer, TrustDictionary serializerDictionary)
        {
            _securityTokenSerializer = securityTokenSerializer;
            _serializerDictionary = serializerDictionary;
        }

        public TrustDictionary SerializerDictionary
        {
            get
            {
                return _serializerDictionary;
            }
        }

        public override void PopulateTokenEntries(IList<SecurityTokenSerializer.TokenEntry> tokenEntryList)
        {
            tokenEntryList.Add(new BinarySecretTokenEntry(this));
        }

        public override void PopulateKeyIdentifierClauseEntries(IList<SecurityTokenSerializer.KeyIdentifierClauseEntry> keyIdentifierClauseEntries)
        {
            keyIdentifierClauseEntries.Add(new BinarySecretClauseEntry(this));
            keyIdentifierClauseEntries.Add(new GenericXmlSecurityKeyIdentifierClauseEntry(this));
        }

        private class BinarySecretTokenEntry : SecurityTokenSerializer.TokenEntry
        {
            private WSTrust _parent;

            public BinarySecretTokenEntry(WSTrust parent)
            {
                _parent = parent;
            }

            protected override XmlDictionaryString LocalName { get { return _parent.SerializerDictionary.BinarySecret; } }
            protected override XmlDictionaryString NamespaceUri { get { return _parent.SerializerDictionary.Namespace; } }
            protected override Type[] GetTokenTypesCore() { return new Type[] { typeof(BinarySecretSecurityToken) }; }
            public override string TokenTypeUri { get { return null; } }
            protected override string ValueTypeUri { get { return null; } }
        }

        internal class BinarySecretClauseEntry : KeyIdentifierClauseEntry
        {
            private WSTrust _parent;
            private TrustDictionary _otherDictionary = null;

            public BinarySecretClauseEntry(WSTrust parent)
            {
                _parent = parent;

                _otherDictionary = null;

                if (parent.SerializerDictionary is TrustDec2005Dictionary)
                {
                    _otherDictionary = parent._securityTokenSerializer.DictionaryManager.TrustFeb2005Dictionary;
                }

                if (parent.SerializerDictionary is TrustFeb2005Dictionary)
                {
                    _otherDictionary = parent._securityTokenSerializer.DictionaryManager.TrustDec2005Dictionary;
                }

                // always set it, so we don't have to worry about null
                if (_otherDictionary == null)
                    _otherDictionary = _parent.SerializerDictionary;
            }

            protected override XmlDictionaryString LocalName
            {
                get { return _parent.SerializerDictionary.BinarySecret; }
            }

            protected override XmlDictionaryString NamespaceUri
            {
                get { return _parent.SerializerDictionary.Namespace; }
            }

            public override SecurityKeyIdentifierClause ReadKeyIdentifierClauseCore(XmlDictionaryReader reader)
            {
                throw new NotImplementedException();
            }

            public override bool SupportsCore(SecurityKeyIdentifierClause keyIdentifierClause)
            {
                throw new NotImplementedException();
            }

            public override bool CanReadKeyIdentifierClauseCore(XmlDictionaryReader reader)
            {
                return (reader.IsStartElement(this.LocalName, this.NamespaceUri) || reader.IsStartElement(this.LocalName, _otherDictionary.Namespace));
            }

            public override void WriteKeyIdentifierClauseCore(XmlDictionaryWriter writer, SecurityKeyIdentifierClause keyIdentifierClause)
            {
                throw new NotImplementedException();
            }
        }

        internal class GenericXmlSecurityKeyIdentifierClauseEntry : KeyIdentifierClauseEntry
        {
            private WSTrust _parent;

            public GenericXmlSecurityKeyIdentifierClauseEntry(WSTrust parent)
            {
                _parent = parent;
            }

            protected override XmlDictionaryString LocalName
            {
                get { return null; }
            }

            protected override XmlDictionaryString NamespaceUri
            {
                get { return null; }
            }

            public override bool CanReadKeyIdentifierClauseCore(XmlDictionaryReader reader)
            {
                return false;
            }

            public override SecurityKeyIdentifierClause ReadKeyIdentifierClauseCore(XmlDictionaryReader reader)
            {
                return null;
            }

            public override bool SupportsCore(SecurityKeyIdentifierClause keyIdentifierClause)
            {
                throw new NotImplementedException();
            }

            public override void WriteKeyIdentifierClauseCore(XmlDictionaryWriter writer, SecurityKeyIdentifierClause keyIdentifierClause)
            {
                throw new NotImplementedException();
            }
        }

        protected static bool CheckElement(XmlElement element, string name, string ns, out string value)
        {
            value = null;
            if (element.LocalName != name || element.NamespaceURI != ns)
                return false;
            if (element.FirstChild is XmlText)
            {
                value = ((XmlText)element.FirstChild).Value;
                return true;
            }
            return false;
        }
    }
}
