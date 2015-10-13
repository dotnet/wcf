// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.ServiceModel;

namespace WcfService
{
    public interface IPushCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveData(string data);

        [OperationContract(IsOneWay = true)]
        void ReceiveStream(Stream stream);

        [OperationContract(IsOneWay = true)]
        void ReceiveLog(List<string> log);

        [OperationContract(IsOneWay = true)]
        void ReceiveStreamWithException(Stream stream);
    }
}
