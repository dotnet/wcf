// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    /// <summary>
    /// Extension point that allows projects/project flavors to modify the generated CodeDom and/or affect how WSDL 
    /// metadata is imported.
    /// </summary>
    internal interface IWcfCodeGenerationExtension
    {
        /// <summary>
        /// this property is used to track if we detected any error during the codegen process, ToolConsole will set the exit code
        /// to the correct value based on the value of this property
        /// </summary>
        bool ErrorDetected { get; set; }

        /// <summary>
        /// Extension method that allows projects/flavors to add/remove state for the WsdlImporter we will use to import
        /// metadata.
        /// 
        /// This method is called before we have created our WSDL import extension, but after the base
        /// implementation has removed the XmlSerializerMessageContractImporter/DataContractSerializerMessageContractImporter
        /// </summary>
        /// <param name="importer"></param>
        void WsdlImporting(WsdlImporter importer);

        /// <summary>
        /// Called after the WSDL has been imported.
        /// </summary>
        /// <param name="importer">
        /// The importer used to import the metadata
        /// </param>
        /// <param name="endpoints">
        /// Collection of endpoints which we imported. Adds the ability for extensions to filter
        /// endpoints.
        /// </param>
        /// <param name="bindings">
        /// Collection of standalone bindings which we imported. Adds the ability for extensions to filter
        /// standalone bindings.
        /// </param>
        /// <param name="contracts">
        /// Collection of standalone contracts which we imported. Adds the ability for extensions to filter
        /// standalone contracts.
        /// </param>
        /// <remarks>
        /// The Errors collection of the importer can be used to report errors/warnings about 
        /// endpoints being filtered.
        /// </remarks>
        void WsdlImported(WsdlImporter importer, Collection<ServiceEndpoint> endpoints, Collection<Binding> bindings, Collection<ContractDescription> contracts);

        /// <summary>
        /// Extension method called before we make the first call to the ServiceContractGenerator.
        /// Can be used to verify/modify the parameters of the service contract generator and/or 
        /// examine the ReferencedTypes, TargetCompileUnit or Configuration properties of the 
        /// ServiceContractGenerator.
        /// </summary>
        /// <param name="generator">
        /// The generator that we will use to generate service contracts
        /// </param>
        void ClientGenerating(ServiceContractGenerator generator);

        /// <summary>
        /// Extension method called after we have completed the/config generation. At this point, the ClientBase-derived proxy
        /// should be present in the generator's TargetCompileUnit and can be modified if required.
        /// </summary>
        /// <param name="generator">
        /// The generator that was used in order to generate the code/config
        /// </param>
        void ClientGenerated(ServiceContractGenerator generator);
    }
}
