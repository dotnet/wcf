// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace WcfService1
{
    // To verify 2 way streaming we simply route the incoming/outcoming data to the callbacks
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DuplexStreamingService : IDuplexStreamingService
    {
        public Stream EchoStream(Stream stream)
        {
            return OperationContext.Current.GetCallbackChannel<IDuplexStreamingCallback>().EchoStream(stream);
        }

        public int GetIntFromStream(Stream stream)
        {
            return OperationContext.Current.GetCallbackChannel<IDuplexStreamingCallback>().GetIntFromStream(stream);
        }

        public Stream GetStreamFromInt(int bytesToStream)
        {
            return OperationContext.Current.GetCallbackChannel<IDuplexStreamingCallback>().GetStreamFromInt(bytesToStream);
        }
    }
}
