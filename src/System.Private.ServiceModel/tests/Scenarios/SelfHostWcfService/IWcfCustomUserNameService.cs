// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;

namespace WcfService
{
    [ServiceContract]
    interface IWcfCustomUserNameService
    {
        [OperationContract(Action = "http://tempuri.org/IWcfCustomUserNameService/Echo")]
        String Echo(String message);
    }
}
