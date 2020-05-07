// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class NoMessageHeaderFixup : MetadataFixup
    {
        public NoMessageHeaderFixup(WsdlImporter importer, Collection<ServiceEndpoint> endpoints, Collection<Binding> bindings, Collection<ContractDescription> contracts)
            : base(importer, endpoints, bindings, contracts) { }

        public override void Fixup()
        {
            if (AppSettings.EnableMessageHeader)
            {
                //No-op
                return;
            }

            foreach (ContractDescription contract in AllContracts())
            {
                foreach (OperationDescription operation in contract.Operations)
                {
                    foreach (MessageDescription message in operation.Messages)
                    {
                        if (message.Headers != null && message.Headers.Count > 0)
                        {
                            message.Headers.Clear();
                        }
                    }
                }
            }
        }
    }
}
