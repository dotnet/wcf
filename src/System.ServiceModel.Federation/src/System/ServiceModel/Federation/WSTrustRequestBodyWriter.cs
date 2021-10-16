// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.Xml;
using Microsoft.IdentityModel.Protocols.WsTrust;

namespace System.ServiceModel.Federation
{
    /// <summary>
    /// Defines a <see cref="BodyWriter"/> that writes a <see cref="WsTrustRequest"/> into a <see cref="XmlDictionaryWriter"/>.
    /// </summary>
    internal class WSTrustRequestBodyWriter : BodyWriter
    {
        /// <summary>
        /// Constructor for the WSTrustRequestBodyWriter.
        /// </summary>
        /// <param name="trustRequest">The <see cref="WsTrustRequest"/> to be serialized in a outgoing Message.</param>
        /// <param name="trustSerializer">The <see cref="WsTrustSerializer"/> used to write the <see cref="WsTrustRequest"/> into a <see cref="XmlDictionaryWriter"/>.</param>
        public WSTrustRequestBodyWriter(WsTrustRequest trustRequest, WsTrustSerializer trustSerializer) : base(true)
        {
            TrustRequest = trustRequest ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(trustRequest));
            TrustSerializer = trustSerializer ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(trustSerializer));
        }

        /// <summary>
        /// Override of the base class method. Serializes the <see cref="WsTrustRequest"/> into the <see cref="XmlDictionaryWriter"/>.
        /// </summary>
        /// <param name="writer"> The <see cref="XmlDictionaryWriter"/> to serialize the <see cref="WsTrustRequest"/> into.</param>
        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            TrustSerializer.WriteRequest(writer, TrustRequest.WsTrustVersion, TrustRequest);
        }

        protected WsTrustRequest TrustRequest { get; }

        protected WsTrustSerializer TrustSerializer { get; }
    }
}
