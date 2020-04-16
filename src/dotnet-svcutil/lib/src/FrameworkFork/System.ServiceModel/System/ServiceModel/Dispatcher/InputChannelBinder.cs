// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Dispatcher
{
    internal class InputChannelBinder : IChannelBinder
    {
        private IInputChannel _channel;
        private Uri _listenUri;

        internal InputChannelBinder(IInputChannel channel, Uri listenUri)
        {
            if (!((channel != null)))
            {
                Fx.Assert("InputChannelBinder.InputChannelBinder: (channel != null)");
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("channel");
            }
            _channel = channel;
            _listenUri = listenUri;
        }

        public IChannel Channel
        {
            get { return _channel; }
        }

        public bool HasSession
        {
            get { return _channel is ISessionChannel<IInputSession>; }
        }

        public Uri ListenUri
        {
            get { return _listenUri; }
        }

        public EndpointAddress LocalAddress
        {
            get { return _channel.LocalAddress; }
        }

        public EndpointAddress RemoteAddress
        {
            get
            {
                throw ExceptionHelper.AsError(NotImplemented.ByDesign);
            }
        }

        public void Abort()
        {
            _channel.Abort();
        }

        public void CloseAfterFault(TimeSpan timeout)
        {
            _channel.Close(timeout);
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _channel.BeginTryReceive(timeout, callback, state);
        }

        public bool EndTryReceive(IAsyncResult result, out RequestContext requestContext)
        {
            Message message;
            if (_channel.EndTryReceive(result, out message))
            {
                requestContext = this.WrapMessage(message);
                return true;
            }
            else
            {
                requestContext = null;
                return false;
            }
        }

        public RequestContext CreateRequestContext(Message message)
        {
            return this.WrapMessage(message);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw TraceUtility.ThrowHelperError(NotImplemented.ByDesign, message);
        }

        public void EndSend(IAsyncResult result)
        {
            throw ExceptionHelper.AsError(NotImplemented.ByDesign);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            throw TraceUtility.ThrowHelperError(NotImplemented.ByDesign, message);
        }

        public bool TryReceive(TimeSpan timeout, out RequestContext requestContext)
        {
            Message message;
            if (_channel.TryReceive(timeout, out message))
            {
                requestContext = this.WrapMessage(message);
                return true;
            }
            else
            {
                requestContext = null;
                return false;
            }
        }

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw TraceUtility.ThrowHelperError(NotImplemented.ByDesign, message);
        }

        public Message EndRequest(IAsyncResult result)
        {
            throw ExceptionHelper.AsError(NotImplemented.ByDesign);
        }

        public Message Request(Message message, TimeSpan timeout)
        {
            throw TraceUtility.ThrowHelperError(NotImplemented.ByDesign, message);
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            return _channel.WaitForMessage(timeout);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _channel.BeginWaitForMessage(timeout, callback, state);
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            return _channel.EndWaitForMessage(result);
        }

        private RequestContext WrapMessage(Message message)
        {
            if (message == null)
            {
                return null;
            }
            else
            {
                return new InputRequestContext(message, this);
            }
        }

        internal class InputRequestContext : RequestContextBase
        {
            private InputChannelBinder _binder;

            internal InputRequestContext(Message request, InputChannelBinder binder)
                : base(request, TimeSpan.Zero, TimeSpan.Zero)
            {
                _binder = binder;
            }

            protected override void OnAbort()
            {
            }

            protected override void OnClose(TimeSpan timeout)
            {
            }

            protected override void OnReply(Message message, TimeSpan timeout)
            {
            }

            protected override IAsyncResult OnBeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
            {
                return new CompletedAsyncResult(callback, state);
            }

            protected override void OnEndReply(IAsyncResult result)
            {
                CompletedAsyncResult.End(result);
            }
        }
    }
}
