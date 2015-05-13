// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace System.ServiceModel
{
    internal sealed class ConfigurationEndpointTrait<TChannel> : EndpointTrait<TChannel>
        where TChannel : class
    {
        private string _endpointConfigurationName;
        private EndpointAddress _remoteAddress;
        private InstanceContext _callbackInstance;

        public ConfigurationEndpointTrait(string endpointConfigurationName,
            EndpointAddress remoteAddress,
            InstanceContext callbackInstance)
        {
            _endpointConfigurationName = endpointConfigurationName;
            _remoteAddress = remoteAddress;
            _callbackInstance = callbackInstance;
        }

        public override bool Equals(object obj)
        {
            ConfigurationEndpointTrait<TChannel> trait1 = obj as ConfigurationEndpointTrait<TChannel>;
            if (trait1 == null) return false;

            if (!object.ReferenceEquals(_callbackInstance, trait1._callbackInstance))
                return false;

            if (string.CompareOrdinal(_endpointConfigurationName, trait1._endpointConfigurationName) != 0)
            {
                return false;
            }

            // EndpointAddress.Equals is used.
            if (_remoteAddress != trait1._remoteAddress)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            if (_callbackInstance != null)
            {
                hashCode ^= _callbackInstance.GetHashCode();
            }

            Fx.Assert(_endpointConfigurationName != null, "endpointConfigurationName should not be null.");
            hashCode ^= _endpointConfigurationName.GetHashCode();

            if (_remoteAddress != null)
            {
                hashCode ^= _remoteAddress.GetHashCode();
            }

            return hashCode;
        }

        public override ChannelFactory<TChannel> CreateChannelFactory()
        {
            if (_callbackInstance != null)
                return CreateDuplexFactory();

            return CreateSimplexFactory();
        }

        private DuplexChannelFactory<TChannel> CreateDuplexFactory()
        {
            if (_remoteAddress != null)
            {
                return new DuplexChannelFactory<TChannel>(_callbackInstance, _endpointConfigurationName, _remoteAddress);
            }

            return new DuplexChannelFactory<TChannel>(_callbackInstance, _endpointConfigurationName);
        }

        private ChannelFactory<TChannel> CreateSimplexFactory()
        {
            if (_remoteAddress != null)
            {
                return new ChannelFactory<TChannel>(_endpointConfigurationName, _remoteAddress);
            }

            return new ChannelFactory<TChannel>(_endpointConfigurationName);
        }
    }
}
