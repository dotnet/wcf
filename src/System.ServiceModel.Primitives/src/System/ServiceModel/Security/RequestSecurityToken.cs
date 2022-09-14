// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using System.Runtime.Serialization;
using System.Xml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Globalization;
using System.Security.Authentication.ExtendedProtection;

namespace System.ServiceModel.Security
{
    internal class RequestSecurityToken : BodyWriter
    {
#pragma warning disable CS0649 // Field is never assign to
        private string _context;
        private string _tokenType;
        private string _requestType;
        private SecurityToken _entropyToken;
        private BinaryNegotiation _negotiationData;
        private XmlElement _rstXml;
        private IList<XmlElement> _requestProperties;
        private ArraySegment<byte> _cachedWriteBuffer;
        private int _cachedWriteBufferLength;
        private int _keySize;
        private SecurityKeyIdentifierClause _renewTarget;
        private SecurityKeyIdentifierClause _closeTarget;
        private SecurityStandardsManager _standardsManager;
        private bool _isReadOnly;
        private object _appliesTo;
        private DataContractSerializer _appliesToSerializer;
        private Type _appliesToType;
#pragma warning restore CS0649 // Field is never assign to

        private object _thisLock = new Object();

        public RequestSecurityToken()
            : this(SecurityStandardsManager.DefaultInstance)
        {
        }

        public RequestSecurityToken(MessageSecurityVersion messageSecurityVersion, SecurityTokenSerializer securityTokenSerializer)
            : this(SecurityUtils.CreateSecurityStandardsManager(messageSecurityVersion, securityTokenSerializer))
        {
        }


        public RequestSecurityToken(MessageSecurityVersion messageSecurityVersion,
                                    SecurityTokenSerializer securityTokenSerializer,
                                    XmlElement requestSecurityTokenXml,
                                    string context,
                                    string tokenType,
                                    string requestType,
                                    int keySize,
                                    SecurityKeyIdentifierClause renewTarget,
                                    SecurityKeyIdentifierClause closeTarget)
            : this(SecurityUtils.CreateSecurityStandardsManager(messageSecurityVersion, securityTokenSerializer),
                   requestSecurityTokenXml,
                   context,
                   tokenType,
                   requestType,
                   keySize,
                   renewTarget,
                   closeTarget)
        {
        }

        public RequestSecurityToken(XmlElement requestSecurityTokenXml,
                                    string context,
                                    string tokenType,
                                    string requestType,
                                    int keySize,
                                    SecurityKeyIdentifierClause renewTarget,
                                    SecurityKeyIdentifierClause closeTarget)
            : this(SecurityStandardsManager.DefaultInstance,
                   requestSecurityTokenXml,
                   context,
                   tokenType,
                   requestType,
                   keySize,
                   renewTarget,
                   closeTarget)
        {
        }

        internal RequestSecurityToken(SecurityStandardsManager standardsManager,
                                      XmlElement rstXml,
                                      string context,
                                      string tokenType,
                                      string requestType,
                                      int keySize,
                                      SecurityKeyIdentifierClause renewTarget,
                                      SecurityKeyIdentifierClause closeTarget)
            : base(true)
        {
            _standardsManager = standardsManager ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(standardsManager)));
            _rstXml = rstXml ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(rstXml));
            _context = context;
            _tokenType = tokenType;
            _keySize = keySize;
            _requestType = requestType;
            _renewTarget = renewTarget;
            _closeTarget = closeTarget;
            IsReceiver = true;
            _isReadOnly = true;
        }

        internal RequestSecurityToken(SecurityStandardsManager standardsManager)
            : this(standardsManager, true)
        {
            // no op
        }

        internal RequestSecurityToken(SecurityStandardsManager standardsManager, bool isBuffered)
            : base(isBuffered)
        {
            _standardsManager = standardsManager ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(standardsManager)));
            _requestType = _standardsManager.TrustDriver.RequestTypeIssue;
            _requestProperties = null;
            IsReceiver = false;
            _isReadOnly = false;
        }

        public ChannelBinding GetChannelBinding()
        {
            if (Message == null)
            {
                return null;
            }

            ChannelBindingMessageProperty channelBindingMessageProperty = null;
            ChannelBindingMessageProperty.TryGet(Message, out channelBindingMessageProperty);
            ChannelBinding channelBinding = null;

            if (channelBindingMessageProperty != null)
            {
                channelBinding = channelBindingMessageProperty.ChannelBinding;
            }

            return channelBinding;
        }

        /// <summary>
        /// Will hold a reference to the outbound message from which we will fish the ChannelBinding out of.
        /// </summary>
        public Message Message { get; set; }


        public string Context
        {
            get
            {
                return _context;
            }
            set
            {
                if (IsReadOnly)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.ObjectIsReadOnly));
                }

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
                if (IsReadOnly)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.ObjectIsReadOnly));
                }

                _tokenType = value;
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
                if (IsReadOnly)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.ObjectIsReadOnly));
                }

                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value), SRP.ValueMustBeNonNegative));
                }

                _keySize = value;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
        }

        public IEnumerable<XmlElement> RequestProperties
        {
            get
            {
                if (IsReceiver)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.ItemNotAvailableInDeserializedRST, "RequestProperties")));
                }
                return _requestProperties;
            }
            set
            {
                if (IsReadOnly)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.ObjectIsReadOnly));
                }

                if (value != null)
                {
                    int index = 0;
                    Collection<XmlElement> coll = new Collection<XmlElement>();
                    foreach (XmlElement property in value)
                    {
                        if (property == null)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(String.Format(CultureInfo.InvariantCulture, "value[{0}]", index)));
                        }

                        coll.Add(property);
                        ++index;
                    }
                    _requestProperties = coll;
                }
                else
                {
                    _requestProperties = null;
                }
            }
        }

        public string RequestType
        {
            get
            {
                return _requestType;
            }
            set
            {
                if (IsReadOnly)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.ObjectIsReadOnly));
                }

                _requestType = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
            }
        }

        public SecurityKeyIdentifierClause RenewTarget
        {
            get
            {
                return _renewTarget;
            }
            set
            {
                if (IsReadOnly)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.ObjectIsReadOnly));
                }

                _renewTarget = value;
            }
        }

        public SecurityKeyIdentifierClause CloseTarget
        {
            get
            {
                return _closeTarget;
            }
            set
            {
                if (IsReadOnly)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.ObjectIsReadOnly));
                }

                _closeTarget = value;
            }
        }

        public XmlElement RequestSecurityTokenXml
        {
            get
            {
                if (!IsReceiver)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.ItemAvailableInDeserializedRSTOnly, "RequestSecurityTokenXml")));
                }
                return _rstXml;
            }
        }

        internal bool IsReceiver { get; }

        internal object AppliesTo
        {
            get
            {
                if (IsReceiver)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.ItemNotAvailableInDeserializedRST, nameof(AppliesTo))));
                }
                return _appliesTo;
            }
        }

        internal DataContractSerializer AppliesToSerializer
        {
            get
            {
                if (IsReceiver)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.ItemNotAvailableInDeserializedRST, nameof(AppliesToSerializer))));
                }
                return _appliesToSerializer;
            }
        }

        internal Type AppliesToType
        {
            get
            {
                if (IsReceiver)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.ItemNotAvailableInDeserializedRST, nameof(AppliesToType))));
                }
                return _appliesToType;
            }
        }

        internal BinaryNegotiation GetBinaryNegotiation()
        {
            if (IsReceiver)
            {
                return _standardsManager.TrustDriver.GetBinaryNegotiation(this);
            }

            return _negotiationData;
        }

        public SecurityToken GetRequestorEntropy()
        {
            return GetRequestorEntropy(null);
        }

        internal SecurityToken GetRequestorEntropy(SecurityTokenResolver resolver)
        {
            if (IsReceiver)
            {
                return _standardsManager.TrustDriver.GetEntropy(this, resolver);
            }
            else
            {
                return _entropyToken;
            }
        }

        public void SetRequestorEntropy(byte[] entropy)
        {
            if (IsReadOnly)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.ObjectIsReadOnly));
            }

            _entropyToken = (entropy != null) ? new NonceToken(entropy) : null;
        }

        public T GetAppliesTo<T>(XmlObjectSerializer serializer)
        {
            if (IsReceiver)
            {
                if (serializer == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(serializer));
                }
                return _standardsManager.TrustDriver.GetAppliesTo<T>(this, serializer);
            }
            else
            {
                return (T)_appliesTo;
            }
        }

        private void OnWriteTo(XmlWriter writer)
        {
            if (IsReceiver)
            {
                _rstXml.WriteTo(writer);
            }
            else
            {
                _standardsManager.TrustDriver.WriteRequestSecurityToken(this, writer);
            }
        }

        public void WriteTo(XmlWriter writer)
        {
            if (writer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(writer));
            }

            if (IsReadOnly)
            {
                // cache the serialized bytes to ensure repeatability
                if (_cachedWriteBuffer.Array == null)
                {
                    MemoryStream stream = new MemoryStream();
                    using (XmlDictionaryWriter binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(stream, XD.Dictionary))
                    {
                        OnWriteTo(binaryWriter);
                        binaryWriter.Flush();
                        stream.Flush();
                        stream.Seek(0, SeekOrigin.Begin);

                        bool gotBuffer = stream.TryGetBuffer(out _cachedWriteBuffer);

                        if (!gotBuffer)
                        {
                            throw new UnauthorizedAccessException(SRP.UnauthorizedAccess_MemStreamBuffer);
                        }

                        _cachedWriteBufferLength = (int)stream.Length;
                    }
                }
                writer.WriteNode(XmlDictionaryReader.CreateBinaryReader(_cachedWriteBuffer.Array, 0, _cachedWriteBufferLength, XD.Dictionary, XmlDictionaryReaderQuotas.Max), false);
            }
            else
            {
                OnWriteTo(writer);
            }
        }

        internal static RequestSecurityToken CreateFrom(SecurityStandardsManager standardsManager, XmlReader reader)
        {
            return standardsManager.TrustDriver.CreateRequestSecurityToken(reader);
        }

        public void MakeReadOnly()
        {
            if (!_isReadOnly)
            {
                _isReadOnly = true;
                if (_requestProperties != null)
                {
                    _requestProperties = new ReadOnlyCollection<XmlElement>(_requestProperties);
                }
                OnMakeReadOnly();
            }
        }

        internal protected virtual void OnWriteCustomAttributes(XmlWriter writer) { }

        internal protected virtual void OnWriteCustomElements(XmlWriter writer) { }

        internal protected virtual void OnMakeReadOnly() { }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            WriteTo(writer);
        }
    }
}
