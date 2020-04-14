// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
