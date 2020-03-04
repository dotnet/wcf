// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
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
