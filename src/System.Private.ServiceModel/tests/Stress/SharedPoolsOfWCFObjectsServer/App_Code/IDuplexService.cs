// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace WcfService1
{
    [ServiceContract(CallbackContract = typeof(IDuplexCallback), SessionMode = SessionMode.Required)]
    public interface IDuplexService
    {
        [OperationContract]
        int SetData(int value, int callbackCallsToMake);


        [OperationContract]
        int GetAsyncCallbackData(int value, int asyncCallbacksToMake);
    }

    public interface IDuplexCallback
    {
        //[OperationContract(IsOneWay = true)]
        [OperationContract]
        int EchoSetData(int value);

        [OperationContract]
        Task<int> EchoGetAsyncCallbackData(int value);
    }
}
