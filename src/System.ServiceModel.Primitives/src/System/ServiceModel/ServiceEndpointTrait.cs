// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Description;

namespace System.ServiceModel
{
    internal sealed class ServiceEndpointTrait<TChannel> : EndpointTrait<TChannel> where TChannel : class
    {
        private InstanceContext _callbackInstance;
        private ServiceEndpoint _serviceEndpoint;

        public ServiceEndpointTrait(ServiceEndpoint endpoint,
            InstanceContext callbackInstance)
        {
            _serviceEndpoint = endpoint;
            _callbackInstance = callbackInstance;
        }

        public override bool Equals(object obj)
        {
            ServiceEndpointTrait<TChannel> trait1 = obj as ServiceEndpointTrait<TChannel>;
            if (trait1 == null)
                return false;

            if (!ReferenceEquals(_callbackInstance, trait1._callbackInstance))
                return false;

            if (!ReferenceEquals(_serviceEndpoint, trait1._serviceEndpoint))
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

            Fx.Assert(_serviceEndpoint != null, "endpoint should not be null.");
            hashCode ^= _serviceEndpoint.GetHashCode();

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
            Fx.Assert(_serviceEndpoint != null, "endpoint should not be null.");
            return new DuplexChannelFactory<TChannel>(_callbackInstance, _serviceEndpoint);
        }

        private ChannelFactory<TChannel> CreateSimplexFactory()
        {
            Fx.Assert(_serviceEndpoint != null, "endpoint should not be null.");
            return new ChannelFactory<TChannel>(_serviceEndpoint);
        }
    }
}
