// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel;

namespace WcfService
{
    [ServiceContract]
    public interface IWcfReliableService
    {
        [OperationContract]
        int GetNextNumber();
        [OperationContract]
        string Echo(string echo);
    }

    [ServiceContract]
    public interface IOneWayWcfReliableService
    {
        [OperationContract(IsOneWay = true)]
        void OneWay(string text);
    }

    [ServiceContract(CallbackContract = typeof(IWcfReliableDuplexService))]
    public interface IWcfReliableDuplexService
    {
        [OperationContract]
        string DuplexEcho(string echo);
    }
}
