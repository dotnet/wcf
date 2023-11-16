// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public abstract class DuplexClientBase<TChannel> : ClientBase<TChannel>
        where TChannel : class
    {
        protected DuplexClientBase(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress)
            : base(callbackInstance, binding, remoteAddress)
        {
        }

        public IDuplexContextChannel InnerDuplexChannel
        {
            get
            {
                return (IDuplexContextChannel)InnerChannel;
            }
        }
    }
}
