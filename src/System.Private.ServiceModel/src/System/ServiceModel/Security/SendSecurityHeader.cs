// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
    internal abstract class SendSecurityHeader : SecurityHeader, IMessageHeaderWithSharedNamespace
    {
        private bool _basicTokenEncrypted;
        private SendSecurityHeaderElementContainer _elementContainer;
        private bool _primarySignatureDone;
        bool _encryptSignature;
        private SignatureConfirmations _signatureValuesGenerated;
        private SignatureConfirmations _signatureConfirmationsToSend;
        private int _idCounter;
        private string _idPrefix;
        private bool _hasSignedTokens;
        private bool _hasEncryptedTokens;
        private MessagePartSpecification _signatureParts;
        private MessagePartSpecification _encryptionParts;
        private SecurityTokenParameters _signingTokenParameters;
        private SecurityTokenParameters _encryptingTokenParameters;
        private List<SecurityToken> _basicTokens = null;
        private List<SecurityTokenParameters> _basicSupportingTokenParameters = null;
        private List<SecurityTokenParameters> _endorsingTokenParameters = null;
        private List<SecurityTokenParameters> _signedEndorsingTokenParameters = null;
        private List<SecurityTokenParameters> _signedTokenParameters = null;
        private SecurityToken _encryptingToken;
        private bool _skipKeyInfoForEncryption;
        private byte[] _primarySignatureValue = null;
        private bool _shouldProtectTokens;
        private BufferManager _bufferManager;
        private bool _shouldSignToHeader = false;
        private SecurityProtocolCorrelationState _correlationState;
        private bool _signThenEncrypt = true;
        private static readonly string[] s_ids = new string[] { "_0", "_1", "_2", "_3", "_4", "_5", "_6", "_7", "_8", "_9" };

        protected SendSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay,
            SecurityStandardsManager standardsManager,
            SecurityAlgorithmSuite algorithmSuite,
            MessageDirection transferDirection)
            : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, transferDirection)
        {
            _elementContainer = new SendSecurityHeaderElementContainer();
        }

        public SendSecurityHeaderElementContainer ElementContainer
        {
            get { return _elementContainer; }
        }

        public BufferManager StreamBufferManager
        {
            get
            {
                if (_bufferManager == null)
                {
                    _bufferManager = BufferManager.CreateBufferManager(0, int.MaxValue);
                }

                return _bufferManager;
            }
            set
            {
                _bufferManager = value;
            }
        }

        public MessagePartSpecification EncryptionParts
        {
            get { return _encryptionParts; }
            set
            {
                ThrowIfProcessingStarted();
                if (value == null)
                {
                    throw TraceUtility.ThrowHelperError(new ArgumentNullException("value"), this.Message);
                }
                if (!value.IsReadOnly)
                {
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(
                        SR.Format(SR.MessagePartSpecificationMustBeImmutable)), this.Message);
                }
                _encryptionParts = value;
            }
        }

        internal byte[] PrimarySignatureValue
        {
            get { return _primarySignatureValue; }
        }

        XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedNamespace
        {
            get { return XD.UtilityDictionary.Namespace; }
        }

        XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedPrefix
        {
            get { return XD.UtilityDictionary.Prefix; }
        }

        public override string Name
        {
            get { return this.StandardsManager.SecurityVersion.HeaderName.Value; }
        }

        public override string Namespace
        {
            get { return this.StandardsManager.SecurityVersion.HeaderNamespace.Value; }
        }

        public bool SignThenEncrypt
        {
            get { return _signThenEncrypt; }
            set
            {
                ThrowIfProcessingStarted();
                _signThenEncrypt = value;
            }
        }

        public MessagePartSpecification SignatureParts
        {
            get { return _signatureParts; }
            set
            {
                ThrowIfProcessingStarted();
                if (value == null)
                {
                    throw TraceUtility.ThrowHelperError(new ArgumentNullException("value"), this.Message);
                }
                if (!value.IsReadOnly)
                {
                    throw TraceUtility.ThrowHelperError(new InvalidOperationException(
                        SR.Format(SR.MessagePartSpecificationMustBeImmutable)), this.Message);
                }
                _signatureParts = value;
            }
        }

        public bool HasSignedTokens
        {
            get
            {
                return this._hasSignedTokens;
            }
        }

        public bool HasEncryptedTokens
        {
            get
            {
                return this._hasEncryptedTokens;
            }
        }

        void AddParameters(ref List<SecurityTokenParameters> list, SecurityTokenParameters item)
        {
            if (list == null)
            {
                list = new List<SecurityTokenParameters>();
            }
            list.Add(item);
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        public string IdPrefix
        {
            get { return _idPrefix; }
            set
            {
                ThrowIfProcessingStarted();
                _idPrefix = string.IsNullOrEmpty(value) || value == "_" ? null : value;
            }
        }

        public void AddTimestamp(TimeSpan timestampValidityDuration)
        {
            // $$$ throw ExceptionHelper.PlatformNotSupported(); // $$$

            DateTime now = DateTime.UtcNow;
            string id = this.RequireMessageProtection ? SecurityUtils.GenerateId() : GenerateId();
            AddTimestamp(new SecurityTimestamp(now, now + timestampValidityDuration, id));
        }

        public void AddTimestamp(SecurityTimestamp timestamp)
        {
            ThrowIfProcessingStarted();
            if (_elementContainer.Timestamp != null)
            {
                throw TraceUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.TimestampAlreadySetForSecurityHeader)), this.Message);
            }
            if (timestamp == null)
            {
                throw TraceUtility.ThrowHelperArgumentNull("timestamp", this.Message);
            }

            _elementContainer.Timestamp = timestamp;
        }

        public void AddBasicSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
        {
            if (token == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
            if (parameters == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
            ThrowIfProcessingStarted();
            SendSecurityHeaderElement tokenElement = new SendSecurityHeaderElement(token.Id, new TokenElement(token, this.StandardsManager));
            tokenElement.MarkedForEncryption = true;
            _elementContainer.AddBasicSupportingToken(tokenElement);
            _hasEncryptedTokens = true;
            _hasSignedTokens = true;
            AddParameters(ref _basicSupportingTokenParameters, parameters);
            if (_basicTokens == null)
            {
                _basicTokens = new List<SecurityToken>();
            }

            //  We maintain a list of the basic tokens for the SignThenEncrypt case as we will 
            //  need this token to write STR entry on OnWriteHeaderContents. 
            _basicTokens.Add(token);

        }

        public void AddEndorsingSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
        {
            throw ExceptionHelper.PlatformNotSupported();

            // $$$

            //if (token == null)
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
            //if (parameters == null)
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
            //ThrowIfProcessingStarted();
            //this.elementContainer.AddEndorsingSupportingToken(token);
            //// The ProviderBackedSecurityToken was added for the ChannelBindingToken (CBT) effort for win7.  
            //// We can assume the key is of type symmetric key.
            ////
            //// Asking for the key type from the token will cause the ProviderBackedSecurityToken 
            //// to attempt to resolve the token and the nego will start.  
            ////
            //// We don't want that.  
            //// We want to defer the nego until after the CBT is available in SecurityAppliedMessage.OnWriteMessage.
            //if (!(token is ProviderBackedSecurityToken))
            //{
            //    this.shouldSignToHeader |= (!this.RequireMessageProtection) && (SecurityUtils.GetSecurityKey<AsymmetricSecurityKey>(token) != null);
            //}
            //this.AddParameters(ref this.endorsingTokenParameters, parameters);
        }

        public void AddSignedEndorsingSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
        {
            throw ExceptionHelper.PlatformNotSupported();

            // $$$
            //if (token == null)
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
            //if (parameters == null)
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
            //ThrowIfProcessingStarted();
            //this.elementContainer.AddSignedEndorsingSupportingToken(token);
            //hasSignedTokens = true;
            //this.shouldSignToHeader |= (!this.RequireMessageProtection) && (SecurityUtils.GetSecurityKey<AsymmetricSecurityKey>(token) != null);
            //this.AddParameters(ref this.signedEndorsingTokenParameters, parameters);
        }

        public void AddSignedSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
        {
            throw ExceptionHelper.PlatformNotSupported();

            // $$$
            //if (token == null)
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
            //if (parameters == null)
            //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
            //ThrowIfProcessingStarted();
            //this.elementContainer.AddSignedSupportingToken(token);
            //hasSignedTokens = true;
            //this.AddParameters(ref this.signedTokenParameters, parameters);
        }

        public string GenerateId()
        {
            int id = _idCounter++;

            if (_idPrefix != null)
            {
                return _idPrefix + id;
            }

            if (id < s_ids.Length)
            {
                return s_ids[id];
            }
            else
            {
                return "_" + id;
            }
        }

        public Message SetupExecution()
        {
            ThrowIfProcessingStarted();
            SetProcessingStarted();

            bool signBody = false;
            if (_elementContainer.SourceSigningToken != null)
            {
                if (_signatureParts == null)
                {
                    throw TraceUtility.ThrowHelperError(new ArgumentNullException("SignatureParts"), this.Message);
                }
                signBody = _signatureParts.IsBodyIncluded;
            }

            bool encryptBody = false;
            if (_elementContainer.SourceEncryptionToken != null)
            {
                if (_encryptionParts == null)
                {
                    throw TraceUtility.ThrowHelperError(new ArgumentNullException("EncryptionParts"), this.Message);
                }
                encryptBody = _encryptionParts.IsBodyIncluded;
            }

            SecurityAppliedMessage message = new SecurityAppliedMessage(this.Message, this, signBody, encryptBody);
            this.Message = message;
            return message;
        }

        void CompleteSignature()
        {
            throw ExceptionHelper.PlatformNotSupported();   // $$$

            //ISignatureValueSecurityElement signedXml = this.CompletePrimarySignatureCore(
            //    _elementContainer.GetSignatureConfirmations(), _elementContainer.GetSignedEndorsingSupportingTokens(),
            //    _elementContainer.GetSignedSupportingTokens(), _elementContainer.GetBasicSupportingTokens(), true);
            //if (signedXml == null)
            //{
            //    return;
            //}
            //_elementContainer.PrimarySignature = new SendSecurityHeaderElement(signedXml.Id, signedXml);
            //_elementContainer.PrimarySignature.MarkedForEncryption = _encryptSignature;
            //AddGeneratedSignatureValue(signedXml.GetSignatureValue(), this.EncryptPrimarySignature);
            //this.primarySignatureDone = true;
            //this.primarySignatureValue = signedXml.GetSignatureValue();
        }
    }

    class TokenElement : ISecurityElement
    {
        SecurityStandardsManager standardsManager;
        SecurityToken token;

        public TokenElement(SecurityToken token, SecurityStandardsManager standardsManager)
        {
            this.token = token;
            this.standardsManager = standardsManager;
        }

        public override bool Equals(object item)
        {
            TokenElement element = item as TokenElement;
            return (element != null && this.token == element.token && this.standardsManager == element.standardsManager);
        }

        public override int GetHashCode()
        {
            return token.GetHashCode() ^ standardsManager.GetHashCode();
        }

        public bool HasId
        {
            get { return true; }
        }

        public string Id
        {
            get { return token.Id; }
        }

        public SecurityToken Token
        {
            get { return token; }
        }

        public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            standardsManager.SecurityTokenSerializer.WriteToken(writer, token);
        }
    }
}
