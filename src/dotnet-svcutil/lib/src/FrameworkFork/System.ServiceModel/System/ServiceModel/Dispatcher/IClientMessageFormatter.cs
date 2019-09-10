// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel.Channels;
using System.Collections;

namespace System.ServiceModel.Dispatcher
{
    public interface IClientMessageFormatter
    {
        Message SerializeRequest(MessageVersion messageVersion, object[] parameters);
        object DeserializeReply(Message message, object[] parameters);
    }
}
