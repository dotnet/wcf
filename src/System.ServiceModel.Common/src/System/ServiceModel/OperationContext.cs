// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.ServiceModel.Channels;
using System.Linq;

namespace System.ServiceModel
{
    public sealed class OperationContext : IExtensibleObject<OperationContext>
    {
        [ThreadStatic]
        private static Holder s_currentContext;

        private IContextChannel _originalContextchannel;
        private Message _clientReply;
        private bool _closeClientReply;
        private ExtensionCollection<OperationContext> _extensions;
        private RequestContext _requestContext;
        private Message _request = null;
        private bool _isServiceReentrant = false;
        private MessageProperties _outgoingMessageProperties;
        private MessageHeaders _outgoingMessageHeaders;
        private MessageVersion _outgoingMessageVersion;

        public event EventHandler OperationCompleted;

        public OperationContext(IContextChannel channel)
        {
            if (channel == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(channel));

            _originalContextchannel = channel;
            _extensions = new ExtensionCollection<OperationContext>(this);
            _extensions.Add(new FieldAccessorExtension());
        }

        internal OperationContext()
            : this(MessageVersion.Soap12WSAddressing10)
        {
        }

        internal OperationContext(MessageVersion outgoingMessageVersion)
        {
            if (outgoingMessageVersion == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(outgoingMessageVersion));

            _outgoingMessageVersion = outgoingMessageVersion;
        }

        public static OperationContext Current
        {
            get
            {
                return CurrentHolder.Context;
            }

            set
            {
                CurrentHolder.Context = value;
            }
        }

        internal static Holder CurrentHolder
        {
            get
            {
                Holder holder = OperationContext.s_currentContext;
                if (holder == null)
                {
                    holder = new Holder();
                    OperationContext.s_currentContext = holder;
                }
                return holder;
            }
        }

        public bool IsUserContext
        {
            get
            {
                return (_request == null);
            }
        }

        public IExtensionCollection<OperationContext> Extensions
        {
            get
            {
                if (_extensions == null)
                {
                    _extensions = new ExtensionCollection<OperationContext>(this);
                }
                return _extensions;
            }
        }

        internal bool IsServiceReentrant
        {
            get { return _isServiceReentrant; }
            set { _isServiceReentrant = value; }
        }

        internal Message IncomingMessage => _clientReply ?? _request;

        internal bool HasOutgoingMessageHeaders => (_outgoingMessageHeaders != null);

        public MessageHeaders OutgoingMessageHeaders
        {
            get
            {
                if (_outgoingMessageHeaders == null)
                    _outgoingMessageHeaders = new MessageHeaders(this.OutgoingMessageVersion);

                return _outgoingMessageHeaders;
            }
        }

        public MessageProperties OutgoingMessageProperties
        {
            get
            {
                if (_outgoingMessageProperties == null)
                    _outgoingMessageProperties = new MessageProperties();

                return _outgoingMessageProperties;
            }
        }

        internal MessageVersion OutgoingMessageVersion => _outgoingMessageVersion;

        public MessageHeaders IncomingMessageHeaders
        {
            get
            {
                Message message = _clientReply ?? _request;
                return message?.Headers;
            }
        }

        public MessageProperties IncomingMessageProperties
        {
            get
            {
                Message message = _clientReply ?? _request;
                return message?.Properties;
            }
        }

        public MessageVersion IncomingMessageVersion
        {
            get
            {
                Message message = _clientReply ?? _request;
                return message?.Version;
            }
        }

        //public InstanceContext InstanceContext
        //{
        //    get { return _instanceContext; }
        //}

        public RequestContext RequestContext
        {
            get { return _requestContext; }
            set { _requestContext = value; }
        }

        internal void ClearClientReplyNoThrow()
        {
            _clientReply = null;
        }

        internal void FireOperationCompleted()
        {
            try
            {
                this.OperationCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                    throw;

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(e);
            }
        }

        internal void SetClientReply(Message message, bool closeMessage)
        {
            Message oldClientReply = null;

            if (!object.Equals(message, _clientReply))
            {
                if (_closeClientReply && (_clientReply != null))
                {
                    oldClientReply = _clientReply;
                }

                _clientReply = message;
            }

            _closeClientReply = closeMessage;

            if (oldClientReply != null)
            {
                oldClientReply.Close();
            }
        }

        internal class Holder
        {
            private OperationContext _context;

            public OperationContext Context
            {
                get
                {
                    return _context;
                }

                set
                {
                    _context = value;
                }
            }
        }

        private class FieldAccessorExtension : IExtension<OperationContext>, IDictionary<string, object>
        {
            private OperationContext _operationContext;

            public object this[string key]
            {
                get
                {
                    switch (key)
                    {
                        case "_clientReply":
                            return _operationContext._clientReply;
                        case "_closeClientReply":
                            return _operationContext._closeClientReply;
                        case "_requestContext":
                            return _operationContext._requestContext;
                        case "_request":
                            return _operationContext._request;
                        case "_outgoingMessageVersion":
                            return _operationContext._outgoingMessageVersion;
                        case "_outgoingMessageHeaders":
                            return _operationContext._outgoingMessageHeaders;
                        case "_outgoingMessageProperties":
                            return _operationContext._outgoingMessageProperties;
                        case "_isServiceReentrant":
                            return _operationContext._isServiceReentrant;
                        case "_originalContextChannel":
                            return _operationContext._originalContextchannel;
                        case "FireOperationCompleted":
                            return (Action)_operationContext.FireOperationCompleted;
                        default:
                            throw Fx.Exception.Argument(nameof(key), "Invalid field get:" + key);
                    }
                }
                set
                {
                    switch (key)
                    {
                        case "_clientReply":
                            _operationContext._clientReply = (Message)value;
                            break;
                        case "_closeClientReply":
                            _operationContext._closeClientReply = (bool)value;
                            break;
                        case "_requestContext":
                            _operationContext._requestContext = (RequestContext)value;
                            break;
                        case "_request":
                            _operationContext._request = (Message)value;
                            break;
                        case "_outgoingMessageVersion":
                            _operationContext._outgoingMessageVersion = (MessageVersion)value;
                            break;
                        case "_outgoingMessageHeaders":
                            _operationContext._outgoingMessageHeaders = (MessageHeaders)value;
                            break;
                        case "_outgoingMessageProperties":
                            _operationContext._outgoingMessageProperties = (MessageProperties)value;
                            break;
                        case "_isServiceReentrant":
                            _operationContext._isServiceReentrant = (bool)value;
                            break;
                        case "_originalContextChannel":
                            _operationContext._originalContextchannel = (IContextChannel)value;
                            break;
                        case "FireOperationCompleted":
                            throw Fx.Exception.Argument(nameof(key), "Can not replace FireOperationCompleted");
                        default:
                            throw Fx.Exception.Argument(nameof(key), "Invalid field set:" + key);
                    }
                }
            }

            public int Count
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsReadOnly => false;

            public ICollection<string> Keys
                =>
                    new List<string>(new[]
                    {
                        "_closeClientReply", "_requestContext", "_request", "_outgoingMessageVersion",
                        "_outgoingMessageHeaders", "_outgoingMessageProperties", "_isServiceReentrant",
                        "_originalContextChannel", "FireOperationCompleted"
                    });

            public ICollection<object> Values => (from key in Keys select this[key]).ToList();

            public void Add(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }

            public void Add(string key, object value)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }

            public bool ContainsKey(string key)
            {
                return Keys.Contains(key);
            }

            public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                return (from key in Keys select new KeyValuePair<string, object>(key, this[key])).GetEnumerator();
            }

            public bool Remove(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }

            public bool Remove(string key)
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(string key, out object value)
            {
                if (Keys.Contains(key))
                {
                    value = this[key];
                    return true;
                }

                value = null;
                return false;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Attach(OperationContext owner)
            {
                _operationContext = owner;
            }

            public void Detach(OperationContext owner)
            {
                Fx.Assert(_operationContext == owner, "Detaching the wrong OperationContext");
                _operationContext = null;
            }
        }
    }
}

