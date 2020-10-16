// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace System.ServiceModel
{
    public sealed class MessageSecurityOverTcp
    {
        internal const MessageCredentialType DefaultClientCredentialType = MessageCredentialType.Windows;
        private MessageCredentialType _clientCredentialType;
        private SecurityAlgorithmSuite _algorithmSuite;

        public MessageSecurityOverTcp()
        {
            _clientCredentialType = DefaultClientCredentialType;
            _algorithmSuite = SecurityAlgorithmSuite.Default;
        }

        [DefaultValue(DefaultClientCredentialType)]
        public MessageCredentialType ClientCredentialType
        {
            get {
                if (_clientCredentialType == MessageCredentialType.IssuedToken || _clientCredentialType == MessageCredentialType.Windows)
                {
                    throw ExceptionHelper.PlatformNotSupported($"MessageSecurityOverTcp.ClientCredentialType is not supported for value {_clientCredentialType}.");
                }

                return _clientCredentialType;
            }
            set
            {
                if (!MessageCredentialTypeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(value)));
                }

                if (value == MessageCredentialType.IssuedToken || value == MessageCredentialType.Windows)
                {
                    throw ExceptionHelper.PlatformNotSupported($"MessageSecurityOverTcp.ClientCredentialType is not supported for value {value}.");
                }

                _clientCredentialType = value;
            }
        }

        [DefaultValue(typeof(SecurityAlgorithmSuite), nameof(SecurityAlgorithmSuite.Default))]
        public SecurityAlgorithmSuite AlgorithmSuite
        {
            get { return _algorithmSuite; }
            set
            {
                _algorithmSuite = value ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal SecurityBindingElement CreateSecurityBindingElement(bool isSecureTransportMode, bool isReliableSession, BindingElement transportBindingElement)
        {
            SecurityBindingElement result;
            SecurityBindingElement oneShotSecurity;
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
                        throw ExceptionHelper.PlatformNotSupported($"{nameof(MessageCredentialType)}.{nameof(MessageCredentialType.Windows)}");
                    case MessageCredentialType.IssuedToken:
                        throw ExceptionHelper.PlatformNotSupported($"{nameof(MessageCredentialType)}.{nameof(MessageCredentialType.IssuedToken)}");
                    default:
                        Fx.Assert("unknown ClientCredentialType");
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
                }
                result = SecurityBindingElement.CreateSecureConversationBindingElement(oneShotSecurity);
            }
            else
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            // set the algorithm suite and issued token params if required
            result.DefaultAlgorithmSuite = oneShotSecurity.DefaultAlgorithmSuite = AlgorithmSuite;

            result.IncludeTimestamp = true;
            if (!isReliableSession)
            {
                result.LocalClientSettings.ReconnectTransportOnFailure = false;
            }
            else
            {
                result.LocalClientSettings.ReconnectTransportOnFailure = true;
            }

            result.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11;
            oneShotSecurity.MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11;

            return result;
        }
    }
}
