// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;

namespace WcfService
{
    [ServiceContract]
    public interface IWSRequestReplyService
    {
        [OperationContract]
        void UploadData(string data);

        [OperationContract]
        string DownloadData();

        [OperationContract]
        void UploadStream(Stream stream);

        [OperationContract]
        Stream DownloadStream();

        [OperationContract]
        Stream DownloadCustomizedStream(TimeSpan readThrottle, TimeSpan streamDuration);

        [OperationContract]
        void ThrowingOperation(Exception exceptionToThrow);

        [OperationContract]
        string DelayOperation(TimeSpan delay);

        // Logging
        [OperationContract]
        List<string> GetLog();
    }
}
