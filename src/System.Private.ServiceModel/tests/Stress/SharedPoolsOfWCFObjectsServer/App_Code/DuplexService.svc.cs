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
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DuplexService : IDuplexService
    {
        public int SetData(int value, int callbackCallsToMake)
        {
            int result = value;
            for (int i = 0; i < callbackCallsToMake; i++)
            {
                result = OperationContext.Current.GetCallbackChannel<IDuplexCallback>().EchoSetData(result + 1);
            }
            return result;
        }

        public int GetAsyncCallbackData(int value, int asyncCallbacksToMake)
        {
            int result = value;
            var tasks = new Task<int>[asyncCallbacksToMake];
            for (int i = 0; i < asyncCallbacksToMake; i++)
            {
                tasks[i] = OperationContext.Current.GetCallbackChannel<IDuplexCallback>().EchoGetAsyncCallbackData(result + i);
            }
            Task.WhenAll(tasks).Wait();
            result = tasks.Sum((t) => t.Result);
            return result;
        }
    }
}
