// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using Microsoft.Xml;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Security;
    using Microsoft.Xml.Schema;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using WsdlNS = System.Web.Services.Description;

    // implemented by Indigo Transports
    internal interface ITransportPolicyImport
    {
        void ImportPolicy(MetadataImporter importer, PolicyConversionContext policyContext);
    }

    public class TransportBindingElementImporter : IWsdlImportExtension, IPolicyImportExtension
    {
        void IWsdlImportExtension.BeforeImport(WsdlNS.ServiceDescriptionCollection wsdlDocuments, XmlSchemaSet xmlSchemas, ICollection<XmlElement> policy)
        {
            WsdlImporter.SoapInPolicyWorkaroundHelper.InsertAdHocTransportPolicy(wsdlDocuments);
        }

        void IWsdlImportExtension.ImportContract(WsdlImporter importer, WsdlContractConversionContext context) { }

        void IWsdlImportExtension.ImportEndpoint(WsdlImporter importer, WsdlEndpointConversionContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

#pragma warning disable 56506 // elliotw, these properties cannot be null in this context
            if (context.Endpoint.Binding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context.Endpoint.Binding");
            }

#pragma warning disable 56506 // brianmcn, CustomBinding.Elements never be null
            TransportBindingElement transportBindingElement = GetBindingElements(context).Find<TransportBindingElement>();

            bool transportHandledExternaly = (transportBindingElement != null) && !StateHelper.IsRegisteredTransportBindingElement(importer, context);
            if (transportHandledExternaly)
                return;

#pragma warning disable 56506 // elliotw, these properties cannot be null in this context
            WsdlNS.SoapBinding soapBinding = (WsdlNS.SoapBinding)context.WsdlBinding.Extensions.Find(typeof(WsdlNS.SoapBinding));
            if (soapBinding != null && transportBindingElement == null)
            {
                CreateLegacyTransportBindingElement(importer, soapBinding, context);
            }

            // Try to import WS-Addressing address from the port
            if (context.WsdlPort != null)
            {
                ImportAddress(context, transportBindingElement);
            }
        }

        private static BindingElementCollection GetBindingElements(WsdlEndpointConversionContext context)
        {
            Binding binding = context.Endpoint.Binding;
            BindingElementCollection elements = binding is CustomBinding ? ((CustomBinding)binding).Elements : binding.CreateBindingElements();
            return elements;
        }

        private static CustomBinding ConvertToCustomBinding(WsdlEndpointConversionContext context)
        {
            CustomBinding customBinding = context.Endpoint.Binding as CustomBinding;
            if (customBinding == null)
            {
                customBinding = new CustomBinding(context.Endpoint.Binding);
                context.Endpoint.Binding = customBinding;
            }
            return customBinding;
        }

        private static void ImportAddress(WsdlEndpointConversionContext context, TransportBindingElement transportBindingElement)
        {
            EndpointAddress address = context.Endpoint.Address = WsdlImporter.WSAddressingHelper.ImportAddress(context.WsdlPort);
            if (address != null)
            {
                context.Endpoint.Address = address;

                // Replace the http BE with https BE only if the uri scheme is https and the transport binding element is a HttpTransportBindingElement but not HttpsTransportBindingElement
                if (address.Uri.Scheme == /*TODO: Uri.UriSchemeHttps*/ "https" && transportBindingElement is HttpTransportBindingElement && !(transportBindingElement is HttpsTransportBindingElement))
                {
                    BindingElementCollection elements = ConvertToCustomBinding(context).Elements;
                    elements.Remove(transportBindingElement);
                    elements.Add(CreateHttpsFromHttp(transportBindingElement as HttpTransportBindingElement));
                }
            }
        }

        private static void CreateLegacyTransportBindingElement(WsdlImporter importer, WsdlNS.SoapBinding soapBinding, WsdlEndpointConversionContext context)
        {
            // We create a transportBindingElement based on the SoapBinding's Transport
            TransportBindingElement transportBindingElement = CreateTransportBindingElements(soapBinding.Transport, null);
            if (transportBindingElement != null)
            {
                ConvertToCustomBinding(context).Elements.Add(transportBindingElement);
                StateHelper.RegisterTransportBindingElement(importer, context);
            }
        }

        private static HttpsTransportBindingElement CreateHttpsFromHttp(HttpTransportBindingElement http)
        {
            if (http == null) return new HttpsTransportBindingElement();

            HttpsTransportBindingElement https = HttpsTransportBindingElement.CreateFromHttpBindingElement(http);

            return https;
        }

        void IPolicyImportExtension.ImportPolicy(MetadataImporter importer, PolicyConversionContext policyContext)
        {
            XmlQualifiedName wsdlBindingQName;
            string transportUri = WsdlImporter.SoapInPolicyWorkaroundHelper.FindAdHocTransportPolicy(policyContext, out wsdlBindingQName);

            if (transportUri != null && !policyContext.BindingElements.Contains(typeof(TransportBindingElement)))
            {
                TransportBindingElement transportBindingElement = CreateTransportBindingElements(transportUri, policyContext);

                if (transportBindingElement != null)
                {
                    ITransportPolicyImport transportPolicyImport = transportBindingElement as ITransportPolicyImport;
                    if (transportPolicyImport != null)
                        transportPolicyImport.ImportPolicy(importer, policyContext);

                    policyContext.BindingElements.Add(transportBindingElement);
                    StateHelper.RegisterTransportBindingElement(importer, wsdlBindingQName);
                }
            }
        }

        private static TransportBindingElement CreateTransportBindingElements(string transportUri, PolicyConversionContext policyContext)
        {
            TransportBindingElement transportBindingElement = null;
            // Try and Create TransportBindingElement
            switch (transportUri)
            {
                case TransportPolicyConstants.HttpTransportUri:
                    transportBindingElement = GetHttpTransportBindingElement(policyContext);
                    break;
                case TransportPolicyConstants.TcpTransportUri:
                    transportBindingElement = new TcpTransportBindingElement();
                    break;
                case TransportPolicyConstants.NamedPipeTransportUri:
                    transportBindingElement = new NamedPipeTransportBindingElement();
                    break;
                case TransportPolicyConstants.PeerTransportUri:
                    throw new NotImplementedException();
                case TransportPolicyConstants.WebSocketTransportUri:
                    HttpTransportBindingElement httpTransport = GetHttpTransportBindingElement(policyContext);
                    httpTransport.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
                    httpTransport.WebSocketSettings.SubProtocol = WebSocketTransportSettings.SoapSubProtocol;
                    transportBindingElement = httpTransport;
                    break;
                default:
                    // There may be another registered converter that can handle this transport.
                    break;
            }

            return transportBindingElement;
        }

        private static HttpTransportBindingElement GetHttpTransportBindingElement(PolicyConversionContext policyContext)
        {
            if (policyContext != null)
            {
                WSSecurityPolicy sp = null;
                PolicyAssertionCollection policyCollection = policyContext.GetBindingAssertions();
                if (WSSecurityPolicy.TryGetSecurityPolicyDriver(policyCollection, out sp) && sp.ContainsWsspHttpsTokenAssertion(policyCollection))
                {
                    HttpsTransportBindingElement httpsBinding = new HttpsTransportBindingElement();
                    httpsBinding.MessageSecurityVersion = sp.GetSupportedMessageSecurityVersion(SecurityVersion.WSSecurity11);
                    return httpsBinding;
                }
            }
            return new HttpTransportBindingElement();
        }
    }

    internal static class StateHelper
    {
        private readonly static object s_stateBagKey = new object();

        private static Dictionary<XmlQualifiedName, XmlQualifiedName> GetGeneratedTransportBindingElements(MetadataImporter importer)
        {
            object retValue;
            if (!importer.State.TryGetValue(StateHelper.s_stateBagKey, out retValue))
            {
                retValue = new Dictionary<XmlQualifiedName, XmlQualifiedName>();
                importer.State.Add(StateHelper.s_stateBagKey, retValue);
            }
            return (Dictionary<XmlQualifiedName, XmlQualifiedName>)retValue;
        }

        internal static void RegisterTransportBindingElement(MetadataImporter importer, XmlQualifiedName wsdlBindingQName)
        {
            GetGeneratedTransportBindingElements(importer)[wsdlBindingQName] = wsdlBindingQName;
        }

        internal static void RegisterTransportBindingElement(MetadataImporter importer, WsdlEndpointConversionContext context)
        {
            XmlQualifiedName wsdlBindingQName = new XmlQualifiedName(context.WsdlBinding.Name, context.WsdlBinding.ServiceDescription.TargetNamespace);
            GetGeneratedTransportBindingElements(importer)[wsdlBindingQName] = wsdlBindingQName;
        }

        internal static bool IsRegisteredTransportBindingElement(WsdlImporter importer, WsdlEndpointConversionContext context)
        {
            XmlQualifiedName key = new XmlQualifiedName(context.WsdlBinding.Name, context.WsdlBinding.ServiceDescription.TargetNamespace);
            return GetGeneratedTransportBindingElements(importer).ContainsKey(key);
        }
    }
}
