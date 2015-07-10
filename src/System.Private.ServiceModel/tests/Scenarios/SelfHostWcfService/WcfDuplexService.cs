// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WcfService
{
    internal class WcfDuplexService : IWcfDuplexService
    {
        public static IWcfDuplexServiceCallback callback;
        public void Ping(Guid guid)
        {
            callback = OperationContext.Current.GetCallbackChannel<IWcfDuplexServiceCallback>();
            // Schedule the callback on another thread to avoid reentrancy.
            Task.Run(() => callback.OnPingCallback(guid));
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    internal class DuplexCallbackService : IDuplexChannelService
    {
        public void Ping(Guid guid)
        {
            IDuplexChannelCallback callback = OperationContext.Current.GetCallbackChannel<IDuplexChannelCallback>();
            callback.OnPingCallback(guid);
        }

    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    internal class DuplexChannelCallbackReturnService : IWcfDuplexTaskReturnService
    {
        public Task<Guid> Ping(Guid guid)
        {
            IWcfDuplexTaskReturnCallback callback = OperationContext.Current.GetCallbackChannel<IWcfDuplexTaskReturnCallback>();
            return callback.ServicePingCallback(guid);
        }
    }
}
