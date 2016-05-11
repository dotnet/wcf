// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WcfService
{
    internal class WcfWebSocketService : IWcfDuplexService
    {
        public static IWcfDuplexServiceCallback callback;
        public void Ping(Guid guid)
        {
            callback = OperationContext.Current.GetCallbackChannel<IWcfDuplexServiceCallback>();
            // Schedule the callback on another thread to avoid reentrancy.
            Task.Run(() => callback.OnPingCallback(guid));
        }
    }

    internal class WcfWebSocketTransportUsageAlwaysService : IWcfDuplexService
    {
        public static IWcfDuplexServiceCallback callback;
        public void Ping(Guid guid)
        {
            callback = OperationContext.Current.GetCallbackChannel<IWcfDuplexServiceCallback>();
            // Schedule the callback on another thread to avoid reentrancy.
            Task.Run(() => callback.OnPingCallback(guid));
        }
    }
}
