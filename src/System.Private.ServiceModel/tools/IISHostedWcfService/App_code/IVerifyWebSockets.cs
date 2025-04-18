// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
#else
using System.ServiceModel;
using System.ServiceModel.Channels;
#endif

namespace WcfService
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IVerifyWebSockets
    {
        // This operation will only get called when using WebSockets and CreateNotificationOnConnection is set to true on the binding WebSocketSettings
        [OperationContract(Action = WebSocketTransportSettings.ConnectionOpenedAction, IsOneWay = true, IsInitiating = true)]
        void ForceWebSocketsUse();

        [OperationContract()]
        bool ValidateWebSocketsUsed();
    }
}
