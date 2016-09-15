// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IdentityModel.Tokens
{
    public class SamlAssertion // : ICanonicalWriterEndRootElementCallback  // Issue #31 in progress
    {
       string assertionId = SamlConstants.AssertionIdPrefix + Guid.NewGuid().ToString();

        // Issue #31 in progress
        //string issuer;
        //DateTime issueInstant = DateTime.UtcNow.ToUniversalTime();
        //SamlConditions conditions;
        //SamlAdvice advice;
        //readonly ImmutableCollection<SamlStatement> statements = new ImmutableCollection<SamlStatement>();
        //ReadOnlyCollection<SecurityKey> cryptoList;

        //SignedXml signature;
        //SigningCredentials signingCredentials;
        //SecurityKey verificationKey;
        //SecurityToken signingToken;

        //HashStream hashStream;
        //XmlTokenStream tokenStream;
        //SecurityTokenSerializer keyInfoSerializer;
        //DictionaryManager dictionaryManager;
        //XmlTokenStream sourceData;

        //bool isReadOnly = false;

        public SamlAssertion()
        {
        }

        // Issue #31 in progress
        //        public SamlAssertion(
        //            string assertionId,
        //            string issuer,
        //            DateTime issueInstant,
        //            SamlConditions samlConditions,
        //            SamlAdvice samlAdvice,
        //            IEnumerable<SamlStatement> samlStatements
        //            )
        //        {
        //            if (string.IsNullOrEmpty(assertionId))
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.Format(SR.SAMLAssertionIdRequired));

        //            if (!IsAssertionIdValid(assertionId))
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.Format(SR.SAMLAssertionIDIsInvalid, assertionId));

        //            if (string.IsNullOrEmpty(issuer))
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.Format(SR.SAMLAssertionIssuerRequired));

        //            if (samlStatements == null)
        //            {
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("samlStatements");
        //            }

        //            this.assertionId = assertionId;
        //            this.issuer = issuer;
        //            this.issueInstant = issueInstant.ToUniversalTime();
        //            this.conditions = samlConditions;
        //            this.advice = samlAdvice;

        //            foreach (SamlStatement samlStatement in samlStatements)
        //            {
        //                if (samlStatement == null)
        //                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.Format(SR.SAMLEntityCannotBeNullOrEmpty, XD.SamlDictionary.Statement.Value));

        //                this.statements.Add(samlStatement);
        //            }

        //            if (this.statements.Count == 0)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.Format(SR.SAMLAssertionRequireOneStatement));
        //        }

        //        public int MinorVersion
        //        {
        //            get { return SamlConstants.MinorVersionValue; }
        //        }

        //        public int MajorVersion
        //        {
        //            get { return SamlConstants.MajorVersionValue; }
        //        }

        public string AssertionId
        {
            get { return this.assertionId; }
            set
            {
                throw ServiceModel.ExceptionHelper.PlatformNotSupported();  // Issue #31 in progress
                //if (isReadOnly)
                //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.ObjectIsReadOnly)));

                //if (string.IsNullOrEmpty(value))
                //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.Format(SR.SAMLAssertionIdRequired));

                //this.assertionId = value;
            }
        }

        // Issue #31 in progress
        //        /// <summary>
        //        /// Indicates whether this assertion was deserialized from XML source
        //        /// and can re-emit the XML data unchanged.
        //        /// </summary>
        //        /// <remarks>
        //        /// <para>
        //        /// The default implementation preserves the source data when read using
        //        /// Saml2AssertionSerializer.ReadAssertion and is willing to re-emit the
        //        /// original data as long as the Id has not changed from the time that 
        //        /// assertion was read.
        //        /// </para>
        //        /// <para>
        //        /// Note that it is vitally important that SAML assertions with different
        //        /// data have different IDs. If implementing a scheme whereby an assertion
        //        /// "template" is loaded and certain bits of data are filled in, the Id 
        //        /// must be changed.
        //        /// </para>
        //        /// </remarks>
        //        /// <returns></returns>
        //        public virtual bool CanWriteSourceData
        //        {
        //            get { return null != this.sourceData; }
        //        }

        //        public string Issuer
        //        {
        //            get { return this.issuer; }
        //            set
        //            {
        //                if (isReadOnly)
        //                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.ObjectIsReadOnly)));

        //                if (string.IsNullOrEmpty(value))
        //                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.Format(SR.SAMLAssertionIssuerRequired));

        //                this.issuer = value;
        //            }
        //        }

        //        public DateTime IssueInstant
        //        {
        //            get { return this.issueInstant; }
        //            set
        //            {
        //                if (isReadOnly)
        //                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.ObjectIsReadOnly)));

        //                this.issueInstant = value;
        //            }
        //        }

        //        public SamlConditions Conditions
        //        {
        //            get { return this.conditions; }
        //            set
        //            {
        //                if (isReadOnly)
        //                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.ObjectIsReadOnly)));

        //                this.conditions = value;
        //            }
        //        }

        //        public SamlAdvice Advice
        //        {
        //            get { return this.advice; }
        //            set
        //            {
        //                if (isReadOnly)
        //                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.ObjectIsReadOnly)));

        //                this.advice = value;
        //            }
        //        }

        //        public IList<SamlStatement> Statements
        //        {
        //            get
        //            {
        //                return this.statements;
        //            }
        //        }

        //        public SigningCredentials SigningCredentials
        //        {
        //            get { return this.signingCredentials; }
        //            set
        //            {
        //                if (isReadOnly)
        //                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.ObjectIsReadOnly)));

        //                this.signingCredentials = value;
        //            }
        //        }

        //        internal SignedXml Signature
        //        {
        //            get { return this.signature; }
        //        }

        //        internal SecurityKey SignatureVerificationKey
        //        {
        //            get { return this.verificationKey; }
        //        }

        //        public SecurityToken SigningToken
        //        {
        //            get { return this.signingToken; }
        //            set
        //            {
        //                if (isReadOnly)
        //                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.ObjectIsReadOnly)));

        //                this.signingToken = value;
        //            }
        //        }

        //        public bool IsReadOnly
        //        {
        //            get { return this.isReadOnly; }
        //        }

        //        internal ReadOnlyCollection<SecurityKey> SecurityKeys
        //        {
        //            get
        //            {
        //                return this.cryptoList;
        //            }
        //        }

        public void MakeReadOnly()
        {
            throw ServiceModel.ExceptionHelper.PlatformNotSupported();  // Issue #31 in progress
            //if (!this.isReadOnly)
            //{
            //    if (this.conditions != null)
            //        this.conditions.MakeReadOnly();

            //    if (this.advice != null)
            //        this.advice.MakeReadOnly();

            //    foreach (SamlStatement statement in this.statements)
            //    {
            //        statement.MakeReadOnly();
            //    }

            //    this.statements.MakeReadOnly();

            //    if (this.cryptoList == null)
            //    {
            //        this.cryptoList = BuildCryptoList();
            //    }

            //    this.isReadOnly = true;
            //}
        }

        // Issue #31 in progress
        //        /// <summary>
        //        /// Captures the XML source data from an EnvelopedSignatureReader. 
        //        /// </summary>
        //        /// <remarks>
        //        /// The EnvelopedSignatureReader that was used to read the data for this
        //        /// assertion should be passed to this method after the &lt;/Assertion>
        //        /// element has been read. This method will preserve the raw XML data
        //        /// that was read, including the signature, so that it may be re-emitted
        //        /// without changes and without the need to re-sign the data. See 
        //        /// CanWriteSourceData and WriteSourceData.
        //        /// </remarks>
        //        /// <param name="reader"></param>
        //        internal virtual void CaptureSourceData(EnvelopedSignatureReader reader)
        //        {
        //            if (null == reader)
        //            {
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");
        //            }

        //            this.sourceData = reader.XmlTokens;
        //        }

        //        protected void ReadSignature(XmlDictionaryReader reader, SecurityTokenSerializer keyInfoSerializer, SecurityTokenResolver outOfBandTokenResolver, SamlSerializer samlSerializer)
        //        {
        //            if (reader == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");

        //            if (samlSerializer == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("samlSerializer");

        //            if (this.signature != null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SAMLSignatureAlreadyRead)));

        //            // If the reader cannot canonicalize then buffer the signature element to a canonicalizing reader.
        //            XmlDictionaryReader effectiveReader = reader;
        //            if (!effectiveReader.CanCanonicalize)
        //            {
        //                MemoryStream stream = new MemoryStream();
        //                XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter(stream, samlSerializer.DictionaryManager.ParentDictionary);
        //                writer.WriteNode(effectiveReader, false);
        //                writer.Flush();
        //                stream.Position = 0;
        //                effectiveReader = XmlDictionaryReader.CreateBinaryReader(stream.GetBuffer(), 0, (int)stream.Length, samlSerializer.DictionaryManager.ParentDictionary, reader.Quotas);
        //                effectiveReader.MoveToContent();
        //                writer.Close();
        //            }
        //            SignedXml signedXml = new SignedXml(new StandardSignedInfo(samlSerializer.DictionaryManager), samlSerializer.DictionaryManager, keyInfoSerializer);
        //            signedXml.TransformFactory = ExtendedTransformFactory.Instance;
        //            signedXml.ReadFrom(effectiveReader);
        //            SecurityKeyIdentifier securityKeyIdentifier = signedXml.Signature.KeyIdentifier;
        //            this.verificationKey = SamlSerializer.ResolveSecurityKey(securityKeyIdentifier, outOfBandTokenResolver);
        //            if (this.verificationKey == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SAMLUnableToResolveSignatureKey, this.issuer)));

        //            this.signature = signedXml;
        //            this.signingToken = SamlSerializer.ResolveSecurityToken(securityKeyIdentifier, outOfBandTokenResolver);
        //            if (this.signingToken == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SamlSigningTokenNotFound)));

        //            if (!ReferenceEquals(reader, effectiveReader))
        //                effectiveReader.Close();
        //        }

        //        void CheckObjectValidity()
        //        {
        //            if (string.IsNullOrEmpty(this.assertionId))
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SAMLAssertionIdRequired)));

        //            if (!IsAssertionIdValid(this.assertionId))
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SAMLAssertionIDIsInvalid, this.assertionId)));

        //            if (string.IsNullOrEmpty(this.issuer))
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SAMLAssertionIssuerRequired)));

        //            if (this.statements.Count == 0)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SAMLAssertionRequireOneStatement)));
        //        }

        //        bool IsAssertionIdValid(string assertionId)
        //        {
        //            if (string.IsNullOrEmpty(assertionId))
        //                return false;

        //            // The first character of the Assertion ID should be a letter or a '_'
        //            return (((assertionId[0] >= 'A') && (assertionId[0] <= 'Z')) ||
        //                ((assertionId[0] >= 'a') && (assertionId[0] <= 'z')) ||
        //                (assertionId[0] == '_'));
        //        }

        //        ReadOnlyCollection<SecurityKey> BuildCryptoList()
        //        {
        //            List<SecurityKey> cryptoList = new List<SecurityKey>();

        //            for (int i = 0; i < this.statements.Count; ++i)
        //            {
        //                SamlSubjectStatement statement = this.statements[i] as SamlSubjectStatement;
        //                if (statement != null)
        //                {
        //                    bool skipCrypto = false;
        //                    SecurityKey crypto = null;
        //                    if (statement.SamlSubject != null)
        //                        crypto = statement.SamlSubject.Crypto;
        //                    InMemorySymmetricSecurityKey inMemorySymmetricSecurityKey = crypto as InMemorySymmetricSecurityKey;
        //                    if (inMemorySymmetricSecurityKey != null)
        //                    {

        //                        // Verify that you have not already added this to crypto list.
        //                        for (int j = 0; j < cryptoList.Count; ++j)
        //                        {
        //                            if ((cryptoList[j] is InMemorySymmetricSecurityKey) && (cryptoList[j].KeySize == inMemorySymmetricSecurityKey.KeySize))
        //                            {
        //                                byte[] key1 = ((InMemorySymmetricSecurityKey)cryptoList[j]).GetSymmetricKey();
        //                                byte[] key2 = inMemorySymmetricSecurityKey.GetSymmetricKey();
        //                                int k = 0;
        //                                for (k = 0; k < key1.Length; ++k)
        //                                {
        //                                    if (key1[k] != key2[k])
        //                                    {
        //                                        break;
        //                                    }
        //                                }
        //                                skipCrypto = (k == key1.Length);
        //                            }

        //                            if (skipCrypto)
        //                                break;
        //                        }
        //                    }
        //                    if (!skipCrypto && (crypto != null))
        //                    {
        //                        cryptoList.Add(crypto);
        //                    }
        //                }
        //            }

        //            return cryptoList.AsReadOnly();

        //        }

        //        void VerifySignature(SignedXml signature, SecurityKey signatureVerificationKey)
        //        {
        //            if (signature == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("signature");

        //            if (signatureVerificationKey == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("signatureVerificatonKey");

        //            signature.StartSignatureVerification(signatureVerificationKey);
        //            signature.EnsureDigestValidity(this.assertionId, tokenStream);
        //            signature.CompleteSignatureVerification();
        //        }

        //        void ICanonicalWriterEndRootElementCallback.OnEndOfRootElement(XmlDictionaryWriter dictionaryWriter)
        //        {
        //            byte[] hashValue = this.hashStream.FlushHashAndGetValue();

        //            PreDigestedSignedInfo signedInfo = new PreDigestedSignedInfo(this.dictionaryManager);
        //            signedInfo.AddEnvelopedSignatureTransform = true;
        //            signedInfo.CanonicalizationMethod = SecurityAlgorithms.ExclusiveC14n;
        //            signedInfo.SignatureMethod = this.signingCredentials.SignatureAlgorithm;
        //            signedInfo.DigestMethod = this.signingCredentials.DigestAlgorithm;
        //            signedInfo.AddReference(this.assertionId, hashValue);

        //            SignedXml signedXml = new SignedXml(signedInfo, this.dictionaryManager, this.keyInfoSerializer);
        //            signedXml.ComputeSignature(this.signingCredentials.SigningKey);
        //            signedXml.Signature.KeyIdentifier = this.signingCredentials.SigningKeyIdentifier;
        //            signedXml.WriteTo(dictionaryWriter);
        //        }

        //        public virtual void ReadXml(XmlDictionaryReader reader, SamlSerializer samlSerializer, SecurityTokenSerializer keyInfoSerializer, SecurityTokenResolver outOfBandTokenResolver)
        //        {
        //            if (reader == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("ReadXml"));

        //            if (samlSerializer == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("samlSerializer"));

        //            XmlDictionaryReader dictionaryReader = XmlDictionaryReader.CreateDictionaryReader(reader);
        //            WrappedReader wrappedReader = new WrappedReader(dictionaryReader);
        //            SamlDictionary dictionary = samlSerializer.DictionaryManager.SamlDictionary;

        //            if (!wrappedReader.IsStartElement(dictionary.Assertion, dictionary.Namespace))
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SAMLElementNotRecognized, wrappedReader.LocalName)));

        //            string attributeValue = wrappedReader.GetAttribute(dictionary.MajorVersion, null);
        //            if (string.IsNullOrEmpty(attributeValue))
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SAMLAssertionMissingMajorVersionAttributeOnRead)));
        //            int majorVersion = Int32.Parse(attributeValue, CultureInfo.InvariantCulture);

        //            attributeValue = wrappedReader.GetAttribute(dictionary.MinorVersion, null);
        //            if (string.IsNullOrEmpty(attributeValue))
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SAMLAssertionMissingMinorVersionAttributeOnRead)));

        //            int minorVersion = Int32.Parse(attributeValue, CultureInfo.InvariantCulture);

        //            if ((majorVersion != SamlConstants.MajorVersionValue) || (minorVersion != SamlConstants.MinorVersionValue))
        //            {
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SAMLTokenVersionNotSupported, majorVersion, minorVersion, SamlConstants.MajorVersionValue, SamlConstants.MinorVersionValue)));
        //            }

        //            attributeValue = wrappedReader.GetAttribute(dictionary.AssertionId, null);
        //            if (string.IsNullOrEmpty(attributeValue))
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SAMLAssertionIdRequired)));

        //            if (!IsAssertionIdValid(attributeValue))
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SAMLAssertionIDIsInvalid, attributeValue)));

        //            this.assertionId = attributeValue;

        //            attributeValue = wrappedReader.GetAttribute(dictionary.Issuer, null);
        //            if (string.IsNullOrEmpty(attributeValue))
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SAMLAssertionMissingIssuerAttributeOnRead)));
        //            this.issuer = attributeValue;

        //            attributeValue = wrappedReader.GetAttribute(dictionary.IssueInstant, null);
        //            if (!string.IsNullOrEmpty(attributeValue))
        //                this.issueInstant = DateTime.ParseExact(
        //                    attributeValue, SamlConstants.AcceptedDateTimeFormats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None).ToUniversalTime();

        //            wrappedReader.MoveToContent();
        //            wrappedReader.Read();

        //            if (wrappedReader.IsStartElement(dictionary.Conditions, dictionary.Namespace))
        //            {
        //                this.conditions = samlSerializer.LoadConditions(wrappedReader, keyInfoSerializer, outOfBandTokenResolver);
        //                if (this.conditions == null)
        //                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SAMLUnableToLoadCondtions)));
        //            }

        //            if (wrappedReader.IsStartElement(dictionary.Advice, dictionary.Namespace))
        //            {
        //                this.advice = samlSerializer.LoadAdvice(wrappedReader, keyInfoSerializer, outOfBandTokenResolver);
        //                if (this.advice == null)
        //                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SAMLUnableToLoadAdvice)));
        //            }

        //            while (wrappedReader.IsStartElement())
        //            {
        //                if (wrappedReader.IsStartElement(samlSerializer.DictionaryManager.XmlSignatureDictionary.Signature, samlSerializer.DictionaryManager.XmlSignatureDictionary.Namespace))
        //                {
        //                    break;
        //                }
        //                else
        //                {
        //                    SamlStatement statement = samlSerializer.LoadStatement(wrappedReader, keyInfoSerializer, outOfBandTokenResolver);
        //                    if (statement == null)
        //                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SAMLUnableToLoadStatement)));
        //                    this.statements.Add(statement);
        //                }
        //            }

        //            if (this.statements.Count == 0)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.SAMLAssertionRequireOneStatementOnRead)));

        //            if (wrappedReader.IsStartElement(samlSerializer.DictionaryManager.XmlSignatureDictionary.Signature, samlSerializer.DictionaryManager.XmlSignatureDictionary.Namespace))
        //                this.ReadSignature(wrappedReader, keyInfoSerializer, outOfBandTokenResolver, samlSerializer);

        //            wrappedReader.MoveToContent();
        //            wrappedReader.ReadEndElement();

        //            this.tokenStream = wrappedReader.XmlTokens;

        //            if (this.signature != null)
        //            {
        //                VerifySignature(this.signature, this.verificationKey);
        //            }

        //            BuildCryptoList();
        //        }

        //        internal void WriteTo(XmlWriter writer, SamlSerializer samlSerializer, SecurityTokenSerializer keyInfoSerializer)
        //        {
        //            if (writer == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");

        //            if ((this.signingCredentials == null) && (this.signature == null))
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SamlAssertionMissingSigningCredentials)));

        //            XmlDictionaryWriter dictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter(writer);

        //            if (this.signingCredentials != null)
        //            {
        //                using (HashAlgorithm hash = CryptoHelper.CreateHashAlgorithm(this.signingCredentials.DigestAlgorithm))
        //                {
        //                    this.hashStream = new HashStream(hash);
        //                    this.keyInfoSerializer = keyInfoSerializer;
        //                    this.dictionaryManager = samlSerializer.DictionaryManager;
        //                    SamlDelegatingWriter delegatingWriter = new SamlDelegatingWriter(dictionaryWriter, this.hashStream, this, samlSerializer.DictionaryManager.ParentDictionary);
        //                    this.WriteXml(delegatingWriter, samlSerializer, keyInfoSerializer);
        //                }
        //            }
        //            else
        //            {
        //                this.tokenStream.SetElementExclusion(null, null);
        //                this.tokenStream.WriteTo(dictionaryWriter, samlSerializer.DictionaryManager);
        //            }
        //        }

        //        public virtual void WriteXml(XmlDictionaryWriter writer, SamlSerializer samlSerializer, SecurityTokenSerializer keyInfoSerializer)
        //        {
        //            CheckObjectValidity();

        //            if (writer == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");

        //            if (samlSerializer == null)
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("samlSerializer"));

        //            SamlDictionary dictionary = samlSerializer.DictionaryManager.SamlDictionary;

        //            try
        //            {
        //                writer.WriteStartElement(dictionary.PreferredPrefix.Value, dictionary.Assertion, dictionary.Namespace);

        //                writer.WriteStartAttribute(dictionary.MajorVersion, null);
        //                writer.WriteValue(SamlConstants.MajorVersionValue);
        //                writer.WriteEndAttribute();
        //                writer.WriteStartAttribute(dictionary.MinorVersion, null);
        //                writer.WriteValue(SamlConstants.MinorVersionValue);
        //                writer.WriteEndAttribute();
        //                writer.WriteStartAttribute(dictionary.AssertionId, null);
        //                writer.WriteString(this.assertionId);
        //                writer.WriteEndAttribute();
        //                writer.WriteStartAttribute(dictionary.Issuer, null);
        //                writer.WriteString(this.issuer);
        //                writer.WriteEndAttribute();
        //                writer.WriteStartAttribute(dictionary.IssueInstant, null);
        //                writer.WriteString(this.issueInstant.ToString(SamlConstants.GeneratedDateTimeFormat, CultureInfo.InvariantCulture));
        //                writer.WriteEndAttribute();

        //                // Write out conditions
        //                if (this.conditions != null)
        //                {
        //                    this.conditions.WriteXml(writer, samlSerializer, keyInfoSerializer);
        //                }

        //                // Write out advice if there is one
        //                if (this.advice != null)
        //                {
        //                    this.advice.WriteXml(writer, samlSerializer, keyInfoSerializer);
        //                }

        //                for (int i = 0; i < this.statements.Count; i++)
        //                {
        //                    this.statements[i].WriteXml(writer, samlSerializer, keyInfoSerializer);
        //                }

        //                writer.WriteEndElement();
        //            }
        //            catch (Exception e)
        //            {
        //                // Always immediately rethrow fatal exceptions.
        //                if (Fx.IsFatal(e)) throw;

        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SAMLTokenNotSerialized), e));
        //            }
        //        }

        //        /// <summary>
        //        /// Writes the source data, if available.
        //        /// </summary>
        //        /// <exception cref="InvalidOperationException">When no source data is available</exception>
        //        /// <param name="writer"></param>
        //        public virtual void WriteSourceData(XmlWriter writer)
        //        {
        //            if (!this.CanWriteSourceData)
        //            {
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
        //                    new InvalidOperationException(SR.Format(SR.ID4140)));
        //            }

        //            // This call will properly just reuse the existing writer if it already qualifies
        //            XmlDictionaryWriter dictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter(writer);
        //            this.sourceData.SetElementExclusion(null, null);
        //            this.sourceData.GetWriter().WriteTo(dictionaryWriter, null );
        //        }

        //        static internal void AddSamlClaimTypes(ICollection<Type> knownClaimTypes)
        //        {
        //            if (knownClaimTypes == null)
        //            {
        //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("knownClaimTypes");
        //            }
        //            knownClaimTypes.Add(typeof(SamlAuthorizationDecisionClaimResource));
        //            knownClaimTypes.Add(typeof(SamlAuthenticationClaimResource));
        //            knownClaimTypes.Add(typeof(SamlAccessDecision));
        //            knownClaimTypes.Add(typeof(SamlAuthorityBinding));
        //            knownClaimTypes.Add(typeof(SamlNameIdentifierClaimResource));
        //        }
    }
}

