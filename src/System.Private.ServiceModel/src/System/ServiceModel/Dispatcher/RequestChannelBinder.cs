// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    internal class RequestChannelBinder : IChannelBinder
    {
        private IRequestChannel _channel;

        internal RequestChannelBinder(IRequestChannel channel)
        {
            if (channel == null)
            {
                Fx.Assert("RequestChannelBinder.RequestChannelBinder: (channel != null)");
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(channel));
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
            get { return EndpointAddress.AnonymousAddress; }
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
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(NotImplemented.ByDesign);
        }

        public bool EndTryReceive(IAsyncResult result, out RequestContext requestContext)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(NotImplemented.ByDesign);
        }

        public RequestContext CreateRequestContext(Message message)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(NotImplemented.ByDesign);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _channel.BeginRequest(message, timeout, callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            ValidateNullReply(_channel.EndRequest(result));
        }

        public void Send(Message message, TimeSpan timeout)
        {
            ValidateNullReply(_channel.Request(message, timeout));
        }

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _channel.BeginRequest(message, timeout, callback, state);
        }

        public Message EndRequest(IAsyncResult result)
        {
            return _channel.EndRequest(result);
        }

        public bool TryReceive(TimeSpan timeout, out RequestContext requestContext)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(NotImplemented.ByDesign);
        }

        public Message Request(Message message, TimeSpan timeout)
        {
            return _channel.Request(message, timeout);
        }

        private void ValidateNullReply(Message message)
        {
            if (message != null && !(message is NullMessage))
            {
                ProtocolException error = ProtocolException.OneWayOperationReturnedNonNull(message);
                throw System.ServiceModel.Diagnostics.TraceUtility.ThrowHelperError(error, message);
            }
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(NotImplemented.ByDesign);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(NotImplemented.ByDesign);
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(NotImplemented.ByDesign);
        }
    }
}
