// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;

namespace System.IdentityModel.Tokens
{
    public class SamlSerializer
    {
        private DictionaryManager _dictionaryManager;

        public SamlSerializer()
        {
        }

        // Interface to plug in external Dictionaries. The external
        // dictionary should already be populated with all strings 
        // required by this assembly.
        public void PopulateDictionary(IXmlDictionary dictionary)
        {
            if (dictionary == null)
                throw ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("dictionary");

            _dictionaryManager = new DictionaryManager(dictionary);
        }

        internal DictionaryManager DictionaryManager
        {
            get
            {
                if (_dictionaryManager == null)
                    _dictionaryManager = new DictionaryManager();

                return _dictionaryManager;
            }
        }

        // Issue #31 in progress
        //        public virtual SamlSecurityToken ReadToken(XmlReader reader, SecurityTokenSerializer keyInfoSerializer, SecurityTokenResolver outOfBandTokenResolver)
        //        {
        //            if (reader == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");

        //            XmlDictionaryReader dictionaryReader = XmlDictionaryReader.CreateDictionaryReader(reader);
        //            WrappedReader wrappedReader = new WrappedReader(dictionaryReader);

        //            SamlAssertion assertion = LoadAssertion(wrappedReader, keyInfoSerializer, outOfBandTokenResolver);
        //            if (assertion == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SAMLUnableToLoadAssertion)));

        //            //if (assertion.Signature == null)
        //            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SamlTokenMissingSignature)));

        //            return new SamlSecurityToken(assertion);
        //        }

        //        public virtual void WriteToken(SamlSecurityToken token, XmlWriter writer, SecurityTokenSerializer keyInfoSerializer)
        //        {
        //            if (token == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");

        //            token.Assertion.WriteTo(writer, this, keyInfoSerializer);
        //        }

        //        public virtual SamlAssertion LoadAssertion(XmlDictionaryReader reader, SecurityTokenSerializer keyInfoSerializer, SecurityTokenResolver outOfBandTokenResolver)
        //        {
        //            if (reader == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");

        //            SamlAssertion assertion = new SamlAssertion();
        //            assertion.ReadXml(reader, this, keyInfoSerializer, outOfBandTokenResolver);

        //            return assertion;
        //        }

        //        public virtual SamlCondition LoadCondition(XmlDictionaryReader reader, SecurityTokenSerializer keyInfoSerializer, SecurityTokenResolver outOfBandTokenResolver)
        //        {
        //            if (reader == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");

        //            if (reader.IsStartElement(DictionaryManager.SamlDictionary.AudienceRestrictionCondition, DictionaryManager.SamlDictionary.Namespace))
        //            {
        //                SamlAudienceRestrictionCondition audienceRestriction = new SamlAudienceRestrictionCondition();
        //                audienceRestriction.ReadXml(reader, this, keyInfoSerializer, outOfBandTokenResolver);
        //                return audienceRestriction;
        //            }
        //            else if (reader.IsStartElement(DictionaryManager.SamlDictionary.DoNotCacheCondition, DictionaryManager.SamlDictionary.Namespace))
        //            {
        //                SamlDoNotCacheCondition doNotCacheCondition = new SamlDoNotCacheCondition();
        //                doNotCacheCondition.ReadXml(reader, this, keyInfoSerializer, outOfBandTokenResolver);
        //                return doNotCacheCondition;
        //            }
        //            else
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.Format(SR.SAMLUnableToLoadUnknownElement, reader.LocalName)));
        //        }

        //        public virtual SamlConditions LoadConditions(XmlDictionaryReader reader, SecurityTokenSerializer keyInfoSerializer, SecurityTokenResolver outOfBandTokenResolver)
        //        {
        //            if (reader == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");

        //            SamlConditions conditions = new SamlConditions();
        //            conditions.ReadXml(reader, this, keyInfoSerializer, outOfBandTokenResolver);

        //            return conditions;
        //        }

        //        public virtual SamlAdvice LoadAdvice(XmlDictionaryReader reader, SecurityTokenSerializer keyInfoSerializer, SecurityTokenResolver outOfBandTokenResolver)
        //        {
        //            if (reader == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");

        //            SamlAdvice advice = new SamlAdvice();
        //            advice.ReadXml(reader, this, keyInfoSerializer, outOfBandTokenResolver);

        //            return advice;
        //        }

        //        public virtual SamlStatement LoadStatement(XmlDictionaryReader reader, SecurityTokenSerializer keyInfoSerializer, SecurityTokenResolver outOfBandTokenResolver)
        //        {
        //            if (reader == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");

        //            if (reader.IsStartElement(DictionaryManager.SamlDictionary.AuthenticationStatement, DictionaryManager.SamlDictionary.Namespace))
        //            {
        //                SamlAuthenticationStatement authStatement = new SamlAuthenticationStatement();
        //                authStatement.ReadXml(reader, this, keyInfoSerializer, outOfBandTokenResolver);
        //                return authStatement;
        //            }
        //            else if (reader.IsStartElement(DictionaryManager.SamlDictionary.AttributeStatement, DictionaryManager.SamlDictionary.Namespace))
        //            {
        //                SamlAttributeStatement attrStatement = new SamlAttributeStatement();
        //                attrStatement.ReadXml(reader, this, keyInfoSerializer, outOfBandTokenResolver);
        //                return attrStatement;
        //            }
        //            else if (reader.IsStartElement(DictionaryManager.SamlDictionary.AuthorizationDecisionStatement, DictionaryManager.SamlDictionary.Namespace))
        //            {
        //                SamlAuthorizationDecisionStatement authDecisionStatement = new SamlAuthorizationDecisionStatement();
        //                authDecisionStatement.ReadXml(reader, this, keyInfoSerializer, outOfBandTokenResolver);
        //                return authDecisionStatement;
        //            }
        //            else
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.Format(SR.SAMLUnableToLoadUnknownElement, reader.LocalName)));
        //        }

        //        public virtual SamlAttribute LoadAttribute(XmlDictionaryReader reader, SecurityTokenSerializer keyInfoSerializer, SecurityTokenResolver outOfBandTokenResolver)
        //        {
        //            // We will load all attributes as string values.
        //            SamlAttribute attribute = new SamlAttribute();
        //            attribute.ReadXml(reader, this, keyInfoSerializer, outOfBandTokenResolver);

        //            return attribute;
        //        }


        //        // Helper metods to read and write SecurityKeyIdentifiers.
        //        internal static SecurityKeyIdentifier ReadSecurityKeyIdentifier(XmlReader reader, SecurityTokenSerializer tokenSerializer)
        //        {
        //            if (tokenSerializer == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenSerializer", SR.Format(SR.SamlSerializerRequiresExternalSerializers));

        //            if (tokenSerializer.CanReadKeyIdentifier(reader))
        //            {
        //                return tokenSerializer.ReadKeyIdentifier(reader);
        //            }

        //            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SamlSerializerUnableToReadSecurityKeyIdentifier)));
        //        }

        //        internal static void WriteSecurityKeyIdentifier(XmlWriter writer, SecurityKeyIdentifier ski, SecurityTokenSerializer tokenSerializer)
        //        {
        //            if (tokenSerializer == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenSerializer", SR.Format(SR.SamlSerializerRequiresExternalSerializers));

        //            bool keyWritten = false;
        //            if (tokenSerializer.CanWriteKeyIdentifier(ski))
        //            {
        //                tokenSerializer.WriteKeyIdentifier(writer, ski);
        //                keyWritten = true;
        //            }

        //            if (!keyWritten)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SamlSerializerUnableToWriteSecurityKeyIdentifier, ski.ToString())));
        //        }

        //        internal static SecurityKey ResolveSecurityKey(SecurityKeyIdentifier ski, SecurityTokenResolver tokenResolver)
        //        {
        //            if (ski == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("ski");

        //            if (tokenResolver != null)
        //            {
        //                for (int i = 0; i < ski.Count; ++i)
        //                {
        //                    SecurityKey key = null;
        //                    if (tokenResolver.TryResolveSecurityKey(ski[i], out key))
        //                        return key;
        //                }
        //            }

        //            if (ski.CanCreateKey)
        //                return ski.CreateKey();

        //            return null;
        //        }

        //        internal static SecurityToken ResolveSecurityToken(SecurityKeyIdentifier ski, SecurityTokenResolver tokenResolver)
        //        {
        //            SecurityToken token = null;

        //            if (tokenResolver != null)
        //            {
        //                tokenResolver.TryResolveToken(ski, out token);
        //            }

        //            if (token == null)
        //            {
        //                // Check if this is a RSA key.
        //                RsaKeyIdentifierClause rsaClause;
        //                if (ski.TryFind<RsaKeyIdentifierClause>(out rsaClause))
        //                    token = new RsaSecurityToken(rsaClause.Rsa);
        //            }

        //            if (token == null)
        //            {
        //                // Check if this is a X509RawDataKeyIdentifier Clause.
        //                X509RawDataKeyIdentifierClause rawDataKeyIdentifierClause;
        //                if (ski.TryFind<X509RawDataKeyIdentifierClause>(out rawDataKeyIdentifierClause))
        //                    token = new X509SecurityToken(new X509Certificate2(rawDataKeyIdentifierClause.GetX509RawData()));
        //            }

        //            return token;
        //        }

    }

}
