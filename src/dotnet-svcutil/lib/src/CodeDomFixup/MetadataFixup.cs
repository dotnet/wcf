// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal abstract class MetadataFixup : IFixup
    {
        public static IFixup[] GetPreFixups(WsdlImporter importer)
        {
            return new IFixup[]
                {
                    // No PreFixups for now.
                };
        }
        public static IFixup[] GetPostFixups(WsdlImporter importer, Collection<ServiceEndpoint> endpoints, Collection<Binding> bindings, Collection<ContractDescription> contracts)
        {
            return new IFixup[]
                {
                    new EndpointSelector(importer, endpoints, bindings, contracts),
                    new NoSoapEncodingFixup(importer, endpoints, bindings, contracts),
                    new NoMessageHeaderFixup(importer, endpoints, bindings, contracts)
                };
        }

        protected readonly WsdlImporter importer;
        protected readonly Collection<ServiceEndpoint> endpoints;
        protected readonly Collection<Binding> bindings;
        protected readonly Collection<ContractDescription> contracts;

        // pre-import fixups should use this ctor; post-import fixups should use the other one
        protected MetadataFixup(WsdlImporter importer) : this(importer, null, null, null) { }
        protected MetadataFixup(WsdlImporter importer, Collection<ServiceEndpoint> endpoints, Collection<Binding> bindings, Collection<ContractDescription> contracts)
        {
            this.importer = importer;
            this.endpoints = endpoints;
            this.bindings = bindings;
            this.contracts = contracts;
        }

        protected IEnumerable<ContractDescription> AllContracts()
        {
            foreach (ServiceEndpoint endpoint in endpoints)
            {
                if (endpoint.Contract != null)
                    yield return endpoint.Contract;
            }
            foreach (ContractDescription contract in contracts)
            {
                yield return contract;
            }
        }

        public abstract void Fixup();

        public void Fixup(CommandProcessorOptions options)
        {
            // Currently MetadataFixups don't use any options, so just call the overload with no parameters.
            Fixup();
        }
    }
}
