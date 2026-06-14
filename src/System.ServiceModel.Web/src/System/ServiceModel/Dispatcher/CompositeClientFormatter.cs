// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Dispatcher
{
    using System.ServiceModel.Channels;

    internal class CompositeClientFormatter : IClientMessageFormatter
    {
        IClientMessageFormatter reply;
        IClientMessageFormatter request;

        public CompositeClientFormatter(IClientMessageFormatter request, IClientMessageFormatter reply)
        {
            this.request = request;
            this.reply = reply;
        }

        public object DeserializeReply(Message message, object[] parameters)
        {
            return this.reply.DeserializeReply(message, parameters);
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            return this.request.SerializeRequest(messageVersion, parameters);
        }
    }
}
