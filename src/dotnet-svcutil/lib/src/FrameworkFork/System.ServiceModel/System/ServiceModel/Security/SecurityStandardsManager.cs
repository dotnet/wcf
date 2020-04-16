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
using Microsoft.Xml;
using System.Runtime.CompilerServices;

namespace System.ServiceModel.Security
{
    internal class SecurityStandardsManager
    {
#pragma warning disable 0649 // Remove this once we do real implementation, this prevents "field is never assigned to" warning
        private static SecurityStandardsManager s_instance;
        private readonly SecureConversationDriver _secureConversationDriver;
        private readonly TrustDriver _trustDriver;
        private readonly SignatureTargetIdManager _idManager;
        private readonly MessageSecurityVersion _messageSecurityVersion;
        private readonly WSUtilitySpecificationVersion _wsUtilitySpecificationVersion;
        private readonly SecurityTokenSerializer _tokenSerializer;
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("messageSecurityVersion", SRServiceModel.MessageSecurityVersionOutOfRange));
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

        internal SecurityTokenSerializer SecurityTokenSerializer
        {
            get { return _tokenSerializer; }
        }

        internal TrustDriver TrustDriver
        {
            get { return _trustDriver; }
        }
    }
}
