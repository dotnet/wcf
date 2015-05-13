// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public sealed class MessageSecurityOverTcp
    {
        internal const MessageCredentialType DefaultClientCredentialType = MessageCredentialType.Windows;

        public MessageSecurityOverTcp()
        {
        }

        [DefaultValue(MessageSecurityOverTcp.DefaultClientCredentialType)]
        public MessageCredentialType ClientCredentialType
        {
            get
            {
                throw ExceptionHelper.PlatformNotSupported("MessageSecurityOverTcp.ClientCredentialType is not supported.");
            }
            set
            {
                throw ExceptionHelper.PlatformNotSupported("MessageSecurityOverTcp.ClientCredentialType is not supported.");
            }
        }

        internal bool InternalShouldSerialize()
        {
            return this.ClientCredentialType != NetTcpDefaults.MessageSecurityClientCredentialType;
        }
    }
}
