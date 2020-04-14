// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Description
{
    public interface IWsdlExportExtension
    {
        void ExportContract(WsdlExporter exporter, WsdlContractConversionContext context);
        void ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context);
    }
}
