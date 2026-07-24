// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace System.ServiceModel
{
    public sealed class MessageSecurityOverMsmq
    {
        internal const MessageCredentialType DefaultClientCredentialType = MessageCredentialType.Windows;

        private MessageCredentialType _clientCredentialType;
        private SecurityAlgorithmSuite _algorithmSuite;
        private bool _wasAlgorithmSuiteSet;

        public MessageSecurityOverMsmq()
        {
            _clientCredentialType = DefaultClientCredentialType;
            _algorithmSuite = SecurityAlgorithmSuite.Default;
        }

        [DefaultValue(MsmqDefaults.DefaultClientCredentialType)]
        public MessageCredentialType ClientCredentialType
        {
            get { return _clientCredentialType; }
            set
            {
                if (!IsMessageCredentialTypeDefined(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _clientCredentialType = value;
            }
        }

        public SecurityAlgorithmSuite AlgorithmSuite
        {
            get { return _algorithmSuite; }
            set
            {
                _algorithmSuite = value ?? throw new ArgumentNullException(nameof(value));
                _wasAlgorithmSuiteSet = true;
            }
        }

        internal bool WasAlgorithmSuiteSet
        {
            get { return _wasAlgorithmSuiteSet; }
        }

        private static bool IsMessageCredentialTypeDefined(MessageCredentialType value)
        {
            return value == MessageCredentialType.None
                || value == MessageCredentialType.Windows
                || value == MessageCredentialType.UserName
                || value == MessageCredentialType.Certificate
                || value == MessageCredentialType.IssuedToken;
        }
    }
}
