// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.Xml;

using DictionaryManager = System.IdentityModel.DictionaryManager;
using ISecurityElement = System.IdentityModel.ISecurityElement;

namespace System.ServiceModel.Security
{
    internal abstract class SecurityHeader : MessageHeader
    {
        private readonly string _actor;
        private readonly SecurityAlgorithmSuite _algorithmSuite;
        private bool _encryptedKeyContainsReferenceList = true;
        private Message _message;
        private readonly bool _mustUnderstand;
        private readonly bool _relay;
        private bool _requireMessageProtection = true;
        private bool _processingStarted;
        private bool _maintainSignatureConfirmationState;
        private readonly SecurityStandardsManager _standardsManager;
        private MessageDirection _transferDirection;
        private SecurityHeaderLayout _layout = SecurityHeaderLayout.Strict;

        public SecurityHeader(Message message,
            string actor, bool mustUnderstand, bool relay,
            SecurityStandardsManager standardsManager, SecurityAlgorithmSuite algorithmSuite,
            MessageDirection transferDirection)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
            }
            if (actor == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("actor");
            }
            if (standardsManager == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("standardsManager");
            }
            if (algorithmSuite == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("algorithmSuite");
            }

            _message = message;
            _actor = actor;
            _mustUnderstand = mustUnderstand;
            _relay = relay;
            _standardsManager = standardsManager;
            _algorithmSuite = algorithmSuite;
            _transferDirection = transferDirection;
        }

        public override string Actor
        {
            get { return _actor; }
        }

        public SecurityAlgorithmSuite AlgorithmSuite
        {
            get { return _algorithmSuite; }
        }

        public bool EncryptedKeyContainsReferenceList
        {
            get { return _encryptedKeyContainsReferenceList; }
            set
            {
                ThrowIfProcessingStarted();
                _encryptedKeyContainsReferenceList = value;
            }
        }

        public bool RequireMessageProtection
        {
            get { return _requireMessageProtection; }
            set
            {
                ThrowIfProcessingStarted();
                _requireMessageProtection = value;
            }
        }

        public bool MaintainSignatureConfirmationState
        {
            get { return _maintainSignatureConfirmationState; }
            set
            {
                ThrowIfProcessingStarted();
                _maintainSignatureConfirmationState = value;
            }
        }

        protected Message Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public override bool MustUnderstand
        {
            get { return _mustUnderstand; }
        }

        public override bool Relay
        {
            get { return _relay; }
        }

        public SecurityHeaderLayout Layout
        {
            get
            {
                return _layout;
            }
            set
            {
                ThrowIfProcessingStarted();
                _layout = value;
            }
        }

        public SecurityStandardsManager StandardsManager
        {
            get { return _standardsManager; }
        }

        public MessageDirection MessageDirection
        {
            get { return _transferDirection; }
        }

        protected MessageVersion Version
        {
            get { return _message.Version; }
        }

        protected void SetProcessingStarted()
        {
            _processingStarted = true;
        }

        protected void ThrowIfProcessingStarted()
        {
            if (_processingStarted)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.OperationCannotBeDoneAfterProcessingIsStarted));
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}(Actor = '{1}')", GetType().Name, this.Actor);
        }
    }
}
