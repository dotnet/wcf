// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Microsoft.IdentityModel.Protocols.WsTrust;

namespace System.ServiceModel.Federation
{
    /// <summary>
    /// A <see cref="WSTrustChannelFactory" /> that creates a <see cref="WSTrustChannel" /> to send a <see cref="WsTrustRequest"/> to a STS.
    /// </summary>
    public class WSTrustChannelFactory : ChannelFactory<IWSTrustChannelContract>
    {
        /// <summary>
        /// Initializes a new instance of a <see cref="WSTrustChannelFactory" /> specifying the <see cref="ServiceEndpoint"/>.
        /// </summary>
        /// <param name="serviceEndpoint">The <see cref="ServiceEndpoint" /> used by the channels created by the factory.</param>
        public WSTrustChannelFactory(ServiceEndpoint serviceEndpoint)
            : base(serviceEndpoint)
        {
            _ = serviceEndpoint ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(serviceEndpoint));
            EndpointAddress = serviceEndpoint.Address;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="WSTrustChannelFactory" /> specifying the <see cref="Binding"/> and <see cref="EndpointAddress"/>.
        /// </summary>
        /// <param name="binding">The <see cref="Binding" /> used by channels created by the factory.</param>
        /// <param name="endpointAddress">The <see cref="EndpointAddress" /> that specifies the address of the STS.</param>
        public WSTrustChannelFactory(Binding binding, EndpointAddress endpointAddress)
            : base(binding, endpointAddress)
        {
            _ = binding ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(binding));

            EndpointAddress = endpointAddress ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(endpointAddress));
        }

        /// <summary>
        /// Creates a <see cref="IWSTrustChannelContract" /> that is used to send an Issue request to a STS.
        /// </summary>
        /// <param name="endpointAddress">The <see cref="EndpointAddress"/> that specifies the address of the STS.</param>
        /// <param name="via">The <see cref="Uri" /> that address that the channel uses to send messages.</param>
        /// <returns>A <see cref="IChannel"/> that can be used to send an Issue request to a STS.</returns>
        public override IWSTrustChannelContract CreateChannel(EndpointAddress endpointAddress, Uri via)
        {
            _ = endpointAddress ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(endpointAddress));
            _ = via ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(via));

            return new WSTrustChannel(base.CreateChannel(endpointAddress, via) as IRequestChannel);
        }

        /// <summary>
        /// Creates a <see cref="IWSTrustChannelContract" /> that is used to send an Issue request to a STS.
        /// </summary>
        /// <returns>A <see cref="IChannel"/> that can be used to send an Issue request to a STS.</returns>
        public IWSTrustChannelContract CreateTrustChannel()
        {
            return new WSTrustChannel(base.CreateChannel(EndpointAddress, EndpointAddress.Uri) as IRequestChannel);
        }

        private EndpointAddress EndpointAddress { get; }
    }
}
