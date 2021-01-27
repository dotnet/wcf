// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.ServiceModel.Description;
    using System.Web.Services.Description;
    using Microsoft.Xml;
    using Microsoft.Xml.Schema;

    public class ContextBindingElementImporter : IPolicyImportExtension, IWsdlImportExtension
    {
        public ContextBindingElementImporter()
        {
            // empty
        }

        public void BeforeImport(ServiceDescriptionCollection wsdlDocuments, XmlSchemaSet xmlSchemas, ICollection<XmlElement> policy)
        {
            // empty
        }

        public void ImportContract(WsdlImporter importer, WsdlContractConversionContext context)
        {
            // empty
        }

        public void ImportEndpoint(WsdlImporter importer, WsdlEndpointConversionContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (context.Endpoint == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context.Endpoint");
            }
            if (context.Endpoint.Binding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context.Endpoint.Binding");
            }

            // Try to post-process the unrecognized RequireHttpCookie assertion to augment the AllowCookies value 
            // of HttpTransportBindingElement

            CustomBinding customBinding = context.Endpoint.Binding as CustomBinding;
            if (customBinding != null)
            {
                UnrecognizedAssertionsBindingElement unrecognized = null;
                unrecognized = customBinding.Elements.Find<UnrecognizedAssertionsBindingElement>();
                HttpTransportBindingElement http = null;
                if (unrecognized != null)
                {
                    XmlElement httpUseCookieAssertion = null;
                    if (ContextBindingElementPolicy.TryGetHttpUseCookieAssertion(unrecognized.BindingAsserions, out httpUseCookieAssertion))
                    {
                        foreach (BindingElement element in customBinding.Elements)
                        {
                            http = element as HttpTransportBindingElement;
                            if (http != null)
                            {
                                http.AllowCookies = true;
                                unrecognized.BindingAsserions.Remove(httpUseCookieAssertion);
                                if (unrecognized.BindingAsserions.Count == 0)
                                {
                                    customBinding.Elements.Remove(unrecognized);
                                }
                                break;
                            }
                        }
                    }
                }

                // Try to upgrade to standard binding

                BindingElementCollection bindingElements = customBinding.CreateBindingElements();
                Binding binding;
                if (!WSHttpContextBinding.TryCreate(bindingElements, out binding)
                    && !NetTcpContextBinding.TryCreate(bindingElements, out binding))
                {
                    // Work around BasicHttpBinding.TryCreate insensitivity to HttpTransportBindingElement.AllowCookies value

                    if (http == null)
                    {
                        foreach (BindingElement bindingElement in bindingElements)
                        {
                            http = bindingElement as HttpTransportBindingElement;
                            if (http != null)
                            {
                                break;
                            }
                        }
                    }
                    if (http != null && http.AllowCookies)
                    {
                        http.AllowCookies = false;
                        if (BasicHttpBinding.TryCreate(bindingElements, out binding))
                        {
                            ((BasicHttpBinding)binding).AllowCookies = true;
                        }
                    }
                }

                if (binding != null)
                {
                    binding.Name = context.Endpoint.Binding.Name;
                    binding.Namespace = context.Endpoint.Binding.Namespace;
                    context.Endpoint.Binding = binding;
                }
            }
        }

        public virtual void ImportPolicy(MetadataImporter importer, PolicyConversionContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (context.BindingElements == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.PolicyImportContextBindingElementCollectionIsNull));
            }

            ContextBindingElement contextBindingElement;
            XmlElement httpUseCookieAssertion = null;
            if (ContextBindingElementPolicy.TryImportRequireContextAssertion(context.GetBindingAssertions(), out contextBindingElement))
            {
                context.BindingElements.Insert(0, contextBindingElement);
            }
            else if (ContextBindingElementPolicy.TryGetHttpUseCookieAssertion(context.GetBindingAssertions(), out httpUseCookieAssertion))
            {
                foreach (BindingElement bindingElement in context.BindingElements)
                {
                    HttpTransportBindingElement http = bindingElement as HttpTransportBindingElement;
                    if (http != null)
                    {
                        http.AllowCookies = true;
                        context.GetBindingAssertions().Remove(httpUseCookieAssertion);
                        break;
                    }
                }
            }
        }
    }
}
