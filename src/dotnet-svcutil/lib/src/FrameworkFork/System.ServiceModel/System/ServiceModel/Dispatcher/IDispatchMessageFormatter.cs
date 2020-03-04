// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    internal interface IDispatchMessageFormatter
    {
        void DeserializeRequest(Message message, object[] parameters);
        Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result);
    }
}
