// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.Xml;
using Microsoft.Xml.Schema;
using WsdlNS = System.Web.Services.Description;

namespace System.ServiceModel.Description
{
    public interface IWsdlImportExtension
    {
        void BeforeImport(WsdlNS.ServiceDescriptionCollection wsdlDocuments, XmlSchemaSet xmlSchemas, ICollection<XmlElement> policy);

        void ImportContract(WsdlImporter importer, WsdlContractConversionContext context);
        void ImportEndpoint(WsdlImporter importer, WsdlEndpointConversionContext context);
    }
}
