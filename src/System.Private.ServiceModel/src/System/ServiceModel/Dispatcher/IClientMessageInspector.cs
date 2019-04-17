// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    public interface IClientMessageInspector
    {
        object BeforeSendRequest(ref Message request, IClientChannel channel);
        void AfterReceiveReply(ref Message reply, object correlationState);
    }
}
