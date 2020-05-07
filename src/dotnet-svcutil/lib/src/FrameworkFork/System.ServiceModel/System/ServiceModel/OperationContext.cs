// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Security.Claims;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel
{
    public sealed class OperationContext : IExtensibleObject<OperationContext>
    {
        [ThreadStatic]
        private static Holder s_currentContext;

        private ServiceChannel _channel;
        private Message _clientReply;
        private bool _closeClientReply;
        private ExtensionCollection<OperationContext> _extensions;
        private RequestContext _requestContext;
        private Message _request;
        private InstanceContext _instanceContext;
        private bool _isServiceReentrant = false;
        internal IPrincipal threadPrincipal;
        private MessageProperties _outgoingMessageProperties;
        private MessageHeaders _outgoingMessageHeaders;
        private MessageVersion _outgoingMessageVersion;
        private EndpointDispatcher _endpointDispatcher;

        public event EventHandler OperationCompleted;

        public OperationContext(IContextChannel channel)
        {
            if (channel == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("channel"));

            ServiceChannel serviceChannel = channel as ServiceChannel;

            //Could be a TransparentProxy
            if (serviceChannel == null)
            {
                serviceChannel = ServiceChannelFactory.GetServiceChannel(channel);
            }

            if (serviceChannel != null)
            {
                _outgoingMessageVersion = serviceChannel.MessageVersion;
                _channel = serviceChannel;
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxInvalidChannelToOperationContext));
            }
        }

        internal OperationContext()
            : this(MessageVersion.Soap12WSAddressing10)
        {
        }

        internal OperationContext(MessageVersion outgoingMessageVersion)
        {
            if (outgoingMessageVersion == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("outgoingMessageVersion"));

            _outgoingMessageVersion = outgoingMessageVersion;
        }

        internal OperationContext(RequestContext requestContext, Message request, ServiceChannel channel)
        {
            _channel = channel;
            _requestContext = requestContext;
            _request = request;
            _outgoingMessageVersion = channel.MessageVersion;
        }

        public IContextChannel Channel
        {
            get { return this.GetCallbackChannel<IContextChannel>(); }
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

        public EndpointDispatcher EndpointDispatcher
        {
            get
            {
                return _endpointDispatcher;
            }
            set
            {
                _endpointDispatcher = value;
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


        internal Message IncomingMessage
        {
            get { return _clientReply ?? _request; }
        }

        internal ServiceChannel InternalServiceChannel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        internal bool HasOutgoingMessageHeaders
        {
            get { return (_outgoingMessageHeaders != null); }
        }

        public MessageHeaders OutgoingMessageHeaders
        {
            get
            {
                if (_outgoingMessageHeaders == null)
                    _outgoingMessageHeaders = new MessageHeaders(this.OutgoingMessageVersion);

                return _outgoingMessageHeaders;
            }
        }

        internal bool HasOutgoingMessageProperties
        {
            get { return (_outgoingMessageProperties != null); }
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

        internal MessageVersion OutgoingMessageVersion
        {
            get { return _outgoingMessageVersion; }
        }

        public MessageHeaders IncomingMessageHeaders
        {
            get
            {
                Message message = _clientReply ?? _request;
                if (message != null)
                    return message.Headers;
                else
                    return null;
            }
        }

        public MessageProperties IncomingMessageProperties
        {
            get
            {
                Message message = _clientReply ?? _request;
                if (message != null)
                    return message.Properties;
                else
                    return null;
            }
        }

        public MessageVersion IncomingMessageVersion
        {
            get
            {
                Message message = _clientReply ?? _request;
                if (message != null)
                    return message.Version;
                else
                    return null;
            }
        }

        public InstanceContext InstanceContext
        {
            get { return _instanceContext; }
        }

        public RequestContext RequestContext
        {
            get { return _requestContext; }
            set { _requestContext = value; }
        }


        public string SessionId
        {
            get
            {
                if (_channel != null)
                {
                    IChannel inner = _channel.InnerChannel;
                    if (inner != null)
                    {
                        ISessionChannel<IDuplexSession> duplex = inner as ISessionChannel<IDuplexSession>;
                        if ((duplex != null) && (duplex.Session != null))
                            return duplex.Session.Id;

                        ISessionChannel<IInputSession> input = inner as ISessionChannel<IInputSession>;
                        if ((input != null) && (input.Session != null))
                            return input.Session.Id;

                        ISessionChannel<IOutputSession> output = inner as ISessionChannel<IOutputSession>;
                        if ((output != null) && (output.Session != null))
                            return output.Session.Id;
                    }
                }
                return null;
            }
        }


        internal IPrincipal ThreadPrincipal
        {
            get { return this.threadPrincipal; }
            set { this.threadPrincipal = value; }
        }

        public ClaimsPrincipal ClaimsPrincipal
        {
            get;
            internal set;
        }

        internal void ClearClientReplyNoThrow()
        {
            _clientReply = null;
        }

        internal void FireOperationCompleted()
        {
            try
            {
                EventHandler handler = this.OperationCompleted;

                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                    throw;

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(e);
            }
        }

        public T GetCallbackChannel<T>()
        {
            if (_channel == null || this.IsUserContext)
                return default(T);

            // yes, we might throw InvalidCastException here.  Is it really
            // better to check and throw something else instead?
            return (T)_channel.Proxy;
        }

        internal void ReInit(RequestContext requestContext, Message request, ServiceChannel channel)
        {
            _requestContext = requestContext;
            _request = request;
            _channel = channel;
        }

        internal void Recycle()
        {
            _requestContext = null;
            _request = null;
            _extensions = null;
            _instanceContext = null;
            this.threadPrincipal = null;
            this.SetClientReply(null, false);
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

        internal void SetInstanceContext(InstanceContext instanceContext)
        {
            _instanceContext = instanceContext;
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
    }
}

