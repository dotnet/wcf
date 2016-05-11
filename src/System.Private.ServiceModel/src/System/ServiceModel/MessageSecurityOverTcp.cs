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
        private MessageCredentialType _messageCredentialType;

        public MessageSecurityOverTcp()
        {
            _messageCredentialType = DefaultClientCredentialType;
        }

        [DefaultValue(MessageSecurityOverTcp.DefaultClientCredentialType)]
        public MessageCredentialType ClientCredentialType
        {
            get
            {
                if (_messageCredentialType != MessageCredentialType.None)
                {
                    throw ExceptionHelper.PlatformNotSupported("MessageSecurityOverTcp.ClientCredentialType is not supported for values other than 'MessageCredentialType.None'.");
                }
                else
                {
                    return _messageCredentialType;
                }
            }
            set
            {
                if (!MessageCredentialTypeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }

                if (value != MessageCredentialType.None)
                {
                    throw ExceptionHelper.PlatformNotSupported("MessageSecurityOverTcp.ClientCredentialType is not supported for values other than 'MessageCredentialType.None'.");
                }
                else
                {
                    _messageCredentialType = value;
                }
            }
        }

        internal bool InternalShouldSerialize()
        {
            return this.ClientCredentialType != NetTcpDefaults.MessageSecurityClientCredentialType;
        }
    }
}
