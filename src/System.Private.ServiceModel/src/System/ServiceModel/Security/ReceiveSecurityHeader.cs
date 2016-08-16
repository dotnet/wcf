// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace System.ServiceModel.Security
{
    internal abstract class ReceiveSecurityHeader : SecurityHeader
    {
        bool _expectBasicTokens;
        bool _expectSignedTokens;
        bool _expectEndorsingTokens;

        protected ReceiveSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay,
            SecurityStandardsManager standardsManager,
            SecurityAlgorithmSuite algorithmSuite,
            int headerIndex,
            MessageDirection direction)
            : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, direction)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        public override string Name
        {
            get { return this.StandardsManager.SecurityVersion.HeaderName.Value; }
        }

        public override string Namespace
        {
            get { return this.StandardsManager.SecurityVersion.HeaderNamespace.Value; }
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        public bool ExpectBasicTokens
        {
            get { return _expectBasicTokens; }
            set
            {
                ThrowIfProcessingStarted();
                _expectBasicTokens = value;
            }
        }

        public bool ExpectSignedTokens
        {
            get { return _expectSignedTokens; }
            set
            {
                ThrowIfProcessingStarted();
                _expectSignedTokens = value;
            }
        }

        public bool ExpectEndorsingTokens
        {
            get { return _expectEndorsingTokens; }
            set
            {
                ThrowIfProcessingStarted();
                _expectEndorsingTokens = value;
            }
        }
    }
}
