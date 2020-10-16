// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using Microsoft.Xml;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel.Security
{
    internal class RequestSecurityTokenResponse : BodyWriter
    {
        private SecurityStandardsManager _standardsManager;
        private string _context;
        private int _keySize;
        private bool _computeKey;
        private string _tokenType;
        private SecurityKeyIdentifierClause _requestedAttachedReference;
        private SecurityKeyIdentifierClause _requestedUnattachedReference;
        private SecurityToken _issuedToken;
        private SecurityToken _proofToken;
        private XmlElement _rstrXml;
        private DateTime _effectiveTime;
        private DateTime _expirationTime;
        private bool _isLifetimeSet;
        private bool _isReceiver;
        private bool _isReadOnly;
        private ArraySegment<byte> _cachedWriteBuffer;
        private int _cachedWriteBufferLength;
        private bool _isRequestedTokenClosed;
        private object _appliesTo;
        private XmlObjectSerializer _appliesToSerializer;
        private Type _appliesToType;
        private Object _thisLock = new Object();
        private System.IdentityModel.XmlBuffer _issuedTokenBuffer;

        public RequestSecurityTokenResponse()
            : this(SecurityStandardsManager.DefaultInstance)
        {
        }

        public RequestSecurityTokenResponse(MessageSecurityVersion messageSecurityVersion, SecurityTokenSerializer securityTokenSerializer)
            : this(SecurityUtils.CreateSecurityStandardsManager(messageSecurityVersion, securityTokenSerializer))
        {
        }

        public RequestSecurityTokenResponse(XmlElement requestSecurityTokenResponseXml,
                                            string context,
                                            string tokenType,
                                            int keySize,
                                            SecurityKeyIdentifierClause requestedAttachedReference,
                                            SecurityKeyIdentifierClause requestedUnattachedReference,
                                            bool computeKey,
                                            DateTime validFrom,
                                            DateTime validTo,
                                            bool isRequestedTokenClosed)
            : this(SecurityStandardsManager.DefaultInstance,
                   requestSecurityTokenResponseXml,
                   context,
                   tokenType,
                   keySize,
                   requestedAttachedReference,
                   requestedUnattachedReference,
                   computeKey,
                   validFrom,
                   validTo,
                   isRequestedTokenClosed,
                   null)
        {
        }

        public RequestSecurityTokenResponse(MessageSecurityVersion messageSecurityVersion,
                                            SecurityTokenSerializer securityTokenSerializer,
                                            XmlElement requestSecurityTokenResponseXml,
                                            string context,
                                            string tokenType,
                                            int keySize,
                                            SecurityKeyIdentifierClause requestedAttachedReference,
                                            SecurityKeyIdentifierClause requestedUnattachedReference,
                                            bool computeKey,
                                            DateTime validFrom,
                                            DateTime validTo,
                                            bool isRequestedTokenClosed)
            : this(SecurityUtils.CreateSecurityStandardsManager(messageSecurityVersion, securityTokenSerializer),
                   requestSecurityTokenResponseXml,
                   context,
                   tokenType,
                   keySize,
                   requestedAttachedReference,
                   requestedUnattachedReference,
                   computeKey,
                   validFrom,
                   validTo,
                   isRequestedTokenClosed,
                   null)
        {
        }

        internal RequestSecurityTokenResponse(SecurityStandardsManager standardsManager)
            : base(true)
        {
            if (standardsManager == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("standardsManager"));
            }
            _standardsManager = standardsManager;
            _effectiveTime = SecurityUtils.MinUtcDateTime;
            _expirationTime = SecurityUtils.MaxUtcDateTime;
            _isRequestedTokenClosed = false;
            _isLifetimeSet = false;
            _isReceiver = false;
            _isReadOnly = false;
        }

        internal RequestSecurityTokenResponse(SecurityStandardsManager standardsManager,
                                              XmlElement rstrXml,
                                              string context,
                                              string tokenType,
                                              int keySize,
                                              SecurityKeyIdentifierClause requestedAttachedReference,
                                              SecurityKeyIdentifierClause requestedUnattachedReference,
                                              bool computeKey,
                                              DateTime validFrom,
                                              DateTime validTo,
                                              bool isRequestedTokenClosed,
                                              System.IdentityModel.XmlBuffer issuedTokenBuffer)
            : base(true)
        {
            if (standardsManager == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("standardsManager"));
            }
            _standardsManager = standardsManager;
            if (rstrXml == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rstrXml");
            _rstrXml = rstrXml;
            _context = context;
            _tokenType = tokenType;
            _keySize = keySize;
            _requestedAttachedReference = requestedAttachedReference;
            _requestedUnattachedReference = requestedUnattachedReference;
            _computeKey = computeKey;
            _effectiveTime = validFrom.ToUniversalTime();
            _expirationTime = validTo.ToUniversalTime();
            _isLifetimeSet = true;
            _isRequestedTokenClosed = isRequestedTokenClosed;
            _issuedTokenBuffer = issuedTokenBuffer;
            _isReceiver = true;
            _isReadOnly = true;
        }

        public string Context
        {
            get
            {
                return _context;
            }
            set
            {
                if (this.IsReadOnly)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));
                _context = value;
            }
        }

        public string TokenType
        {
            get
            {
                return _tokenType;
            }
            set
            {
                if (this.IsReadOnly)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));
                _tokenType = value;
            }
        }

        public SecurityKeyIdentifierClause RequestedAttachedReference
        {
            get
            {
                return _requestedAttachedReference;
            }
            set
            {
                if (this.IsReadOnly)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));
                _requestedAttachedReference = value;
            }
        }

        public SecurityKeyIdentifierClause RequestedUnattachedReference
        {
            get
            {
                return _requestedUnattachedReference;
            }
            set
            {
                if (this.IsReadOnly)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));
                _requestedUnattachedReference = value;
            }
        }

        public DateTime ValidFrom
        {
            get
            {
                return _effectiveTime;
            }
        }

        public DateTime ValidTo
        {
            get
            {
                return _expirationTime;
            }
        }

        public bool ComputeKey
        {
            get
            {
                return _computeKey;
            }
            set
            {
                if (this.IsReadOnly)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));
                _computeKey = value;
            }
        }

        public int KeySize
        {
            get
            {
                return _keySize;
            }
            set
            {
                if (this.IsReadOnly)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));
                if (value < 0)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", SRServiceModel.ValueMustBeNonNegative));
                _keySize = value;
            }
        }

        public bool IsRequestedTokenClosed
        {
            get
            {
                return _isRequestedTokenClosed;
            }
            set
            {
                if (this.IsReadOnly)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));
                _isRequestedTokenClosed = value;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
        }

        protected Object ThisLock
        {
            get
            {
                return _thisLock;
            }
        }

        internal bool IsReceiver
        {
            get
            {
                return _isReceiver;
            }
        }

        internal SecurityStandardsManager StandardsManager
        {
            get
            {
                return _standardsManager;
            }
            set
            {
                if (this.IsReadOnly)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));
                _standardsManager = (value != null ? value : SecurityStandardsManager.DefaultInstance);
            }
        }

        public SecurityToken EntropyToken
        {
            get
            {
                if (_isReceiver)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.ItemNotAvailableInDeserializedRSTR, "EntropyToken")));
                }
                return null;
            }
        }

        public SecurityToken RequestedSecurityToken
        {
            get
            {
                if (_isReceiver)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.ItemNotAvailableInDeserializedRSTR, "IssuedToken")));
                }
                return _issuedToken;
            }
            set
            {
                if (_isReadOnly)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));
                _issuedToken = value;
            }
        }

        public SecurityToken RequestedProofToken
        {
            get
            {
                if (_isReceiver)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.ItemNotAvailableInDeserializedRSTR, "ProofToken")));
                }
                return _proofToken;
            }
            set
            {
                if (_isReadOnly)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));
                _proofToken = value;
            }
        }

        public XmlElement RequestSecurityTokenResponseXml
        {
            get
            {
                if (!_isReceiver)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.ItemAvailableInDeserializedRSTROnly, "RequestSecurityTokenXml")));
                }
                return _rstrXml;
            }
        }

        internal object AppliesTo
        {
            get
            {
                if (_isReceiver)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.ItemNotAvailableInDeserializedRST, "AppliesTo")));
                }
                return _appliesTo;
            }
        }

        internal XmlObjectSerializer AppliesToSerializer
        {
            get
            {
                if (_isReceiver)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.ItemNotAvailableInDeserializedRST, "AppliesToSerializer")));
                }
                return _appliesToSerializer;
            }
        }

        internal Type AppliesToType
        {
            get
            {
                if (_isReceiver)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.ItemNotAvailableInDeserializedRST, "AppliesToType")));
                }
                return _appliesToType;
            }
        }

        internal bool IsLifetimeSet
        {
            get
            {
                if (_isReceiver)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.ItemNotAvailableInDeserializedRSTR, "IsLifetimeSet")));
                }
                return _isLifetimeSet;
            }
        }


        internal SecurityToken GetIssuerEntropy(SecurityTokenResolver resolver)
        {
            if (_isReceiver)
            {
                return _standardsManager.TrustDriver.GetEntropy(this, resolver);
            }
            else
                return null;
        }

        public void SetLifetime(DateTime validFrom, DateTime validTo)
        {
            if (this.IsReadOnly)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));
            if (validFrom.ToUniversalTime() > validTo.ToUniversalTime())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRServiceModel.EffectiveGreaterThanExpiration);
            }
            _effectiveTime = validFrom.ToUniversalTime();
            _expirationTime = validTo.ToUniversalTime();
            _isLifetimeSet = true;
        }

        public void SetAppliesTo<T>(T appliesTo, XmlObjectSerializer serializer)
        {
            if (this.IsReadOnly)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));
            if (appliesTo != null && serializer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serializer");
            }
            _appliesTo = appliesTo;
            _appliesToSerializer = serializer;
            _appliesToType = typeof(T);
        }

        public void GetAppliesToQName(out string localName, out string namespaceUri)
        {
            if (!_isReceiver)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.ItemAvailableInDeserializedRSTOnly, "MatchesAppliesTo")));
            _standardsManager.TrustDriver.GetAppliesToQName(this, out localName, out namespaceUri);
        }

        public T GetAppliesTo<T>()
        {
            return this.GetAppliesTo<T>(DataContractSerializerDefaults.CreateSerializer(typeof(T), DataContractSerializerDefaults.MaxItemsInObjectGraph));
        }

        public T GetAppliesTo<T>(XmlObjectSerializer serializer)
        {
            if (_isReceiver)
            {
                if (serializer == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serializer");
                }
                return _standardsManager.TrustDriver.GetAppliesTo<T>(this, serializer);
            }
            else
            {
                return (T)_appliesTo;
            }
        }


        private void OnWriteTo(XmlWriter w)
        {
            if (_isReceiver)
            {
                _rstrXml.WriteTo(w);
            }
            else
            {
                _standardsManager.TrustDriver.WriteRequestSecurityTokenResponse(this, w);
            }
        }

        public void WriteTo(XmlWriter writer)
        {
            if (writer == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");
            if (this.IsReadOnly)
            {
                // cache the serialized bytes to ensure repeatability
                if (_cachedWriteBuffer.Array == null)
                {
                    MemoryStream stream = new MemoryStream();
                    using (XmlDictionaryWriter binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(stream, XD.Dictionary))
                    {
                        this.OnWriteTo(binaryWriter);
                        binaryWriter.Flush();
                        stream.Flush();
                        stream.Seek(0, SeekOrigin.Begin);
                        bool gotBuffer = stream.TryGetBuffer(out _cachedWriteBuffer);

                        if (!gotBuffer)
                        {
                            throw new UnauthorizedAccessException(SRServiceModel.UnauthorizedAccess_MemStreamBuffer);
                        }

                        _cachedWriteBufferLength = (int)stream.Length;
                    }
                }
                writer.WriteNode(XmlDictionaryReader.CreateBinaryReader(_cachedWriteBuffer.Array, 0, _cachedWriteBufferLength, XD.Dictionary, XmlDictionaryReaderQuotas.Max), false);
            }
            else
                this.OnWriteTo(writer);
        }

        public static RequestSecurityTokenResponse CreateFrom(XmlReader reader)
        {
            return CreateFrom(SecurityStandardsManager.DefaultInstance, reader);
        }

        public static RequestSecurityTokenResponse CreateFrom(XmlReader reader, MessageSecurityVersion messageSecurityVersion, SecurityTokenSerializer securityTokenSerializer)
        {
            return CreateFrom(SecurityUtils.CreateSecurityStandardsManager(messageSecurityVersion, securityTokenSerializer), reader);
        }

        internal static RequestSecurityTokenResponse CreateFrom(SecurityStandardsManager standardsManager, XmlReader reader)
        {
            return standardsManager.TrustDriver.CreateRequestSecurityTokenResponse(reader);
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            WriteTo(writer);
        }

        public void MakeReadOnly()
        {
            if (!_isReadOnly)
            {
                _isReadOnly = true;
                this.OnMakeReadOnly();
            }
        }

        protected internal virtual void OnWriteCustomAttributes(XmlWriter writer)
        { }

        protected internal virtual void OnWriteCustomElements(XmlWriter writer)
        { }

        protected virtual void OnMakeReadOnly() { }
    }
}
