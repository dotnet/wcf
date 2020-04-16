// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public sealed class MessageSecurityOverTcp
    {
        internal const MessageCredentialType DefaultClientCredentialType = MessageCredentialType.Windows;

        private MessageCredentialType _clientCredentialType;
        private MessageCredentialType _messageCredentialType;

        public MessageSecurityOverTcp()
        {
            _messageCredentialType = DefaultClientCredentialType;
        }

        [DefaultValue(MessageSecurityOverTcp.DefaultClientCredentialType)]
        public MessageCredentialType ClientCredentialType
        {
            get { return _clientCredentialType; }
            set
            {
                if (!MessageCredentialTypeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                _clientCredentialType = value;
            }
        }


        // TODO: [MethodImpl(MethodImplOptions.NoInlining)]
        internal SecurityBindingElement CreateSecurityBindingElement(bool isSecureTransportMode, bool isReliableSession, BindingElement transportBindingElement)
        {
            throw new NotImplementedException();
        }

        internal static bool TryCreate(SecurityBindingElement sbe, bool isReliableSession, BindingElement transportBindingElement, out MessageSecurityOverTcp messageSecurity)
        {
            throw new NotImplementedException();
        }
    }
}
