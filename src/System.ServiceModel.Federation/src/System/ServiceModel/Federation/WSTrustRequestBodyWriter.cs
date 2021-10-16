// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.Xml;
using Microsoft.IdentityModel.Protocols.WsTrust;

namespace System.ServiceModel.Federation
{
    /// <summary>
    /// Defines a Body Writer that writes out a RequestSecurityToken into an XmlDictionaryWriter.
    /// </summary>
    internal class WSTrustRequestBodyWriter : BodyWriter
    {
        WsTrustSerializer trustSerializer;
        WsTrustRequest _trustRequest;

        /// <summary>
        /// Constructor for the WSTrustRequestBodyWriter.
        /// </summary>
        /// <param name="trustRequest">The RequestSecurityToken object to be serialized in the outgoing Message.</param>
        /// <param name="trustSerializer">Serializer is responsible for writting the requestSecurityToken into a XmlDictionaryWritter.</param>
        public WSTrustRequestBodyWriter(WsTrustRequest trustRequest, WsTrustSerializer trustSerializer) : base(true)
        {
            _ = trustRequest ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(trustRequest));
            _ = trustSerializer ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(trustSerializer));

            _trustRequest = trustRequest;
            this.trustSerializer = trustSerializer;
        }

        /// <summary>
        /// Override of the base class method. Serializes the requestSecurityToken to the outgoing stream.
        /// </summary>
        /// <param name="writer">Writer into which the requestSecurityToken should be written.</param>
        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            trustSerializer.WriteRequest(writer, _trustRequest.WsTrustVersion, _trustRequest);
        }
    }
}
