// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public abstract class DuplexClientBase<TChannel> : ClientBase<TChannel>
        where TChannel : class
    {

        protected DuplexClientBase(InstanceContext callbackInstance)
        {
            throw new PlatformNotSupportedException(SRServiceModel.ConfigurationFilesNotSupported);
        }
        protected DuplexClientBase(InstanceContext callbackInstance, string endpointConfigurationName)
        {
            throw new PlatformNotSupportedException(SRServiceModel.ConfigurationFilesNotSupported);
        }
        protected DuplexClientBase(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress)
        {
            throw new PlatformNotSupportedException(SRServiceModel.ConfigurationFilesNotSupported);
        }
        protected DuplexClientBase(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress)
        {
            throw new PlatformNotSupportedException(SRServiceModel.ConfigurationFilesNotSupported);
        }
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
