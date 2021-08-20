// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    internal sealed class ProgrammaticEndpointTrait<TChannel> : EndpointTrait<TChannel>
        where TChannel : class
    {
        private EndpointAddress _remoteAddress;
        private Binding _binding;
        private InstanceContext _callbackInstance;

        public ProgrammaticEndpointTrait(Binding binding,
            EndpointAddress remoteAddress,
            InstanceContext callbackInstance)
            : base()
        {
            _binding = binding;
            _remoteAddress = remoteAddress;
            _callbackInstance = callbackInstance;
        }

        public override bool Equals(object obj)
        {
            ProgrammaticEndpointTrait<TChannel> trait1 = obj as ProgrammaticEndpointTrait<TChannel>;
            if (trait1 == null)
                return false;

            if (!ReferenceEquals(_callbackInstance, trait1._callbackInstance))
                return false;

            // EndpointAddress.Equals is used.
            if (_remoteAddress != trait1._remoteAddress)
                return false;

            if (!ReferenceEquals(_binding, trait1._binding))
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

            Fx.Assert(_remoteAddress != null, "remoteAddress should not be null.");
            hashCode ^= _remoteAddress.GetHashCode();


            Fx.Assert(_binding != null, "binding should not be null.");
            hashCode ^= _binding.GetHashCode();

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
            Fx.Assert(_remoteAddress != null, "remoteAddress should not be null.");
            Fx.Assert(_binding != null, "binding should not be null.");

            return new DuplexChannelFactory<TChannel>(_callbackInstance, _binding, _remoteAddress);
        }

        private ChannelFactory<TChannel> CreateSimplexFactory()
        {
            Fx.Assert(_remoteAddress != null, "remoteAddress should not be null.");
            Fx.Assert(_binding != null, "binding should not be null.");

            return new ChannelFactory<TChannel>(_binding, _remoteAddress);
        }
    }
}
