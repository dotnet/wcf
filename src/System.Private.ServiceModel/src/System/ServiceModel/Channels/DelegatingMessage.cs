// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;

namespace System.ServiceModel.Channels
{
    abstract class DelegatingMessage : Message
    {
        private Message _innerMessage;

        protected DelegatingMessage(Message innerMessage)
        {
            if (innerMessage == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("innerMessage");
            }
            _innerMessage = innerMessage;
        }

        public override bool IsEmpty
        {
            get
            {
                return _innerMessage.IsEmpty;
            }
        }

        public override bool IsFault
        {
            get { return _innerMessage.IsFault; }
        }

        public override MessageHeaders Headers
        {
            get { return _innerMessage.Headers; }
        }

        public override MessageProperties Properties
        {
            get { return _innerMessage.Properties; }
        }

        public override MessageVersion Version
        {
            get { return _innerMessage.Version; }
        }

        protected Message InnerMessage
        {
            get { return _innerMessage; }
        }

        protected override void OnClose()
        {
            base.OnClose();
            _innerMessage.Close();
        }

        protected override void OnWriteStartEnvelope(XmlDictionaryWriter writer)
        {
            _innerMessage.WriteStartEnvelope(writer);
        }

        protected override void OnWriteStartHeaders(XmlDictionaryWriter writer)
        {
            _innerMessage.WriteStartHeaders(writer);
        }

        protected override void OnWriteStartBody(XmlDictionaryWriter writer)
        {
            _innerMessage.WriteStartBody(writer);
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            _innerMessage.WriteBodyContents(writer);
        }

        protected override string OnGetBodyAttribute(string localName, string ns)
        {
            return _innerMessage.GetBodyAttribute(localName, ns);
        }

        protected override void OnBodyToString(XmlDictionaryWriter writer)
        {
            _innerMessage.BodyToString(writer);
        }
    }
}

