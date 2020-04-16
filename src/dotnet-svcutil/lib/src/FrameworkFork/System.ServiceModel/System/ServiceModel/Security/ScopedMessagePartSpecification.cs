// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ServiceModel.Channels;
using Microsoft.Xml;

namespace System.ServiceModel.Security
{
    public class ScopedMessagePartSpecification
    {
        private MessagePartSpecification _channelParts;
        private Dictionary<string, MessagePartSpecification> _actionParts;
        private Dictionary<string, MessagePartSpecification> _readOnlyNormalizedActionParts;
        private bool _isReadOnly;

        public ScopedMessagePartSpecification()
        {
            _channelParts = new MessagePartSpecification();
            _actionParts = new Dictionary<string, MessagePartSpecification>();
        }

        public ICollection<string> Actions
        {
            get
            {
                return _actionParts.Keys;
            }
        }

        public MessagePartSpecification ChannelParts
        {
            get
            {
                return _channelParts;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
        }

        public ScopedMessagePartSpecification(ScopedMessagePartSpecification other)
            : this()
        {
            if (other == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("other"));

            _channelParts.Union(other._channelParts);
            if (other._actionParts != null)
            {
                foreach (string action in other._actionParts.Keys)
                {
                    MessagePartSpecification p = new MessagePartSpecification();
                    p.Union(other._actionParts[action]);
                    _actionParts[action] = p;
                }
            }
        }

        internal ScopedMessagePartSpecification(ScopedMessagePartSpecification other, bool newIncludeBody)
            : this(other)
        {
            _channelParts.IsBodyIncluded = newIncludeBody;
            foreach (string action in _actionParts.Keys)
                _actionParts[action].IsBodyIncluded = newIncludeBody;
        }

        public void AddParts(MessagePartSpecification parts)
        {
            if (parts == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("parts"));

            ThrowIfReadOnly();

            _channelParts.Union(parts);
        }

        public void AddParts(MessagePartSpecification parts, string action)
        {
            if (action == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("action"));
            if (parts == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("parts"));

            ThrowIfReadOnly();

            if (!_actionParts.ContainsKey(action))
                _actionParts[action] = new MessagePartSpecification();
            _actionParts[action].Union(parts);
        }

        internal void AddParts(MessagePartSpecification parts, XmlDictionaryString action)
        {
            if (action == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("action"));
            AddParts(parts, action.Value);
        }

        internal bool IsEmpty()
        {
            bool result;
            if (!_channelParts.IsEmpty())
            {
                result = false;
            }
            else
            {
                result = true;
                foreach (string action in this.Actions)
                {
                    MessagePartSpecification parts;
                    if (TryGetParts(action, true, out parts))
                    {
                        if (!parts.IsEmpty())
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        public bool TryGetParts(string action, bool excludeChannelScope, out MessagePartSpecification parts)
        {
            if (action == null)
                action = MessageHeaders.WildcardAction;
            parts = null;

            if (_isReadOnly)
            {
                if (_readOnlyNormalizedActionParts.ContainsKey(action))
                    if (excludeChannelScope)
                        parts = _actionParts[action];
                    else
                        parts = _readOnlyNormalizedActionParts[action];
            }
            else if (_actionParts.ContainsKey(action))
            {
                MessagePartSpecification p = new MessagePartSpecification();
                p.Union(_actionParts[action]);
                if (!excludeChannelScope)
                    p.Union(_channelParts);
                parts = p;
            }

            return parts != null;
        }

        internal void CopyTo(ScopedMessagePartSpecification target)
        {
            if (target == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("target");
            }
            target.ChannelParts.IsBodyIncluded = this.ChannelParts.IsBodyIncluded;
            foreach (XmlQualifiedName headerType in ChannelParts.HeaderTypes)
            {
                if (!target._channelParts.IsHeaderIncluded(headerType.Name, headerType.Namespace))
                {
                    target.ChannelParts.HeaderTypes.Add(headerType);
                }
            }
            foreach (string action in _actionParts.Keys)
            {
                target.AddParts(_actionParts[action], action);
            }
        }

        public bool TryGetParts(string action, out MessagePartSpecification parts)
        {
            return this.TryGetParts(action, false, out parts);
        }

        public void MakeReadOnly()
        {
            if (!_isReadOnly)
            {
                _readOnlyNormalizedActionParts = new Dictionary<string, MessagePartSpecification>();
                foreach (string action in _actionParts.Keys)
                {
                    MessagePartSpecification p = new MessagePartSpecification();
                    p.Union(_actionParts[action]);
                    p.Union(_channelParts);
                    p.MakeReadOnly();
                    _readOnlyNormalizedActionParts[action] = p;
                }
                _isReadOnly = true;
            }
        }

        private void ThrowIfReadOnly()
        {
            if (_isReadOnly)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));
        }
    }
}
