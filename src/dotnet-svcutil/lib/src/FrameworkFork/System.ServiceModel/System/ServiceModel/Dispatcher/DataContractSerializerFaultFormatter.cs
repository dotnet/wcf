// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.ServiceModel.Dispatcher
{
    internal class DataContractSerializerFaultFormatter : FaultFormatter
    {
        internal DataContractSerializerFaultFormatter(Type[] detailTypes)
            : base(detailTypes)
        {
        }

        internal DataContractSerializerFaultFormatter(SynchronizedCollection<FaultContractInfo> faultContractInfoCollection)
            : base(faultContractInfoCollection)
        {
        }
    }
}
