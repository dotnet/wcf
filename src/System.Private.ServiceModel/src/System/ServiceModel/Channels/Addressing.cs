// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.Xml;

namespace System.ServiceModel.Channels
{
    internal abstract class AddressingHeader : DictionaryHeader, IMessageHeaderWithSharedNamespace
    {
        private AddressingVersion _version;

        protected AddressingHeader(AddressingVersion version)
        {
            _version = version;
        }

        internal AddressingVersion Version
        {
            get { return _version; }
        }

        XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedPrefix
        {
            get { return XD.AddressingDictionary.Prefix; }
        }

        XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedNamespace
        {
            get { return _version.DictionaryNamespace; }
        }

        public override XmlDictionaryString DictionaryNamespace
        {
            get { return _version.DictionaryNamespace; }
        }
    }

    internal class ActionHeader : AddressingHeader
    {
        private string _action;
        private const bool mustUnderstandValue = true;

        private ActionHeader(string action, AddressingVersion version)
            : base(version)
        {
            _action = action;
        }

        public string Action
        {
            get { return _action; }
        }

        public override bool MustUnderstand
        {
            get { return mustUnderstandValue; }
        }

        public override XmlDictionaryString DictionaryName
        {
            get { return XD.AddressingDictionary.Action; }
        }

        public static ActionHeader Create(string action, AddressingVersion addressingVersion)
        {
            if (action == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("action"));
            if (addressingVersion == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("addressingVersion");
            return new ActionHeader(action, addressingVersion);
        }

        public static ActionHeader Create(XmlDictionaryString dictionaryAction, AddressingVersion addressingVersion)
        {
            if (dictionaryAction == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("action"));
            if (addressingVersion == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("addressingVersion");
            return new DictionaryActionHeader(dictionaryAction, addressingVersion);
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteString(_action);
        }

        public static string ReadHeaderValue(XmlDictionaryReader reader, AddressingVersion addressingVersion)
        {
            Fx.Assert(reader.IsStartElement(XD.AddressingDictionary.Action, addressingVersion.DictionaryNamespace), "");
            string act = reader.ReadElementContentAsString();

            if (act.Length > 0 && (act[0] <= 32 || act[act.Length - 1] <= 32))
                act = XmlUtil.Trim(act);

            return act;
        }

        public static ActionHeader ReadHeader(XmlDictionaryReader reader, AddressingVersion version,
            string actor, bool mustUnderstand, bool relay)
        {
            string action = ReadHeaderValue(reader, version);

            if (actor.Length == 0 && mustUnderstand == mustUnderstandValue && !relay)
            {
                return new ActionHeader(action, version);
            }
            else
            {
                return new FullActionHeader(action, actor, mustUnderstand, relay, version);
            }
        }

        internal class DictionaryActionHeader : ActionHeader
        {
            private XmlDictionaryString _dictionaryAction;

            public DictionaryActionHeader(XmlDictionaryString dictionaryAction, AddressingVersion version)
                : base(dictionaryAction.Value, version)
            {
                _dictionaryAction = dictionaryAction;
            }

            protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
            {
                writer.WriteString(_dictionaryAction);
            }
        }

        internal class FullActionHeader : ActionHeader
        {
            private string _actor;
            private bool _mustUnderstand;
            private bool _relay;

            public FullActionHeader(string action, string actor, bool mustUnderstand, bool relay, AddressingVersion version)
                : base(action, version)
            {
                _actor = actor;
                _mustUnderstand = mustUnderstand;
                _relay = relay;
            }

            public override string Actor
            {
                get { return _actor; }
            }

            public override bool MustUnderstand
            {
                get { return _mustUnderstand; }
            }

            public override bool Relay
            {
                get { return _relay; }
            }
        }
    }

    internal class FromHeader : AddressingHeader
    {
        private EndpointAddress _from;
        private const bool mustUnderstandValue = false;

        private FromHeader(EndpointAddress from, AddressingVersion version)
            : base(version)
        {
            _from = from;
        }

        public EndpointAddress From
        {
            get { return _from; }
        }

        public override XmlDictionaryString DictionaryName
        {
            get { return XD.AddressingDictionary.From; }
        }

        public override bool MustUnderstand
        {
            get { return mustUnderstandValue; }
        }

        public static FromHeader Create(EndpointAddress from, AddressingVersion addressingVersion)
        {
            if (from == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("from"));
            if (addressingVersion == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("addressingVersion");
            return new FromHeader(from, addressingVersion);
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            _from.WriteContentsTo(this.Version, writer);
        }

        public static FromHeader ReadHeader(XmlDictionaryReader reader, AddressingVersion version,
            string actor, bool mustUnderstand, bool relay)
        {
            EndpointAddress from = ReadHeaderValue(reader, version);

            if (actor.Length == 0 && mustUnderstand == mustUnderstandValue && !relay)
            {
                return new FromHeader(from, version);
            }
            else
            {
                return new FullFromHeader(from, actor, mustUnderstand, relay, version);
            }
        }

        public static EndpointAddress ReadHeaderValue(XmlDictionaryReader reader, AddressingVersion addressingVersion)
        {
            Fx.Assert(reader.IsStartElement(XD.AddressingDictionary.From, addressingVersion.DictionaryNamespace), "");
            return EndpointAddress.ReadFrom(addressingVersion, reader);
        }

        internal class FullFromHeader : FromHeader
        {
            private string _actor;
            private bool _mustUnderstand;
            private bool _relay;

            public FullFromHeader(EndpointAddress from, string actor, bool mustUnderstand, bool relay, AddressingVersion version)
                : base(from, version)
            {
                _actor = actor;
                _mustUnderstand = mustUnderstand;
                _relay = relay;
            }

            public override string Actor
            {
                get { return _actor; }
            }

            public override bool MustUnderstand
            {
                get { return _mustUnderstand; }
            }

            public override bool Relay
            {
                get { return _relay; }
            }
        }
    }

    internal class FaultToHeader : AddressingHeader
    {
        private EndpointAddress _faultTo;
        private const bool mustUnderstandValue = false;

        private FaultToHeader(EndpointAddress faultTo, AddressingVersion version)
            : base(version)
        {
            _faultTo = faultTo;
        }

        public EndpointAddress FaultTo
        {
            get { return _faultTo; }
        }

        public override XmlDictionaryString DictionaryName
        {
            get { return XD.AddressingDictionary.FaultTo; }
        }

        public override bool MustUnderstand
        {
            get { return mustUnderstandValue; }
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            _faultTo.WriteContentsTo(this.Version, writer);
        }

        public static FaultToHeader Create(EndpointAddress faultTo, AddressingVersion addressingVersion)
        {
            if (faultTo == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("faultTo"));
            if (addressingVersion == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("addressingVersion");
            return new FaultToHeader(faultTo, addressingVersion);
        }

        public static FaultToHeader ReadHeader(XmlDictionaryReader reader, AddressingVersion version,
            string actor, bool mustUnderstand, bool relay)
        {
            EndpointAddress faultTo = ReadHeaderValue(reader, version);

            if (actor.Length == 0 && mustUnderstand == mustUnderstandValue && !relay)
            {
                return new FaultToHeader(faultTo, version);
            }
            else
            {
                return new FullFaultToHeader(faultTo, actor, mustUnderstand, relay, version);
            }
        }

        public static EndpointAddress ReadHeaderValue(XmlDictionaryReader reader, AddressingVersion version)
        {
            Fx.Assert(reader.IsStartElement(XD.AddressingDictionary.FaultTo, version.DictionaryNamespace), "");
            return EndpointAddress.ReadFrom(version, reader);
        }

        internal class FullFaultToHeader : FaultToHeader
        {
            private string _actor;
            private bool _mustUnderstand;
            private bool _relay;

            public FullFaultToHeader(EndpointAddress faultTo, string actor, bool mustUnderstand, bool relay, AddressingVersion version)
                : base(faultTo, version)
            {
                _actor = actor;
                _mustUnderstand = mustUnderstand;
                _relay = relay;
            }

            public override string Actor
            {
                get { return _actor; }
            }

            public override bool MustUnderstand
            {
                get { return _mustUnderstand; }
            }

            public override bool Relay
            {
                get { return _relay; }
            }
        }
    }

    internal class ToHeader : AddressingHeader
    {
        private Uri _to;
        private const bool mustUnderstandValue = true;

        private static ToHeader s_anonymousToHeader10;
        private static ToHeader s_anonymousToHeader200408;

        protected ToHeader(Uri to, AddressingVersion version)
            : base(version)
        {
            _to = to;
        }

        private static ToHeader AnonymousTo10
        {
            get
            {
                if (s_anonymousToHeader10 == null)
                    s_anonymousToHeader10 = new AnonymousToHeader(AddressingVersion.WSAddressing10);
                return s_anonymousToHeader10;
            }
        }

        private static ToHeader AnonymousTo200408
        {
            get
            {
                if (s_anonymousToHeader200408 == null)
                    s_anonymousToHeader200408 = new AnonymousToHeader(AddressingVersion.WSAddressingAugust2004);
                return s_anonymousToHeader200408;
            }
        }

        public override XmlDictionaryString DictionaryName
        {
            get { return XD.AddressingDictionary.To; }
        }

        public override bool MustUnderstand
        {
            get { return mustUnderstandValue; }
        }

        public Uri To
        {
            get { return _to; }
        }

        public static ToHeader Create(Uri toUri, XmlDictionaryString dictionaryTo, AddressingVersion addressingVersion)
        {
            if (addressingVersion == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("addressingVersion");

            if (((object)toUri == (object)addressingVersion.AnonymousUri))
            {
                if (addressingVersion == AddressingVersion.WSAddressing10)
                    return AnonymousTo10;
                else
                    return AnonymousTo200408;
            }
            else
            {
                return new DictionaryToHeader(toUri, dictionaryTo, addressingVersion);
            }
        }

        public static ToHeader Create(Uri to, AddressingVersion addressingVersion)
        {
            if ((object)to == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("to"));
            }
            else if ((object)to == (object)addressingVersion.AnonymousUri)
            {
                if (addressingVersion == AddressingVersion.WSAddressing10)
                    return AnonymousTo10;
                else
                    return AnonymousTo200408;
            }
            else
            {
                return new ToHeader(to, addressingVersion);
            }
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteString(_to.AbsoluteUri);
        }

        public static Uri ReadHeaderValue(XmlDictionaryReader reader, AddressingVersion version)
        {
            return ReadHeaderValue(reader, version, null);
        }

        public static Uri ReadHeaderValue(XmlDictionaryReader reader, AddressingVersion version, UriCache uriCache)
        {
            Fx.Assert(reader.IsStartElement(XD.AddressingDictionary.To, version.DictionaryNamespace), "");

            string toString = reader.ReadElementContentAsString();

            if ((object)toString == (object)version.Anonymous)
            {
                return version.AnonymousUri;
            }

            if (uriCache == null)
            {
                return new Uri(toString);
            }

            return uriCache.CreateUri(toString);
        }

        public static ToHeader ReadHeader(XmlDictionaryReader reader, AddressingVersion version, UriCache uriCache,
            string actor, bool mustUnderstand, bool relay)
        {
            Uri to = ReadHeaderValue(reader, version, uriCache);

            if (actor.Length == 0 && mustUnderstand == mustUnderstandValue && !relay)
            {
                if ((object)to == (object)version.Anonymous)
                {
                    if (version == AddressingVersion.WSAddressing10)
                        return AnonymousTo10;
                    else if (version == AddressingVersion.WSAddressingAugust2004)
                        return AnonymousTo200408;
                    else
                        throw ExceptionHelper.PlatformNotSupported();
                }
                else
                {
                    return new ToHeader(to, version);
                }
            }
            else
            {
                return new FullToHeader(to, actor, mustUnderstand, relay, version);
            }
        }

        private class AnonymousToHeader : ToHeader
        {
            public AnonymousToHeader(AddressingVersion version)
                : base(version.AnonymousUri, version)
            {
            }

            protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
            {
                writer.WriteString(this.Version.DictionaryAnonymous);
            }
        }

        internal class DictionaryToHeader : ToHeader
        {
            private XmlDictionaryString _dictionaryTo;

            public DictionaryToHeader(Uri to, XmlDictionaryString dictionaryTo, AddressingVersion version)
                : base(to, version)
            {
                _dictionaryTo = dictionaryTo;
            }

            protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
            {
                writer.WriteString(_dictionaryTo);
            }
        }

        internal class FullToHeader : ToHeader
        {
            private string _actor;
            private bool _mustUnderstand;
            private bool _relay;

            public FullToHeader(Uri to, string actor, bool mustUnderstand, bool relay, AddressingVersion version)
                : base(to, version)
            {
                _actor = actor;
                _mustUnderstand = mustUnderstand;
                _relay = relay;
            }

            public override string Actor
            {
                get { return _actor; }
            }

            public override bool MustUnderstand
            {
                get { return _mustUnderstand; }
            }

            public override bool Relay
            {
                get { return _relay; }
            }
        }
    }

    internal class ReplyToHeader : AddressingHeader
    {
        private EndpointAddress _replyTo;
        private const bool mustUnderstandValue = false;
        private static ReplyToHeader s_anonymousReplyToHeader10;

        private ReplyToHeader(EndpointAddress replyTo, AddressingVersion version)
            : base(version)
        {
            _replyTo = replyTo;
        }

        public EndpointAddress ReplyTo
        {
            get { return _replyTo; }
        }

        public override XmlDictionaryString DictionaryName
        {
            get { return XD.AddressingDictionary.ReplyTo; }
        }

        public override bool MustUnderstand
        {
            get { return mustUnderstandValue; }
        }

        public static ReplyToHeader AnonymousReplyTo10
        {
            get
            {
                if (s_anonymousReplyToHeader10 == null)
                    s_anonymousReplyToHeader10 = new ReplyToHeader(EndpointAddress.AnonymousAddress, AddressingVersion.WSAddressing10);
                return s_anonymousReplyToHeader10;
            }
        }


        public static ReplyToHeader Create(EndpointAddress replyTo, AddressingVersion addressingVersion)
        {
            if (replyTo == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("replyTo"));
            if (addressingVersion == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("addressingVersion"));
            return new ReplyToHeader(replyTo, addressingVersion);
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            _replyTo.WriteContentsTo(this.Version, writer);
        }

        public static ReplyToHeader ReadHeader(XmlDictionaryReader reader, AddressingVersion version,
            string actor, bool mustUnderstand, bool relay)
        {
            EndpointAddress replyTo = ReadHeaderValue(reader, version);

            if (actor.Length == 0 && mustUnderstand == mustUnderstandValue && !relay)
            {
                if ((object)replyTo == (object)EndpointAddress.AnonymousAddress)
                {
                    if (version == AddressingVersion.WSAddressing10)
                        return AnonymousReplyTo10;
                    else
                        // Verify that only WSA10 is supported
                        throw ExceptionHelper.PlatformNotSupported();
                }
                return new ReplyToHeader(replyTo, version);
            }
            else
            {
                return new FullReplyToHeader(replyTo, actor, mustUnderstand, relay, version);
            }
        }

        public static EndpointAddress ReadHeaderValue(XmlDictionaryReader reader, AddressingVersion version)
        {
            Fx.Assert(reader.IsStartElement(XD.AddressingDictionary.ReplyTo, version.DictionaryNamespace), "");
            return EndpointAddress.ReadFrom(version, reader);
        }

        internal class FullReplyToHeader : ReplyToHeader
        {
            private string _actor;
            private bool _mustUnderstand;
            private bool _relay;

            public FullReplyToHeader(EndpointAddress replyTo, string actor, bool mustUnderstand, bool relay, AddressingVersion version)
                : base(replyTo, version)
            {
                _actor = actor;
                _mustUnderstand = mustUnderstand;
                _relay = relay;
            }

            public override string Actor
            {
                get { return _actor; }
            }

            public override bool MustUnderstand
            {
                get { return _mustUnderstand; }
            }

            public override bool Relay
            {
                get { return _relay; }
            }
        }
    }

    internal class MessageIDHeader : AddressingHeader
    {
        private UniqueId _messageId;
        private const bool mustUnderstandValue = false;

        private MessageIDHeader(UniqueId messageId, AddressingVersion version)
            : base(version)
        {
            _messageId = messageId;
        }

        public override XmlDictionaryString DictionaryName
        {
            get { return XD.AddressingDictionary.MessageId; }
        }

        public UniqueId MessageId
        {
            get { return _messageId; }
        }

        public override bool MustUnderstand
        {
            get { return mustUnderstandValue; }
        }

        public static MessageIDHeader Create(UniqueId messageId, AddressingVersion addressingVersion)
        {
            if (object.ReferenceEquals(messageId, null))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("messageId"));
            if (addressingVersion == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("addressingVersion"));
            return new MessageIDHeader(messageId, addressingVersion);
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteValue(_messageId);
        }

        public static UniqueId ReadHeaderValue(XmlDictionaryReader reader, AddressingVersion version)
        {
            Fx.Assert(reader.IsStartElement(XD.AddressingDictionary.MessageId, version.DictionaryNamespace), "");
            return reader.ReadElementContentAsUniqueId();
        }

        public static MessageIDHeader ReadHeader(XmlDictionaryReader reader, AddressingVersion version,
            string actor, bool mustUnderstand, bool relay)
        {
            UniqueId messageId = ReadHeaderValue(reader, version);

            if (actor.Length == 0 && mustUnderstand == mustUnderstandValue && !relay)
            {
                return new MessageIDHeader(messageId, version);
            }
            else
            {
                return new FullMessageIDHeader(messageId, actor, mustUnderstand, relay, version);
            }
        }

        internal class FullMessageIDHeader : MessageIDHeader
        {
            private string _actor;
            private bool _mustUnderstand;
            private bool _relay;

            public FullMessageIDHeader(UniqueId messageId, string actor, bool mustUnderstand, bool relay, AddressingVersion version)
                : base(messageId, version)
            {
                _actor = actor;
                _mustUnderstand = mustUnderstand;
                _relay = relay;
            }

            public override string Actor
            {
                get { return _actor; }
            }

            public override bool MustUnderstand
            {
                get { return _mustUnderstand; }
            }

            public override bool Relay
            {
                get { return _relay; }
            }
        }
    }

    internal class RelatesToHeader : AddressingHeader
    {
        private UniqueId _messageId;
        private const bool mustUnderstandValue = false;
        internal static readonly Uri ReplyRelationshipType = new Uri(Addressing10Strings.ReplyRelationship);

        private RelatesToHeader(UniqueId messageId, AddressingVersion version)
            : base(version)
        {
            _messageId = messageId;
        }

        public override XmlDictionaryString DictionaryName
        {
            get { return XD.AddressingDictionary.RelatesTo; }
        }

        public UniqueId UniqueId
        {
            get { return _messageId; }
        }

        public override bool MustUnderstand
        {
            get { return mustUnderstandValue; }
        }

        public virtual Uri RelationshipType
        {
            get { return ReplyRelationshipType; }
        }

        public static RelatesToHeader Create(UniqueId messageId, AddressingVersion addressingVersion)
        {
            if (object.ReferenceEquals(messageId, null))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("messageId"));
            if (addressingVersion == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("addressingVersion"));
            return new RelatesToHeader(messageId, addressingVersion);
        }

        public static RelatesToHeader Create(UniqueId messageId, AddressingVersion addressingVersion, Uri relationshipType)
        {
            if (object.ReferenceEquals(messageId, null))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("messageId"));
            if (addressingVersion == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("addressingVersion"));
            if (relationshipType == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("relationshipType"));
            if (relationshipType == ReplyRelationshipType)
            {
                return new RelatesToHeader(messageId, addressingVersion);
            }
            else
            {
                return new FullRelatesToHeader(messageId, "", false, false, addressingVersion);
            }
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteValue(_messageId);
        }

        public static void ReadHeaderValue(XmlDictionaryReader reader, AddressingVersion version, out Uri relationshipType, out UniqueId messageId)
        {
            AddressingDictionary addressingDictionary = XD.AddressingDictionary;

            // The RelationshipType attribute has no namespace.
            relationshipType = ReplyRelationshipType;
            /*
            string relation = reader.GetAttribute(addressingDictionary.RelationshipType, addressingDictionary.Empty);
            if (relation == null)
            {
                relationshipType = ReplyRelationshipType;
            }
            else
            {
                relationshipType = new Uri(relation);
            }
            */
            Fx.Assert(reader.IsStartElement(addressingDictionary.RelatesTo, version.DictionaryNamespace), "");
            messageId = reader.ReadElementContentAsUniqueId();
        }

        public static RelatesToHeader ReadHeader(XmlDictionaryReader reader, AddressingVersion version,
            string actor, bool mustUnderstand, bool relay)
        {
            UniqueId messageId;
            Uri relationship;
            ReadHeaderValue(reader, version, out relationship, out messageId);

            if (actor.Length == 0 && mustUnderstand == mustUnderstandValue && !relay && (object)relationship == (object)ReplyRelationshipType)
            {
                return new RelatesToHeader(messageId, version);
            }
            else
            {
                return new FullRelatesToHeader(messageId, actor, mustUnderstand, relay, version);
            }
        }

        internal class FullRelatesToHeader : RelatesToHeader
        {
            private string _actor;
            private bool _mustUnderstand;
            private bool _relay;
            //Uri relationship;

            public FullRelatesToHeader(UniqueId messageId, string actor, bool mustUnderstand, bool relay, AddressingVersion version)
                : base(messageId, version)
            {
                //this.relationship = relationship;
                _actor = actor;
                _mustUnderstand = mustUnderstand;
                _relay = relay;
            }

            public override string Actor
            {
                get { return _actor; }
            }

            public override bool MustUnderstand
            {
                get { return _mustUnderstand; }
            }

            /*
            public override Uri RelationshipType
            {
                get { return relationship; }
            }
            */

            public override bool Relay
            {
                get { return _relay; }
            }

            protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
            {
                /*
                if ((object)relationship != (object)ReplyRelationshipType)
                {
                    // The RelationshipType attribute has no namespace.
                    writer.WriteStartAttribute(AddressingStrings.RelationshipType, AddressingStrings.Empty);
                    writer.WriteString(relationship.AbsoluteUri);
                    writer.WriteEndAttribute();
                }
                */
                writer.WriteValue(_messageId);
            }
        }
    }
}
