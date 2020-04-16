// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IdentityModel.Tokens
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Selectors;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel.Security;
    using Microsoft.Xml;
    using KeyIdentifierEntry = System.IdentityModel.Selectors.SecurityTokenSerializer.KeyIdentifierEntry;

    internal class XmlDsigSep2000 : SecurityTokenSerializer.SerializerEntries
    {
        private KeyInfoSerializer _securityTokenSerializer;

        public XmlDsigSep2000(KeyInfoSerializer securityTokenSerializer)
        {
            _securityTokenSerializer = securityTokenSerializer;
        }
        public override void PopulateKeyIdentifierEntries(IList<KeyIdentifierEntry> keyIdentifierEntries)
        {
            keyIdentifierEntries.Add(new KeyInfoEntry(_securityTokenSerializer));
        }

        public override void PopulateKeyIdentifierClauseEntries(IList<SecurityTokenSerializer.KeyIdentifierClauseEntry> keyIdentifierClauseEntries)
        {
            keyIdentifierClauseEntries.Add(new KeyNameClauseEntry());
            keyIdentifierClauseEntries.Add(new KeyValueClauseEntry());
            keyIdentifierClauseEntries.Add(new X509CertificateClauseEntry());
        }

        internal class KeyInfoEntry : KeyIdentifierEntry
        {
            private KeyInfoSerializer _securityTokenSerializer;

            public KeyInfoEntry(KeyInfoSerializer securityTokenSerializer)
            {
                _securityTokenSerializer = securityTokenSerializer;
            }

            protected override XmlDictionaryString LocalName
            {
                get
                {
                    return XD.XmlSignatureDictionary.KeyInfo;
                }
            }

            protected override XmlDictionaryString NamespaceUri
            {
                get
                {
                    return XD.XmlSignatureDictionary.Namespace;
                }
            }

            public override SecurityKeyIdentifier ReadKeyIdentifierCore(XmlDictionaryReader reader)
            {
                throw new NotImplementedException();
            }

            public override bool SupportsCore(SecurityKeyIdentifier keyIdentifier)
            {
                return true;
            }

            public override void WriteKeyIdentifierCore(XmlDictionaryWriter writer, SecurityKeyIdentifier keyIdentifier)
            {
                throw new NotImplementedException();
            }
        }

        // <ds:KeyName>name</ds:KeyName>
        internal class KeyNameClauseEntry : SecurityTokenSerializer.KeyIdentifierClauseEntry
        {
            protected override XmlDictionaryString LocalName
            {
                get
                {
                    return XD.XmlSignatureDictionary.KeyName;
                }
            }

            protected override XmlDictionaryString NamespaceUri
            {
                get
                {
                    return XD.XmlSignatureDictionary.Namespace;
                }
            }

            public override SecurityKeyIdentifierClause ReadKeyIdentifierClauseCore(XmlDictionaryReader reader)
            {
                throw new NotImplementedException();
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
        // so far, we only support one type of KeyValue - RSAKeyValue
        //   <ds:KeyValue>
        //     <ds:RSAKeyValue>
        //       <ds:Modulus>xA7SEU+...</ds:Modulus>
        //         <ds:Exponent>AQAB</Exponent>
        //     </ds:RSAKeyValue>
        //   </ds:KeyValue>
        internal class KeyValueClauseEntry : SecurityTokenSerializer.KeyIdentifierClauseEntry
        {
            protected override XmlDictionaryString LocalName
            {
                get
                {
                    return XD.XmlSignatureDictionary.KeyValue;
                }
            }

            protected override XmlDictionaryString NamespaceUri
            {
                get
                {
                    return XD.XmlSignatureDictionary.Namespace;
                }
            }


            public override SecurityKeyIdentifierClause ReadKeyIdentifierClauseCore(XmlDictionaryReader reader)
            {
                throw new NotImplementedException();
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

        // so far, we only support two types of X509Data directly under KeyInfo  - X509Certificate and X509SKI
        //   <ds:X509Data>
        //     <ds:X509Certificate>...</ds:X509Certificate>
        //      or
        //     <X509SKI>... </X509SKI>
        //   </ds:X509Data>
        // only support 1 certificate right now
        internal class X509CertificateClauseEntry : SecurityTokenSerializer.KeyIdentifierClauseEntry
        {
            protected override XmlDictionaryString LocalName
            {
                get
                {
                    return XD.XmlSignatureDictionary.X509Data;
                }
            }

            protected override XmlDictionaryString NamespaceUri
            {
                get
                {
                    return XD.XmlSignatureDictionary.Namespace;
                }
            }

            public override SecurityKeyIdentifierClause ReadKeyIdentifierClauseCore(XmlDictionaryReader reader)
            {
                throw new NotImplementedException();
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
    }
}
