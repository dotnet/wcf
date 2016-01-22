// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace WcfService1
{
    [ServiceContract(CallbackContract = typeof(IDuplexStreamingCallback))]
    public interface IDuplexStreamingService : IStreamingService
    {
    }

    public interface IDuplexStreamingCallback
    {
        [OperationContract]
        Stream GetStreamFromInt(int data);

        [OperationContract]
        int GetIntFromStream(Stream stream);

        [OperationContract]
        Stream EchoStream(Stream stream);
    }
}
