// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
