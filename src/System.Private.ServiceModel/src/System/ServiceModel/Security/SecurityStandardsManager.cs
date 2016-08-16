// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security.Tokens;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Xml;
using System.Runtime.CompilerServices;

namespace System.ServiceModel.Security
{
    internal class SecurityStandardsManager
    {
#pragma warning disable 0649 // Remove this once we do real implementation, this prevents "field is never assigned to" warning
        private static SecurityStandardsManager s_instance;
        private readonly MessageSecurityVersion _messageSecurityVersion;
        private readonly WSUtilitySpecificationVersion _wsUtilitySpecificationVersion;
        private readonly SecureConversationDriver _secureConversationDriver;
        private readonly TrustDriver _trustDriver;
        private readonly SignatureTargetIdManager _idManager;
        private readonly SecurityTokenSerializer _tokenSerializer;
        private WSSecurityTokenSerializer _wsSecurityTokenSerializer;
#pragma warning restore 0649


        [MethodImpl(MethodImplOptions.NoInlining)]
        public SecurityStandardsManager()
            : this(WSSecurityTokenSerializer.DefaultInstance)
        {
        }

        public SecurityStandardsManager(SecurityTokenSerializer tokenSerializer)
            : this(MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11, tokenSerializer)
        {
        }

        public SecurityStandardsManager(MessageSecurityVersion messageSecurityVersion, SecurityTokenSerializer tokenSerializer)
        {
            if (messageSecurityVersion == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("messageSecurityVersion"));
            if (tokenSerializer == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenSerializer");

            _messageSecurityVersion = messageSecurityVersion;
            _tokenSerializer = tokenSerializer;
            if (messageSecurityVersion.SecureConversationVersion == SecureConversationVersion.WSSecureConversation13)
                _secureConversationDriver = new WSSecureConversationDec2005.DriverDec2005();
            else
                _secureConversationDriver = new WSSecureConversationFeb2005.DriverFeb2005();

            if (this.SecurityVersion == SecurityVersion.WSSecurity10 || this.SecurityVersion == SecurityVersion.WSSecurity11)
            {
                _idManager = WSSecurityJan2004.IdManager.Instance;
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("messageSecurityVersion", SR.Format(SR.MessageSecurityVersionOutOfRange)));
            }

            _wsUtilitySpecificationVersion = WSUtilitySpecificationVersion.Default;
            if (messageSecurityVersion.MessageSecurityTokenVersion.TrustVersion == TrustVersion.WSTrust13)
                _trustDriver = new WSTrustDec2005.DriverDec2005(this);
            else
                _trustDriver = new WSTrustFeb2005.DriverFeb2005(this);
        }

        public static SecurityStandardsManager DefaultInstance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new SecurityStandardsManager();
                return s_instance;
            }
        }

        public SecurityVersion SecurityVersion
        {
            get { return _messageSecurityVersion == null ? null : _messageSecurityVersion.SecurityVersion; }
        }

        public MessageSecurityVersion MessageSecurityVersion
        {
            get { return _messageSecurityVersion; }
        }

        public TrustVersion TrustVersion
        {
            get { return _messageSecurityVersion.TrustVersion; }
        }

        public SecureConversationVersion SecureConversationVersion
        {
            get { return _messageSecurityVersion.SecureConversationVersion; }
        }

        internal SecurityTokenSerializer SecurityTokenSerializer
        {
            get { return _tokenSerializer; }
        }

        internal WSUtilitySpecificationVersion WSUtilitySpecificationVersion
        {
            get { return _wsUtilitySpecificationVersion; }
        }

        internal SignatureTargetIdManager IdManager
        {
            get { return _idManager; }
        }

        internal SecureConversationDriver SecureConversationDriver
        {
            get { return _secureConversationDriver; }
        }

        internal TrustDriver TrustDriver
        {
            get { return _trustDriver; }
        }

        WSSecurityTokenSerializer WSSecurityTokenSerializer
        {
            get
            {
                if (_wsSecurityTokenSerializer == null)
                {
                    WSSecurityTokenSerializer wsSecurityTokenSerializer = _tokenSerializer as WSSecurityTokenSerializer;
                    if (wsSecurityTokenSerializer == null)
                    {
                        wsSecurityTokenSerializer = new WSSecurityTokenSerializer(SecurityVersion);
                    }
                    _wsSecurityTokenSerializer = wsSecurityTokenSerializer;
                }
                return _wsSecurityTokenSerializer;
            }
        }

        internal bool TryCreateKeyIdentifierClauseFromTokenXml(XmlElement element, SecurityTokenReferenceStyle tokenReferenceStyle, out SecurityKeyIdentifierClause securityKeyIdentifierClause)
        {
            return WSSecurityTokenSerializer.TryCreateKeyIdentifierClauseFromTokenXml(element, tokenReferenceStyle, out securityKeyIdentifierClause);
        }

        internal SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromTokenXml(XmlElement element, SecurityTokenReferenceStyle tokenReferenceStyle)
        {
            return WSSecurityTokenSerializer.CreateKeyIdentifierClauseFromTokenXml(element, tokenReferenceStyle);
        }

        internal SendSecurityHeader CreateSendSecurityHeader(Message message,
            string actor, bool mustUnderstand, bool relay,
            SecurityAlgorithmSuite algorithmSuite, MessageDirection direction)
        {
            return SecurityVersion.CreateSendSecurityHeader(message, actor, mustUnderstand, relay, this, algorithmSuite, direction);
        }

        internal ReceiveSecurityHeader CreateReceiveSecurityHeader(Message message,
            string actor,
            SecurityAlgorithmSuite algorithmSuite, MessageDirection direction)
        {
            ReceiveSecurityHeader header = TryCreateReceiveSecurityHeader(message, actor, algorithmSuite, direction);
            if (header == null)
            {
                if (String.IsNullOrEmpty(actor))
                    throw System.ServiceModel.Diagnostics.TraceUtility.ThrowHelperError(new MessageSecurityException(
                        SR.Format(SR.UnableToFindSecurityHeaderInMessageNoActor)), message);
                else
                    throw System.ServiceModel.Diagnostics.TraceUtility.ThrowHelperError(new MessageSecurityException(
                        SR.Format(SR.UnableToFindSecurityHeaderInMessage, actor)), message);
            }
            return header;
        }

        internal ReceiveSecurityHeader TryCreateReceiveSecurityHeader(Message message,
            string actor,
            SecurityAlgorithmSuite algorithmSuite, MessageDirection direction)
        {
            return this.SecurityVersion.TryCreateReceiveSecurityHeader(message, actor, this, algorithmSuite, direction);
        }

        internal bool DoesMessageContainSecurityHeader(Message message)
        {
            return this.SecurityVersion.DoesMessageContainSecurityHeader(message);
        }

        internal bool TryGetSecurityContextIds(Message message, string[] actors, bool isStrictMode, ICollection<UniqueId> results)
        {
            if (results == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("results");
            }
            SecureConversationDriver driver = this.SecureConversationDriver;
            int securityHeaderIndex = this.SecurityVersion.FindIndexOfSecurityHeader(message, actors);
            if (securityHeaderIndex < 0)
            {
                return false;
            }
            bool addedContextIds = false;
            using (XmlDictionaryReader reader = message.Headers.GetReaderAtHeader(securityHeaderIndex))
            {
                if (!reader.IsStartElement())
                {
                    return false;
                }
                if (reader.IsEmptyElement)
                {
                    return false;
                }
                reader.ReadStartElement();
                while (reader.IsStartElement())
                {
                    if (driver.IsAtSecurityContextToken(reader))
                    {
                        results.Add(driver.GetSecurityContextTokenId(reader));
                        addedContextIds = true;
                        if (isStrictMode)
                        {
                            break;
                        }
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
            }
            return addedContextIds;
        }

    }
}
