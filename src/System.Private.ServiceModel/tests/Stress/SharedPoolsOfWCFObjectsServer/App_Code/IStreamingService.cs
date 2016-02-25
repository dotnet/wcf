// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfService1
{
    [ServiceContract]
    public interface IStreamingService
    {
        [OperationContract]
        Stream GetStreamFromInt(int data);

        [OperationContract]
        int GetIntFromStream(Stream stream);

        [OperationContractAttribute(Action = "http://tempuri.org/IStreamingService/EchoStream", ReplyAction = "http://tempuri.org/IStreamingService/EchoStreamResponse")]
        Stream EchoStream(Stream stream);
    }
}
