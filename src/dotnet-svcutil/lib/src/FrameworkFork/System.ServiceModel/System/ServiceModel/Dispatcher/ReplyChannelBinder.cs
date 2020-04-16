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
    internal class ReplyChannelBinder : IChannelBinder
    {
        private IReplyChannel _channel;
        private Uri _listenUri;

        internal ReplyChannelBinder(IReplyChannel channel, Uri listenUri)
        {
            if (channel == null)
            {
                Fx.Assert("ReplyChannelBinder.ReplyChannelBinder: (channel != null)");
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(NotImplemented.ByDesign);
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
            return _channel.BeginTryReceiveRequest(timeout, callback, state);
        }

        public bool EndTryReceive(IAsyncResult result, out RequestContext requestContext)
        {
            return _channel.EndTryReceiveRequest(result, out requestContext);
        }

        public RequestContext CreateRequestContext(Message message)
        {
            throw TraceUtility.ThrowHelperError(NotImplemented.ByDesign, message);
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

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw TraceUtility.ThrowHelperError(NotImplemented.ByDesign, message);
        }

        public Message EndRequest(IAsyncResult result)
        {
            throw ExceptionHelper.AsError(NotImplemented.ByDesign);
        }

        public bool TryReceive(TimeSpan timeout, out RequestContext requestContext)
        {
            return _channel.TryReceiveRequest(timeout, out requestContext);
        }

        public Message Request(Message message, TimeSpan timeout)
        {
            throw TraceUtility.ThrowHelperError(NotImplemented.ByDesign, message);
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            throw ExceptionHelper.AsError(NotImplemented.ByDesign);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw ExceptionHelper.AsError(NotImplemented.ByDesign);
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            throw ExceptionHelper.AsError(NotImplemented.ByDesign);
        }
    }
}
