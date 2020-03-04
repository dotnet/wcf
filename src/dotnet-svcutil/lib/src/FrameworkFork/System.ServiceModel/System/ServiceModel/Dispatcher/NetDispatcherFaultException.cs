// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Dispatcher
{
    internal class NetDispatcherFaultException : FaultException
    {
        public NetDispatcherFaultException(string reason, FaultCode code, Exception innerException)
            : base(reason, code, FaultCodeConstants.Actions.NetDispatcher, innerException)
        {
        }
        public NetDispatcherFaultException(FaultReason reason, FaultCode code, Exception innerException)
            : base(reason, code, FaultCodeConstants.Actions.NetDispatcher, innerException)
        {
        }
    }
}
