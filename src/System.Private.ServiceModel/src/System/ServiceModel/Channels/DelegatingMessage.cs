// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;
using System.ServiceModel.Channels;

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
            this._innerMessage = innerMessage;
        }

        public override bool IsEmpty
        {
            get
            {
                return this._innerMessage.IsEmpty;
            }
        }

        public override bool IsFault
        {
            get { return this._innerMessage.IsFault; }
        }

        public override MessageHeaders Headers
        {
            get { return this._innerMessage.Headers; }
        }

        public override MessageProperties Properties
        {
            get { return this._innerMessage.Properties; }
        }

        public override MessageVersion Version
        {
            get { return this._innerMessage.Version; }
        }

        protected Message InnerMessage
        {
            get { return this._innerMessage; }
        }

        protected override void OnClose()
        {
            base.OnClose();
            this._innerMessage.Close();
        }

        protected override void OnWriteStartEnvelope(XmlDictionaryWriter writer)
        {
            this._innerMessage.WriteStartEnvelope(writer);
        }

        protected override void OnWriteStartHeaders(XmlDictionaryWriter writer)
        {
            this._innerMessage.WriteStartHeaders(writer);
        }

        protected override void OnWriteStartBody(XmlDictionaryWriter writer)
        {
            this._innerMessage.WriteStartBody(writer);
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            this._innerMessage.WriteBodyContents(writer);
        }

        protected override string OnGetBodyAttribute(string localName, string ns)
        {
            return this._innerMessage.GetBodyAttribute(localName, ns);
        }

        protected override void OnBodyToString(XmlDictionaryWriter writer)
        {
            this._innerMessage.BodyToString(writer);
        }
    }
}

