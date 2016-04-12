// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.ServiceModel;

namespace WcfService
{
    [ServiceContract(CallbackContract = typeof(IPushCallback))]
    public interface IWSDuplexService
    {
        // Request-Reply operations
        [OperationContract]
        string GetExceptionString();

        [OperationContract]
        void UploadData(string data);

        [OperationContract]
        string DownloadData();

        [OperationContract(IsOneWay = true)]
        void UploadStream(Stream stream);

        [OperationContract]
        Stream DownloadStream();

        // Duplex operations
        [OperationContract(IsOneWay = true)]
        void StartPushingData();

        [OperationContract(IsOneWay = true)]
        void StopPushingData();

        [OperationContract(IsOneWay = true)]
        void StartPushingStream();

        [OperationContract(IsOneWay = true)]
        void StartPushingStreamLongWait();

        [OperationContract(IsOneWay = true)]
        void StopPushingStream();

        // Logging
        [OperationContract(IsOneWay = true)]
        void GetLog();
    }
}
