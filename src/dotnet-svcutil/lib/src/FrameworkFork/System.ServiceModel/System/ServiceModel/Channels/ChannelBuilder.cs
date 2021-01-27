// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;

namespace System.ServiceModel.Channels
{
    internal class ChannelBuilder
    {
        private CustomBinding _binding;
        private BindingContext _context;
        private BindingParameterCollection _bindingParameters;

        public ChannelBuilder(BindingContext context, bool addChannelDemuxerIfRequired)
        {
            _context = context;
            if (addChannelDemuxerIfRequired)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
            _binding = new CustomBinding(context.Binding, context.RemainingBindingElements);
            _bindingParameters = context.BindingParameters;
        }

        public ChannelBuilder(Binding binding, BindingParameterCollection bindingParameters, bool addChannelDemuxerIfRequired)
        {
            _binding = new CustomBinding(binding);
            _bindingParameters = bindingParameters;
            if (addChannelDemuxerIfRequired)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
        }

        public ChannelBuilder(ChannelBuilder channelBuilder)
        {
            _binding = new CustomBinding(channelBuilder.Binding);
            _bindingParameters = channelBuilder.BindingParameters;
        }

        public CustomBinding Binding
        {
            get { return _binding; }
            set { _binding = value; }
        }

        public BindingParameterCollection BindingParameters
        {
            get { return _bindingParameters; }
            set { _bindingParameters = value; }
        }


        public IChannelFactory<TChannel> BuildChannelFactory<TChannel>()
        {
            if (_context != null)
            {
                IChannelFactory<TChannel> factory = _context.BuildInnerChannelFactory<TChannel>();
                _context = null;
                return factory;
            }
            else
            {
                return _binding.BuildChannelFactory<TChannel>(_bindingParameters);
            }
        }

        public bool CanBuildChannelFactory<TChannel>()
        {
            return _binding.CanBuildChannelFactory<TChannel>(_bindingParameters);
        }
    }
}
