// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ServiceModel;
using System.Xml;
using KeyIdentifierClauseEntry = System.IdentityModel.Selectors.SecurityTokenSerializer.KeyIdentifierClauseEntry;
using StrEntry = System.IdentityModel.Selectors.SecurityTokenSerializer.StrEntry;
using TokenEntry = System.IdentityModel.Selectors.SecurityTokenSerializer.TokenEntry;

namespace System.IdentityModel.Tokens
{
    internal class WSSecurityXXX2005 : WSSecurityJan2004
    {
        public WSSecurityXXX2005(KeyInfoSerializer securityTokenSerializer)
            : base(securityTokenSerializer)
        {
        }

        public override void PopulateStrEntries(IList<StrEntry> strEntries)
        {
            PopulateJan2004StrEntries(strEntries);
            strEntries.Add(new X509ThumbprintStrEntry(SecurityTokenSerializer.EmitBspRequiredAttributes));
        }

        public override void PopulateTokenEntries(IList<TokenEntry> tokenEntryList)
        {
            PopulateJan2004TokenEntries(tokenEntryList);
        }

        public override void PopulateKeyIdentifierClauseEntries(IList<KeyIdentifierClauseEntry> clauseEntries)
        {
            List<StrEntry> strEntries = new List<StrEntry>();
            SecurityTokenSerializer.PopulateStrEntries(strEntries);
            SecurityTokenReferenceXXX2005ClauseEntry strClause = new SecurityTokenReferenceXXX2005ClauseEntry(SecurityTokenSerializer.EmitBspRequiredAttributes, strEntries);
            clauseEntries.Add(strClause);
        }

        private class SecurityTokenReferenceXXX2005ClauseEntry : SecurityTokenReferenceJan2004ClauseEntry
        {
            public SecurityTokenReferenceXXX2005ClauseEntry(bool emitBspRequiredAttributes, IList<StrEntry> strEntries)
                : base(emitBspRequiredAttributes, strEntries)
            {
            }

            protected override string ReadTokenType(XmlDictionaryReader reader)
            {
                return reader.GetAttribute(XD.SecurityXXX2005Dictionary.TokenTypeAttribute, XD.SecurityXXX2005Dictionary.Namespace);
            }

            public override void WriteKeyIdentifierClauseCore(XmlDictionaryWriter writer, SecurityKeyIdentifierClause keyIdentifierClause)
            {
                for (int i = 0; i < StrEntries.Count; ++i)
                {
                    if (StrEntries[i].SupportsCore(keyIdentifierClause))
                    {
                        writer.WriteStartElement(XD.SecurityJan2004Dictionary.Prefix.Value, XD.SecurityJan2004Dictionary.SecurityTokenReference, XD.SecurityJan2004Dictionary.Namespace);

                        string tokenTypeUri = GetTokenTypeUri(StrEntries[i], keyIdentifierClause);
                        if (tokenTypeUri != null)
                        {
                            writer.WriteAttributeString(XD.SecurityXXX2005Dictionary.Prefix.Value, XD.SecurityXXX2005Dictionary.TokenTypeAttribute, XD.SecurityXXX2005Dictionary.Namespace, tokenTypeUri);
                        }

                        StrEntries[i].WriteContent(writer, keyIdentifierClause);
                        writer.WriteEndElement();
                        return;
                    }
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.StandardsManagerCannotWriteObject, keyIdentifierClause.GetType())));
            }

            private string GetTokenTypeUri(StrEntry str, SecurityKeyIdentifierClause keyIdentifierClause)
            {
                bool emitTokenType = EmitTokenType(str);
                if (emitTokenType)
                {
                    string tokenTypeUri;
                    if (str is LocalReferenceStrEntry)
                    {
                        tokenTypeUri = (str as LocalReferenceStrEntry).GetLocalTokenTypeUri(keyIdentifierClause);
                        // only emit token type for SAML,Kerberos and Encrypted References
                        switch (tokenTypeUri)
                        {
                            case SecurityXXX2005Strings.Saml20TokenType:
                            case SecurityXXX2005Strings.SamlTokenType:
                            case SecurityXXX2005Strings.EncryptedKeyTokenType:
                            case SecurityJan2004Strings.KerberosTokenTypeGSS:
                                break;

                            default:
                                tokenTypeUri = null;
                                break;
                        }
                    }
                    else
                    {
                        tokenTypeUri = str.GetTokenTypeUri();
                    }

                    return tokenTypeUri;
                }
                else
                {
                    return null;
                }
            }

            private bool EmitTokenType(StrEntry str)
            {
                // On .NET Framework, only returns true for scenarios unsupported on Core, e.g. SAML
                return false;
            }
        }

        private class X509ThumbprintStrEntry : KeyIdentifierStrEntry
        {
            protected override Type ClauseType { get { return typeof(X509ThumbprintKeyIdentifierClause); } }
            public override Type TokenType { get { return typeof(X509SecurityToken); } }
            protected override string ValueTypeUri { get { return SecurityXXX2005Strings.ThumbprintSha1ValueType; } }

            public X509ThumbprintStrEntry(bool emitBspRequiredAttributes)
                : base(emitBspRequiredAttributes)
            {
            }

            protected override SecurityKeyIdentifierClause CreateClause(byte[] bytes, byte[] derivationNonce, int derivationLength)
            {
                return new X509ThumbprintKeyIdentifierClause(bytes);
            }
            public override string GetTokenTypeUri()
            {
                return XD.SecurityXXX2005Dictionary.ThumbprintSha1ValueType.Value;
            }
        }
    }
}
