// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
#else
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
#endif

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
