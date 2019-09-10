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
