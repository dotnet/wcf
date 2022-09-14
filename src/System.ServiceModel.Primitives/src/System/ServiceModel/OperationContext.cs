// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.Security.Claims;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Threading;

namespace System.ServiceModel
{
    public sealed class OperationContext : IExtensibleObject<OperationContext>
    {
        static OperationContext()
        {
            DisableAsyncFlow = AppContext.TryGetSwitch("System.ServiceModel.OperationContext.DisableAsyncFlow", out var enabled) && enabled;
            if (!DisableAsyncFlow)
            {
                s_asyncContext = new AsyncLocal<OperationContext>();
            }
        }

        [ThreadStatic]
        private static Holder s_currentContext;
        private static AsyncLocal<OperationContext> s_asyncContext;
        private Message _clientReply;
        private bool _closeClientReply;
        private ExtensionCollection<OperationContext> _extensions;
        private Message _request;
        private bool _isServiceReentrant = false;
        internal IPrincipal threadPrincipal;
        private MessageProperties _outgoingMessageProperties;
        private MessageHeaders _outgoingMessageHeaders;
        private EndpointDispatcher _endpointDispatcher;

        public event EventHandler OperationCompleted;

        public OperationContext(IContextChannel channel)
        {
            if (channel == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(channel)));
            }

            ServiceChannel serviceChannel = channel as ServiceChannel;

            //Could be a TransparentProxy
            if (serviceChannel == null)
            {
                serviceChannel = ServiceChannelFactory.GetServiceChannel(channel);
            }

            if (serviceChannel != null)
            {
                OutgoingMessageVersion = serviceChannel.MessageVersion;
                InternalServiceChannel = serviceChannel;
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.SFxInvalidChannelToOperationContext));
            }
        }

        internal OperationContext()
            : this(MessageVersion.Soap12WSAddressing10)
        {
        }

        internal OperationContext(MessageVersion outgoingMessageVersion)
        {
            OutgoingMessageVersion = outgoingMessageVersion ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(outgoingMessageVersion)));
        }

        internal OperationContext(RequestContext requestContext, Message request, ServiceChannel channel)
        {
            InternalServiceChannel = channel;
            RequestContext = requestContext;
            _request = request;
            OutgoingMessageVersion = channel.MessageVersion;
        }

        public IContextChannel Channel
        {
            get { return GetCallbackChannel<IContextChannel>(); }
        }

        public static OperationContext Current
        {
            get
            {
                if (DisableAsyncFlow)
                {
                    return CurrentHolder.Context;
                }
                else
                {
                    return s_asyncContext.Value;
                }
            }

            set
            {
                if (DisableAsyncFlow)
                {
                    CurrentHolder.Context = value;
                }
                else
                {
                    s_asyncContext.Value = value;
                }
            }
        }

        internal static Holder CurrentHolder
        {
            get
            {
                Holder holder;
                holder = s_currentContext;
                if (holder == null)
                {
                    holder = new Holder();
                    s_currentContext = holder;
                }

                return holder;
            }
        }

        internal static bool DisableAsyncFlow { get; }

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

        internal ServiceChannel InternalServiceChannel { get; set; }

        internal bool HasOutgoingMessageHeaders
        {
            get { return (_outgoingMessageHeaders != null); }
        }

        public MessageHeaders OutgoingMessageHeaders
        {
            get
            {
                if (_outgoingMessageHeaders == null)
                {
                    _outgoingMessageHeaders = new MessageHeaders(OutgoingMessageVersion);
                }

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
                {
                    _outgoingMessageProperties = new MessageProperties();
                }

                return _outgoingMessageProperties;
            }
        }

        internal MessageVersion OutgoingMessageVersion { get; }

        public MessageHeaders IncomingMessageHeaders
        {
            get
            {
                Message message = _clientReply ?? _request;
                if (message != null)
                {
                    return message.Headers;
                }
                else
                {
                    return null;
                }
            }
        }

        public MessageProperties IncomingMessageProperties
        {
            get
            {
                Message message = _clientReply ?? _request;
                if (message != null)
                {
                    return message.Properties;
                }
                else
                {
                    return null;
                }
            }
        }

        public MessageVersion IncomingMessageVersion
        {
            get
            {
                Message message = _clientReply ?? _request;
                if (message != null)
                {
                    return message.Version;
                }
                else
                {
                    return null;
                }
            }
        }

        public InstanceContext InstanceContext { get; private set; }

        public RequestContext RequestContext { get; set; }


        public string SessionId
        {
            get
            {
                if (InternalServiceChannel != null)
                {
                    IChannel inner = InternalServiceChannel.InnerChannel;
                    if (inner != null)
                    {
                        ISessionChannel<IDuplexSession> duplex = inner as ISessionChannel<IDuplexSession>;
                        if ((duplex != null) && (duplex.Session != null))
                        {
                            return duplex.Session.Id;
                        }

                        ISessionChannel<IInputSession> input = inner as ISessionChannel<IInputSession>;
                        if ((input != null) && (input.Session != null))
                        {
                            return input.Session.Id;
                        }

                        ISessionChannel<IOutputSession> output = inner as ISessionChannel<IOutputSession>;
                        if ((output != null) && (output.Session != null))
                        {
                            return output.Session.Id;
                        }
                    }
                }
                return null;
            }
        }


        internal IPrincipal ThreadPrincipal
        {
            get { return threadPrincipal; }
            set { threadPrincipal = value; }
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
                EventHandler handler = OperationCompleted;

                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(e);
            }
        }

        public T GetCallbackChannel<T>()
        {
            if (InternalServiceChannel == null || IsUserContext)
            {
                return default(T);
            }

            // yes, we might throw InvalidCastException here.  Is it really
            // better to check and throw something else instead?
            return (T)InternalServiceChannel.Proxy;
        }

        internal void ReInit(RequestContext requestContext, Message request, ServiceChannel channel)
        {
            RequestContext = requestContext;
            _request = request;
            InternalServiceChannel = channel;
        }

        internal void Recycle()
        {
            RequestContext = null;
            _request = null;
            _extensions = null;
            InstanceContext = null;
            threadPrincipal = null;
            SetClientReply(null, false);
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
            InstanceContext = instanceContext;
        }

        internal class Holder
        {
            public OperationContext Context { get; set; }
        }
    }
}

