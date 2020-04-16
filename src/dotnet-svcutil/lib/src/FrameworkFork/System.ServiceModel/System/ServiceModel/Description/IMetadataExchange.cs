// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    [ServiceContract]
    public interface IMetadataExchange
    {
        [OperationContract(Action = MetadataStrings.WSTransfer.GetAction, ReplyAction = MetadataStrings.WSTransfer.GetResponseAction)]
        Message Get(Message request);

        [OperationContract(Action = MetadataStrings.WSTransfer.GetAction, ReplyAction = MetadataStrings.WSTransfer.GetResponseAction, AsyncPattern = true)]
        IAsyncResult BeginGet(Message request, AsyncCallback callback, object state);
        Message EndGet(IAsyncResult result);
    }
}
