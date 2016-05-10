// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Diagnostics;
using System.Xml;

namespace System.ServiceModel.Channels
{
    /// <summary>
    /// Base class for non-SOAP messages
    /// </summary>
    internal abstract class ContentOnlyMessage : Message
    {
        private MessageHeaders _headers;
        private MessageProperties _properties;

        protected ContentOnlyMessage()
        {
            _headers = new MessageHeaders(MessageVersion.None);
        }

        public override MessageHeaders Headers
        {
            get
            {
                if (IsDisposed)
                {
                    throw TraceUtility.ThrowHelperError(CreateMessageDisposedException(), this);
                }

                return _headers;
            }
        }

        public override MessageProperties Properties
        {
            get
            {
                if (IsDisposed)
                {
                    throw TraceUtility.ThrowHelperError(CreateMessageDisposedException(), this);
                }

                if (_properties == null)
                {
                    _properties = new MessageProperties();
                }

                return _properties;
            }
        }

        public override MessageVersion Version
        {
            get
            {
                return _headers.MessageVersion;
            }
        }

        protected override void OnBodyToString(XmlDictionaryWriter writer)
        {
            OnWriteBodyContents(writer);
        }
    }

    internal class StringMessage : ContentOnlyMessage
    {
        private string _data;

        public StringMessage(string data)
            : base()
        {
            _data = data;
        }

        public override bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_data);
            }
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            if (_data != null && _data.Length > 0)
            {
                writer.WriteElementString("BODY", _data);
            }
        }
    }

    internal class NullMessage : StringMessage
    {
        public NullMessage()
            : base(string.Empty)
        {
        }
    }
}
