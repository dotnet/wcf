// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
#else
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
#endif

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
