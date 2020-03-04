// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    internal interface IClientFaultFormatter
    {
        FaultException Deserialize(MessageFault messageFault, string action);
    }
}
