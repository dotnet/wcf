// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Description
{
    public interface IPolicyExportExtension
    {
        void ExportPolicy(MetadataExporter exporter, PolicyConversionContext context);
    }
}
