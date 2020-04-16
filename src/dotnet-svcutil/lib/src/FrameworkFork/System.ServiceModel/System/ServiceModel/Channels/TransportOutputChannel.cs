// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel;
using Microsoft.Xml;
using System.Runtime.Diagnostics;

namespace System.ServiceModel.Channels
{
    public abstract class TransportOutputChannel : OutputChannel
    {
        private bool _anyHeadersToAdd;
        private bool _manualAddressing;
        private MessageVersion _messageVersion;
        private EndpointAddress _to;
        private Uri _via;
        private ToHeader _toHeader;
        private EventTraceActivity _channelEventTraceActivity;

        protected TransportOutputChannel(ChannelManagerBase channelManager, EndpointAddress to, Uri via, bool manualAddressing, MessageVersion messageVersion)
            : base(channelManager)
        {
            _manualAddressing = manualAddressing;
            _messageVersion = messageVersion;
            _to = to;
            _via = via;

            if (!manualAddressing && to != null)
            {
                Uri toUri;
                if (to.IsAnonymous)
                {
                    toUri = _messageVersion.Addressing.AnonymousUri;
                }
                else if (to.IsNone)
                {
                    toUri = _messageVersion.Addressing.NoneUri;
                }
                else
                {
                    toUri = to.Uri;
                }

                if (toUri != null)
                {
                    XmlDictionaryString dictionaryTo = new ToDictionary(toUri.AbsoluteUri).To;
                    _toHeader = ToHeader.Create(toUri, dictionaryTo, messageVersion.Addressing);
                }

                _anyHeadersToAdd = to.Headers.Count > 0;
            }

            if (FxTrace.Trace.IsEnd2EndActivityTracingEnabled)
            {
                _channelEventTraceActivity = EventTraceActivity.GetFromThreadOrCreate();
            }
        }

        protected bool ManualAddressing
        {
            get
            {
                return _manualAddressing;
            }
        }

        public MessageVersion MessageVersion
        {
            get
            {
                return _messageVersion;
            }
        }

        public override EndpointAddress RemoteAddress
        {
            get
            {
                return _to;
            }
        }

        public override Uri Via
        {
            get
            {
                return _via;
            }
        }

        public EventTraceActivity EventTraceActivity
        {
            get
            {
                return _channelEventTraceActivity;
            }
        }

        protected override void AddHeadersTo(Message message)
        {
            base.AddHeadersTo(message);

            if (_toHeader != null)
            {
                // we don't use to.ApplyTo(message) since it's faster to cache and
                // use the actual <To> header then to call message.Headers.To = Uri...
                message.Headers.SetToHeader(_toHeader);
                if (_anyHeadersToAdd)
                {
                    _to.Headers.AddHeadersTo(message);
                }
            }
        }

        internal class ToDictionary : IXmlDictionary
        {
            private XmlDictionaryString _to;

            public ToDictionary(string to)
            {
                _to = new XmlDictionaryString(this, to, 0);
            }

            public XmlDictionaryString To
            {
                get
                {
                    return _to;
                }
            }

            public bool TryLookup(string value, out XmlDictionaryString result)
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                if (value == _to.Value)
                {
                    result = _to;
                    return true;
                }
                result = null;
                return false;
            }

            public bool TryLookup(int key, out XmlDictionaryString result)
            {
                if (key == 0)
                {
                    result = _to;
                    return true;
                }
                result = null;
                return false;
            }

            public bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result)
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                if (value == _to)
                {
                    result = _to;
                    return true;
                }
                result = null;
                return false;
            }
        }
    }
}
