// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace System.ServiceModel.Security
{
    abstract class DelegatingHeader : MessageHeader
    {
        MessageHeader _innerHeader;

        protected DelegatingHeader(MessageHeader innerHeader)
        {
            if (innerHeader == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("innerHeader");
            }
            _innerHeader = innerHeader;
        }

        public override bool MustUnderstand
        {
            get
            {
                return _innerHeader.MustUnderstand;
            }
        }

        public override string Name
        {
            get
            {
                return _innerHeader.Name;
            }
        }

        public override string Namespace
        {
            get
            {
                return _innerHeader.Namespace;
            }
        }

        public override bool Relay
        {
            get
            {
                return _innerHeader.Relay;
            }
        }

        public override string Actor
        {
            get
            {
                return _innerHeader.Actor;
            }
        }

        protected MessageHeader InnerHeader
        {
            get
            {
                return _innerHeader;
            }
        }

        protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            _innerHeader.WriteStartHeader(writer, messageVersion);
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            _innerHeader.WriteHeaderContents(writer, messageVersion);
        }
    }
}

