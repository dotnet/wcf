// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Channels
{
    public interface IReplySessionChannel : IReplyChannel, ISessionChannel<IInputSession>
    {
    }
}
