// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;

namespace WcfService
{
    [ServiceContract (CallbackContract = typeof(IWcfDuplexServiceCallback))]
    public interface IWcfDuplexService
    {
        [OperationContract]
        void Ping(Guid guid);
    }

    public interface IWcfDuplexServiceCallback
    {
        [OperationContract]
        void OnPingCallback(Guid guid);
    }

    [ServiceContract(CallbackContract = typeof(IDuplexChannelCallback))]
    public interface IDuplexChannelService
    {
        [OperationContract(IsOneWay = true)]
        void Ping(Guid guid);
    }

    public interface IDuplexChannelCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnPingCallback(Guid guid);
    }
}
