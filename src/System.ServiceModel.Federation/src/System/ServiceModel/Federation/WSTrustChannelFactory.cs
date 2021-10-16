// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Microsoft.IdentityModel.Protocols.WsTrust;

namespace System.ServiceModel.Federation
{
    /// <summary>
    /// A <see cref="WSTrustChannelFactory" /> that produces <see cref="WSTrustChannel" /> objects used to communicate with a WS-Trust endpoint.
    /// </summary>
    [ComVisible(false)]
    public class WSTrustChannelFactory : ChannelFactory<IWSTrustChannelContract>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WSTrustChannelFactory" /> class with a specified endpoint.
        /// configuration name.
        /// </summary>
        /// <param name="endpointConfigurationName">The configuration name used for the endpoint.</param>
        public WSTrustChannelFactory(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WSTrustChannelFactory" /> class with a specified endpoint.
        /// </summary>
        /// <param name="endpoint">The <see cref="ServiceEndpoint" />for the channels produced by the factory.</param>
        public WSTrustChannelFactory(ServiceEndpoint endpoint)
            : base(endpoint)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WSTrustChannelFactory" /> class associated with a specified
        /// name for the endpoint configuration and remote address.
        /// </summary>
        /// <param name="endpointConfigurationName">The configuration name used for the endpoint.</param>
        /// <param name="remoteAddress">The <see cref="EndpointAddress" /> that provides the location of the service.</param>
        public WSTrustChannelFactory(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WSTrustChannelFactory" /> class with a specified binding
        /// and endpoint address.
        /// </summary>
        /// <param name="binding">The <see cref="Binding" /> specified for the channels produced by the factory</param>
        /// <param name="remoteAddress">The <see cref="EndpointAddress" /> that provides the location of the service.</param>        
        public WSTrustChannelFactory(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }

        /// <summary>
        /// Gets or sets the version of WS-Trust the created channels will use for serializing messages.
        /// </summary>
        /// <remarks>
        /// Default is <see cref="WsTrustVersion.Trust13"/>
        /// </remarks>
        public WsTrustVersion TrustVersion { get; set; } = WsTrustVersion.Trust13;

        /// <summary>
        /// Creates a <see cref="WSTrustChannel" /> that is used to send messages to a service at a specific 
        /// endpoint address through a specified transport address.
        /// </summary>
        /// <param name="address">The <see cref="EndpointAddress" /> that provides the location of the service.</param>
        /// <param name="via">The <see cref="Uri" /> that contains the transport address to which the channel sends messages.</param>
        /// <returns></returns>
        public override IWSTrustChannelContract CreateChannel(EndpointAddress address, Uri via)
        {
            return CreateTrustChannel(base.CreateChannel(address, via) as IRequestChannel);
        }

        /// <summary>
        /// Creates a <see cref="WSTrustChannel" /> using parameters that reflect the configuration of
        /// this factory.
        /// </summary>
        /// <param name="requestChannel">The <see cref="IRequestChannel"/> that will use used to send and receive messages.</param>
        /// <returns></returns>
        protected virtual WSTrustChannel CreateTrustChannel(IRequestChannel requestChannel)
        {
            return new WSTrustChannel(this, requestChannel, TrustVersion);
        }
    }
}
