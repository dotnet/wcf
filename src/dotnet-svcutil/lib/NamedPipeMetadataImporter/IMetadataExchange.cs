// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    [ServiceContract]
    public interface IMetadataExchange
    {
        [OperationContract(Action = WSTransfer.GetAction, ReplyAction = WSTransfer.GetResponseAction)]
        Message Get(Message request);

        [OperationContract(Action = WSTransfer.GetAction, ReplyAction = WSTransfer.GetResponseAction, AsyncPattern = true)]
        IAsyncResult BeginGet(Message request, AsyncCallback callback, object state);
        Message EndGet(IAsyncResult result);
    }

    public static class WSTransfer
    {
        public const string Prefix = "wxf";
        public const string Name = "WS-Transfer";
        public const string Namespace = "http://schemas.xmlsoap.org/ws/2004/09/transfer";

        public const string GetAction = Namespace + "/Get";
        public const string GetResponseAction = Namespace + "/GetResponse";
    }
}
