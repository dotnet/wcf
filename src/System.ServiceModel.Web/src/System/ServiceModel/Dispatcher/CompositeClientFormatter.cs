// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    internal class CompositeClientFormatter : IClientMessageFormatter
    {
        private readonly IClientMessageFormatter _reply;
        private readonly IClientMessageFormatter _request;

        public CompositeClientFormatter(IClientMessageFormatter request, IClientMessageFormatter reply)
        {
            _request = request;
            _reply = reply;
        }

        public object DeserializeReply(Message message, object[] parameters)
        {
            return _reply.DeserializeReply(message, parameters);
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            return _request.SerializeRequest(messageVersion, parameters);
        }
    }
}
