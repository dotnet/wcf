// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.ServiceModel.Description
{
    public interface IPolicyImportExtension
    {
        void ImportPolicy(MetadataImporter importer, PolicyConversionContext context);
    }
}
