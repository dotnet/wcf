// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Channels
{
    internal class ChannelBuilder
    {
        private BindingContext _context;

        public ChannelBuilder(BindingContext context, bool addChannelDemuxerIfRequired)
        {
            _context = context;
            if (addChannelDemuxerIfRequired)
            {
                AddDemuxerBindingElement(context.RemainingBindingElements);
            }
            Binding = new CustomBinding(context.Binding, context.RemainingBindingElements);
            BindingParameters = context.BindingParameters;
        }

        public ChannelBuilder(Binding binding, BindingParameterCollection bindingParameters, bool addChannelDemuxerIfRequired)
        {
            Binding = new CustomBinding(binding);
            BindingParameters = bindingParameters;
            if (addChannelDemuxerIfRequired)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
        }

        public ChannelBuilder(ChannelBuilder channelBuilder)
        {
            Binding = new CustomBinding(channelBuilder.Binding);
            BindingParameters = channelBuilder.BindingParameters;
        }

        public CustomBinding Binding { get; set; }

        public BindingParameterCollection BindingParameters { get; set; }

        private void AddDemuxerBindingElement(BindingElementCollection elements)
        {
            if (elements.Find<ChannelDemuxerBindingElement>() == null)
            {
                // add the channel demuxer binding element right above the transport
                TransportBindingElement transport = elements.Find<TransportBindingElement>();
                if (transport == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.TransportBindingElementNotFound));
                }
                // cache the context state in the demuxer so that the same context state can be provided to the transport
                // when building auxilliary channels and listeners (for ex, for security negotiation)
                elements.Insert(elements.IndexOf(transport), new ChannelDemuxerBindingElement(true));
            }
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
                return Binding.BuildChannelFactory<TChannel>(BindingParameters);
            }
        }

        public bool CanBuildChannelFactory<TChannel>()
        {
            return Binding.CanBuildChannelFactory<TChannel>(BindingParameters);
        }
    }
}
