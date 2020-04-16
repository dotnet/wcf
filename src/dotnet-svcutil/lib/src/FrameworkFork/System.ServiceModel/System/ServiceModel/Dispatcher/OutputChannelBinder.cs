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
    internal class OutputChannelBinder : IChannelBinder
    {
        private IOutputChannel _channel;

        internal OutputChannelBinder(IOutputChannel channel)
        {
            if (channel == null)
            {
                Fx.Assert("OutputChannelBinder.OutputChannelBinder: (channel != null)");
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("channel");
            }
            _channel = channel;
        }

        public IChannel Channel
        {
            get { return _channel; }
        }

        public bool HasSession
        {
            get { return _channel is ISessionChannel<IOutputSession>; }
        }

        public Uri ListenUri
        {
            get { return null; }
        }

        public EndpointAddress LocalAddress
        {
            get
            {
                throw ExceptionHelper.AsError(NotImplemented.ByDesign);
            }
        }

        public EndpointAddress RemoteAddress
        {
            get { return _channel.RemoteAddress; }
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
            throw ExceptionHelper.AsError(NotImplemented.ByDesign);
        }

        public bool EndTryReceive(IAsyncResult result, out RequestContext requestContext)
        {
            throw ExceptionHelper.AsError(NotImplemented.ByDesign);
        }

        public RequestContext CreateRequestContext(Message message)
        {
            throw ExceptionHelper.AsError(NotImplemented.ByDesign);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _channel.BeginSend(message, timeout, callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            _channel.EndSend(result);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            _channel.Send(message, timeout);
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
            throw ExceptionHelper.AsError(NotImplemented.ByDesign);
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
