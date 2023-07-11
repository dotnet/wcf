// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using Microsoft.Xml;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using Microsoft.Xml.Schema;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using WsdlNS = System.Web.Services.Description;

    public class StandardBindingImporter : IWsdlImportExtension
    {
        void IWsdlImportExtension.BeforeImport(WsdlNS.ServiceDescriptionCollection wsdlDocuments, XmlSchemaSet xmlSchemas, ICollection<XmlElement> policy) { }
        void IWsdlImportExtension.ImportContract(WsdlImporter importer, WsdlContractConversionContext context) { }

        void IWsdlImportExtension.ImportEndpoint(WsdlImporter importer, WsdlEndpointConversionContext endpointContext)
        {
            if (endpointContext == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpointContext");

#pragma warning disable 56506 // elenak, endpointContext.Endpoint is never null
            if (endpointContext.Endpoint.Binding == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpointContext.Binding");

            if (endpointContext.Endpoint.Binding is CustomBinding)
            {
                BindingElementCollection elements = ((CustomBinding)endpointContext.Endpoint.Binding).Elements;

                Binding binding = null;
                TransportBindingElement transport = elements.Find<TransportBindingElement>();

                if (transport is HttpTransportBindingElement)
                {
                    if (WSHttpBindingBase.TryCreate(elements, out binding))
                    {
                        SetBinding(endpointContext.Endpoint, binding);
                    }

                    else if (BasicHttpBinding.TryCreate(elements, out binding))
                    {
                        SetBinding(endpointContext.Endpoint, binding);
                    }
                    else if (NetHttpBinding.TryCreate(elements, out binding))
                    {
                        SetBinding(endpointContext.Endpoint, binding);
                    }
                }
                else if (transport is TcpTransportBindingElement && NetTcpBinding.TryCreate(elements, out binding))
                {
                    SetBinding(endpointContext.Endpoint, binding);
                }
                else if (transport is NamedPipeTransportBindingElement && NetNamedPipeBinding.TryCreate(elements, out binding))
                {
                    SetBinding(endpointContext.Endpoint, binding);
                }
            }
        }

        private void SetBinding(ServiceEndpoint endpoint, Binding binding)
        {
            binding.Name = endpoint.Binding.Name;
            binding.Namespace = endpoint.Binding.Namespace;
            endpoint.Binding = binding;
        }
    }
}
