// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System.Collections.Generic;
    using System.ServiceModel.Description;
    using Microsoft.Xml;
    using Microsoft.Xml.Schema;
    using WsdlNS = System.Web.Services.Description;

    public class UdpTransportImporter : IPolicyImportExtension, IWsdlImportExtension
    {
        private string _udpTransportUriKey = "SoapUdpTransportImporter.udpTransportUriKey";

        public UdpTransportImporter() { }

        public void BeforeImport(WsdlNS.ServiceDescriptionCollection wsdlDocuments, XmlSchemaSet xmlSchemas, ICollection<XmlElement> policy)
        {
            foreach (WsdlNS.ServiceDescription wsdl in wsdlDocuments)
            {
                if (wsdl != null)
                {
                    foreach (WsdlNS.Binding wsdlBinding in wsdl.Bindings)
                    {
                        if (wsdlBinding != null && wsdlBinding.Extensions != null)
                        {
                            WsdlNS.SoapBinding soapBinding = (WsdlNS.SoapBinding)wsdlBinding.Extensions.Find(typeof(WsdlNS.SoapBinding));
                            if (soapBinding != null)
                            {
                                string transportUri = soapBinding.Transport;
                                if (!string.IsNullOrEmpty(transportUri) && transportUri.Equals(UdpConstants.WsdlSoapUdpTransportUri, StringComparison.Ordinal))
                                {
                                    WsdlImporter.SoapInPolicyWorkaroundHelper.InsertAdHocPolicy(wsdlBinding, soapBinding.Transport, _udpTransportUriKey);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ImportPolicy(MetadataImporter importer, PolicyConversionContext context)
        {
            XmlQualifiedName wsdlBindingQName;
            string transportUri = WsdlImporter.SoapInPolicyWorkaroundHelper.FindAdHocPolicy(context, _udpTransportUriKey, out wsdlBindingQName);

            if (transportUri != null && transportUri.Equals(UdpConstants.WsdlSoapUdpTransportUri, StringComparison.Ordinal) && !context.BindingElements.Contains(typeof(TransportBindingElement)))
            {
                UdpTransportBindingElement transport = new UdpTransportBindingElement();
                ((ITransportPolicyImport)transport).ImportPolicy(importer, context);

                StateHelper.RegisterTransportBindingElement(importer, wsdlBindingQName);
                context.BindingElements.Add(transport);
            }
        }

        public void ImportContract(WsdlImporter importer, WsdlContractConversionContext context)
        {
        }

        public void ImportEndpoint(WsdlImporter importer, WsdlEndpointConversionContext context)
        {
            if (context == null)
            {
                throw FxTrace.Exception.ArgumentNull("context");
            }

            if (context.Endpoint.Binding == null)
            {
                throw FxTrace.Exception.ArgumentNull("context.Endpoint.Binding");
            }

            BindingElementCollection bindingElements = context.Endpoint.Binding.CreateBindingElements();
            TransportBindingElement transportBindingElement = bindingElements.Find<TransportBindingElement>();
            if (transportBindingElement is UdpTransportBindingElement)
            {
                ImportEndpointAddress(context);
            }

            if (context.Endpoint.Binding is CustomBinding)
            {
                Binding newEndpointBinding = null;
                if (transportBindingElement is UdpTransportBindingElement)
                {
                    Binding udpBinding;
                    if (UdpBinding.TryCreate(bindingElements, out udpBinding))
                    {
                        newEndpointBinding = udpBinding;
                    }

                    if (newEndpointBinding != null)
                    {
                        newEndpointBinding.Name = context.Endpoint.Binding.Name;
                        newEndpointBinding.Namespace = context.Endpoint.Binding.Namespace;
                        context.Endpoint.Binding = newEndpointBinding;
                    }
                }
            }
        }

        private void ImportEndpointAddress(WsdlEndpointConversionContext context)
        {
            EndpointAddress address = null;

            if (context.WsdlPort != null)
            {
                address = context.Endpoint.Address = WsdlImporter.WSAddressingHelper.ImportAddress(context.WsdlPort);
            }

            if (address != null)
            {
                context.Endpoint.Address = address;
            }
        }
    }
}
