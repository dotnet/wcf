// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace System.ServiceModel
{
    public class MessageSecurityOverHttp
    {
        internal const MessageCredentialType DefaultClientCredentialType = MessageCredentialType.Windows;
        internal const bool DefaultNegotiateServiceCredential = true;

        private MessageCredentialType _clientCredentialType;
        private SecurityAlgorithmSuite _algorithmSuite;
        private bool _wasAlgorithmSuiteSet;

        public MessageSecurityOverHttp()
        {
            _clientCredentialType = DefaultClientCredentialType;
            NegotiateServiceCredential = DefaultNegotiateServiceCredential;
            _algorithmSuite = SecurityAlgorithmSuite.Default;
        }

        public MessageCredentialType ClientCredentialType
        {
            get { return _clientCredentialType; }
            set
            {
                if (!MessageCredentialTypeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }

                _clientCredentialType = value;
            }
        }
        public bool NegotiateServiceCredential { get; set; }

        public SecurityAlgorithmSuite AlgorithmSuite
        {
            get { return _algorithmSuite; }
            set
            {
                _algorithmSuite = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                _wasAlgorithmSuiteSet = true;
            }
        }

        protected virtual bool IsSecureConversationEnabled()
        {
            return true;
        }

        internal SecurityBindingElement CreateSecurityBindingElement(bool isSecureTransportMode, bool isReliableSession, MessageSecurityVersion version)
        {
            if (isReliableSession && !IsSecureConversationEnabled())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.SecureConversationRequiredByReliableSession));
            }

            SecurityBindingElement result;
            SecurityBindingElement oneShotSecurity;

            bool isKerberosSelected = false;
            if (isSecureTransportMode)
            {
                switch (_clientCredentialType)
                {
                    case MessageCredentialType.None:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.ClientCredentialTypeMustBeSpecifiedForMixedMode));
                    case MessageCredentialType.UserName:
                        oneShotSecurity = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
                        break;
                    case MessageCredentialType.Certificate:
                        oneShotSecurity = SecurityBindingElement.CreateCertificateOverTransportBindingElement();
                        break;
                    case MessageCredentialType.Windows:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
                    case MessageCredentialType.IssuedToken:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
                    default:
                        Fx.Assert("unknown ClientCredentialType");
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
                }

                if (IsSecureConversationEnabled())
                {
                    result = SecurityBindingElement.CreateSecureConversationBindingElement(oneShotSecurity, true);
                }
                else
                {
                    result = oneShotSecurity;
                }
            }
            else
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            // set the algorithm suite and issued token params if required
            if (_wasAlgorithmSuiteSet || (!isKerberosSelected))
            {
                result.DefaultAlgorithmSuite = oneShotSecurity.DefaultAlgorithmSuite = AlgorithmSuite;
            }
            else if (isKerberosSelected)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            result.IncludeTimestamp = true;
            oneShotSecurity.MessageSecurityVersion = version;
            result.MessageSecurityVersion = version;
            if (!isReliableSession)
            {
                result.LocalClientSettings.ReconnectTransportOnFailure = false;
            }
            else
            {
                result.LocalClientSettings.ReconnectTransportOnFailure = true;
            }

            return result;
        }
    }
}
