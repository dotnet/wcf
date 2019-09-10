//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

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