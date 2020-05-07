// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class NoSoapEncodingFixup : MetadataFixup
    {
        public NoSoapEncodingFixup(WsdlImporter importer, Collection<ServiceEndpoint> endpoints, Collection<Binding> bindings, Collection<ContractDescription> contracts)
            : base(importer, endpoints, bindings, contracts) { }

        public override void Fixup()
        {
            if (AppSettings.EnableSoapEncoding)
            {
                //No-op
                return;
            }

            foreach (ContractDescription contract in AllContracts())
            {
                if (new SoapEncodingOperationFilter(contract).Filter())
                {
                    MetadataConversionError warning = new MetadataConversionError(string.Format(CultureInfo.InvariantCulture, SR.ErrIncompatibleContractSoapEncodingFormat, contract.Name), isWarning: true);
                    if (!importer.Errors.Contains(warning))
                    {
                        importer.Errors.Add(warning);
                    }
                }
            }
        }

        private class SoapEncodingOperationFilter
        {
            private ContractDescription _contract;
            private bool _filteredAny = false;
            internal SoapEncodingOperationFilter(ContractDescription contract)
            {
                _contract = contract;
            }

            internal bool Filter()
            {
                CollectionHelpers.MapList<OperationDescription>(_contract.Operations, IsNotSoapEncoding, OnFiltered);
                return _filteredAny;
            }

            private static bool IsNotSoapEncoding(OperationDescription op)
            {
                XmlSerializerOperationBehavior behavior = op.Behaviors.Find<XmlSerializerOperationBehavior>();
                return behavior == null || behavior.XmlSerializerFormatAttribute.Use != OperationFormatUse.Encoded;
            }

            private void OnFiltered(OperationDescription op, int index)
            {
                _filteredAny = true;
            }
        }
    }
}
