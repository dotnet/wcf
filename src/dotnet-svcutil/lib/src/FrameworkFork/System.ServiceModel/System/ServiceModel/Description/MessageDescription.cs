// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using Microsoft.Xml;
using System.Diagnostics;
using System.Net.Security;
using System.ServiceModel.Security;
using System.ComponentModel;

namespace System.ServiceModel.Description
{
    [DebuggerDisplay("Action={_action}, Direction={_direction}, MessageType={_messageType}")]
    public class MessageDescription
    {
        private static Type s_typeOfUntypedMessage;
        private string _action;
        private MessageDirection _direction;
        private MessageDescriptionItems _items;
        private XmlName _messageName;
        private Type _messageType;
        private XmlQualifiedName _xsdType;
        private ProtectionLevel _protectionLevel;
        private bool _hasProtectionLevel;

        public MessageDescription(string action, MessageDirection direction) : this(action, direction, null) { }
        internal MessageDescription(string action, MessageDirection direction, MessageDescriptionItems items)
        {
            if (!MessageDirectionHelper.IsDefined(direction))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("direction"));

            _action = action;
            _direction = direction;
            _items = items;
        }

        internal MessageDescription(MessageDescription other)
        {
            _action = other._action;
            _direction = other._direction;
            this.Items.Body = other.Items.Body.Clone();
            foreach (MessageHeaderDescription mhd in other.Items.Headers)
            {
                this.Items.Headers.Add(mhd.Clone() as MessageHeaderDescription);
            }
            foreach (MessagePropertyDescription mpd in other.Items.Properties)
            {
                this.Items.Properties.Add(mpd.Clone() as MessagePropertyDescription);
            }
            this.MessageName = other.MessageName;
            this.MessageType = other.MessageType;
            this.XsdTypeName = other.XsdTypeName;
            _hasProtectionLevel = other._hasProtectionLevel;
            this.ProtectionLevel = other.ProtectionLevel;
        }

        internal MessageDescription Clone()
        {
            return new MessageDescription(this);
        }

        public string Action
        {
            get { return _action; }
            internal set { _action = value; }
        }

        public MessageBodyDescription Body
        {
            get { return Items.Body; }
        }

        public MessageDirection Direction
        {
            get { return _direction; }
        }

        public MessageHeaderDescriptionCollection Headers
        {
            get { return Items.Headers; }
        }

        public MessagePropertyDescriptionCollection Properties
        {
            get { return Items.Properties; }
        }

        internal MessageDescriptionItems Items
        {
            get
            {
                if (_items == null)
                    _items = new MessageDescriptionItems();
                return _items;
            }
        }

        public ProtectionLevel ProtectionLevel
        {
            get { return _protectionLevel; }
            set
            {
                if (!ProtectionLevelHelper.IsDefined(value))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                _protectionLevel = value;
                _hasProtectionLevel = true;
            }
        }

        public bool ShouldSerializeProtectionLevel()
        {
            return this.HasProtectionLevel;
        }

        public bool HasProtectionLevel
        {
            get { return _hasProtectionLevel; }
        }

        internal static Type TypeOfUntypedMessage
        {
            get
            {
                if (s_typeOfUntypedMessage == null)
                {
                    s_typeOfUntypedMessage = typeof(Message);
                }
                return s_typeOfUntypedMessage;
            }
        }

        internal XmlName MessageName
        {
            get { return _messageName; }
            set { _messageName = value; }
        }

        // Not serializable on purpose, metadata import/export cannot
        // produce it, only available when binding to runtime
        [DefaultValue(null)]
        public Type MessageType
        {
            get { return _messageType; }
            set { _messageType = value; }
        }

        internal bool IsTypedMessage
        {
            get
            {
                return _messageType != null;
            }
        }

        internal bool IsUntypedMessage
        {
            get
            {
                return (Body.ReturnValue != null && Body.Parts.Count == 0 && Body.ReturnValue.Type == TypeOfUntypedMessage) ||
                     (Body.ReturnValue == null && Body.Parts.Count == 1 && Body.Parts[0].Type == TypeOfUntypedMessage);
            }
        }

        internal bool IsVoid
        {
            get
            {
                return !IsTypedMessage && Body.Parts.Count == 0 && (Body.ReturnValue == null || Body.ReturnValue.Type == typeof(void));
            }
        }

        internal XmlQualifiedName XsdTypeName
        {
            get { return _xsdType; }
            set { _xsdType = value; }
        }

        internal void ResetProtectionLevel()
        {
            _protectionLevel = ProtectionLevel.None;
            _hasProtectionLevel = false;
        }
    }

    internal class MessageDescriptionItems
    {
        private MessageHeaderDescriptionCollection _headers;
        private MessageBodyDescription _body;
        private MessagePropertyDescriptionCollection _properties;

        internal MessageBodyDescription Body
        {
            get
            {
                if (_body == null)
                    _body = new MessageBodyDescription();
                return _body;
            }
            set
            {
                _body = value;
            }
        }

        internal MessageHeaderDescriptionCollection Headers
        {
            get
            {
                if (_headers == null)
                    _headers = new MessageHeaderDescriptionCollection();
                return _headers;
            }
        }

        internal MessagePropertyDescriptionCollection Properties
        {
            get
            {
                if (_properties == null)
                    _properties = new MessagePropertyDescriptionCollection();
                return _properties;
            }
        }
    }
}
