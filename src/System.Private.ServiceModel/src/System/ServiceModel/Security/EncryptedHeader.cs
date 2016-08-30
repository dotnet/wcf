// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Globalization;
using System.Xml;
using System.IO;

using ISecurityElement = System.IdentityModel.ISecurityElement;

namespace System.ServiceModel.Security
{
    sealed class EncryptedHeader : DelegatingHeader
    {
        private EncryptedHeaderXml _headerXml;
        private string _name;
        private string _namespaceUri;
        private MessageVersion _version;

        public EncryptedHeader(MessageHeader plainTextHeader, EncryptedHeaderXml headerXml, string name, string namespaceUri, MessageVersion version)
            : base(plainTextHeader)
        {
            if (!headerXml.HasId || headerXml.Id == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SR.Format(SR.EncryptedHeaderXmlMustHaveId)));
            }
            this._headerXml = headerXml;
            this._name = name;
            this._namespaceUri = namespaceUri;
            this._version = version;
        }

        public string Id
        {
            get { return this._headerXml.Id; }
        }

        public override string Name
        {
            get { return this._name; }
        }

        public override string Namespace
        {
            get { return this._namespaceUri; }
        }

        public override string Actor
        {
            get
            {
                return this._headerXml.Actor;
            }
        }

        public override bool MustUnderstand
        {
            get
            {
                return this._headerXml.MustUnderstand;
            }
        }

        public override bool Relay
        {
            get
            {
                return this._headerXml.Relay;
            }
        }

        internal MessageHeader OriginalHeader
        {
            get { return this.InnerHeader; }
        }

        public override bool IsMessageVersionSupported(MessageVersion messageVersion)
        {
            return this._version.Equals( messageVersion );
        }

        protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            if (!IsMessageVersionSupported(messageVersion))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.Format(SR.MessageHeaderVersionNotSupported, String.Format(CultureInfo.InvariantCulture, "{0}:{1}", this.Namespace, this.Name), _version.ToString()), "version"));
            }

            this._headerXml.WriteHeaderElement(writer);
            WriteHeaderAttributes(writer, messageVersion);
            this._headerXml.WriteHeaderId(writer);
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            this._headerXml.WriteHeaderContents(writer);
        }
    }
}

