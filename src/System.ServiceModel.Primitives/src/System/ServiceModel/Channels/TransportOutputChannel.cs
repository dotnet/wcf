// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Xml;
using System.Runtime.Diagnostics;

namespace System.ServiceModel.Channels
{
    internal abstract class TransportOutputChannel : OutputChannel
    {
        private bool _anyHeadersToAdd;
        private EndpointAddress _to;
        private Uri _via;
        private ToHeader _toHeader;

        protected TransportOutputChannel(ChannelManagerBase channelManager, EndpointAddress to, Uri via, bool manualAddressing, MessageVersion messageVersion)
            : base(channelManager)
        {
            ManualAddressing = manualAddressing;
            MessageVersion = messageVersion;
            _to = to;
            _via = via;

            if (!manualAddressing && to != null)
            {
                Uri toUri;
                if (to.IsAnonymous)
                {
                    toUri = MessageVersion.Addressing.AnonymousUri;
                }
                else if (to.IsNone)
                {
                    toUri = MessageVersion.Addressing.NoneUri;
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
                EventTraceActivity = EventTraceActivity.GetFromThreadOrCreate();
            }
        }

        protected bool ManualAddressing { get; }

        public MessageVersion MessageVersion { get; }

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

        public EventTraceActivity EventTraceActivity { get; }

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
            public ToDictionary(string to)
            {
                To = new XmlDictionaryString(this, to, 0);
            }

            public XmlDictionaryString To { get; }

            public bool TryLookup(string value, out XmlDictionaryString result)
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }

                if (value == To.Value)
                {
                    result = To;
                    return true;
                }
                result = null;
                return false;
            }

            public bool TryLookup(int key, out XmlDictionaryString result)
            {
                if (key == 0)
                {
                    result = To;
                    return true;
                }
                result = null;
                return false;
            }

            public bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result)
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(value));
                }

                if (value == To)
                {
                    result = To;
                    return true;
                }
                result = null;
                return false;
            }
        }
    }
}
