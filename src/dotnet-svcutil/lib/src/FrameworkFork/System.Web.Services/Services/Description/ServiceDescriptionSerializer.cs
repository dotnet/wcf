// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Web.Services.Description
{
    internal class ServiceDescriptionSerializationWriter : Microsoft.Xml.Serialization.XmlSerializationWriter
    {
        public void Write125_definitions(object o)
        {
            WriteStartDocument();
            if (o == null)
            {
                WriteNullTagLiteral(@"definitions", @"http://schemas.xmlsoap.org/wsdl/");
                return;
            }
            TopLevelElement();
            Write124_ServiceDescription(@"definitions", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.Web.Services.Description.ServiceDescription)o), true, false);
        }

        private void Write124_ServiceDescription(string n, string ns, global::System.Web.Services.Description.ServiceDescription o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.ServiceDescription))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"ServiceDescription", @"http://schemas.xmlsoap.org/wsdl/");
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@ExtensibleAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            WriteAttribute(@"targetNamespace", @"", ((global::System.String)o.@TargetNamespace));
            if ((o.@DocumentationElement) is Microsoft.Xml.XmlNode || o.@DocumentationElement == null)
            {
                WriteElementLiteral((Microsoft.Xml.XmlNode)o.@DocumentationElement, @"documentation", @"http://schemas.xmlsoap.org/wsdl/", false, true);
            }
            else
            {
                throw CreateInvalidAnyTypeException(o.@DocumentationElement);
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        if ((a[ia]) is Microsoft.Xml.XmlNode || a[ia] == null)
                        {
                            WriteElementLiteral((Microsoft.Xml.XmlNode)a[ia], @"", null, false, true);
                        }
                        else
                        {
                            throw CreateInvalidAnyTypeException(a[ia]);
                        }
                    }
                }
            }
            {
                global::System.Web.Services.Description.ImportCollection a = (global::System.Web.Services.Description.ImportCollection)o.@Imports;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write4_Import(@"import", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.Web.Services.Description.Import)a[ia]), false, false);
                    }
                }
            }
            Write67_Types(@"types", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.Web.Services.Description.Types)o.@Types), false, false);
            {
                global::System.Web.Services.Description.MessageCollection a = (global::System.Web.Services.Description.MessageCollection)o.@Messages;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write69_Message(@"message", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.Web.Services.Description.Message)a[ia]), false, false);
                    }
                }
            }
            {
                global::System.Web.Services.Description.PortTypeCollection a = (global::System.Web.Services.Description.PortTypeCollection)o.@PortTypes;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write75_PortType(@"portType", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.Web.Services.Description.PortType)a[ia]), false, false);
                    }
                }
            }
            {
                global::System.Web.Services.Description.BindingCollection a = (global::System.Web.Services.Description.BindingCollection)o.@Bindings;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write117_Binding(@"binding", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.Web.Services.Description.Binding)a[ia]), false, false);
                    }
                }
            }
            {
                global::System.Web.Services.Description.ServiceCollection a = (global::System.Web.Services.Description.ServiceCollection)o.@Services;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write123_Service(@"service", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.Web.Services.Description.Service)a[ia]), false, false);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write123_Service(string n, string ns, global::System.Web.Services.Description.Service o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.Service))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"Service", @"http://schemas.xmlsoap.org/wsdl/");
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@ExtensibleAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            if ((o.@DocumentationElement) is Microsoft.Xml.XmlNode || o.@DocumentationElement == null)
            {
                WriteElementLiteral((Microsoft.Xml.XmlNode)o.@DocumentationElement, @"documentation", @"http://schemas.xmlsoap.org/wsdl/", false, true);
            }
            else
            {
                throw CreateInvalidAnyTypeException(o.@DocumentationElement);
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        if ((a[ia]) is Microsoft.Xml.XmlNode || a[ia] == null)
                        {
                            WriteElementLiteral((Microsoft.Xml.XmlNode)a[ia], @"", null, false, true);
                        }
                        else
                        {
                            throw CreateInvalidAnyTypeException(a[ia]);
                        }
                    }
                }
            }
            {
                global::System.Web.Services.Description.PortCollection a = (global::System.Web.Services.Description.PortCollection)o.@Ports;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write122_Port(@"port", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.Web.Services.Description.Port)a[ia]), false, false);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write122_Port(string n, string ns, global::System.Web.Services.Description.Port o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.Port))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"Port", @"http://schemas.xmlsoap.org/wsdl/");
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@ExtensibleAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            WriteAttribute(@"binding", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@Binding)));
            if ((o.@DocumentationElement) is Microsoft.Xml.XmlNode || o.@DocumentationElement == null)
            {
                WriteElementLiteral((Microsoft.Xml.XmlNode)o.@DocumentationElement, @"documentation", @"http://schemas.xmlsoap.org/wsdl/", false, true);
            }
            else
            {
                throw CreateInvalidAnyTypeException(o.@DocumentationElement);
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        global::System.Object ai = (global::System.Object)a[ia];
                        {
                            if (ai is global::System.Web.Services.Description.Soap12AddressBinding)
                            {
                                Write121_Soap12AddressBinding(@"address", @"http://schemas.xmlsoap.org/wsdl/soap12/", ((global::System.Web.Services.Description.Soap12AddressBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.HttpAddressBinding)
                            {
                                Write118_HttpAddressBinding(@"address", @"http://schemas.xmlsoap.org/wsdl/http/", ((global::System.Web.Services.Description.HttpAddressBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.SoapAddressBinding)
                            {
                                Write119_SoapAddressBinding(@"address", @"http://schemas.xmlsoap.org/wsdl/soap/", ((global::System.Web.Services.Description.SoapAddressBinding)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.XmlElement)
                            {
                                Microsoft.Xml.XmlElement elem = (Microsoft.Xml.XmlElement)ai;
                                if ((elem) is Microsoft.Xml.XmlNode || elem == null)
                                {
                                    WriteElementLiteral((Microsoft.Xml.XmlNode)elem, @"", null, false, true);
                                }
                                else
                                {
                                    throw CreateInvalidAnyTypeException(elem);
                                }
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write119_SoapAddressBinding(string n, string ns, global::System.Web.Services.Description.SoapAddressBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.SoapAddressBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"SoapAddressBinding", @"http://schemas.xmlsoap.org/wsdl/soap/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            WriteAttribute(@"location", @"", ((global::System.String)o.@Location));
            WriteEndElement(o);
        }

        private void Write118_HttpAddressBinding(string n, string ns, global::System.Web.Services.Description.HttpAddressBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.HttpAddressBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"HttpAddressBinding", @"http://schemas.xmlsoap.org/wsdl/http/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            WriteAttribute(@"location", @"", ((global::System.String)o.@Location));
            WriteEndElement(o);
        }

        private void Write121_Soap12AddressBinding(string n, string ns, global::System.Web.Services.Description.Soap12AddressBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.Soap12AddressBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Soap12AddressBinding", @"http://schemas.xmlsoap.org/wsdl/soap12/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            WriteAttribute(@"location", @"", ((global::System.String)o.@Location));
            WriteEndElement(o);
        }

        private void Write117_Binding(string n, string ns, global::System.Web.Services.Description.Binding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.Binding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"Binding", @"http://schemas.xmlsoap.org/wsdl/");
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@ExtensibleAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            WriteAttribute(@"type", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@Type)));
            if ((o.@DocumentationElement) is Microsoft.Xml.XmlNode || o.@DocumentationElement == null)
            {
                WriteElementLiteral((Microsoft.Xml.XmlNode)o.@DocumentationElement, @"documentation", @"http://schemas.xmlsoap.org/wsdl/", false, true);
            }
            else
            {
                throw CreateInvalidAnyTypeException(o.@DocumentationElement);
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        global::System.Object ai = (global::System.Object)a[ia];
                        {
                            if (ai is global::System.Web.Services.Description.Soap12Binding)
                            {
                                Write84_Soap12Binding(@"binding", @"http://schemas.xmlsoap.org/wsdl/soap12/", ((global::System.Web.Services.Description.Soap12Binding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.HttpBinding)
                            {
                                Write77_HttpBinding(@"binding", @"http://schemas.xmlsoap.org/wsdl/http/", ((global::System.Web.Services.Description.HttpBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.SoapBinding)
                            {
                                Write80_SoapBinding(@"binding", @"http://schemas.xmlsoap.org/wsdl/soap/", ((global::System.Web.Services.Description.SoapBinding)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.XmlElement)
                            {
                                Microsoft.Xml.XmlElement elem = (Microsoft.Xml.XmlElement)ai;
                                if ((elem) is Microsoft.Xml.XmlNode || elem == null)
                                {
                                    WriteElementLiteral((Microsoft.Xml.XmlNode)elem, @"", null, false, true);
                                }
                                else
                                {
                                    throw CreateInvalidAnyTypeException(elem);
                                }
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            {
                global::System.Web.Services.Description.OperationBindingCollection a = (global::System.Web.Services.Description.OperationBindingCollection)o.@Operations;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write116_OperationBinding(@"operation", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.Web.Services.Description.OperationBinding)a[ia]), false, false);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write116_OperationBinding(string n, string ns, global::System.Web.Services.Description.OperationBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.OperationBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"OperationBinding", @"http://schemas.xmlsoap.org/wsdl/");
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@ExtensibleAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            if ((o.@DocumentationElement) is Microsoft.Xml.XmlNode || o.@DocumentationElement == null)
            {
                WriteElementLiteral((Microsoft.Xml.XmlNode)o.@DocumentationElement, @"documentation", @"http://schemas.xmlsoap.org/wsdl/", false, true);
            }
            else
            {
                throw CreateInvalidAnyTypeException(o.@DocumentationElement);
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        global::System.Object ai = (global::System.Object)a[ia];
                        {
                            if (ai is global::System.Web.Services.Description.Soap12OperationBinding)
                            {
                                Write88_Soap12OperationBinding(@"operation", @"http://schemas.xmlsoap.org/wsdl/soap12/", ((global::System.Web.Services.Description.Soap12OperationBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.HttpOperationBinding)
                            {
                                Write85_HttpOperationBinding(@"operation", @"http://schemas.xmlsoap.org/wsdl/http/", ((global::System.Web.Services.Description.HttpOperationBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.SoapOperationBinding)
                            {
                                Write86_SoapOperationBinding(@"operation", @"http://schemas.xmlsoap.org/wsdl/soap/", ((global::System.Web.Services.Description.SoapOperationBinding)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.XmlElement)
                            {
                                Microsoft.Xml.XmlElement elem = (Microsoft.Xml.XmlElement)ai;
                                if ((elem) is Microsoft.Xml.XmlNode || elem == null)
                                {
                                    WriteElementLiteral((Microsoft.Xml.XmlNode)elem, @"", null, false, true);
                                }
                                else
                                {
                                    throw CreateInvalidAnyTypeException(elem);
                                }
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            Write110_InputBinding(@"input", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.Web.Services.Description.InputBinding)o.@Input), false, false);
            Write111_OutputBinding(@"output", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.Web.Services.Description.OutputBinding)o.@Output), false, false);
            {
                global::System.Web.Services.Description.FaultBindingCollection a = (global::System.Web.Services.Description.FaultBindingCollection)o.@Faults;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write115_FaultBinding(@"fault", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.Web.Services.Description.FaultBinding)a[ia]), false, false);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write115_FaultBinding(string n, string ns, global::System.Web.Services.Description.FaultBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.FaultBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"FaultBinding", @"http://schemas.xmlsoap.org/wsdl/");
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@ExtensibleAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            if ((o.@DocumentationElement) is Microsoft.Xml.XmlNode || o.@DocumentationElement == null)
            {
                WriteElementLiteral((Microsoft.Xml.XmlNode)o.@DocumentationElement, @"documentation", @"http://schemas.xmlsoap.org/wsdl/", false, true);
            }
            else
            {
                throw CreateInvalidAnyTypeException(o.@DocumentationElement);
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        global::System.Object ai = (global::System.Object)a[ia];
                        {
                            if (ai is global::System.Web.Services.Description.Soap12FaultBinding)
                            {
                                Write114_Soap12FaultBinding(@"fault", @"http://schemas.xmlsoap.org/wsdl/soap12/", ((global::System.Web.Services.Description.Soap12FaultBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.SoapFaultBinding)
                            {
                                Write112_SoapFaultBinding(@"fault", @"http://schemas.xmlsoap.org/wsdl/soap/", ((global::System.Web.Services.Description.SoapFaultBinding)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.XmlElement)
                            {
                                Microsoft.Xml.XmlElement elem = (Microsoft.Xml.XmlElement)ai;
                                if ((elem) is Microsoft.Xml.XmlNode || elem == null)
                                {
                                    WriteElementLiteral((Microsoft.Xml.XmlNode)elem, @"", null, false, true);
                                }
                                else
                                {
                                    throw CreateInvalidAnyTypeException(elem);
                                }
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write112_SoapFaultBinding(string n, string ns, global::System.Web.Services.Description.SoapFaultBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.SoapFaultBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"SoapFaultBinding", @"http://schemas.xmlsoap.org/wsdl/soap/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            if (((global::System.Web.Services.Description.SoapBindingUse)o.@Use) != global::System.Web.Services.Description.SoapBindingUse.@Default)
            {
                WriteAttribute(@"use", @"", Write98_SoapBindingUse(((global::System.Web.Services.Description.SoapBindingUse)o.@Use)));
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            WriteAttribute(@"namespace", @"", ((global::System.String)o.@Namespace));
            if ((((global::System.String)o.@Encoding) != null) && (((global::System.String)o.@Encoding).Length != 0))
            {
                WriteAttribute(@"encodingStyle", @"", ((global::System.String)o.@Encoding));
            }
            WriteEndElement(o);
        }

        private string Write98_SoapBindingUse(global::System.Web.Services.Description.SoapBindingUse v)
        {
            string s = null;
            switch (v)
            {
                case global::System.Web.Services.Description.SoapBindingUse.@Encoded: s = @"encoded"; break;
                case global::System.Web.Services.Description.SoapBindingUse.@Literal: s = @"literal"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"System.Web.Services.Description.SoapBindingUse");
            }
            return s;
        }

        private void Write114_Soap12FaultBinding(string n, string ns, global::System.Web.Services.Description.Soap12FaultBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.Soap12FaultBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Soap12FaultBinding", @"http://schemas.xmlsoap.org/wsdl/soap12/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            if (((global::System.Web.Services.Description.SoapBindingUse)o.@Use) != global::System.Web.Services.Description.SoapBindingUse.@Default)
            {
                WriteAttribute(@"use", @"", Write100_SoapBindingUse(((global::System.Web.Services.Description.SoapBindingUse)o.@Use)));
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            WriteAttribute(@"namespace", @"", ((global::System.String)o.@Namespace));
            if ((((global::System.String)o.@Encoding) != null) && (((global::System.String)o.@Encoding).Length != 0))
            {
                WriteAttribute(@"encodingStyle", @"", ((global::System.String)o.@Encoding));
            }
            WriteEndElement(o);
        }

        private string Write100_SoapBindingUse(global::System.Web.Services.Description.SoapBindingUse v)
        {
            string s = null;
            switch (v)
            {
                case global::System.Web.Services.Description.SoapBindingUse.@Encoded: s = @"encoded"; break;
                case global::System.Web.Services.Description.SoapBindingUse.@Literal: s = @"literal"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"System.Web.Services.Description.SoapBindingUse");
            }
            return s;
        }

        private void Write111_OutputBinding(string n, string ns, global::System.Web.Services.Description.OutputBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.OutputBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"OutputBinding", @"http://schemas.xmlsoap.org/wsdl/");
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@ExtensibleAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            if ((o.@DocumentationElement) is Microsoft.Xml.XmlNode || o.@DocumentationElement == null)
            {
                WriteElementLiteral((Microsoft.Xml.XmlNode)o.@DocumentationElement, @"documentation", @"http://schemas.xmlsoap.org/wsdl/", false, true);
            }
            else
            {
                throw CreateInvalidAnyTypeException(o.@DocumentationElement);
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        global::System.Object ai = (global::System.Object)a[ia];
                        {
                            if (ai is global::System.Web.Services.Description.Soap12BodyBinding)
                            {
                                Write102_Soap12BodyBinding(@"body", @"http://schemas.xmlsoap.org/wsdl/soap12/", ((global::System.Web.Services.Description.Soap12BodyBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.Soap12HeaderBinding)
                            {
                                Write109_Soap12HeaderBinding(@"header", @"http://schemas.xmlsoap.org/wsdl/soap12/", ((global::System.Web.Services.Description.Soap12HeaderBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.SoapHeaderBinding)
                            {
                                Write106_SoapHeaderBinding(@"header", @"http://schemas.xmlsoap.org/wsdl/soap/", ((global::System.Web.Services.Description.SoapHeaderBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.SoapBodyBinding)
                            {
                                Write99_SoapBodyBinding(@"body", @"http://schemas.xmlsoap.org/wsdl/soap/", ((global::System.Web.Services.Description.SoapBodyBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.MimeXmlBinding)
                            {
                                Write94_MimeXmlBinding(@"mimeXml", @"http://schemas.xmlsoap.org/wsdl/mime/", ((global::System.Web.Services.Description.MimeXmlBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.MimeContentBinding)
                            {
                                Write93_MimeContentBinding(@"content", @"http://schemas.xmlsoap.org/wsdl/mime/", ((global::System.Web.Services.Description.MimeContentBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.MimeTextBinding)
                            {
                                Write97_MimeTextBinding(@"text", @"http://microsoft.com/wsdl/mime/textMatching/", ((global::System.Web.Services.Description.MimeTextBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.MimeMultipartRelatedBinding)
                            {
                                Write104_MimeMultipartRelatedBinding(@"multipartRelated", @"http://schemas.xmlsoap.org/wsdl/mime/", ((global::System.Web.Services.Description.MimeMultipartRelatedBinding)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.XmlElement)
                            {
                                Microsoft.Xml.XmlElement elem = (Microsoft.Xml.XmlElement)ai;
                                if ((elem) is Microsoft.Xml.XmlNode || elem == null)
                                {
                                    WriteElementLiteral((Microsoft.Xml.XmlNode)elem, @"", null, false, true);
                                }
                                else
                                {
                                    throw CreateInvalidAnyTypeException(elem);
                                }
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write104_MimeMultipartRelatedBinding(string n, string ns, global::System.Web.Services.Description.MimeMultipartRelatedBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.MimeMultipartRelatedBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"MimeMultipartRelatedBinding", @"http://schemas.xmlsoap.org/wsdl/mime/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            {
                global::System.Web.Services.Description.MimePartCollection a = (global::System.Web.Services.Description.MimePartCollection)o.@Parts;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write103_MimePart(@"part", @"http://schemas.xmlsoap.org/wsdl/mime/", ((global::System.Web.Services.Description.MimePart)a[ia]), false, false);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write103_MimePart(string n, string ns, global::System.Web.Services.Description.MimePart o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.MimePart))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"MimePart", @"http://schemas.xmlsoap.org/wsdl/mime/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        global::System.Object ai = (global::System.Object)a[ia];
                        {
                            if (ai is global::System.Web.Services.Description.Soap12BodyBinding)
                            {
                                Write102_Soap12BodyBinding(@"body", @"http://schemas.xmlsoap.org/wsdl/soap12/", ((global::System.Web.Services.Description.Soap12BodyBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.SoapBodyBinding)
                            {
                                Write99_SoapBodyBinding(@"body", @"http://schemas.xmlsoap.org/wsdl/soap/", ((global::System.Web.Services.Description.SoapBodyBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.MimeContentBinding)
                            {
                                Write93_MimeContentBinding(@"content", @"http://schemas.xmlsoap.org/wsdl/mime/", ((global::System.Web.Services.Description.MimeContentBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.MimeXmlBinding)
                            {
                                Write94_MimeXmlBinding(@"mimeXml", @"http://schemas.xmlsoap.org/wsdl/mime/", ((global::System.Web.Services.Description.MimeXmlBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.MimeTextBinding)
                            {
                                Write97_MimeTextBinding(@"text", @"http://microsoft.com/wsdl/mime/textMatching/", ((global::System.Web.Services.Description.MimeTextBinding)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.XmlElement)
                            {
                                Microsoft.Xml.XmlElement elem = (Microsoft.Xml.XmlElement)ai;
                                if ((elem) is Microsoft.Xml.XmlNode || elem == null)
                                {
                                    WriteElementLiteral((Microsoft.Xml.XmlNode)elem, @"", null, false, true);
                                }
                                else
                                {
                                    throw CreateInvalidAnyTypeException(elem);
                                }
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write97_MimeTextBinding(string n, string ns, global::System.Web.Services.Description.MimeTextBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.MimeTextBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"MimeTextBinding", @"http://microsoft.com/wsdl/mime/textMatching/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            {
                global::System.Web.Services.Description.MimeTextMatchCollection a = (global::System.Web.Services.Description.MimeTextMatchCollection)o.@Matches;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write96_MimeTextMatch(@"match", @"http://microsoft.com/wsdl/mime/textMatching/", ((global::System.Web.Services.Description.MimeTextMatch)a[ia]), false, false);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write96_MimeTextMatch(string n, string ns, global::System.Web.Services.Description.MimeTextMatch o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.MimeTextMatch))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"MimeTextMatch", @"http://microsoft.com/wsdl/mime/textMatching/");
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            WriteAttribute(@"type", @"", ((global::System.String)o.@Type));
            if (((global::System.Int32)o.@Group) != 1)
            {
                WriteAttribute(@"group", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@Group)));
            }
            if (((global::System.Int32)o.@Capture) != 0)
            {
                WriteAttribute(@"capture", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@Capture)));
            }
            if (((global::System.String)o.@RepeatsString) != @"1")
            {
                WriteAttribute(@"repeats", @"", ((global::System.String)o.@RepeatsString));
            }
            WriteAttribute(@"pattern", @"", ((global::System.String)o.@Pattern));
            WriteAttribute(@"ignoreCase", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IgnoreCase)));
            {
                global::System.Web.Services.Description.MimeTextMatchCollection a = (global::System.Web.Services.Description.MimeTextMatchCollection)o.@Matches;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write96_MimeTextMatch(@"match", @"http://microsoft.com/wsdl/mime/textMatching/", ((global::System.Web.Services.Description.MimeTextMatch)a[ia]), false, false);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write94_MimeXmlBinding(string n, string ns, global::System.Web.Services.Description.MimeXmlBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.MimeXmlBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"MimeXmlBinding", @"http://schemas.xmlsoap.org/wsdl/mime/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            WriteAttribute(@"part", @"", ((global::System.String)o.@Part));
            WriteEndElement(o);
        }

        private void Write93_MimeContentBinding(string n, string ns, global::System.Web.Services.Description.MimeContentBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.MimeContentBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"MimeContentBinding", @"http://schemas.xmlsoap.org/wsdl/mime/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            WriteAttribute(@"part", @"", ((global::System.String)o.@Part));
            WriteAttribute(@"type", @"", ((global::System.String)o.@Type));
            WriteEndElement(o);
        }

        private void Write99_SoapBodyBinding(string n, string ns, global::System.Web.Services.Description.SoapBodyBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.SoapBodyBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"SoapBodyBinding", @"http://schemas.xmlsoap.org/wsdl/soap/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            if (((global::System.Web.Services.Description.SoapBindingUse)o.@Use) != global::System.Web.Services.Description.SoapBindingUse.@Default)
            {
                WriteAttribute(@"use", @"", Write98_SoapBindingUse(((global::System.Web.Services.Description.SoapBindingUse)o.@Use)));
            }
            if ((((global::System.String)o.@Namespace) != null) && (((global::System.String)o.@Namespace).Length != 0))
            {
                WriteAttribute(@"namespace", @"", ((global::System.String)o.@Namespace));
            }
            if ((((global::System.String)o.@Encoding) != null) && (((global::System.String)o.@Encoding).Length != 0))
            {
                WriteAttribute(@"encodingStyle", @"", ((global::System.String)o.@Encoding));
            }
            WriteAttribute(@"parts", @"", ((global::System.String)o.@PartsString));
            WriteEndElement(o);
        }

        private void Write102_Soap12BodyBinding(string n, string ns, global::System.Web.Services.Description.Soap12BodyBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.Soap12BodyBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Soap12BodyBinding", @"http://schemas.xmlsoap.org/wsdl/soap12/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            if (((global::System.Web.Services.Description.SoapBindingUse)o.@Use) != global::System.Web.Services.Description.SoapBindingUse.@Default)
            {
                WriteAttribute(@"use", @"", Write100_SoapBindingUse(((global::System.Web.Services.Description.SoapBindingUse)o.@Use)));
            }
            if ((((global::System.String)o.@Namespace) != null) && (((global::System.String)o.@Namespace).Length != 0))
            {
                WriteAttribute(@"namespace", @"", ((global::System.String)o.@Namespace));
            }
            if ((((global::System.String)o.@Encoding) != null) && (((global::System.String)o.@Encoding).Length != 0))
            {
                WriteAttribute(@"encodingStyle", @"", ((global::System.String)o.@Encoding));
            }
            WriteAttribute(@"parts", @"", ((global::System.String)o.@PartsString));
            WriteEndElement(o);
        }

        private void Write106_SoapHeaderBinding(string n, string ns, global::System.Web.Services.Description.SoapHeaderBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.SoapHeaderBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"SoapHeaderBinding", @"http://schemas.xmlsoap.org/wsdl/soap/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            WriteAttribute(@"message", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@Message)));
            WriteAttribute(@"part", @"", ((global::System.String)o.@Part));
            if (((global::System.Web.Services.Description.SoapBindingUse)o.@Use) != global::System.Web.Services.Description.SoapBindingUse.@Default)
            {
                WriteAttribute(@"use", @"", Write98_SoapBindingUse(((global::System.Web.Services.Description.SoapBindingUse)o.@Use)));
            }
            if ((((global::System.String)o.@Encoding) != null) && (((global::System.String)o.@Encoding).Length != 0))
            {
                WriteAttribute(@"encodingStyle", @"", ((global::System.String)o.@Encoding));
            }
            if ((((global::System.String)o.@Namespace) != null) && (((global::System.String)o.@Namespace).Length != 0))
            {
                WriteAttribute(@"namespace", @"", ((global::System.String)o.@Namespace));
            }
            Write105_SoapHeaderFaultBinding(@"headerfault", @"http://schemas.xmlsoap.org/wsdl/soap/", ((global::System.Web.Services.Description.SoapHeaderFaultBinding)o.@Fault), false, false);
            WriteEndElement(o);
        }

        private void Write105_SoapHeaderFaultBinding(string n, string ns, global::System.Web.Services.Description.SoapHeaderFaultBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.SoapHeaderFaultBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"SoapHeaderFaultBinding", @"http://schemas.xmlsoap.org/wsdl/soap/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            WriteAttribute(@"message", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@Message)));
            WriteAttribute(@"part", @"", ((global::System.String)o.@Part));
            if (((global::System.Web.Services.Description.SoapBindingUse)o.@Use) != global::System.Web.Services.Description.SoapBindingUse.@Default)
            {
                WriteAttribute(@"use", @"", Write98_SoapBindingUse(((global::System.Web.Services.Description.SoapBindingUse)o.@Use)));
            }
            if ((((global::System.String)o.@Encoding) != null) && (((global::System.String)o.@Encoding).Length != 0))
            {
                WriteAttribute(@"encodingStyle", @"", ((global::System.String)o.@Encoding));
            }
            if ((((global::System.String)o.@Namespace) != null) && (((global::System.String)o.@Namespace).Length != 0))
            {
                WriteAttribute(@"namespace", @"", ((global::System.String)o.@Namespace));
            }
            WriteEndElement(o);
        }

        private void Write109_Soap12HeaderBinding(string n, string ns, global::System.Web.Services.Description.Soap12HeaderBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.Soap12HeaderBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Soap12HeaderBinding", @"http://schemas.xmlsoap.org/wsdl/soap12/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            WriteAttribute(@"message", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@Message)));
            WriteAttribute(@"part", @"", ((global::System.String)o.@Part));
            if (((global::System.Web.Services.Description.SoapBindingUse)o.@Use) != global::System.Web.Services.Description.SoapBindingUse.@Default)
            {
                WriteAttribute(@"use", @"", Write100_SoapBindingUse(((global::System.Web.Services.Description.SoapBindingUse)o.@Use)));
            }
            if ((((global::System.String)o.@Encoding) != null) && (((global::System.String)o.@Encoding).Length != 0))
            {
                WriteAttribute(@"encodingStyle", @"", ((global::System.String)o.@Encoding));
            }
            if ((((global::System.String)o.@Namespace) != null) && (((global::System.String)o.@Namespace).Length != 0))
            {
                WriteAttribute(@"namespace", @"", ((global::System.String)o.@Namespace));
            }
            Write107_SoapHeaderFaultBinding(@"headerfault", @"http://schemas.xmlsoap.org/wsdl/soap12/", ((global::System.Web.Services.Description.SoapHeaderFaultBinding)o.@Fault), false, false);
            WriteEndElement(o);
        }

        private void Write107_SoapHeaderFaultBinding(string n, string ns, global::System.Web.Services.Description.SoapHeaderFaultBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.SoapHeaderFaultBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"SoapHeaderFaultBinding", @"http://schemas.xmlsoap.org/wsdl/soap12/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            WriteAttribute(@"message", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@Message)));
            WriteAttribute(@"part", @"", ((global::System.String)o.@Part));
            if (((global::System.Web.Services.Description.SoapBindingUse)o.@Use) != global::System.Web.Services.Description.SoapBindingUse.@Default)
            {
                WriteAttribute(@"use", @"", Write100_SoapBindingUse(((global::System.Web.Services.Description.SoapBindingUse)o.@Use)));
            }
            if ((((global::System.String)o.@Encoding) != null) && (((global::System.String)o.@Encoding).Length != 0))
            {
                WriteAttribute(@"encodingStyle", @"", ((global::System.String)o.@Encoding));
            }
            if ((((global::System.String)o.@Namespace) != null) && (((global::System.String)o.@Namespace).Length != 0))
            {
                WriteAttribute(@"namespace", @"", ((global::System.String)o.@Namespace));
            }
            WriteEndElement(o);
        }

        private void Write110_InputBinding(string n, string ns, global::System.Web.Services.Description.InputBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.InputBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"InputBinding", @"http://schemas.xmlsoap.org/wsdl/");
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@ExtensibleAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            if ((o.@DocumentationElement) is Microsoft.Xml.XmlNode || o.@DocumentationElement == null)
            {
                WriteElementLiteral((Microsoft.Xml.XmlNode)o.@DocumentationElement, @"documentation", @"http://schemas.xmlsoap.org/wsdl/", false, true);
            }
            else
            {
                throw CreateInvalidAnyTypeException(o.@DocumentationElement);
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        global::System.Object ai = (global::System.Object)a[ia];
                        {
                            if (ai is global::System.Web.Services.Description.Soap12BodyBinding)
                            {
                                Write102_Soap12BodyBinding(@"body", @"http://schemas.xmlsoap.org/wsdl/soap12/", ((global::System.Web.Services.Description.Soap12BodyBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.Soap12HeaderBinding)
                            {
                                Write109_Soap12HeaderBinding(@"header", @"http://schemas.xmlsoap.org/wsdl/soap12/", ((global::System.Web.Services.Description.Soap12HeaderBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.SoapBodyBinding)
                            {
                                Write99_SoapBodyBinding(@"body", @"http://schemas.xmlsoap.org/wsdl/soap/", ((global::System.Web.Services.Description.SoapBodyBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.SoapHeaderBinding)
                            {
                                Write106_SoapHeaderBinding(@"header", @"http://schemas.xmlsoap.org/wsdl/soap/", ((global::System.Web.Services.Description.SoapHeaderBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.MimeTextBinding)
                            {
                                Write97_MimeTextBinding(@"text", @"http://microsoft.com/wsdl/mime/textMatching/", ((global::System.Web.Services.Description.MimeTextBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.HttpUrlReplacementBinding)
                            {
                                Write91_HttpUrlReplacementBinding(@"urlReplacement", @"http://schemas.xmlsoap.org/wsdl/http/", ((global::System.Web.Services.Description.HttpUrlReplacementBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.HttpUrlEncodedBinding)
                            {
                                Write90_HttpUrlEncodedBinding(@"urlEncoded", @"http://schemas.xmlsoap.org/wsdl/http/", ((global::System.Web.Services.Description.HttpUrlEncodedBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.MimeContentBinding)
                            {
                                Write93_MimeContentBinding(@"content", @"http://schemas.xmlsoap.org/wsdl/mime/", ((global::System.Web.Services.Description.MimeContentBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.MimeMultipartRelatedBinding)
                            {
                                Write104_MimeMultipartRelatedBinding(@"multipartRelated", @"http://schemas.xmlsoap.org/wsdl/mime/", ((global::System.Web.Services.Description.MimeMultipartRelatedBinding)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.MimeXmlBinding)
                            {
                                Write94_MimeXmlBinding(@"mimeXml", @"http://schemas.xmlsoap.org/wsdl/mime/", ((global::System.Web.Services.Description.MimeXmlBinding)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.XmlElement)
                            {
                                Microsoft.Xml.XmlElement elem = (Microsoft.Xml.XmlElement)ai;
                                if ((elem) is Microsoft.Xml.XmlNode || elem == null)
                                {
                                    WriteElementLiteral((Microsoft.Xml.XmlNode)elem, @"", null, false, true);
                                }
                                else
                                {
                                    throw CreateInvalidAnyTypeException(elem);
                                }
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write90_HttpUrlEncodedBinding(string n, string ns, global::System.Web.Services.Description.HttpUrlEncodedBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.HttpUrlEncodedBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"HttpUrlEncodedBinding", @"http://schemas.xmlsoap.org/wsdl/http/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            WriteEndElement(o);
        }

        private void Write91_HttpUrlReplacementBinding(string n, string ns, global::System.Web.Services.Description.HttpUrlReplacementBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.HttpUrlReplacementBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"HttpUrlReplacementBinding", @"http://schemas.xmlsoap.org/wsdl/http/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            WriteEndElement(o);
        }

        private void Write86_SoapOperationBinding(string n, string ns, global::System.Web.Services.Description.SoapOperationBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.SoapOperationBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"SoapOperationBinding", @"http://schemas.xmlsoap.org/wsdl/soap/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            WriteAttribute(@"soapAction", @"", ((global::System.String)o.@SoapAction));
            if (((global::System.Web.Services.Description.SoapBindingStyle)o.@Style) != global::System.Web.Services.Description.SoapBindingStyle.@Default)
            {
                WriteAttribute(@"style", @"", Write79_SoapBindingStyle(((global::System.Web.Services.Description.SoapBindingStyle)o.@Style)));
            }
            WriteEndElement(o);
        }

        private string Write79_SoapBindingStyle(global::System.Web.Services.Description.SoapBindingStyle v)
        {
            string s = null;
            switch (v)
            {
                case global::System.Web.Services.Description.SoapBindingStyle.@Document: s = @"document"; break;
                case global::System.Web.Services.Description.SoapBindingStyle.@Rpc: s = @"rpc"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"System.Web.Services.Description.SoapBindingStyle");
            }
            return s;
        }

        private void Write85_HttpOperationBinding(string n, string ns, global::System.Web.Services.Description.HttpOperationBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.HttpOperationBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"HttpOperationBinding", @"http://schemas.xmlsoap.org/wsdl/http/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            WriteAttribute(@"location", @"", ((global::System.String)o.@Location));
            WriteEndElement(o);
        }

        private void Write88_Soap12OperationBinding(string n, string ns, global::System.Web.Services.Description.Soap12OperationBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.Soap12OperationBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Soap12OperationBinding", @"http://schemas.xmlsoap.org/wsdl/soap12/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            WriteAttribute(@"soapAction", @"", ((global::System.String)o.@SoapAction));
            if (((global::System.Web.Services.Description.SoapBindingStyle)o.@Style) != global::System.Web.Services.Description.SoapBindingStyle.@Default)
            {
                WriteAttribute(@"style", @"", Write82_SoapBindingStyle(((global::System.Web.Services.Description.SoapBindingStyle)o.@Style)));
            }
            if (((global::System.Boolean)o.@SoapActionRequired) != false)
            {
                WriteAttribute(@"soapActionRequired", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@SoapActionRequired)));
            }
            WriteEndElement(o);
        }

        private string Write82_SoapBindingStyle(global::System.Web.Services.Description.SoapBindingStyle v)
        {
            string s = null;
            switch (v)
            {
                case global::System.Web.Services.Description.SoapBindingStyle.@Document: s = @"document"; break;
                case global::System.Web.Services.Description.SoapBindingStyle.@Rpc: s = @"rpc"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"System.Web.Services.Description.SoapBindingStyle");
            }
            return s;
        }

        private void Write80_SoapBinding(string n, string ns, global::System.Web.Services.Description.SoapBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.SoapBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"SoapBinding", @"http://schemas.xmlsoap.org/wsdl/soap/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            WriteAttribute(@"transport", @"", ((global::System.String)o.@Transport));
            if (((global::System.Web.Services.Description.SoapBindingStyle)o.@Style) != global::System.Web.Services.Description.SoapBindingStyle.@Document)
            {
                WriteAttribute(@"style", @"", Write79_SoapBindingStyle(((global::System.Web.Services.Description.SoapBindingStyle)o.@Style)));
            }
            WriteEndElement(o);
        }

        private void Write77_HttpBinding(string n, string ns, global::System.Web.Services.Description.HttpBinding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.HttpBinding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"HttpBinding", @"http://schemas.xmlsoap.org/wsdl/http/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            WriteAttribute(@"verb", @"", ((global::System.String)o.@Verb));
            WriteEndElement(o);
        }

        private void Write84_Soap12Binding(string n, string ns, global::System.Web.Services.Description.Soap12Binding o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.Soap12Binding))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Soap12Binding", @"http://schemas.xmlsoap.org/wsdl/soap12/");
            if (((global::System.Boolean)o.@Required) != false)
            {
                WriteAttribute(@"required", @"http://schemas.xmlsoap.org/wsdl/", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@Required)));
            }
            WriteAttribute(@"transport", @"", ((global::System.String)o.@Transport));
            if (((global::System.Web.Services.Description.SoapBindingStyle)o.@Style) != global::System.Web.Services.Description.SoapBindingStyle.@Document)
            {
                WriteAttribute(@"style", @"", Write82_SoapBindingStyle(((global::System.Web.Services.Description.SoapBindingStyle)o.@Style)));
            }
            WriteEndElement(o);
        }

        private void Write75_PortType(string n, string ns, global::System.Web.Services.Description.PortType o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.PortType))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"PortType", @"http://schemas.xmlsoap.org/wsdl/");
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@ExtensibleAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            if ((o.@DocumentationElement) is Microsoft.Xml.XmlNode || o.@DocumentationElement == null)
            {
                WriteElementLiteral((Microsoft.Xml.XmlNode)o.@DocumentationElement, @"documentation", @"http://schemas.xmlsoap.org/wsdl/", false, true);
            }
            else
            {
                throw CreateInvalidAnyTypeException(o.@DocumentationElement);
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        if ((a[ia]) is Microsoft.Xml.XmlNode || a[ia] == null)
                        {
                            WriteElementLiteral((Microsoft.Xml.XmlNode)a[ia], @"", null, false, true);
                        }
                        else
                        {
                            throw CreateInvalidAnyTypeException(a[ia]);
                        }
                    }
                }
            }
            {
                global::System.Web.Services.Description.OperationCollection a = (global::System.Web.Services.Description.OperationCollection)o.@Operations;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write74_Operation(@"operation", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.Web.Services.Description.Operation)a[ia]), false, false);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write74_Operation(string n, string ns, global::System.Web.Services.Description.Operation o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.Operation))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"Operation", @"http://schemas.xmlsoap.org/wsdl/");
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@ExtensibleAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            if ((((global::System.String)o.@ParameterOrderString) != null) && (((global::System.String)o.@ParameterOrderString).Length != 0))
            {
                WriteAttribute(@"parameterOrder", @"", ((global::System.String)o.@ParameterOrderString));
            }
            if ((o.@DocumentationElement) is Microsoft.Xml.XmlNode || o.@DocumentationElement == null)
            {
                WriteElementLiteral((Microsoft.Xml.XmlNode)o.@DocumentationElement, @"documentation", @"http://schemas.xmlsoap.org/wsdl/", false, true);
            }
            else
            {
                throw CreateInvalidAnyTypeException(o.@DocumentationElement);
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        if ((a[ia]) is Microsoft.Xml.XmlNode || a[ia] == null)
                        {
                            WriteElementLiteral((Microsoft.Xml.XmlNode)a[ia], @"", null, false, true);
                        }
                        else
                        {
                            throw CreateInvalidAnyTypeException(a[ia]);
                        }
                    }
                }
            }
            {
                global::System.Web.Services.Description.OperationMessageCollection a = (global::System.Web.Services.Description.OperationMessageCollection)o.@Messages;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        global::System.Web.Services.Description.OperationMessage ai = (global::System.Web.Services.Description.OperationMessage)a[ia];
                        {
                            if (ai is global::System.Web.Services.Description.OperationOutput)
                            {
                                Write72_OperationOutput(@"output", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.Web.Services.Description.OperationOutput)ai), false, false);
                            }
                            else if (ai is global::System.Web.Services.Description.OperationInput)
                            {
                                Write71_OperationInput(@"input", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.Web.Services.Description.OperationInput)ai), false, false);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            {
                global::System.Web.Services.Description.OperationFaultCollection a = (global::System.Web.Services.Description.OperationFaultCollection)o.@Faults;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write73_OperationFault(@"fault", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.Web.Services.Description.OperationFault)a[ia]), false, false);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write73_OperationFault(string n, string ns, global::System.Web.Services.Description.OperationFault o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.OperationFault))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"OperationFault", @"http://schemas.xmlsoap.org/wsdl/");
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@ExtensibleAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            WriteAttribute(@"message", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@Message)));
            if ((o.@DocumentationElement) is Microsoft.Xml.XmlNode || o.@DocumentationElement == null)
            {
                WriteElementLiteral((Microsoft.Xml.XmlNode)o.@DocumentationElement, @"documentation", @"http://schemas.xmlsoap.org/wsdl/", false, true);
            }
            else
            {
                throw CreateInvalidAnyTypeException(o.@DocumentationElement);
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        if ((a[ia]) is Microsoft.Xml.XmlNode || a[ia] == null)
                        {
                            WriteElementLiteral((Microsoft.Xml.XmlNode)a[ia], @"", null, false, true);
                        }
                        else
                        {
                            throw CreateInvalidAnyTypeException(a[ia]);
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write71_OperationInput(string n, string ns, global::System.Web.Services.Description.OperationInput o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.OperationInput))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"OperationInput", @"http://schemas.xmlsoap.org/wsdl/");
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@ExtensibleAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            WriteAttribute(@"message", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@Message)));
            if ((o.@DocumentationElement) is Microsoft.Xml.XmlNode || o.@DocumentationElement == null)
            {
                WriteElementLiteral((Microsoft.Xml.XmlNode)o.@DocumentationElement, @"documentation", @"http://schemas.xmlsoap.org/wsdl/", false, true);
            }
            else
            {
                throw CreateInvalidAnyTypeException(o.@DocumentationElement);
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        if ((a[ia]) is Microsoft.Xml.XmlNode || a[ia] == null)
                        {
                            WriteElementLiteral((Microsoft.Xml.XmlNode)a[ia], @"", null, false, true);
                        }
                        else
                        {
                            throw CreateInvalidAnyTypeException(a[ia]);
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write72_OperationOutput(string n, string ns, global::System.Web.Services.Description.OperationOutput o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.OperationOutput))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"OperationOutput", @"http://schemas.xmlsoap.org/wsdl/");
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@ExtensibleAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            WriteAttribute(@"message", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@Message)));
            if ((o.@DocumentationElement) is Microsoft.Xml.XmlNode || o.@DocumentationElement == null)
            {
                WriteElementLiteral((Microsoft.Xml.XmlNode)o.@DocumentationElement, @"documentation", @"http://schemas.xmlsoap.org/wsdl/", false, true);
            }
            else
            {
                throw CreateInvalidAnyTypeException(o.@DocumentationElement);
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        if ((a[ia]) is Microsoft.Xml.XmlNode || a[ia] == null)
                        {
                            WriteElementLiteral((Microsoft.Xml.XmlNode)a[ia], @"", null, false, true);
                        }
                        else
                        {
                            throw CreateInvalidAnyTypeException(a[ia]);
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write69_Message(string n, string ns, global::System.Web.Services.Description.Message o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.Message))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"Message", @"http://schemas.xmlsoap.org/wsdl/");
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@ExtensibleAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            if ((o.@DocumentationElement) is Microsoft.Xml.XmlNode || o.@DocumentationElement == null)
            {
                WriteElementLiteral((Microsoft.Xml.XmlNode)o.@DocumentationElement, @"documentation", @"http://schemas.xmlsoap.org/wsdl/", false, true);
            }
            else
            {
                throw CreateInvalidAnyTypeException(o.@DocumentationElement);
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        if ((a[ia]) is Microsoft.Xml.XmlNode || a[ia] == null)
                        {
                            WriteElementLiteral((Microsoft.Xml.XmlNode)a[ia], @"", null, false, true);
                        }
                        else
                        {
                            throw CreateInvalidAnyTypeException(a[ia]);
                        }
                    }
                }
            }
            {
                global::System.Web.Services.Description.MessagePartCollection a = (global::System.Web.Services.Description.MessagePartCollection)o.@Parts;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write68_MessagePart(@"part", @"http://schemas.xmlsoap.org/wsdl/", ((global::System.Web.Services.Description.MessagePart)a[ia]), false, false);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write68_MessagePart(string n, string ns, global::System.Web.Services.Description.MessagePart o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.MessagePart))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"MessagePart", @"http://schemas.xmlsoap.org/wsdl/");
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@ExtensibleAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            WriteAttribute(@"element", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@Element)));
            WriteAttribute(@"type", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@Type)));
            if ((o.@DocumentationElement) is Microsoft.Xml.XmlNode || o.@DocumentationElement == null)
            {
                WriteElementLiteral((Microsoft.Xml.XmlNode)o.@DocumentationElement, @"documentation", @"http://schemas.xmlsoap.org/wsdl/", false, true);
            }
            else
            {
                throw CreateInvalidAnyTypeException(o.@DocumentationElement);
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        if ((a[ia]) is Microsoft.Xml.XmlNode || a[ia] == null)
                        {
                            WriteElementLiteral((Microsoft.Xml.XmlNode)a[ia], @"", null, false, true);
                        }
                        else
                        {
                            throw CreateInvalidAnyTypeException(a[ia]);
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write67_Types(string n, string ns, global::System.Web.Services.Description.Types o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.Types))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"Types", @"http://schemas.xmlsoap.org/wsdl/");
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@ExtensibleAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            if ((o.@DocumentationElement) is Microsoft.Xml.XmlNode || o.@DocumentationElement == null)
            {
                WriteElementLiteral((Microsoft.Xml.XmlNode)o.@DocumentationElement, @"documentation", @"http://schemas.xmlsoap.org/wsdl/", false, true);
            }
            else
            {
                throw CreateInvalidAnyTypeException(o.@DocumentationElement);
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        if ((a[ia]) is Microsoft.Xml.XmlNode || a[ia] == null)
                        {
                            WriteElementLiteral((Microsoft.Xml.XmlNode)a[ia], @"", null, false, true);
                        }
                        else
                        {
                            throw CreateInvalidAnyTypeException(a[ia]);
                        }
                    }
                }
            }
            {
                Microsoft.Xml.Serialization.XmlSchemas a = (Microsoft.Xml.Serialization.XmlSchemas)o.@Schemas;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write66_XmlSchema(@"schema", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchema)a[ia]), false, false);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write66_XmlSchema(string n, string ns, Microsoft.Xml.Schema.XmlSchema o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchema))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchema", @"http://www.w3.org/2001/XMLSchema");
            if (((Microsoft.Xml.Schema.XmlSchemaForm)o.@AttributeFormDefault) != Microsoft.Xml.Schema.XmlSchemaForm.@None)
            {
                WriteAttribute(@"attributeFormDefault", @"", Write6_XmlSchemaForm(((Microsoft.Xml.Schema.XmlSchemaForm)o.@AttributeFormDefault)));
            }
            if (((Microsoft.Xml.Schema.XmlSchemaDerivationMethod)o.@BlockDefault) != (Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@None))
            {
                WriteAttribute(@"blockDefault", @"", Write7_XmlSchemaDerivationMethod(((Microsoft.Xml.Schema.XmlSchemaDerivationMethod)o.@BlockDefault)));
            }
            if (((Microsoft.Xml.Schema.XmlSchemaDerivationMethod)o.@FinalDefault) != (Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@None))
            {
                WriteAttribute(@"finalDefault", @"", Write7_XmlSchemaDerivationMethod(((Microsoft.Xml.Schema.XmlSchemaDerivationMethod)o.@FinalDefault)));
            }
            if (((Microsoft.Xml.Schema.XmlSchemaForm)o.@ElementFormDefault) != Microsoft.Xml.Schema.XmlSchemaForm.@None)
            {
                WriteAttribute(@"elementFormDefault", @"", Write6_XmlSchemaForm(((Microsoft.Xml.Schema.XmlSchemaForm)o.@ElementFormDefault)));
            }
            WriteAttribute(@"targetNamespace", @"", ((global::System.String)o.@TargetNamespace));
            WriteAttribute(@"version", @"", ((global::System.String)o.@Version));
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Includes;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Microsoft.Xml.Schema.XmlSchemaObject ai = (Microsoft.Xml.Schema.XmlSchemaObject)a[ia];
                        {
                            if (ai is Microsoft.Xml.Schema.XmlSchemaRedefine)
                            {
                                Write64_XmlSchemaRedefine(@"redefine", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaRedefine)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaImport)
                            {
                                Write13_XmlSchemaImport(@"import", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaImport)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaInclude)
                            {
                                Write12_XmlSchemaInclude(@"include", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaInclude)ai), false, false);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Items;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Microsoft.Xml.Schema.XmlSchemaObject ai = (Microsoft.Xml.Schema.XmlSchemaObject)a[ia];
                        {
                            if (ai is Microsoft.Xml.Schema.XmlSchemaElement)
                            {
                                Write52_XmlSchemaElement(@"element", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaElement)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaComplexType)
                            {
                                Write62_XmlSchemaComplexType(@"complexType", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaComplexType)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaSimpleType)
                            {
                                Write34_XmlSchemaSimpleType(@"simpleType", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSimpleType)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaAttribute)
                            {
                                Write36_XmlSchemaAttribute(@"attribute", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAttribute)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaAttributeGroup)
                            {
                                Write40_XmlSchemaAttributeGroup(@"attributeGroup", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAttributeGroup)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaNotation)
                            {
                                Write65_XmlSchemaNotation(@"notation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaNotation)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaGroup)
                            {
                                Write63_XmlSchemaGroup(@"group", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaGroup)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaAnnotation)
                            {
                                Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)ai), false, false);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write11_XmlSchemaAnnotation(string n, string ns, Microsoft.Xml.Schema.XmlSchemaAnnotation o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaAnnotation))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaAnnotation", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Items;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Microsoft.Xml.Schema.XmlSchemaObject ai = (Microsoft.Xml.Schema.XmlSchemaObject)a[ia];
                        {
                            if (ai is Microsoft.Xml.Schema.XmlSchemaAppInfo)
                            {
                                Write10_XmlSchemaAppInfo(@"appinfo", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAppInfo)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaDocumentation)
                            {
                                Write9_XmlSchemaDocumentation(@"documentation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaDocumentation)ai), false, false);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write9_XmlSchemaDocumentation(string n, string ns, Microsoft.Xml.Schema.XmlSchemaDocumentation o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaDocumentation))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaDocumentation", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"source", @"", ((global::System.String)o.@Source));
            WriteAttribute(@"lang", @"http://www.w3.org/XML/1998/namespace", ((global::System.String)o.@Language));
            {
                Microsoft.Xml.XmlNode[] a = (Microsoft.Xml.XmlNode[])o.@Markup;
                if (a != null)
                {
                    for (int ia = 0; ia < a.Length; ia++)
                    {
                        Microsoft.Xml.XmlNode ai = (Microsoft.Xml.XmlNode)a[ia];
                        {
                            if (ai is Microsoft.Xml.XmlElement)
                            {
                                Microsoft.Xml.XmlElement elem = (Microsoft.Xml.XmlElement)ai;
                                if ((elem) is Microsoft.Xml.XmlNode || elem == null)
                                {
                                    WriteElementLiteral((Microsoft.Xml.XmlNode)elem, @"", null, false, true);
                                }
                                else
                                {
                                    throw CreateInvalidAnyTypeException(elem);
                                }
                            }
                            else if (ai is Microsoft.Xml.XmlNode)
                            {
                                ((Microsoft.Xml.XmlNode)ai).WriteTo(Writer);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write10_XmlSchemaAppInfo(string n, string ns, Microsoft.Xml.Schema.XmlSchemaAppInfo o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaAppInfo))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaAppInfo", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"source", @"", ((global::System.String)o.@Source));
            {
                Microsoft.Xml.XmlNode[] a = (Microsoft.Xml.XmlNode[])o.@Markup;
                if (a != null)
                {
                    for (int ia = 0; ia < a.Length; ia++)
                    {
                        Microsoft.Xml.XmlNode ai = (Microsoft.Xml.XmlNode)a[ia];
                        {
                            if (ai is Microsoft.Xml.XmlElement)
                            {
                                Microsoft.Xml.XmlElement elem = (Microsoft.Xml.XmlElement)ai;
                                if ((elem) is Microsoft.Xml.XmlNode || elem == null)
                                {
                                    WriteElementLiteral((Microsoft.Xml.XmlNode)elem, @"", null, false, true);
                                }
                                else
                                {
                                    throw CreateInvalidAnyTypeException(elem);
                                }
                            }
                            else if (ai is Microsoft.Xml.XmlNode)
                            {
                                ((Microsoft.Xml.XmlNode)ai).WriteTo(Writer);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write63_XmlSchemaGroup(string n, string ns, Microsoft.Xml.Schema.XmlSchemaGroup o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaGroup))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaGroup", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            {
                if (o.@Particle is Microsoft.Xml.Schema.XmlSchemaAll)
                {
                    Write55_XmlSchemaAll(@"all", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAll)o.@Particle), false, false);
                }
                else if (o.@Particle is Microsoft.Xml.Schema.XmlSchemaChoice)
                {
                    Write54_XmlSchemaChoice(@"choice", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaChoice)o.@Particle), false, false);
                }
                else if (o.@Particle is Microsoft.Xml.Schema.XmlSchemaSequence)
                {
                    Write53_XmlSchemaSequence(@"sequence", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSequence)o.@Particle), false, false);
                }
                else
                {
                    if (o.@Particle != null)
                    {
                        throw CreateUnknownTypeException(o.@Particle);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write53_XmlSchemaSequence(string n, string ns, Microsoft.Xml.Schema.XmlSchemaSequence o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaSequence))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaSequence", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"minOccurs", @"", ((global::System.String)o.@MinOccursString));
            WriteAttribute(@"maxOccurs", @"", ((global::System.String)o.@MaxOccursString));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Items;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Microsoft.Xml.Schema.XmlSchemaObject ai = (Microsoft.Xml.Schema.XmlSchemaObject)a[ia];
                        {
                            if (ai is Microsoft.Xml.Schema.XmlSchemaChoice)
                            {
                                Write54_XmlSchemaChoice(@"choice", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaChoice)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaSequence)
                            {
                                Write53_XmlSchemaSequence(@"sequence", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSequence)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaGroupRef)
                            {
                                Write44_XmlSchemaGroupRef(@"group", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaGroupRef)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaElement)
                            {
                                Write52_XmlSchemaElement(@"element", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaElement)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaAny)
                            {
                                Write46_XmlSchemaAny(@"any", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAny)ai), false, false);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write46_XmlSchemaAny(string n, string ns, Microsoft.Xml.Schema.XmlSchemaAny o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaAny))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaAny", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"minOccurs", @"", ((global::System.String)o.@MinOccursString));
            WriteAttribute(@"maxOccurs", @"", ((global::System.String)o.@MaxOccursString));
            WriteAttribute(@"namespace", @"", ((global::System.String)o.@Namespace));
            if (((Microsoft.Xml.Schema.XmlSchemaContentProcessing)o.@ProcessContents) != Microsoft.Xml.Schema.XmlSchemaContentProcessing.@None)
            {
                WriteAttribute(@"processContents", @"", Write38_XmlSchemaContentProcessing(((Microsoft.Xml.Schema.XmlSchemaContentProcessing)o.@ProcessContents)));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private string Write38_XmlSchemaContentProcessing(Microsoft.Xml.Schema.XmlSchemaContentProcessing v)
        {
            string s = null;
            switch (v)
            {
                case Microsoft.Xml.Schema.XmlSchemaContentProcessing.@Skip: s = @"skip"; break;
                case Microsoft.Xml.Schema.XmlSchemaContentProcessing.@Lax: s = @"lax"; break;
                case Microsoft.Xml.Schema.XmlSchemaContentProcessing.@Strict: s = @"strict"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"System.Xml.Schema.XmlSchemaContentProcessing");
            }
            return s;
        }

        private void Write52_XmlSchemaElement(string n, string ns, Microsoft.Xml.Schema.XmlSchemaElement o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaElement))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaElement", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"minOccurs", @"", ((global::System.String)o.@MinOccursString));
            WriteAttribute(@"maxOccurs", @"", ((global::System.String)o.@MaxOccursString));
            if (((global::System.Boolean)o.@IsAbstract) != false)
            {
                WriteAttribute(@"abstract", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsAbstract)));
            }
            if (((Microsoft.Xml.Schema.XmlSchemaDerivationMethod)o.@Block) != (Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@None))
            {
                WriteAttribute(@"block", @"", Write7_XmlSchemaDerivationMethod(((Microsoft.Xml.Schema.XmlSchemaDerivationMethod)o.@Block)));
            }
            WriteAttribute(@"default", @"", ((global::System.String)o.@DefaultValue));
            if (((Microsoft.Xml.Schema.XmlSchemaDerivationMethod)o.@Final) != (Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@None))
            {
                WriteAttribute(@"final", @"", Write7_XmlSchemaDerivationMethod(((Microsoft.Xml.Schema.XmlSchemaDerivationMethod)o.@Final)));
            }
            WriteAttribute(@"fixed", @"", ((global::System.String)o.@FixedValue));
            if (((Microsoft.Xml.Schema.XmlSchemaForm)o.@Form) != Microsoft.Xml.Schema.XmlSchemaForm.@None)
            {
                WriteAttribute(@"form", @"", Write6_XmlSchemaForm(((Microsoft.Xml.Schema.XmlSchemaForm)o.@Form)));
            }
            if ((((global::System.String)o.@Name) != null) && (((global::System.String)o.@Name).Length != 0))
            {
                WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            }
            if (((global::System.Boolean)o.@IsNillable) != false)
            {
                WriteAttribute(@"nillable", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsNillable)));
            }
            WriteAttribute(@"ref", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@RefName)));
            WriteAttribute(@"substitutionGroup", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@SubstitutionGroup)));
            WriteAttribute(@"type", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@SchemaTypeName)));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            {
                if (o.@SchemaType is Microsoft.Xml.Schema.XmlSchemaComplexType)
                {
                    Write62_XmlSchemaComplexType(@"complexType", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaComplexType)o.@SchemaType), false, false);
                }
                else if (o.@SchemaType is Microsoft.Xml.Schema.XmlSchemaSimpleType)
                {
                    Write34_XmlSchemaSimpleType(@"simpleType", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSimpleType)o.@SchemaType), false, false);
                }
                else
                {
                    if (o.@SchemaType != null)
                    {
                        throw CreateUnknownTypeException(o.@SchemaType);
                    }
                }
            }
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Constraints;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Microsoft.Xml.Schema.XmlSchemaObject ai = (Microsoft.Xml.Schema.XmlSchemaObject)a[ia];
                        {
                            if (ai is Microsoft.Xml.Schema.XmlSchemaKeyref)
                            {
                                Write51_XmlSchemaKeyref(@"keyref", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaKeyref)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaUnique)
                            {
                                Write50_XmlSchemaUnique(@"unique", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaUnique)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaKey)
                            {
                                Write49_XmlSchemaKey(@"key", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaKey)ai), false, false);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write49_XmlSchemaKey(string n, string ns, Microsoft.Xml.Schema.XmlSchemaKey o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaKey))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaKey", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            Write47_XmlSchemaXPath(@"selector", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaXPath)o.@Selector), false, false);
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Fields;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write47_XmlSchemaXPath(@"field", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaXPath)a[ia]), false, false);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write47_XmlSchemaXPath(string n, string ns, Microsoft.Xml.Schema.XmlSchemaXPath o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaXPath))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaXPath", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            if ((((global::System.String)o.@XPath) != null) && (((global::System.String)o.@XPath).Length != 0))
            {
                WriteAttribute(@"xpath", @"", ((global::System.String)o.@XPath));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write50_XmlSchemaUnique(string n, string ns, Microsoft.Xml.Schema.XmlSchemaUnique o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaUnique))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaUnique", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            Write47_XmlSchemaXPath(@"selector", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaXPath)o.@Selector), false, false);
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Fields;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write47_XmlSchemaXPath(@"field", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaXPath)a[ia]), false, false);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write51_XmlSchemaKeyref(string n, string ns, Microsoft.Xml.Schema.XmlSchemaKeyref o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaKeyref))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaKeyref", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            WriteAttribute(@"refer", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@Refer)));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            Write47_XmlSchemaXPath(@"selector", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaXPath)o.@Selector), false, false);
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Fields;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write47_XmlSchemaXPath(@"field", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaXPath)a[ia]), false, false);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write34_XmlSchemaSimpleType(string n, string ns, Microsoft.Xml.Schema.XmlSchemaSimpleType o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaSimpleType))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaSimpleType", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            if (((Microsoft.Xml.Schema.XmlSchemaDerivationMethod)o.@Final) != (Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@None))
            {
                WriteAttribute(@"final", @"", Write7_XmlSchemaDerivationMethod(((Microsoft.Xml.Schema.XmlSchemaDerivationMethod)o.@Final)));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            {
                if (o.@Content is Microsoft.Xml.Schema.XmlSchemaSimpleTypeUnion)
                {
                    Write33_XmlSchemaSimpleTypeUnion(@"union", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSimpleTypeUnion)o.@Content), false, false);
                }
                else if (o.@Content is Microsoft.Xml.Schema.XmlSchemaSimpleTypeRestriction)
                {
                    Write32_XmlSchemaSimpleTypeRestriction(@"restriction", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSimpleTypeRestriction)o.@Content), false, false);
                }
                else if (o.@Content is Microsoft.Xml.Schema.XmlSchemaSimpleTypeList)
                {
                    Write17_XmlSchemaSimpleTypeList(@"list", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSimpleTypeList)o.@Content), false, false);
                }
                else
                {
                    if (o.@Content != null)
                    {
                        throw CreateUnknownTypeException(o.@Content);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write17_XmlSchemaSimpleTypeList(string n, string ns, Microsoft.Xml.Schema.XmlSchemaSimpleTypeList o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaSimpleTypeList))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaSimpleTypeList", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"itemType", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@ItemTypeName)));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            Write34_XmlSchemaSimpleType(@"simpleType", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSimpleType)o.@ItemType), false, false);
            WriteEndElement(o);
        }

        private void Write32_XmlSchemaSimpleTypeRestriction(string n, string ns, Microsoft.Xml.Schema.XmlSchemaSimpleTypeRestriction o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaSimpleTypeRestriction))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaSimpleTypeRestriction", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"base", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@BaseTypeName)));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            Write34_XmlSchemaSimpleType(@"simpleType", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSimpleType)o.@BaseType), false, false);
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Facets;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Microsoft.Xml.Schema.XmlSchemaObject ai = (Microsoft.Xml.Schema.XmlSchemaObject)a[ia];
                        {
                            if (ai is Microsoft.Xml.Schema.XmlSchemaLengthFacet)
                            {
                                Write23_XmlSchemaLengthFacet(@"length", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaLengthFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaTotalDigitsFacet)
                            {
                                Write24_XmlSchemaTotalDigitsFacet(@"totalDigits", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaTotalDigitsFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaMaxLengthFacet)
                            {
                                Write22_XmlSchemaMaxLengthFacet(@"maxLength", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaMaxLengthFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaFractionDigitsFacet)
                            {
                                Write20_XmlSchemaFractionDigitsFacet(@"fractionDigits", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaFractionDigitsFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaMinLengthFacet)
                            {
                                Write31_XmlSchemaMinLengthFacet(@"minLength", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaMinLengthFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaMaxExclusiveFacet)
                            {
                                Write28_XmlSchemaMaxExclusiveFacet(@"maxExclusive", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaMaxExclusiveFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaWhiteSpaceFacet)
                            {
                                Write29_XmlSchemaWhiteSpaceFacet(@"whiteSpace", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaWhiteSpaceFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaMinExclusiveFacet)
                            {
                                Write30_XmlSchemaMinExclusiveFacet(@"minExclusive", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaMinExclusiveFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaPatternFacet)
                            {
                                Write25_XmlSchemaPatternFacet(@"pattern", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaPatternFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaMinInclusiveFacet)
                            {
                                Write21_XmlSchemaMinInclusiveFacet(@"minInclusive", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaMinInclusiveFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaMaxInclusiveFacet)
                            {
                                Write27_XmlSchemaMaxInclusiveFacet(@"maxInclusive", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaMaxInclusiveFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaEnumerationFacet)
                            {
                                Write26_XmlSchemaEnumerationFacet(@"enumeration", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaEnumerationFacet)ai), false, false);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write26_XmlSchemaEnumerationFacet(string n, string ns, Microsoft.Xml.Schema.XmlSchemaEnumerationFacet o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaEnumerationFacet))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaEnumerationFacet", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"value", @"", ((global::System.String)o.@Value));
            if (((global::System.Boolean)o.@IsFixed) != false)
            {
                WriteAttribute(@"fixed", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsFixed)));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write27_XmlSchemaMaxInclusiveFacet(string n, string ns, Microsoft.Xml.Schema.XmlSchemaMaxInclusiveFacet o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaMaxInclusiveFacet))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaMaxInclusiveFacet", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"value", @"", ((global::System.String)o.@Value));
            if (((global::System.Boolean)o.@IsFixed) != false)
            {
                WriteAttribute(@"fixed", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsFixed)));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write21_XmlSchemaMinInclusiveFacet(string n, string ns, Microsoft.Xml.Schema.XmlSchemaMinInclusiveFacet o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaMinInclusiveFacet))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaMinInclusiveFacet", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"value", @"", ((global::System.String)o.@Value));
            if (((global::System.Boolean)o.@IsFixed) != false)
            {
                WriteAttribute(@"fixed", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsFixed)));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write25_XmlSchemaPatternFacet(string n, string ns, Microsoft.Xml.Schema.XmlSchemaPatternFacet o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaPatternFacet))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaPatternFacet", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"value", @"", ((global::System.String)o.@Value));
            if (((global::System.Boolean)o.@IsFixed) != false)
            {
                WriteAttribute(@"fixed", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsFixed)));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write30_XmlSchemaMinExclusiveFacet(string n, string ns, Microsoft.Xml.Schema.XmlSchemaMinExclusiveFacet o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaMinExclusiveFacet))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaMinExclusiveFacet", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"value", @"", ((global::System.String)o.@Value));
            if (((global::System.Boolean)o.@IsFixed) != false)
            {
                WriteAttribute(@"fixed", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsFixed)));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write29_XmlSchemaWhiteSpaceFacet(string n, string ns, Microsoft.Xml.Schema.XmlSchemaWhiteSpaceFacet o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaWhiteSpaceFacet))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaWhiteSpaceFacet", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"value", @"", ((global::System.String)o.@Value));
            if (((global::System.Boolean)o.@IsFixed) != false)
            {
                WriteAttribute(@"fixed", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsFixed)));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write28_XmlSchemaMaxExclusiveFacet(string n, string ns, Microsoft.Xml.Schema.XmlSchemaMaxExclusiveFacet o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaMaxExclusiveFacet))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaMaxExclusiveFacet", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"value", @"", ((global::System.String)o.@Value));
            if (((global::System.Boolean)o.@IsFixed) != false)
            {
                WriteAttribute(@"fixed", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsFixed)));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write31_XmlSchemaMinLengthFacet(string n, string ns, Microsoft.Xml.Schema.XmlSchemaMinLengthFacet o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaMinLengthFacet))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaMinLengthFacet", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"value", @"", ((global::System.String)o.@Value));
            if (((global::System.Boolean)o.@IsFixed) != false)
            {
                WriteAttribute(@"fixed", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsFixed)));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write20_XmlSchemaFractionDigitsFacet(string n, string ns, Microsoft.Xml.Schema.XmlSchemaFractionDigitsFacet o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaFractionDigitsFacet))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaFractionDigitsFacet", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"value", @"", ((global::System.String)o.@Value));
            if (((global::System.Boolean)o.@IsFixed) != false)
            {
                WriteAttribute(@"fixed", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsFixed)));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write22_XmlSchemaMaxLengthFacet(string n, string ns, Microsoft.Xml.Schema.XmlSchemaMaxLengthFacet o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaMaxLengthFacet))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaMaxLengthFacet", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"value", @"", ((global::System.String)o.@Value));
            if (((global::System.Boolean)o.@IsFixed) != false)
            {
                WriteAttribute(@"fixed", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsFixed)));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write24_XmlSchemaTotalDigitsFacet(string n, string ns, Microsoft.Xml.Schema.XmlSchemaTotalDigitsFacet o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaTotalDigitsFacet))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaTotalDigitsFacet", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"value", @"", ((global::System.String)o.@Value));
            if (((global::System.Boolean)o.@IsFixed) != false)
            {
                WriteAttribute(@"fixed", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsFixed)));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write23_XmlSchemaLengthFacet(string n, string ns, Microsoft.Xml.Schema.XmlSchemaLengthFacet o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaLengthFacet))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaLengthFacet", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"value", @"", ((global::System.String)o.@Value));
            if (((global::System.Boolean)o.@IsFixed) != false)
            {
                WriteAttribute(@"fixed", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsFixed)));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write33_XmlSchemaSimpleTypeUnion(string n, string ns, Microsoft.Xml.Schema.XmlSchemaSimpleTypeUnion o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaSimpleTypeUnion))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaSimpleTypeUnion", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            {
                Microsoft.Xml.XmlQualifiedName[] a = (Microsoft.Xml.XmlQualifiedName[])o.@MemberTypes;
                if (a != null)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlQualifiedName ai = (Microsoft.Xml.XmlQualifiedName)a[i];
                        if (i != 0) sb.Append(" ");
                        sb.Append(FromXmlQualifiedName(ai));
                    }
                    if (sb.Length != 0)
                    {
                        WriteAttribute(@"memberTypes", @"", sb.ToString());
                    }
                }
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@BaseTypes;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write34_XmlSchemaSimpleType(@"simpleType", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSimpleType)a[ia]), false, false);
                    }
                }
            }
            WriteEndElement(o);
        }

        private string Write7_XmlSchemaDerivationMethod(Microsoft.Xml.Schema.XmlSchemaDerivationMethod v)
        {
            string s = null;
            switch (v)
            {
                case Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@Empty: s = @""; break;
                case Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@Substitution: s = @"substitution"; break;
                case Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@Extension: s = @"extension"; break;
                case Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@Restriction: s = @"restriction"; break;
                case Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@List: s = @"list"; break;
                case Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@Union: s = @"union"; break;
                case Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@All: s = @"#all"; break;
                default:
                    s = FromEnum(((System.Int64)v), new string[] { @"",
                    @"substitution",
                    @"extension",
                    @"restriction",
                    @"list",
                    @"union",
                    @"#all" }, new System.Int64[] { (long)Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@Empty,
                    (long)Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@Substitution,
                    (long)Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@Extension,
                    (long)Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@Restriction,
                    (long)Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@List,
                    (long)Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@Union,
                    (long)Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@All }, @"System.Xml.Schema.XmlSchemaDerivationMethod"); break;
            }
            return s;
        }

        private void Write62_XmlSchemaComplexType(string n, string ns, Microsoft.Xml.Schema.XmlSchemaComplexType o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaComplexType))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaComplexType", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            if (((Microsoft.Xml.Schema.XmlSchemaDerivationMethod)o.@Final) != (Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@None))
            {
                WriteAttribute(@"final", @"", Write7_XmlSchemaDerivationMethod(((Microsoft.Xml.Schema.XmlSchemaDerivationMethod)o.@Final)));
            }
            if (((global::System.Boolean)o.@IsAbstract) != false)
            {
                WriteAttribute(@"abstract", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsAbstract)));
            }
            if (((Microsoft.Xml.Schema.XmlSchemaDerivationMethod)o.@Block) != (Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@None))
            {
                WriteAttribute(@"block", @"", Write7_XmlSchemaDerivationMethod(((Microsoft.Xml.Schema.XmlSchemaDerivationMethod)o.@Block)));
            }
            if (((global::System.Boolean)o.@IsMixed) != false)
            {
                WriteAttribute(@"mixed", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsMixed)));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            {
                if (o.@ContentModel is Microsoft.Xml.Schema.XmlSchemaSimpleContent)
                {
                    Write61_XmlSchemaSimpleContent(@"simpleContent", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSimpleContent)o.@ContentModel), false, false);
                }
                else if (o.@ContentModel is Microsoft.Xml.Schema.XmlSchemaComplexContent)
                {
                    Write58_XmlSchemaComplexContent(@"complexContent", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaComplexContent)o.@ContentModel), false, false);
                }
                else
                {
                    if (o.@ContentModel != null)
                    {
                        throw CreateUnknownTypeException(o.@ContentModel);
                    }
                }
            }
            {
                if (o.@Particle is Microsoft.Xml.Schema.XmlSchemaChoice)
                {
                    Write54_XmlSchemaChoice(@"choice", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaChoice)o.@Particle), false, false);
                }
                else if (o.@Particle is Microsoft.Xml.Schema.XmlSchemaAll)
                {
                    Write55_XmlSchemaAll(@"all", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAll)o.@Particle), false, false);
                }
                else if (o.@Particle is Microsoft.Xml.Schema.XmlSchemaSequence)
                {
                    Write53_XmlSchemaSequence(@"sequence", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSequence)o.@Particle), false, false);
                }
                else if (o.@Particle is Microsoft.Xml.Schema.XmlSchemaGroupRef)
                {
                    Write44_XmlSchemaGroupRef(@"group", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaGroupRef)o.@Particle), false, false);
                }
                else
                {
                    if (o.@Particle != null)
                    {
                        throw CreateUnknownTypeException(o.@Particle);
                    }
                }
            }
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Attributes;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Microsoft.Xml.Schema.XmlSchemaObject ai = (Microsoft.Xml.Schema.XmlSchemaObject)a[ia];
                        {
                            if (ai is Microsoft.Xml.Schema.XmlSchemaAttributeGroupRef)
                            {
                                Write37_XmlSchemaAttributeGroupRef(@"attributeGroup", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAttributeGroupRef)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaAttribute)
                            {
                                Write36_XmlSchemaAttribute(@"attribute", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAttribute)ai), false, false);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            Write39_XmlSchemaAnyAttribute(@"anyAttribute", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnyAttribute)o.@AnyAttribute), false, false);
            WriteEndElement(o);
        }

        private void Write39_XmlSchemaAnyAttribute(string n, string ns, Microsoft.Xml.Schema.XmlSchemaAnyAttribute o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaAnyAttribute))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaAnyAttribute", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"namespace", @"", ((global::System.String)o.@Namespace));
            if (((Microsoft.Xml.Schema.XmlSchemaContentProcessing)o.@ProcessContents) != Microsoft.Xml.Schema.XmlSchemaContentProcessing.@None)
            {
                WriteAttribute(@"processContents", @"", Write38_XmlSchemaContentProcessing(((Microsoft.Xml.Schema.XmlSchemaContentProcessing)o.@ProcessContents)));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write36_XmlSchemaAttribute(string n, string ns, Microsoft.Xml.Schema.XmlSchemaAttribute o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaAttribute))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaAttribute", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"default", @"", ((global::System.String)o.@DefaultValue));
            WriteAttribute(@"fixed", @"", ((global::System.String)o.@FixedValue));
            if (((Microsoft.Xml.Schema.XmlSchemaForm)o.@Form) != Microsoft.Xml.Schema.XmlSchemaForm.@None)
            {
                WriteAttribute(@"form", @"", Write6_XmlSchemaForm(((Microsoft.Xml.Schema.XmlSchemaForm)o.@Form)));
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            WriteAttribute(@"ref", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@RefName)));
            WriteAttribute(@"type", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@SchemaTypeName)));
            if (((Microsoft.Xml.Schema.XmlSchemaUse)o.@Use) != Microsoft.Xml.Schema.XmlSchemaUse.@None)
            {
                WriteAttribute(@"use", @"", Write35_XmlSchemaUse(((Microsoft.Xml.Schema.XmlSchemaUse)o.@Use)));
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            Write34_XmlSchemaSimpleType(@"simpleType", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSimpleType)o.@SchemaType), false, false);
            WriteEndElement(o);
        }

        private string Write35_XmlSchemaUse(Microsoft.Xml.Schema.XmlSchemaUse v)
        {
            string s = null;
            switch (v)
            {
                case Microsoft.Xml.Schema.XmlSchemaUse.@Optional: s = @"optional"; break;
                case Microsoft.Xml.Schema.XmlSchemaUse.@Prohibited: s = @"prohibited"; break;
                case Microsoft.Xml.Schema.XmlSchemaUse.@Required: s = @"required"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"System.Xml.Schema.XmlSchemaUse");
            }
            return s;
        }

        private string Write6_XmlSchemaForm(Microsoft.Xml.Schema.XmlSchemaForm v)
        {
            string s = null;
            switch (v)
            {
                case Microsoft.Xml.Schema.XmlSchemaForm.@Qualified: s = @"qualified"; break;
                case Microsoft.Xml.Schema.XmlSchemaForm.@Unqualified: s = @"unqualified"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"System.Xml.Schema.XmlSchemaForm");
            }
            return s;
        }

        private void Write37_XmlSchemaAttributeGroupRef(string n, string ns, Microsoft.Xml.Schema.XmlSchemaAttributeGroupRef o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaAttributeGroupRef))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaAttributeGroupRef", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"ref", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@RefName)));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write44_XmlSchemaGroupRef(string n, string ns, Microsoft.Xml.Schema.XmlSchemaGroupRef o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaGroupRef))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaGroupRef", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"minOccurs", @"", ((global::System.String)o.@MinOccursString));
            WriteAttribute(@"maxOccurs", @"", ((global::System.String)o.@MaxOccursString));
            WriteAttribute(@"ref", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@RefName)));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write55_XmlSchemaAll(string n, string ns, Microsoft.Xml.Schema.XmlSchemaAll o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaAll))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaAll", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"minOccurs", @"", ((global::System.String)o.@MinOccursString));
            WriteAttribute(@"maxOccurs", @"", ((global::System.String)o.@MaxOccursString));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Items;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Write52_XmlSchemaElement(@"element", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaElement)a[ia]), false, false);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write54_XmlSchemaChoice(string n, string ns, Microsoft.Xml.Schema.XmlSchemaChoice o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaChoice))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaChoice", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"minOccurs", @"", ((global::System.String)o.@MinOccursString));
            WriteAttribute(@"maxOccurs", @"", ((global::System.String)o.@MaxOccursString));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Items;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Microsoft.Xml.Schema.XmlSchemaObject ai = (Microsoft.Xml.Schema.XmlSchemaObject)a[ia];
                        {
                            if (ai is Microsoft.Xml.Schema.XmlSchemaSequence)
                            {
                                Write53_XmlSchemaSequence(@"sequence", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSequence)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaChoice)
                            {
                                Write54_XmlSchemaChoice(@"choice", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaChoice)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaGroupRef)
                            {
                                Write44_XmlSchemaGroupRef(@"group", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaGroupRef)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaElement)
                            {
                                Write52_XmlSchemaElement(@"element", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaElement)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaAny)
                            {
                                Write46_XmlSchemaAny(@"any", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAny)ai), false, false);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write58_XmlSchemaComplexContent(string n, string ns, Microsoft.Xml.Schema.XmlSchemaComplexContent o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaComplexContent))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaComplexContent", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"mixed", @"", Microsoft.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsMixed)));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            {
                if (o.@Content is Microsoft.Xml.Schema.XmlSchemaComplexContentRestriction)
                {
                    Write57_Item(@"restriction", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaComplexContentRestriction)o.@Content), false, false);
                }
                else if (o.@Content is Microsoft.Xml.Schema.XmlSchemaComplexContentExtension)
                {
                    Write56_Item(@"extension", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaComplexContentExtension)o.@Content), false, false);
                }
                else
                {
                    if (o.@Content != null)
                    {
                        throw CreateUnknownTypeException(o.@Content);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write56_Item(string n, string ns, Microsoft.Xml.Schema.XmlSchemaComplexContentExtension o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaComplexContentExtension))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaComplexContentExtension", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"base", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@BaseTypeName)));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            {
                if (o.@Particle is Microsoft.Xml.Schema.XmlSchemaAll)
                {
                    Write55_XmlSchemaAll(@"all", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAll)o.@Particle), false, false);
                }
                else if (o.@Particle is Microsoft.Xml.Schema.XmlSchemaSequence)
                {
                    Write53_XmlSchemaSequence(@"sequence", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSequence)o.@Particle), false, false);
                }
                else if (o.@Particle is Microsoft.Xml.Schema.XmlSchemaChoice)
                {
                    Write54_XmlSchemaChoice(@"choice", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaChoice)o.@Particle), false, false);
                }
                else if (o.@Particle is Microsoft.Xml.Schema.XmlSchemaGroupRef)
                {
                    Write44_XmlSchemaGroupRef(@"group", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaGroupRef)o.@Particle), false, false);
                }
                else
                {
                    if (o.@Particle != null)
                    {
                        throw CreateUnknownTypeException(o.@Particle);
                    }
                }
            }
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Attributes;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Microsoft.Xml.Schema.XmlSchemaObject ai = (Microsoft.Xml.Schema.XmlSchemaObject)a[ia];
                        {
                            if (ai is Microsoft.Xml.Schema.XmlSchemaAttribute)
                            {
                                Write36_XmlSchemaAttribute(@"attribute", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAttribute)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaAttributeGroupRef)
                            {
                                Write37_XmlSchemaAttributeGroupRef(@"attributeGroup", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAttributeGroupRef)ai), false, false);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            Write39_XmlSchemaAnyAttribute(@"anyAttribute", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnyAttribute)o.@AnyAttribute), false, false);
            WriteEndElement(o);
        }

        private void Write57_Item(string n, string ns, Microsoft.Xml.Schema.XmlSchemaComplexContentRestriction o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaComplexContentRestriction))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaComplexContentRestriction", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"base", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@BaseTypeName)));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            {
                if (o.@Particle is Microsoft.Xml.Schema.XmlSchemaAll)
                {
                    Write55_XmlSchemaAll(@"all", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAll)o.@Particle), false, false);
                }
                else if (o.@Particle is Microsoft.Xml.Schema.XmlSchemaSequence)
                {
                    Write53_XmlSchemaSequence(@"sequence", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSequence)o.@Particle), false, false);
                }
                else if (o.@Particle is Microsoft.Xml.Schema.XmlSchemaChoice)
                {
                    Write54_XmlSchemaChoice(@"choice", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaChoice)o.@Particle), false, false);
                }
                else if (o.@Particle is Microsoft.Xml.Schema.XmlSchemaGroupRef)
                {
                    Write44_XmlSchemaGroupRef(@"group", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaGroupRef)o.@Particle), false, false);
                }
                else
                {
                    if (o.@Particle != null)
                    {
                        throw CreateUnknownTypeException(o.@Particle);
                    }
                }
            }
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Attributes;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Microsoft.Xml.Schema.XmlSchemaObject ai = (Microsoft.Xml.Schema.XmlSchemaObject)a[ia];
                        {
                            if (ai is Microsoft.Xml.Schema.XmlSchemaAttribute)
                            {
                                Write36_XmlSchemaAttribute(@"attribute", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAttribute)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaAttributeGroupRef)
                            {
                                Write37_XmlSchemaAttributeGroupRef(@"attributeGroup", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAttributeGroupRef)ai), false, false);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            Write39_XmlSchemaAnyAttribute(@"anyAttribute", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnyAttribute)o.@AnyAttribute), false, false);
            WriteEndElement(o);
        }

        private void Write61_XmlSchemaSimpleContent(string n, string ns, Microsoft.Xml.Schema.XmlSchemaSimpleContent o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaSimpleContent))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaSimpleContent", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            {
                if (o.@Content is Microsoft.Xml.Schema.XmlSchemaSimpleContentExtension)
                {
                    Write60_Item(@"extension", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSimpleContentExtension)o.@Content), false, false);
                }
                else if (o.@Content is Microsoft.Xml.Schema.XmlSchemaSimpleContentRestriction)
                {
                    Write59_Item(@"restriction", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSimpleContentRestriction)o.@Content), false, false);
                }
                else
                {
                    if (o.@Content != null)
                    {
                        throw CreateUnknownTypeException(o.@Content);
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write59_Item(string n, string ns, Microsoft.Xml.Schema.XmlSchemaSimpleContentRestriction o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaSimpleContentRestriction))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaSimpleContentRestriction", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"base", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@BaseTypeName)));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            Write34_XmlSchemaSimpleType(@"simpleType", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSimpleType)o.@BaseType), false, false);
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Facets;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Microsoft.Xml.Schema.XmlSchemaObject ai = (Microsoft.Xml.Schema.XmlSchemaObject)a[ia];
                        {
                            if (ai is Microsoft.Xml.Schema.XmlSchemaMinLengthFacet)
                            {
                                Write31_XmlSchemaMinLengthFacet(@"minLength", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaMinLengthFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaMaxLengthFacet)
                            {
                                Write22_XmlSchemaMaxLengthFacet(@"maxLength", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaMaxLengthFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaLengthFacet)
                            {
                                Write23_XmlSchemaLengthFacet(@"length", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaLengthFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaFractionDigitsFacet)
                            {
                                Write20_XmlSchemaFractionDigitsFacet(@"fractionDigits", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaFractionDigitsFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaTotalDigitsFacet)
                            {
                                Write24_XmlSchemaTotalDigitsFacet(@"totalDigits", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaTotalDigitsFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaMinExclusiveFacet)
                            {
                                Write30_XmlSchemaMinExclusiveFacet(@"minExclusive", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaMinExclusiveFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaMaxInclusiveFacet)
                            {
                                Write27_XmlSchemaMaxInclusiveFacet(@"maxInclusive", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaMaxInclusiveFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaMaxExclusiveFacet)
                            {
                                Write28_XmlSchemaMaxExclusiveFacet(@"maxExclusive", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaMaxExclusiveFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaMinInclusiveFacet)
                            {
                                Write21_XmlSchemaMinInclusiveFacet(@"minInclusive", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaMinInclusiveFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaWhiteSpaceFacet)
                            {
                                Write29_XmlSchemaWhiteSpaceFacet(@"whiteSpace", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaWhiteSpaceFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaEnumerationFacet)
                            {
                                Write26_XmlSchemaEnumerationFacet(@"enumeration", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaEnumerationFacet)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaPatternFacet)
                            {
                                Write25_XmlSchemaPatternFacet(@"pattern", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaPatternFacet)ai), false, false);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Attributes;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Microsoft.Xml.Schema.XmlSchemaObject ai = (Microsoft.Xml.Schema.XmlSchemaObject)a[ia];
                        {
                            if (ai is Microsoft.Xml.Schema.XmlSchemaAttribute)
                            {
                                Write36_XmlSchemaAttribute(@"attribute", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAttribute)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaAttributeGroupRef)
                            {
                                Write37_XmlSchemaAttributeGroupRef(@"attributeGroup", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAttributeGroupRef)ai), false, false);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            Write39_XmlSchemaAnyAttribute(@"anyAttribute", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnyAttribute)o.@AnyAttribute), false, false);
            WriteEndElement(o);
        }

        private void Write60_Item(string n, string ns, Microsoft.Xml.Schema.XmlSchemaSimpleContentExtension o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaSimpleContentExtension))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaSimpleContentExtension", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"base", @"", FromXmlQualifiedName(((Microsoft.Xml.XmlQualifiedName)o.@BaseTypeName)));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Attributes;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Microsoft.Xml.Schema.XmlSchemaObject ai = (Microsoft.Xml.Schema.XmlSchemaObject)a[ia];
                        {
                            if (ai is Microsoft.Xml.Schema.XmlSchemaAttribute)
                            {
                                Write36_XmlSchemaAttribute(@"attribute", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAttribute)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaAttributeGroupRef)
                            {
                                Write37_XmlSchemaAttributeGroupRef(@"attributeGroup", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAttributeGroupRef)ai), false, false);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            Write39_XmlSchemaAnyAttribute(@"anyAttribute", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnyAttribute)o.@AnyAttribute), false, false);
            WriteEndElement(o);
        }

        private void Write65_XmlSchemaNotation(string n, string ns, Microsoft.Xml.Schema.XmlSchemaNotation o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaNotation))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaNotation", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            WriteAttribute(@"public", @"", ((global::System.String)o.@Public));
            WriteAttribute(@"system", @"", ((global::System.String)o.@System));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write40_XmlSchemaAttributeGroup(string n, string ns, Microsoft.Xml.Schema.XmlSchemaAttributeGroup o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaAttributeGroup))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaAttributeGroup", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"name", @"", ((global::System.String)o.@Name));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Attributes;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Microsoft.Xml.Schema.XmlSchemaObject ai = (Microsoft.Xml.Schema.XmlSchemaObject)a[ia];
                        {
                            if (ai is Microsoft.Xml.Schema.XmlSchemaAttributeGroupRef)
                            {
                                Write37_XmlSchemaAttributeGroupRef(@"attributeGroup", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAttributeGroupRef)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaAttribute)
                            {
                                Write36_XmlSchemaAttribute(@"attribute", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAttribute)ai), false, false);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            Write39_XmlSchemaAnyAttribute(@"anyAttribute", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnyAttribute)o.@AnyAttribute), false, false);
            WriteEndElement(o);
        }

        private void Write12_XmlSchemaInclude(string n, string ns, Microsoft.Xml.Schema.XmlSchemaInclude o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaInclude))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaInclude", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"schemaLocation", @"", ((global::System.String)o.@SchemaLocation));
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write13_XmlSchemaImport(string n, string ns, Microsoft.Xml.Schema.XmlSchemaImport o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaImport))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaImport", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"schemaLocation", @"", ((global::System.String)o.@SchemaLocation));
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"namespace", @"", ((global::System.String)o.@Namespace));
            Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)o.@Annotation), false, false);
            WriteEndElement(o);
        }

        private void Write64_XmlSchemaRedefine(string n, string ns, Microsoft.Xml.Schema.XmlSchemaRedefine o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(Microsoft.Xml.Schema.XmlSchemaRedefine))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            EscapeName = false;
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"XmlSchemaRedefine", @"http://www.w3.org/2001/XMLSchema");
            WriteAttribute(@"schemaLocation", @"", ((global::System.String)o.@SchemaLocation));
            WriteAttribute(@"id", @"", ((global::System.String)o.@Id));
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@UnhandledAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            {
                Microsoft.Xml.Schema.XmlSchemaObjectCollection a = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Items;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        Microsoft.Xml.Schema.XmlSchemaObject ai = (Microsoft.Xml.Schema.XmlSchemaObject)a[ia];
                        {
                            if (ai is Microsoft.Xml.Schema.XmlSchemaSimpleType)
                            {
                                Write34_XmlSchemaSimpleType(@"simpleType", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaSimpleType)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaComplexType)
                            {
                                Write62_XmlSchemaComplexType(@"complexType", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaComplexType)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaGroup)
                            {
                                Write63_XmlSchemaGroup(@"group", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaGroup)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaAttributeGroup)
                            {
                                Write40_XmlSchemaAttributeGroup(@"attributeGroup", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAttributeGroup)ai), false, false);
                            }
                            else if (ai is Microsoft.Xml.Schema.XmlSchemaAnnotation)
                            {
                                Write11_XmlSchemaAnnotation(@"annotation", @"http://www.w3.org/2001/XMLSchema", ((Microsoft.Xml.Schema.XmlSchemaAnnotation)ai), false, false);
                            }
                            else
                            {
                                if (ai != null)
                                {
                                    throw CreateUnknownTypeException(ai);
                                }
                            }
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        private void Write4_Import(string n, string ns, global::System.Web.Services.Description.Import o, bool isNullable, bool needType)
        {
            if ((object)o == null)
            {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType)
            {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Web.Services.Description.Import))
                {
                }
                else
                {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, o.@Namespaces);
            if (needType) WriteXsiType(@"Import", @"http://schemas.xmlsoap.org/wsdl/");
            {
                Microsoft.Xml.XmlAttribute[] a = (Microsoft.Xml.XmlAttribute[])o.@ExtensibleAttributes;
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        Microsoft.Xml.XmlAttribute ai = (Microsoft.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteAttribute(@"namespace", @"", ((global::System.String)o.@Namespace));
            WriteAttribute(@"location", @"", ((global::System.String)o.@Location));
            if ((o.@DocumentationElement) is Microsoft.Xml.XmlNode || o.@DocumentationElement == null)
            {
                WriteElementLiteral((Microsoft.Xml.XmlNode)o.@DocumentationElement, @"documentation", @"http://schemas.xmlsoap.org/wsdl/", false, true);
            }
            else
            {
                throw CreateInvalidAnyTypeException(o.@DocumentationElement);
            }
            {
                global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
                if (a != null)
                {
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++)
                    {
                        if ((a[ia]) is Microsoft.Xml.XmlNode || a[ia] == null)
                        {
                            WriteElementLiteral((Microsoft.Xml.XmlNode)a[ia], @"", null, false, true);
                        }
                        else
                        {
                            throw CreateInvalidAnyTypeException(a[ia]);
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        protected override void InitCallbacks()
        {
        }
    }
    internal class ServiceDescriptionSerializationReader : Microsoft.Xml.Serialization.XmlSerializationReader
    {
        public object Read125_definitions()
        {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
            {
                if (((object)Reader.LocalName == (object)_id1_definitions && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o = Read124_ServiceDescription(true, true);
                }
                else
                {
                    throw CreateUnknownNodeException();
                }
            }
            else
            {
                UnknownNode(null, @"http://schemas.xmlsoap.org/wsdl/:definitions");
            }
            return (object)o;
        }

        private global::System.Web.Services.Description.ServiceDescription Read124_ServiceDescription(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id3_ServiceDescription && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.ServiceDescription o;
            o = new global::System.Web.Services.Description.ServiceDescription();
            Microsoft.Xml.XmlAttribute[] a_1 = null;
            int ca_1 = 0;
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_4 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            global::System.Web.Services.Description.ImportCollection a_5 = (global::System.Web.Services.Description.ImportCollection)o.@Imports;
            global::System.Web.Services.Description.MessageCollection a_7 = (global::System.Web.Services.Description.MessageCollection)o.@Messages;
            global::System.Web.Services.Description.PortTypeCollection a_8 = (global::System.Web.Services.Description.PortTypeCollection)o.@PortTypes;
            global::System.Web.Services.Description.BindingCollection a_9 = (global::System.Web.Services.Description.BindingCollection)o.@Bindings;
            global::System.Web.Services.Description.ServiceCollection a_10 = (global::System.Web.Services.Description.ServiceCollection)o.@Services;
            bool[] paramsRead = new bool[12];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (!paramsRead[11] && ((object)Reader.LocalName == (object)_id6_targetNamespace && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@TargetNamespace = Reader.Value;
                    paramsRead[11] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute)); a_1[ca_1++] = attr;
                }
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations0 = 0;
            int readerCount0 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@DocumentationElement = (Microsoft.Xml.XmlElement)ReadXmlNode(false);
                        paramsRead[0] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id8_import && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        if ((object)(a_5) == null) Reader.Skip(); else a_5.Add(Read4_Import(false, true));
                    }
                    else if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id9_types && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@Types = Read67_Types(false, true);
                        paramsRead[6] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id10_message && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        if ((object)(a_7) == null) Reader.Skip(); else a_7.Add(Read69_Message(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id11_portType && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        if ((object)(a_8) == null) Reader.Skip(); else a_8.Add(Read75_PortType(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id12_binding && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        if ((object)(a_9) == null) Reader.Skip(); else a_9.Add(Read117_Binding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id13_service && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        if ((object)(a_10) == null) Reader.Skip(); else a_10.Add(Read123_Service(false, true));
                    }
                    else
                    {
                        a_4.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:documentation, http://schemas.xmlsoap.org/wsdl/:import, http://schemas.xmlsoap.org/wsdl/:types, http://schemas.xmlsoap.org/wsdl/:message, http://schemas.xmlsoap.org/wsdl/:portType, http://schemas.xmlsoap.org/wsdl/:binding, http://schemas.xmlsoap.org/wsdl/:service");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations0, ref readerCount0);
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.Service Read123_Service(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id14_Service && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.Service o;
            o = new global::System.Web.Services.Description.Service();
            Microsoft.Xml.XmlAttribute[] a_1 = null;
            int ca_1 = 0;
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_4 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            global::System.Web.Services.Description.PortCollection a_5 = (global::System.Web.Services.Description.PortCollection)o.@Ports;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute)); a_1[ca_1++] = attr;
                }
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations1 = 0;
            int readerCount1 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@DocumentationElement = (Microsoft.Xml.XmlElement)ReadXmlNode(false);
                        paramsRead[0] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id15_port && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        if ((object)(a_5) == null) Reader.Skip(); else a_5.Add(Read122_Port(false, true));
                    }
                    else
                    {
                        a_4.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:documentation, http://schemas.xmlsoap.org/wsdl/:port");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations1, ref readerCount1);
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.Port Read122_Port(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id16_Port && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.Port o;
            o = new global::System.Web.Services.Description.Port();
            Microsoft.Xml.XmlAttribute[] a_1 = null;
            int ca_1 = 0;
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_4 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id12_binding && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Binding = ToXmlQualifiedName(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute)); a_1[ca_1++] = attr;
                }
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations2 = 0;
            int readerCount2 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@DocumentationElement = (Microsoft.Xml.XmlElement)ReadXmlNode(false);
                        paramsRead[0] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id17_address && (object)Reader.NamespaceURI == (object)_id18_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read118_HttpAddressBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id17_address && (object)Reader.NamespaceURI == (object)_id19_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read119_SoapAddressBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id17_address && (object)Reader.NamespaceURI == (object)_id20_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read121_Soap12AddressBinding(false, true));
                    }
                    else
                    {
                        a_4.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:documentation, http://schemas.xmlsoap.org/wsdl/http/:address, http://schemas.xmlsoap.org/wsdl/soap/:address, http://schemas.xmlsoap.org/wsdl/soap12/:address");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations2, ref readerCount2);
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.Soap12AddressBinding Read121_Soap12AddressBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id21_Soap12AddressBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id20_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.Soap12AddressBinding o;
            o = new global::System.Web.Services.Description.Soap12AddressBinding();
            bool[] paramsRead = new bool[2];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id23_location && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Location = Reader.Value;
                    paramsRead[1] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :location");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations3 = 0;
            int readerCount3 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations3, ref readerCount3);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.SoapAddressBinding Read119_SoapAddressBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id24_SoapAddressBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id19_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.SoapAddressBinding o;
            o = new global::System.Web.Services.Description.SoapAddressBinding();
            bool[] paramsRead = new bool[2];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id23_location && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Location = Reader.Value;
                    paramsRead[1] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :location");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations4 = 0;
            int readerCount4 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations4, ref readerCount4);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.HttpAddressBinding Read118_HttpAddressBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id25_HttpAddressBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id18_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.HttpAddressBinding o;
            o = new global::System.Web.Services.Description.HttpAddressBinding();
            bool[] paramsRead = new bool[2];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id23_location && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Location = Reader.Value;
                    paramsRead[1] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :location");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations5 = 0;
            int readerCount5 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations5, ref readerCount5);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.Binding Read117_Binding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id26_Binding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.Binding o;
            o = new global::System.Web.Services.Description.Binding();
            Microsoft.Xml.XmlAttribute[] a_1 = null;
            int ca_1 = 0;
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_4 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            global::System.Web.Services.Description.OperationBindingCollection a_5 = (global::System.Web.Services.Description.OperationBindingCollection)o.@Operations;
            bool[] paramsRead = new bool[7];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id27_type && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Type = ToXmlQualifiedName(Reader.Value);
                    paramsRead[6] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute)); a_1[ca_1++] = attr;
                }
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations6 = 0;
            int readerCount6 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@DocumentationElement = (Microsoft.Xml.XmlElement)ReadXmlNode(false);
                        paramsRead[0] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id12_binding && (object)Reader.NamespaceURI == (object)_id18_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read77_HttpBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id12_binding && (object)Reader.NamespaceURI == (object)_id19_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read80_SoapBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id12_binding && (object)Reader.NamespaceURI == (object)_id20_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read84_Soap12Binding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id28_operation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        if ((object)(a_5) == null) Reader.Skip(); else a_5.Add(Read116_OperationBinding(false, true));
                    }
                    else
                    {
                        a_4.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:documentation, http://schemas.xmlsoap.org/wsdl/http/:binding, http://schemas.xmlsoap.org/wsdl/soap/:binding, http://schemas.xmlsoap.org/wsdl/soap12/:binding, http://schemas.xmlsoap.org/wsdl/:operation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations6, ref readerCount6);
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.OperationBinding Read116_OperationBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id29_OperationBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.OperationBinding o;
            o = new global::System.Web.Services.Description.OperationBinding();
            Microsoft.Xml.XmlAttribute[] a_1 = null;
            int ca_1 = 0;
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_4 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            global::System.Web.Services.Description.FaultBindingCollection a_7 = (global::System.Web.Services.Description.FaultBindingCollection)o.@Faults;
            bool[] paramsRead = new bool[8];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute)); a_1[ca_1++] = attr;
                }
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations7 = 0;
            int readerCount7 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@DocumentationElement = (Microsoft.Xml.XmlElement)ReadXmlNode(false);
                        paramsRead[0] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id28_operation && (object)Reader.NamespaceURI == (object)_id18_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read85_HttpOperationBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id28_operation && (object)Reader.NamespaceURI == (object)_id19_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read86_SoapOperationBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id28_operation && (object)Reader.NamespaceURI == (object)_id20_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read88_Soap12OperationBinding(false, true));
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id30_input && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@Input = Read110_InputBinding(false, true);
                        paramsRead[5] = true;
                    }
                    else if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id31_output && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@Output = Read111_OutputBinding(false, true);
                        paramsRead[6] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id32_fault && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        if ((object)(a_7) == null) Reader.Skip(); else a_7.Add(Read115_FaultBinding(false, true));
                    }
                    else
                    {
                        a_4.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:documentation, http://schemas.xmlsoap.org/wsdl/http/:operation, http://schemas.xmlsoap.org/wsdl/soap/:operation, http://schemas.xmlsoap.org/wsdl/soap12/:operation, http://schemas.xmlsoap.org/wsdl/:input, http://schemas.xmlsoap.org/wsdl/:output, http://schemas.xmlsoap.org/wsdl/:fault");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations7, ref readerCount7);
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.FaultBinding Read115_FaultBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id33_FaultBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.FaultBinding o;
            o = new global::System.Web.Services.Description.FaultBinding();
            Microsoft.Xml.XmlAttribute[] a_1 = null;
            int ca_1 = 0;
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_4 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            bool[] paramsRead = new bool[5];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute)); a_1[ca_1++] = attr;
                }
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations8 = 0;
            int readerCount8 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@DocumentationElement = (Microsoft.Xml.XmlElement)ReadXmlNode(false);
                        paramsRead[0] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id32_fault && (object)Reader.NamespaceURI == (object)_id19_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read112_SoapFaultBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id32_fault && (object)Reader.NamespaceURI == (object)_id20_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read114_Soap12FaultBinding(false, true));
                    }
                    else
                    {
                        a_4.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:documentation, http://schemas.xmlsoap.org/wsdl/soap/:fault, http://schemas.xmlsoap.org/wsdl/soap12/:fault");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations8, ref readerCount8);
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.Soap12FaultBinding Read114_Soap12FaultBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id34_Soap12FaultBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id20_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.Soap12FaultBinding o;
            o = new global::System.Web.Services.Description.Soap12FaultBinding();
            bool[] paramsRead = new bool[5];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id35_use && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Use = Read100_SoapBindingUse(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[2] = true;
                }
                else if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id36_namespace && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Namespace = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id37_encodingStyle && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Encoding = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :use, :name, :namespace, :encodingStyle");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations9 = 0;
            int readerCount9 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations9, ref readerCount9);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.SoapBindingUse Read100_SoapBindingUse(string s)
        {
            switch (s)
            {
                case @"encoded": return global::System.Web.Services.Description.SoapBindingUse.@Encoded;
                case @"literal": return global::System.Web.Services.Description.SoapBindingUse.@Literal;
                default: throw CreateUnknownConstantException(s, typeof(global::System.Web.Services.Description.SoapBindingUse));
            }
        }

        private global::System.Web.Services.Description.SoapFaultBinding Read112_SoapFaultBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id38_SoapFaultBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id19_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.SoapFaultBinding o;
            o = new global::System.Web.Services.Description.SoapFaultBinding();
            bool[] paramsRead = new bool[5];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id35_use && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Use = Read98_SoapBindingUse(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[2] = true;
                }
                else if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id36_namespace && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Namespace = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id37_encodingStyle && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Encoding = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :use, :name, :namespace, :encodingStyle");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations10 = 0;
            int readerCount10 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations10, ref readerCount10);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.SoapBindingUse Read98_SoapBindingUse(string s)
        {
            switch (s)
            {
                case @"encoded": return global::System.Web.Services.Description.SoapBindingUse.@Encoded;
                case @"literal": return global::System.Web.Services.Description.SoapBindingUse.@Literal;
                default: throw CreateUnknownConstantException(s, typeof(global::System.Web.Services.Description.SoapBindingUse));
            }
        }

        private global::System.Web.Services.Description.OutputBinding Read111_OutputBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id39_OutputBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.OutputBinding o;
            o = new global::System.Web.Services.Description.OutputBinding();
            Microsoft.Xml.XmlAttribute[] a_1 = null;
            int ca_1 = 0;
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_4 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            bool[] paramsRead = new bool[5];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute)); a_1[ca_1++] = attr;
                }
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations11 = 0;
            int readerCount11 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@DocumentationElement = (Microsoft.Xml.XmlElement)ReadXmlNode(false);
                        paramsRead[0] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id40_content && (object)Reader.NamespaceURI == (object)_id41_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read93_MimeContentBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id42_mimeXml && (object)Reader.NamespaceURI == (object)_id41_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read94_MimeXmlBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id43_multipartRelated && (object)Reader.NamespaceURI == (object)_id41_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read104_MimeMultipartRelatedBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id44_text && (object)Reader.NamespaceURI == (object)_id45_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read97_MimeTextBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id46_body && (object)Reader.NamespaceURI == (object)_id19_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read99_SoapBodyBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id47_header && (object)Reader.NamespaceURI == (object)_id19_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read106_SoapHeaderBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id46_body && (object)Reader.NamespaceURI == (object)_id20_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read102_Soap12BodyBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id47_header && (object)Reader.NamespaceURI == (object)_id20_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read109_Soap12HeaderBinding(false, true));
                    }
                    else
                    {
                        a_4.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:documentation, http://schemas.xmlsoap.org/wsdl/mime/:content, http://schemas.xmlsoap.org/wsdl/mime/:mimeXml, http://schemas.xmlsoap.org/wsdl/mime/:multipartRelated, http://microsoft.com/wsdl/mime/textMatching/:text, http://schemas.xmlsoap.org/wsdl/soap/:body, http://schemas.xmlsoap.org/wsdl/soap/:header, http://schemas.xmlsoap.org/wsdl/soap12/:body, http://schemas.xmlsoap.org/wsdl/soap12/:header");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations11, ref readerCount11);
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.Soap12HeaderBinding Read109_Soap12HeaderBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id48_Soap12HeaderBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id20_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.Soap12HeaderBinding o;
            o = new global::System.Web.Services.Description.Soap12HeaderBinding();
            bool[] paramsRead = new bool[7];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id10_message && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Message = ToXmlQualifiedName(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id49_part && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Part = Reader.Value;
                    paramsRead[2] = true;
                }
                else if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id35_use && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Use = Read100_SoapBindingUse(Reader.Value);
                    paramsRead[3] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id37_encodingStyle && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Encoding = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id36_namespace && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Namespace = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :message, :part, :use, :encodingStyle, :namespace");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations12 = 0;
            int readerCount12 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id50_headerfault && (object)Reader.NamespaceURI == (object)_id20_Item))
                    {
                        o.@Fault = Read107_SoapHeaderFaultBinding(false, true);
                        paramsRead[6] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/soap12/:headerfault");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/soap12/:headerfault");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations12, ref readerCount12);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.SoapHeaderFaultBinding Read107_SoapHeaderFaultBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id51_SoapHeaderFaultBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id20_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.SoapHeaderFaultBinding o;
            o = new global::System.Web.Services.Description.SoapHeaderFaultBinding();
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id10_message && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Message = ToXmlQualifiedName(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id49_part && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Part = Reader.Value;
                    paramsRead[2] = true;
                }
                else if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id35_use && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Use = Read100_SoapBindingUse(Reader.Value);
                    paramsRead[3] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id37_encodingStyle && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Encoding = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id36_namespace && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Namespace = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :message, :part, :use, :encodingStyle, :namespace");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations13 = 0;
            int readerCount13 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations13, ref readerCount13);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.Soap12BodyBinding Read102_Soap12BodyBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id52_Soap12BodyBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id20_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.Soap12BodyBinding o;
            o = new global::System.Web.Services.Description.Soap12BodyBinding();
            bool[] paramsRead = new bool[5];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id35_use && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Use = Read100_SoapBindingUse(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id36_namespace && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Namespace = Reader.Value;
                    paramsRead[2] = true;
                }
                else if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id37_encodingStyle && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Encoding = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id53_parts && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@PartsString = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :use, :namespace, :encodingStyle, :parts");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations14 = 0;
            int readerCount14 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations14, ref readerCount14);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.SoapHeaderBinding Read106_SoapHeaderBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id54_SoapHeaderBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id19_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.SoapHeaderBinding o;
            o = new global::System.Web.Services.Description.SoapHeaderBinding();
            bool[] paramsRead = new bool[7];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id10_message && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Message = ToXmlQualifiedName(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id49_part && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Part = Reader.Value;
                    paramsRead[2] = true;
                }
                else if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id35_use && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Use = Read98_SoapBindingUse(Reader.Value);
                    paramsRead[3] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id37_encodingStyle && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Encoding = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id36_namespace && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Namespace = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :message, :part, :use, :encodingStyle, :namespace");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations15 = 0;
            int readerCount15 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id50_headerfault && (object)Reader.NamespaceURI == (object)_id19_Item))
                    {
                        o.@Fault = Read105_SoapHeaderFaultBinding(false, true);
                        paramsRead[6] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/soap/:headerfault");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/soap/:headerfault");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations15, ref readerCount15);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.SoapHeaderFaultBinding Read105_SoapHeaderFaultBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id51_SoapHeaderFaultBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id19_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.SoapHeaderFaultBinding o;
            o = new global::System.Web.Services.Description.SoapHeaderFaultBinding();
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id10_message && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Message = ToXmlQualifiedName(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id49_part && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Part = Reader.Value;
                    paramsRead[2] = true;
                }
                else if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id35_use && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Use = Read98_SoapBindingUse(Reader.Value);
                    paramsRead[3] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id37_encodingStyle && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Encoding = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id36_namespace && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Namespace = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :message, :part, :use, :encodingStyle, :namespace");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations16 = 0;
            int readerCount16 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations16, ref readerCount16);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.SoapBodyBinding Read99_SoapBodyBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id55_SoapBodyBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id19_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.SoapBodyBinding o;
            o = new global::System.Web.Services.Description.SoapBodyBinding();
            bool[] paramsRead = new bool[5];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id35_use && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Use = Read98_SoapBindingUse(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id36_namespace && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Namespace = Reader.Value;
                    paramsRead[2] = true;
                }
                else if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id37_encodingStyle && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Encoding = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id53_parts && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@PartsString = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :use, :namespace, :encodingStyle, :parts");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations17 = 0;
            int readerCount17 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations17, ref readerCount17);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.MimeTextBinding Read97_MimeTextBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id56_MimeTextBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id45_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.MimeTextBinding o;
            o = new global::System.Web.Services.Description.MimeTextBinding();
            global::System.Web.Services.Description.MimeTextMatchCollection a_1 = (global::System.Web.Services.Description.MimeTextMatchCollection)o.@Matches;
            bool[] paramsRead = new bool[2];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations18 = 0;
            int readerCount18 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (((object)Reader.LocalName == (object)_id57_match && (object)Reader.NamespaceURI == (object)_id45_Item))
                    {
                        if ((object)(a_1) == null) Reader.Skip(); else a_1.Add(Read96_MimeTextMatch(false, true));
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://microsoft.com/wsdl/mime/textMatching/:match");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://microsoft.com/wsdl/mime/textMatching/:match");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations18, ref readerCount18);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.MimeTextMatch Read96_MimeTextMatch(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id58_MimeTextMatch && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id45_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.MimeTextMatch o;
            o = new global::System.Web.Services.Description.MimeTextMatch();
            global::System.Web.Services.Description.MimeTextMatchCollection a_7 = (global::System.Web.Services.Description.MimeTextMatchCollection)o.@Matches;
            bool[] paramsRead = new bool[8];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id27_type && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Type = Reader.Value;
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id59_group && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Group = Microsoft.Xml.XmlConvert.ToInt32(Reader.Value);
                    paramsRead[2] = true;
                }
                else if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id60_capture && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Capture = Microsoft.Xml.XmlConvert.ToInt32(Reader.Value);
                    paramsRead[3] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id61_repeats && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@RepeatsString = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id62_pattern && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Pattern = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id63_ignoreCase && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IgnoreCase = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[6] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @":name, :type, :group, :capture, :repeats, :pattern, :ignoreCase");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations19 = 0;
            int readerCount19 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (((object)Reader.LocalName == (object)_id57_match && (object)Reader.NamespaceURI == (object)_id45_Item))
                    {
                        if ((object)(a_7) == null) Reader.Skip(); else a_7.Add(Read96_MimeTextMatch(false, true));
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://microsoft.com/wsdl/mime/textMatching/:match");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://microsoft.com/wsdl/mime/textMatching/:match");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations19, ref readerCount19);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.MimeMultipartRelatedBinding Read104_MimeMultipartRelatedBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id64_MimeMultipartRelatedBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id41_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.MimeMultipartRelatedBinding o;
            o = new global::System.Web.Services.Description.MimeMultipartRelatedBinding();
            global::System.Web.Services.Description.MimePartCollection a_1 = (global::System.Web.Services.Description.MimePartCollection)o.@Parts;
            bool[] paramsRead = new bool[2];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations20 = 0;
            int readerCount20 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (((object)Reader.LocalName == (object)_id49_part && (object)Reader.NamespaceURI == (object)_id41_Item))
                    {
                        if ((object)(a_1) == null) Reader.Skip(); else a_1.Add(Read103_MimePart(false, true));
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/mime/:part");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/mime/:part");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations20, ref readerCount20);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.MimePart Read103_MimePart(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id65_MimePart && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id41_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.MimePart o;
            o = new global::System.Web.Services.Description.MimePart();
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_1 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            bool[] paramsRead = new bool[2];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations21 = 0;
            int readerCount21 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (((object)Reader.LocalName == (object)_id40_content && (object)Reader.NamespaceURI == (object)_id41_Item))
                    {
                        if ((object)(a_1) == null) Reader.Skip(); else a_1.Add(Read93_MimeContentBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id42_mimeXml && (object)Reader.NamespaceURI == (object)_id41_Item))
                    {
                        if ((object)(a_1) == null) Reader.Skip(); else a_1.Add(Read94_MimeXmlBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id44_text && (object)Reader.NamespaceURI == (object)_id45_Item))
                    {
                        if ((object)(a_1) == null) Reader.Skip(); else a_1.Add(Read97_MimeTextBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id46_body && (object)Reader.NamespaceURI == (object)_id19_Item))
                    {
                        if ((object)(a_1) == null) Reader.Skip(); else a_1.Add(Read99_SoapBodyBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id46_body && (object)Reader.NamespaceURI == (object)_id20_Item))
                    {
                        if ((object)(a_1) == null) Reader.Skip(); else a_1.Add(Read102_Soap12BodyBinding(false, true));
                    }
                    else
                    {
                        a_1.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/mime/:content, http://schemas.xmlsoap.org/wsdl/mime/:mimeXml, http://microsoft.com/wsdl/mime/textMatching/:text, http://schemas.xmlsoap.org/wsdl/soap/:body, http://schemas.xmlsoap.org/wsdl/soap12/:body");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations21, ref readerCount21);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.MimeXmlBinding Read94_MimeXmlBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id66_MimeXmlBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id41_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.MimeXmlBinding o;
            o = new global::System.Web.Services.Description.MimeXmlBinding();
            bool[] paramsRead = new bool[2];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id49_part && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Part = Reader.Value;
                    paramsRead[1] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :part");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations22 = 0;
            int readerCount22 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations22, ref readerCount22);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.MimeContentBinding Read93_MimeContentBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id67_MimeContentBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id41_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.MimeContentBinding o;
            o = new global::System.Web.Services.Description.MimeContentBinding();
            bool[] paramsRead = new bool[3];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id49_part && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Part = Reader.Value;
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id27_type && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Type = Reader.Value;
                    paramsRead[2] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :part, :type");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations23 = 0;
            int readerCount23 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations23, ref readerCount23);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.InputBinding Read110_InputBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id68_InputBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.InputBinding o;
            o = new global::System.Web.Services.Description.InputBinding();
            Microsoft.Xml.XmlAttribute[] a_1 = null;
            int ca_1 = 0;
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_4 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            bool[] paramsRead = new bool[5];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute)); a_1[ca_1++] = attr;
                }
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations24 = 0;
            int readerCount24 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@DocumentationElement = (Microsoft.Xml.XmlElement)ReadXmlNode(false);
                        paramsRead[0] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id69_urlEncoded && (object)Reader.NamespaceURI == (object)_id18_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read90_HttpUrlEncodedBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id70_urlReplacement && (object)Reader.NamespaceURI == (object)_id18_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read91_HttpUrlReplacementBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id40_content && (object)Reader.NamespaceURI == (object)_id41_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read93_MimeContentBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id42_mimeXml && (object)Reader.NamespaceURI == (object)_id41_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read94_MimeXmlBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id43_multipartRelated && (object)Reader.NamespaceURI == (object)_id41_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read104_MimeMultipartRelatedBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id44_text && (object)Reader.NamespaceURI == (object)_id45_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read97_MimeTextBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id46_body && (object)Reader.NamespaceURI == (object)_id19_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read99_SoapBodyBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id47_header && (object)Reader.NamespaceURI == (object)_id19_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read106_SoapHeaderBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id46_body && (object)Reader.NamespaceURI == (object)_id20_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read102_Soap12BodyBinding(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id47_header && (object)Reader.NamespaceURI == (object)_id20_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read109_Soap12HeaderBinding(false, true));
                    }
                    else
                    {
                        a_4.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:documentation, http://schemas.xmlsoap.org/wsdl/http/:urlEncoded, http://schemas.xmlsoap.org/wsdl/http/:urlReplacement, http://schemas.xmlsoap.org/wsdl/mime/:content, http://schemas.xmlsoap.org/wsdl/mime/:mimeXml, http://schemas.xmlsoap.org/wsdl/mime/:multipartRelated, http://microsoft.com/wsdl/mime/textMatching/:text, http://schemas.xmlsoap.org/wsdl/soap/:body, http://schemas.xmlsoap.org/wsdl/soap/:header, http://schemas.xmlsoap.org/wsdl/soap12/:body, http://schemas.xmlsoap.org/wsdl/soap12/:header");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations24, ref readerCount24);
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.HttpUrlReplacementBinding Read91_HttpUrlReplacementBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id71_HttpUrlReplacementBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id18_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.HttpUrlReplacementBinding o;
            o = new global::System.Web.Services.Description.HttpUrlReplacementBinding();
            bool[] paramsRead = new bool[1];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations25 = 0;
            int readerCount25 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations25, ref readerCount25);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.HttpUrlEncodedBinding Read90_HttpUrlEncodedBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id72_HttpUrlEncodedBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id18_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.HttpUrlEncodedBinding o;
            o = new global::System.Web.Services.Description.HttpUrlEncodedBinding();
            bool[] paramsRead = new bool[1];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations26 = 0;
            int readerCount26 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations26, ref readerCount26);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.Soap12OperationBinding Read88_Soap12OperationBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id73_Soap12OperationBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id20_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.Soap12OperationBinding o;
            o = new global::System.Web.Services.Description.Soap12OperationBinding();
            bool[] paramsRead = new bool[4];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id74_soapAction && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@SoapAction = Reader.Value;
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id75_style && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Style = Read82_SoapBindingStyle(Reader.Value);
                    paramsRead[2] = true;
                }
                else if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id76_soapActionRequired && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@SoapActionRequired = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[3] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :soapAction, :style, :soapActionRequired");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations27 = 0;
            int readerCount27 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations27, ref readerCount27);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.SoapBindingStyle Read82_SoapBindingStyle(string s)
        {
            switch (s)
            {
                case @"document": return global::System.Web.Services.Description.SoapBindingStyle.@Document;
                case @"rpc": return global::System.Web.Services.Description.SoapBindingStyle.@Rpc;
                default: throw CreateUnknownConstantException(s, typeof(global::System.Web.Services.Description.SoapBindingStyle));
            }
        }

        private global::System.Web.Services.Description.SoapOperationBinding Read86_SoapOperationBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id77_SoapOperationBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id19_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.SoapOperationBinding o;
            o = new global::System.Web.Services.Description.SoapOperationBinding();
            bool[] paramsRead = new bool[3];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id74_soapAction && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@SoapAction = Reader.Value;
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id75_style && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Style = Read79_SoapBindingStyle(Reader.Value);
                    paramsRead[2] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :soapAction, :style");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations28 = 0;
            int readerCount28 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations28, ref readerCount28);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.SoapBindingStyle Read79_SoapBindingStyle(string s)
        {
            switch (s)
            {
                case @"document": return global::System.Web.Services.Description.SoapBindingStyle.@Document;
                case @"rpc": return global::System.Web.Services.Description.SoapBindingStyle.@Rpc;
                default: throw CreateUnknownConstantException(s, typeof(global::System.Web.Services.Description.SoapBindingStyle));
            }
        }

        private global::System.Web.Services.Description.HttpOperationBinding Read85_HttpOperationBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id78_HttpOperationBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id18_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.HttpOperationBinding o;
            o = new global::System.Web.Services.Description.HttpOperationBinding();
            bool[] paramsRead = new bool[2];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id23_location && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Location = Reader.Value;
                    paramsRead[1] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :location");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations29 = 0;
            int readerCount29 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations29, ref readerCount29);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.Soap12Binding Read84_Soap12Binding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id79_Soap12Binding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id20_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.Soap12Binding o;
            o = new global::System.Web.Services.Description.Soap12Binding();
            bool[] paramsRead = new bool[3];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id80_transport && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Transport = Reader.Value;
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id75_style && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Style = Read82_SoapBindingStyle(Reader.Value);
                    paramsRead[2] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :transport, :style");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations30 = 0;
            int readerCount30 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations30, ref readerCount30);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.SoapBinding Read80_SoapBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id81_SoapBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id19_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.SoapBinding o;
            o = new global::System.Web.Services.Description.SoapBinding();
            bool[] paramsRead = new bool[3];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id80_transport && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Transport = Reader.Value;
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id75_style && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Style = Read79_SoapBindingStyle(Reader.Value);
                    paramsRead[2] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :transport, :style");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations31 = 0;
            int readerCount31 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations31, ref readerCount31);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.HttpBinding Read77_HttpBinding(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id82_HttpBinding && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id18_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.HttpBinding o;
            o = new global::System.Web.Services.Description.HttpBinding();
            bool[] paramsRead = new bool[2];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id22_required && (object)Reader.NamespaceURI == (object)_id2_Item))
                {
                    o.@Required = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[0] = true;
                }
                else if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id83_verb && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Verb = Reader.Value;
                    paramsRead[1] = true;
                }
                else if (!IsXmlnsAttribute(Reader.Name))
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:required, :verb");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations32 = 0;
            int readerCount32 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    UnknownNode((object)o, @"");
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations32, ref readerCount32);
            }
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.PortType Read75_PortType(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id84_PortType && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.PortType o;
            o = new global::System.Web.Services.Description.PortType();
            Microsoft.Xml.XmlAttribute[] a_1 = null;
            int ca_1 = 0;
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_4 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            global::System.Web.Services.Description.OperationCollection a_5 = (global::System.Web.Services.Description.OperationCollection)o.@Operations;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute)); a_1[ca_1++] = attr;
                }
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations33 = 0;
            int readerCount33 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@DocumentationElement = (Microsoft.Xml.XmlElement)ReadXmlNode(false);
                        paramsRead[0] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id28_operation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        if ((object)(a_5) == null) Reader.Skip(); else a_5.Add(Read74_Operation(false, true));
                    }
                    else
                    {
                        a_4.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:documentation, http://schemas.xmlsoap.org/wsdl/:operation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations33, ref readerCount33);
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.Operation Read74_Operation(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id85_Operation && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.Operation o;
            o = new global::System.Web.Services.Description.Operation();
            Microsoft.Xml.XmlAttribute[] a_1 = null;
            int ca_1 = 0;
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_4 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            global::System.Web.Services.Description.OperationMessageCollection a_6 = (global::System.Web.Services.Description.OperationMessageCollection)o.@Messages;
            global::System.Web.Services.Description.OperationFaultCollection a_7 = (global::System.Web.Services.Description.OperationFaultCollection)o.@Faults;
            bool[] paramsRead = new bool[8];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id86_parameterOrder && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@ParameterOrderString = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute)); a_1[ca_1++] = attr;
                }
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations34 = 0;
            int readerCount34 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@DocumentationElement = (Microsoft.Xml.XmlElement)ReadXmlNode(false);
                        paramsRead[0] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id30_input && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read71_OperationInput(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id31_output && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read72_OperationOutput(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id32_fault && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        if ((object)(a_7) == null) Reader.Skip(); else a_7.Add(Read73_OperationFault(false, true));
                    }
                    else
                    {
                        a_4.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:documentation, http://schemas.xmlsoap.org/wsdl/:input, http://schemas.xmlsoap.org/wsdl/:output, http://schemas.xmlsoap.org/wsdl/:fault");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations34, ref readerCount34);
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.OperationFault Read73_OperationFault(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id87_OperationFault && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.OperationFault o;
            o = new global::System.Web.Services.Description.OperationFault();
            Microsoft.Xml.XmlAttribute[] a_1 = null;
            int ca_1 = 0;
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_5 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id10_message && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Message = ToXmlQualifiedName(Reader.Value);
                    paramsRead[4] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute)); a_1[ca_1++] = attr;
                }
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations35 = 0;
            int readerCount35 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@DocumentationElement = (Microsoft.Xml.XmlElement)ReadXmlNode(false);
                        paramsRead[0] = true;
                    }
                    else
                    {
                        a_5.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:documentation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations35, ref readerCount35);
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.OperationOutput Read72_OperationOutput(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id88_OperationOutput && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.OperationOutput o;
            o = new global::System.Web.Services.Description.OperationOutput();
            Microsoft.Xml.XmlAttribute[] a_1 = null;
            int ca_1 = 0;
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_5 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id10_message && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Message = ToXmlQualifiedName(Reader.Value);
                    paramsRead[4] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute)); a_1[ca_1++] = attr;
                }
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations36 = 0;
            int readerCount36 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@DocumentationElement = (Microsoft.Xml.XmlElement)ReadXmlNode(false);
                        paramsRead[0] = true;
                    }
                    else
                    {
                        a_5.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:documentation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations36, ref readerCount36);
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.OperationInput Read71_OperationInput(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id89_OperationInput && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.OperationInput o;
            o = new global::System.Web.Services.Description.OperationInput();
            Microsoft.Xml.XmlAttribute[] a_1 = null;
            int ca_1 = 0;
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_5 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id10_message && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Message = ToXmlQualifiedName(Reader.Value);
                    paramsRead[4] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute)); a_1[ca_1++] = attr;
                }
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations37 = 0;
            int readerCount37 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@DocumentationElement = (Microsoft.Xml.XmlElement)ReadXmlNode(false);
                        paramsRead[0] = true;
                    }
                    else
                    {
                        a_5.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:documentation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations37, ref readerCount37);
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.Message Read69_Message(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id90_Message && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.Message o;
            o = new global::System.Web.Services.Description.Message();
            Microsoft.Xml.XmlAttribute[] a_1 = null;
            int ca_1 = 0;
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_4 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            global::System.Web.Services.Description.MessagePartCollection a_5 = (global::System.Web.Services.Description.MessagePartCollection)o.@Parts;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute)); a_1[ca_1++] = attr;
                }
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations38 = 0;
            int readerCount38 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@DocumentationElement = (Microsoft.Xml.XmlElement)ReadXmlNode(false);
                        paramsRead[0] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id49_part && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        if ((object)(a_5) == null) Reader.Skip(); else a_5.Add(Read68_MessagePart(false, true));
                    }
                    else
                    {
                        a_4.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:documentation, http://schemas.xmlsoap.org/wsdl/:part");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations38, ref readerCount38);
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.MessagePart Read68_MessagePart(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id91_MessagePart && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.MessagePart o;
            o = new global::System.Web.Services.Description.MessagePart();
            Microsoft.Xml.XmlAttribute[] a_1 = null;
            int ca_1 = 0;
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_4 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            bool[] paramsRead = new bool[7];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[3] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id92_element && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Element = ToXmlQualifiedName(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id27_type && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Type = ToXmlQualifiedName(Reader.Value);
                    paramsRead[6] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute)); a_1[ca_1++] = attr;
                }
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations39 = 0;
            int readerCount39 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@DocumentationElement = (Microsoft.Xml.XmlElement)ReadXmlNode(false);
                        paramsRead[0] = true;
                    }
                    else
                    {
                        a_4.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:documentation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations39, ref readerCount39);
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.Types Read67_Types(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id93_Types && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.Types o;
            o = new global::System.Web.Services.Description.Types();
            Microsoft.Xml.XmlAttribute[] a_1 = null;
            int ca_1 = 0;
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_3 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            Microsoft.Xml.Serialization.XmlSchemas a_4 = (Microsoft.Xml.Serialization.XmlSchemas)o.@Schemas;
            bool[] paramsRead = new bool[5];
            while (Reader.MoveToNextAttribute())
            {
                if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute)); a_1[ca_1++] = attr;
                }
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations40 = 0;
            int readerCount40 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@DocumentationElement = (Microsoft.Xml.XmlElement)ReadXmlNode(false);
                        paramsRead[0] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id94_schema && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read66_XmlSchema(false, true));
                    }
                    else
                    {
                        a_3.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:documentation, http://www.w3.org/2001/XMLSchema:schema");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations40, ref readerCount40);
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchema Read66_XmlSchema(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id96_XmlSchema && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchema o;
            o = new Microsoft.Xml.Schema.XmlSchema();
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_7 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Includes;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_8 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Items;
            Microsoft.Xml.XmlAttribute[] a_10 = null;
            int ca_10 = 0;
            bool[] paramsRead = new bool[11];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id97_attributeFormDefault && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@AttributeFormDefault = Read6_XmlSchemaForm(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id98_blockDefault && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@BlockDefault = Read7_XmlSchemaDerivationMethod(Reader.Value);
                    paramsRead[2] = true;
                }
                else if (!paramsRead[3] && ((object)Reader.LocalName == (object)_id99_finalDefault && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@FinalDefault = Read7_XmlSchemaDerivationMethod(Reader.Value);
                    paramsRead[3] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id100_elementFormDefault && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@ElementFormDefault = Read6_XmlSchemaForm(Reader.Value);
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id6_targetNamespace && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@TargetNamespace = CollapseWhitespace(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id101_version && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Version = CollapseWhitespace(Reader.Value);
                    paramsRead[6] = true;
                }
                else if (!paramsRead[9] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[9] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_10 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_10, ca_10, typeof(Microsoft.Xml.XmlAttribute)); a_10[ca_10++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_10, ca_10, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_10, ca_10, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations41 = 0;
            int readerCount41 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (((object)Reader.LocalName == (object)_id103_include && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_7) == null) Reader.Skip(); else a_7.Add(Read12_XmlSchemaInclude(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id8_import && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_7) == null) Reader.Skip(); else a_7.Add(Read13_XmlSchemaImport(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id104_redefine && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_7) == null) Reader.Skip(); else a_7.Add(Read64_XmlSchemaRedefine(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id105_simpleType && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_8) == null) Reader.Skip(); else a_8.Add(Read34_XmlSchemaSimpleType(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id106_complexType && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_8) == null) Reader.Skip(); else a_8.Add(Read62_XmlSchemaComplexType(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_8) == null) Reader.Skip(); else a_8.Add(Read11_XmlSchemaAnnotation(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id108_notation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_8) == null) Reader.Skip(); else a_8.Add(Read65_XmlSchemaNotation(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id59_group && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_8) == null) Reader.Skip(); else a_8.Add(Read63_XmlSchemaGroup(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id92_element && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_8) == null) Reader.Skip(); else a_8.Add(Read52_XmlSchemaElement(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id109_attribute && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_8) == null) Reader.Skip(); else a_8.Add(Read36_XmlSchemaAttribute(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id110_attributeGroup && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_8) == null) Reader.Skip(); else a_8.Add(Read40_XmlSchemaAttributeGroup(false, true));
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:include, http://www.w3.org/2001/XMLSchema:import, http://www.w3.org/2001/XMLSchema:redefine, http://www.w3.org/2001/XMLSchema:simpleType, http://www.w3.org/2001/XMLSchema:complexType, http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:notation, http://www.w3.org/2001/XMLSchema:group, http://www.w3.org/2001/XMLSchema:element, http://www.w3.org/2001/XMLSchema:attribute, http://www.w3.org/2001/XMLSchema:attributeGroup");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:include, http://www.w3.org/2001/XMLSchema:import, http://www.w3.org/2001/XMLSchema:redefine, http://www.w3.org/2001/XMLSchema:simpleType, http://www.w3.org/2001/XMLSchema:complexType, http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:notation, http://www.w3.org/2001/XMLSchema:group, http://www.w3.org/2001/XMLSchema:element, http://www.w3.org/2001/XMLSchema:attribute, http://www.w3.org/2001/XMLSchema:attributeGroup");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations41, ref readerCount41);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_10, ca_10, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaAttributeGroup Read40_XmlSchemaAttributeGroup(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id111_XmlSchemaAttributeGroup && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaAttributeGroup o;
            o = new Microsoft.Xml.Schema.XmlSchemaAttributeGroup();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_5 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Attributes;
            bool[] paramsRead = new bool[7];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations42 = 0;
            int readerCount42 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id109_attribute && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_5) == null) Reader.Skip(); else a_5.Add(Read36_XmlSchemaAttribute(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id110_attributeGroup && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_5) == null) Reader.Skip(); else a_5.Add(Read37_XmlSchemaAttributeGroupRef(false, true));
                    }
                    else if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id112_anyAttribute && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@AnyAttribute = Read39_XmlSchemaAnyAttribute(false, true);
                        paramsRead[6] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:attribute, http://www.w3.org/2001/XMLSchema:attributeGroup, http://www.w3.org/2001/XMLSchema:anyAttribute");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:attribute, http://www.w3.org/2001/XMLSchema:attributeGroup, http://www.w3.org/2001/XMLSchema:anyAttribute");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations42, ref readerCount42);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaAnyAttribute Read39_XmlSchemaAnyAttribute(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id113_XmlSchemaAnyAttribute && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaAnyAttribute o;
            o = new Microsoft.Xml.Schema.XmlSchemaAnyAttribute();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id36_namespace && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Namespace = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id114_processContents && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@ProcessContents = Read38_XmlSchemaContentProcessing(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations43 = 0;
            int readerCount43 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations43, ref readerCount43);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaAnnotation Read11_XmlSchemaAnnotation(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id115_XmlSchemaAnnotation && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaAnnotation o;
            o = new Microsoft.Xml.Schema.XmlSchemaAnnotation();
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_2 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Items;
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[4];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations44 = 0;
            int readerCount44 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_2) == null) Reader.Skip(); else a_2.Add(Read9_XmlSchemaDocumentation(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id116_appinfo && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_2) == null) Reader.Skip(); else a_2.Add(Read10_XmlSchemaAppInfo(false, true));
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:documentation, http://www.w3.org/2001/XMLSchema:appinfo");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:documentation, http://www.w3.org/2001/XMLSchema:appinfo");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations44, ref readerCount44);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaAppInfo Read10_XmlSchemaAppInfo(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id117_XmlSchemaAppInfo && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaAppInfo o;
            o = new Microsoft.Xml.Schema.XmlSchemaAppInfo();
            Microsoft.Xml.XmlNode[] a_2 = null;
            int ca_2 = 0;
            bool[] paramsRead = new bool[3];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id118_source && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Source = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    UnknownNode((object)o, @":source");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@Markup = (Microsoft.Xml.XmlNode[])ShrinkArray(a_2, ca_2, typeof(Microsoft.Xml.XmlNode), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations45 = 0;
            int readerCount45 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    a_2 = (Microsoft.Xml.XmlNode[])EnsureArrayIndex(a_2, ca_2, typeof(Microsoft.Xml.XmlNode)); a_2[ca_2++] = (Microsoft.Xml.XmlNode)ReadXmlNode(false);
                }
                else if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Text ||
                Reader.NodeType == Microsoft.Xml.XmlNodeType.CDATA ||
                Reader.NodeType == Microsoft.Xml.XmlNodeType.Whitespace ||
                Reader.NodeType == Microsoft.Xml.XmlNodeType.SignificantWhitespace)
                {
                    a_2 = (Microsoft.Xml.XmlNode[])EnsureArrayIndex(a_2, ca_2, typeof(Microsoft.Xml.XmlNode)); a_2[ca_2++] = (Microsoft.Xml.XmlNode)Document.CreateTextNode(Reader.ReadString());
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations45, ref readerCount45);
            }
            o.@Markup = (Microsoft.Xml.XmlNode[])ShrinkArray(a_2, ca_2, typeof(Microsoft.Xml.XmlNode), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaDocumentation Read9_XmlSchemaDocumentation(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id119_XmlSchemaDocumentation && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaDocumentation o;
            o = new Microsoft.Xml.Schema.XmlSchemaDocumentation();
            Microsoft.Xml.XmlNode[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[4];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id118_source && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Source = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id120_lang && (object)Reader.NamespaceURI == (object)_id121_Item))
                {
                    o.@Language = Reader.Value;
                    paramsRead[2] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    UnknownNode((object)o, @":source, http://www.w3.org/XML/1998/namespace");
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@Markup = (Microsoft.Xml.XmlNode[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlNode), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations46 = 0;
            int readerCount46 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    a_3 = (Microsoft.Xml.XmlNode[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlNode)); a_3[ca_3++] = (Microsoft.Xml.XmlNode)ReadXmlNode(false);
                }
                else if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Text ||
                Reader.NodeType == Microsoft.Xml.XmlNodeType.CDATA ||
                Reader.NodeType == Microsoft.Xml.XmlNodeType.Whitespace ||
                Reader.NodeType == Microsoft.Xml.XmlNodeType.SignificantWhitespace)
                {
                    a_3 = (Microsoft.Xml.XmlNode[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlNode)); a_3[ca_3++] = (Microsoft.Xml.XmlNode)Document.CreateTextNode(Reader.ReadString());
                }
                else
                {
                    UnknownNode((object)o, @"");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations46, ref readerCount46);
            }
            o.@Markup = (Microsoft.Xml.XmlNode[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlNode), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaContentProcessing Read38_XmlSchemaContentProcessing(string s)
        {
            switch (s)
            {
                case @"skip": return Microsoft.Xml.Schema.XmlSchemaContentProcessing.@Skip;
                case @"lax": return Microsoft.Xml.Schema.XmlSchemaContentProcessing.@Lax;
                case @"strict": return Microsoft.Xml.Schema.XmlSchemaContentProcessing.@Strict;
                default: throw CreateUnknownConstantException(s, typeof(Microsoft.Xml.Schema.XmlSchemaContentProcessing));
            }
        }

        private Microsoft.Xml.Schema.XmlSchemaAttributeGroupRef Read37_XmlSchemaAttributeGroupRef(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id122_XmlSchemaAttributeGroupRef && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaAttributeGroupRef o;
            o = new Microsoft.Xml.Schema.XmlSchemaAttributeGroupRef();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[5];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id123_ref && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@RefName = ToXmlQualifiedName(Reader.Value);
                    paramsRead[4] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations47 = 0;
            int readerCount47 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations47, ref readerCount47);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaAttribute Read36_XmlSchemaAttribute(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id124_XmlSchemaAttribute && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaAttribute o;
            o = new Microsoft.Xml.Schema.XmlSchemaAttribute();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[12];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id125_default && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@DefaultValue = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id126_fixed && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@FixedValue = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id127_form && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Form = Read6_XmlSchemaForm(Reader.Value);
                    paramsRead[6] = true;
                }
                else if (!paramsRead[7] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[7] = true;
                }
                else if (!paramsRead[8] && ((object)Reader.LocalName == (object)_id123_ref && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@RefName = ToXmlQualifiedName(Reader.Value);
                    paramsRead[8] = true;
                }
                else if (!paramsRead[9] && ((object)Reader.LocalName == (object)_id27_type && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@SchemaTypeName = ToXmlQualifiedName(Reader.Value);
                    paramsRead[9] = true;
                }
                else if (!paramsRead[11] && ((object)Reader.LocalName == (object)_id35_use && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Use = Read35_XmlSchemaUse(Reader.Value);
                    paramsRead[11] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations48 = 0;
            int readerCount48 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[10] && ((object)Reader.LocalName == (object)_id105_simpleType && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@SchemaType = Read34_XmlSchemaSimpleType(false, true);
                        paramsRead[10] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:simpleType");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:simpleType");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations48, ref readerCount48);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaSimpleType Read34_XmlSchemaSimpleType(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id128_XmlSchemaSimpleType && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaSimpleType o;
            o = new Microsoft.Xml.Schema.XmlSchemaSimpleType();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[7];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id129_final && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Final = Read7_XmlSchemaDerivationMethod(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations49 = 0;
            int readerCount49 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id130_list && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Content = Read17_XmlSchemaSimpleTypeList(false, true);
                        paramsRead[6] = true;
                    }
                    else if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id131_restriction && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Content = Read32_XmlSchemaSimpleTypeRestriction(false, true);
                        paramsRead[6] = true;
                    }
                    else if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id132_union && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Content = Read33_XmlSchemaSimpleTypeUnion(false, true);
                        paramsRead[6] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:list, http://www.w3.org/2001/XMLSchema:restriction, http://www.w3.org/2001/XMLSchema:union");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:list, http://www.w3.org/2001/XMLSchema:restriction, http://www.w3.org/2001/XMLSchema:union");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations49, ref readerCount49);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaSimpleTypeUnion Read33_XmlSchemaSimpleTypeUnion(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id133_XmlSchemaSimpleTypeUnion && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaSimpleTypeUnion o;
            o = new Microsoft.Xml.Schema.XmlSchemaSimpleTypeUnion();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_4 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@BaseTypes;
            Microsoft.Xml.XmlQualifiedName[] a_5 = null;
            int ca_5 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (((object)Reader.LocalName == (object)_id134_memberTypes && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    string listValues = Reader.Value;
                    string[] vals = listValues.Split(null);
                    for (int i = 0; i < vals.Length; i++)
                    {
                        a_5 = (Microsoft.Xml.XmlQualifiedName[])EnsureArrayIndex(a_5, ca_5, typeof(Microsoft.Xml.XmlQualifiedName)); a_5[ca_5++] = ToXmlQualifiedName(vals[i]);
                    }
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            o.@MemberTypes = (Microsoft.Xml.XmlQualifiedName[])ShrinkArray(a_5, ca_5, typeof(Microsoft.Xml.XmlQualifiedName), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                o.@MemberTypes = (Microsoft.Xml.XmlQualifiedName[])ShrinkArray(a_5, ca_5, typeof(Microsoft.Xml.XmlQualifiedName), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations50 = 0;
            int readerCount50 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id105_simpleType && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read34_XmlSchemaSimpleType(false, true));
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:simpleType");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:simpleType");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations50, ref readerCount50);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            o.@MemberTypes = (Microsoft.Xml.XmlQualifiedName[])ShrinkArray(a_5, ca_5, typeof(Microsoft.Xml.XmlQualifiedName), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaSimpleTypeRestriction Read32_XmlSchemaSimpleTypeRestriction(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id135_XmlSchemaSimpleTypeRestriction && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaSimpleTypeRestriction o;
            o = new Microsoft.Xml.Schema.XmlSchemaSimpleTypeRestriction();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_6 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Facets;
            bool[] paramsRead = new bool[7];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id136_base && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@BaseTypeName = ToXmlQualifiedName(Reader.Value);
                    paramsRead[4] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations51 = 0;
            int readerCount51 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id105_simpleType && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@BaseType = Read34_XmlSchemaSimpleType(false, true);
                        paramsRead[5] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id137_fractionDigits && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read20_XmlSchemaFractionDigitsFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id138_minInclusive && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read21_XmlSchemaMinInclusiveFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id139_maxLength && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read22_XmlSchemaMaxLengthFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id140_length && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read23_XmlSchemaLengthFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id141_totalDigits && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read24_XmlSchemaTotalDigitsFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id62_pattern && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read25_XmlSchemaPatternFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id142_enumeration && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read26_XmlSchemaEnumerationFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id143_maxInclusive && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read27_XmlSchemaMaxInclusiveFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id144_maxExclusive && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read28_XmlSchemaMaxExclusiveFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id145_whiteSpace && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read29_XmlSchemaWhiteSpaceFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id146_minExclusive && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read30_XmlSchemaMinExclusiveFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id147_minLength && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read31_XmlSchemaMinLengthFacet(false, true));
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:simpleType, http://www.w3.org/2001/XMLSchema:fractionDigits, http://www.w3.org/2001/XMLSchema:minInclusive, http://www.w3.org/2001/XMLSchema:maxLength, http://www.w3.org/2001/XMLSchema:length, http://www.w3.org/2001/XMLSchema:totalDigits, http://www.w3.org/2001/XMLSchema:pattern, http://www.w3.org/2001/XMLSchema:enumeration, http://www.w3.org/2001/XMLSchema:maxInclusive, http://www.w3.org/2001/XMLSchema:maxExclusive, http://www.w3.org/2001/XMLSchema:whiteSpace, http://www.w3.org/2001/XMLSchema:minExclusive, http://www.w3.org/2001/XMLSchema:minLength");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:simpleType, http://www.w3.org/2001/XMLSchema:fractionDigits, http://www.w3.org/2001/XMLSchema:minInclusive, http://www.w3.org/2001/XMLSchema:maxLength, http://www.w3.org/2001/XMLSchema:length, http://www.w3.org/2001/XMLSchema:totalDigits, http://www.w3.org/2001/XMLSchema:pattern, http://www.w3.org/2001/XMLSchema:enumeration, http://www.w3.org/2001/XMLSchema:maxInclusive, http://www.w3.org/2001/XMLSchema:maxExclusive, http://www.w3.org/2001/XMLSchema:whiteSpace, http://www.w3.org/2001/XMLSchema:minExclusive, http://www.w3.org/2001/XMLSchema:minLength");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations51, ref readerCount51);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaMinLengthFacet Read31_XmlSchemaMinLengthFacet(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id148_XmlSchemaMinLengthFacet && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaMinLengthFacet o;
            o = new Microsoft.Xml.Schema.XmlSchemaMinLengthFacet();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id149_value && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Value = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id126_fixed && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IsFixed = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations52 = 0;
            int readerCount52 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations52, ref readerCount52);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaMinExclusiveFacet Read30_XmlSchemaMinExclusiveFacet(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id150_XmlSchemaMinExclusiveFacet && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaMinExclusiveFacet o;
            o = new Microsoft.Xml.Schema.XmlSchemaMinExclusiveFacet();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id149_value && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Value = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id126_fixed && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IsFixed = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations53 = 0;
            int readerCount53 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations53, ref readerCount53);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaWhiteSpaceFacet Read29_XmlSchemaWhiteSpaceFacet(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id151_XmlSchemaWhiteSpaceFacet && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaWhiteSpaceFacet o;
            o = new Microsoft.Xml.Schema.XmlSchemaWhiteSpaceFacet();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id149_value && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Value = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id126_fixed && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IsFixed = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations54 = 0;
            int readerCount54 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations54, ref readerCount54);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaMaxExclusiveFacet Read28_XmlSchemaMaxExclusiveFacet(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id152_XmlSchemaMaxExclusiveFacet && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaMaxExclusiveFacet o;
            o = new Microsoft.Xml.Schema.XmlSchemaMaxExclusiveFacet();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id149_value && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Value = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id126_fixed && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IsFixed = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations55 = 0;
            int readerCount55 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations55, ref readerCount55);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaMaxInclusiveFacet Read27_XmlSchemaMaxInclusiveFacet(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id153_XmlSchemaMaxInclusiveFacet && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaMaxInclusiveFacet o;
            o = new Microsoft.Xml.Schema.XmlSchemaMaxInclusiveFacet();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id149_value && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Value = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id126_fixed && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IsFixed = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations56 = 0;
            int readerCount56 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations56, ref readerCount56);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaEnumerationFacet Read26_XmlSchemaEnumerationFacet(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id154_XmlSchemaEnumerationFacet && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaEnumerationFacet o;
            o = new Microsoft.Xml.Schema.XmlSchemaEnumerationFacet();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id149_value && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Value = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id126_fixed && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IsFixed = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations57 = 0;
            int readerCount57 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations57, ref readerCount57);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaPatternFacet Read25_XmlSchemaPatternFacet(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id155_XmlSchemaPatternFacet && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaPatternFacet o;
            o = new Microsoft.Xml.Schema.XmlSchemaPatternFacet();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id149_value && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Value = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id126_fixed && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IsFixed = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations58 = 0;
            int readerCount58 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations58, ref readerCount58);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaTotalDigitsFacet Read24_XmlSchemaTotalDigitsFacet(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id156_XmlSchemaTotalDigitsFacet && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaTotalDigitsFacet o;
            o = new Microsoft.Xml.Schema.XmlSchemaTotalDigitsFacet();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id149_value && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Value = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id126_fixed && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IsFixed = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations59 = 0;
            int readerCount59 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations59, ref readerCount59);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaLengthFacet Read23_XmlSchemaLengthFacet(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id157_XmlSchemaLengthFacet && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaLengthFacet o;
            o = new Microsoft.Xml.Schema.XmlSchemaLengthFacet();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id149_value && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Value = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id126_fixed && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IsFixed = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations60 = 0;
            int readerCount60 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations60, ref readerCount60);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaMaxLengthFacet Read22_XmlSchemaMaxLengthFacet(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id158_XmlSchemaMaxLengthFacet && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaMaxLengthFacet o;
            o = new Microsoft.Xml.Schema.XmlSchemaMaxLengthFacet();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id149_value && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Value = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id126_fixed && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IsFixed = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations61 = 0;
            int readerCount61 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations61, ref readerCount61);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaMinInclusiveFacet Read21_XmlSchemaMinInclusiveFacet(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id159_XmlSchemaMinInclusiveFacet && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaMinInclusiveFacet o;
            o = new Microsoft.Xml.Schema.XmlSchemaMinInclusiveFacet();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id149_value && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Value = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id126_fixed && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IsFixed = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations62 = 0;
            int readerCount62 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations62, ref readerCount62);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaFractionDigitsFacet Read20_XmlSchemaFractionDigitsFacet(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id160_XmlSchemaFractionDigitsFacet && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaFractionDigitsFacet o;
            o = new Microsoft.Xml.Schema.XmlSchemaFractionDigitsFacet();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id149_value && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Value = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id126_fixed && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IsFixed = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations63 = 0;
            int readerCount63 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations63, ref readerCount63);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaSimpleTypeList Read17_XmlSchemaSimpleTypeList(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id161_XmlSchemaSimpleTypeList && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaSimpleTypeList o;
            o = new Microsoft.Xml.Schema.XmlSchemaSimpleTypeList();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id162_itemType && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@ItemTypeName = ToXmlQualifiedName(Reader.Value);
                    paramsRead[4] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations64 = 0;
            int readerCount64 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id105_simpleType && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@ItemType = Read34_XmlSchemaSimpleType(false, true);
                        paramsRead[5] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:simpleType");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:simpleType");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations64, ref readerCount64);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private System.Collections.Hashtable _XmlSchemaDerivationMethodValues;

        internal System.Collections.Hashtable XmlSchemaDerivationMethodValues
        {
            get
            {
                if ((object)_XmlSchemaDerivationMethodValues == null)
                {
                    System.Collections.Hashtable h = new System.Collections.Hashtable();
                    h.Add(@"", (long)Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@Empty);
                    h.Add(@"substitution", (long)Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@Substitution);
                    h.Add(@"extension", (long)Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@Extension);
                    h.Add(@"restriction", (long)Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@Restriction);
                    h.Add(@"list", (long)Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@List);
                    h.Add(@"union", (long)Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@Union);
                    h.Add(@"#all", (long)Microsoft.Xml.Schema.XmlSchemaDerivationMethod.@All);
                    _XmlSchemaDerivationMethodValues = h;
                }
                return _XmlSchemaDerivationMethodValues;
            }
        }

        private Microsoft.Xml.Schema.XmlSchemaDerivationMethod Read7_XmlSchemaDerivationMethod(string s)
        {
            return (Microsoft.Xml.Schema.XmlSchemaDerivationMethod)ToEnum(s, XmlSchemaDerivationMethodValues, @"System.Xml.Schema.XmlSchemaDerivationMethod");
        }

        private Microsoft.Xml.Schema.XmlSchemaUse Read35_XmlSchemaUse(string s)
        {
            switch (s)
            {
                case @"optional": return Microsoft.Xml.Schema.XmlSchemaUse.@Optional;
                case @"prohibited": return Microsoft.Xml.Schema.XmlSchemaUse.@Prohibited;
                case @"required": return Microsoft.Xml.Schema.XmlSchemaUse.@Required;
                default: throw CreateUnknownConstantException(s, typeof(Microsoft.Xml.Schema.XmlSchemaUse));
            }
        }

        private Microsoft.Xml.Schema.XmlSchemaForm Read6_XmlSchemaForm(string s)
        {
            switch (s)
            {
                case @"qualified": return Microsoft.Xml.Schema.XmlSchemaForm.@Qualified;
                case @"unqualified": return Microsoft.Xml.Schema.XmlSchemaForm.@Unqualified;
                default: throw CreateUnknownConstantException(s, typeof(Microsoft.Xml.Schema.XmlSchemaForm));
            }
        }

        private Microsoft.Xml.Schema.XmlSchemaElement Read52_XmlSchemaElement(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id163_XmlSchemaElement && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaElement o;
            o = new Microsoft.Xml.Schema.XmlSchemaElement();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_18 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Constraints;
            bool[] paramsRead = new bool[19];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id164_minOccurs && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@MinOccursString = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id165_maxOccurs && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@MaxOccursString = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id166_abstract && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IsAbstract = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[6] = true;
                }
                else if (!paramsRead[7] && ((object)Reader.LocalName == (object)_id167_block && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Block = Read7_XmlSchemaDerivationMethod(Reader.Value);
                    paramsRead[7] = true;
                }
                else if (!paramsRead[8] && ((object)Reader.LocalName == (object)_id125_default && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@DefaultValue = Reader.Value;
                    paramsRead[8] = true;
                }
                else if (!paramsRead[9] && ((object)Reader.LocalName == (object)_id129_final && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Final = Read7_XmlSchemaDerivationMethod(Reader.Value);
                    paramsRead[9] = true;
                }
                else if (!paramsRead[10] && ((object)Reader.LocalName == (object)_id126_fixed && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@FixedValue = Reader.Value;
                    paramsRead[10] = true;
                }
                else if (!paramsRead[11] && ((object)Reader.LocalName == (object)_id127_form && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Form = Read6_XmlSchemaForm(Reader.Value);
                    paramsRead[11] = true;
                }
                else if (!paramsRead[12] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[12] = true;
                }
                else if (!paramsRead[13] && ((object)Reader.LocalName == (object)_id168_nillable && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IsNillable = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[13] = true;
                }
                else if (!paramsRead[14] && ((object)Reader.LocalName == (object)_id123_ref && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@RefName = ToXmlQualifiedName(Reader.Value);
                    paramsRead[14] = true;
                }
                else if (!paramsRead[15] && ((object)Reader.LocalName == (object)_id169_substitutionGroup && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@SubstitutionGroup = ToXmlQualifiedName(Reader.Value);
                    paramsRead[15] = true;
                }
                else if (!paramsRead[16] && ((object)Reader.LocalName == (object)_id27_type && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@SchemaTypeName = ToXmlQualifiedName(Reader.Value);
                    paramsRead[16] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations65 = 0;
            int readerCount65 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[17] && ((object)Reader.LocalName == (object)_id105_simpleType && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@SchemaType = Read34_XmlSchemaSimpleType(false, true);
                        paramsRead[17] = true;
                    }
                    else if (!paramsRead[17] && ((object)Reader.LocalName == (object)_id106_complexType && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@SchemaType = Read62_XmlSchemaComplexType(false, true);
                        paramsRead[17] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id170_key && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_18) == null) Reader.Skip(); else a_18.Add(Read49_XmlSchemaKey(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id171_unique && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_18) == null) Reader.Skip(); else a_18.Add(Read50_XmlSchemaUnique(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id172_keyref && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_18) == null) Reader.Skip(); else a_18.Add(Read51_XmlSchemaKeyref(false, true));
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:simpleType, http://www.w3.org/2001/XMLSchema:complexType, http://www.w3.org/2001/XMLSchema:key, http://www.w3.org/2001/XMLSchema:unique, http://www.w3.org/2001/XMLSchema:keyref");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:simpleType, http://www.w3.org/2001/XMLSchema:complexType, http://www.w3.org/2001/XMLSchema:key, http://www.w3.org/2001/XMLSchema:unique, http://www.w3.org/2001/XMLSchema:keyref");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations65, ref readerCount65);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaKeyref Read51_XmlSchemaKeyref(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id173_XmlSchemaKeyref && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaKeyref o;
            o = new Microsoft.Xml.Schema.XmlSchemaKeyref();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_6 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Fields;
            bool[] paramsRead = new bool[8];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[7] && ((object)Reader.LocalName == (object)_id174_refer && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Refer = ToXmlQualifiedName(Reader.Value);
                    paramsRead[7] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations66 = 0;
            int readerCount66 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id175_selector && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Selector = Read47_XmlSchemaXPath(false, true);
                        paramsRead[5] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id176_field && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read47_XmlSchemaXPath(false, true));
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:selector, http://www.w3.org/2001/XMLSchema:field");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:selector, http://www.w3.org/2001/XMLSchema:field");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations66, ref readerCount66);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaXPath Read47_XmlSchemaXPath(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id177_XmlSchemaXPath && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaXPath o;
            o = new Microsoft.Xml.Schema.XmlSchemaXPath();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[5];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id178_xpath && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@XPath = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations67 = 0;
            int readerCount67 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations67, ref readerCount67);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaUnique Read50_XmlSchemaUnique(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id179_XmlSchemaUnique && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaUnique o;
            o = new Microsoft.Xml.Schema.XmlSchemaUnique();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_6 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Fields;
            bool[] paramsRead = new bool[7];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations68 = 0;
            int readerCount68 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id175_selector && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Selector = Read47_XmlSchemaXPath(false, true);
                        paramsRead[5] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id176_field && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read47_XmlSchemaXPath(false, true));
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:selector, http://www.w3.org/2001/XMLSchema:field");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:selector, http://www.w3.org/2001/XMLSchema:field");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations68, ref readerCount68);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaKey Read49_XmlSchemaKey(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id180_XmlSchemaKey && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaKey o;
            o = new Microsoft.Xml.Schema.XmlSchemaKey();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_6 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Fields;
            bool[] paramsRead = new bool[7];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations69 = 0;
            int readerCount69 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id175_selector && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Selector = Read47_XmlSchemaXPath(false, true);
                        paramsRead[5] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id176_field && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read47_XmlSchemaXPath(false, true));
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:selector, http://www.w3.org/2001/XMLSchema:field");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:selector, http://www.w3.org/2001/XMLSchema:field");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations69, ref readerCount69);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaComplexType Read62_XmlSchemaComplexType(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id181_XmlSchemaComplexType && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaComplexType o;
            o = new Microsoft.Xml.Schema.XmlSchemaComplexType();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_11 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Attributes;
            bool[] paramsRead = new bool[13];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id129_final && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Final = Read7_XmlSchemaDerivationMethod(Reader.Value);
                    paramsRead[5] = true;
                }
                else if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id166_abstract && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IsAbstract = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[6] = true;
                }
                else if (!paramsRead[7] && ((object)Reader.LocalName == (object)_id167_block && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Block = Read7_XmlSchemaDerivationMethod(Reader.Value);
                    paramsRead[7] = true;
                }
                else if (!paramsRead[8] && ((object)Reader.LocalName == (object)_id182_mixed && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IsMixed = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[8] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations70 = 0;
            int readerCount70 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[9] && ((object)Reader.LocalName == (object)_id183_complexContent && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@ContentModel = Read58_XmlSchemaComplexContent(false, true);
                        paramsRead[9] = true;
                    }
                    else if (!paramsRead[9] && ((object)Reader.LocalName == (object)_id184_simpleContent && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@ContentModel = Read61_XmlSchemaSimpleContent(false, true);
                        paramsRead[9] = true;
                    }
                    else if (!paramsRead[10] && ((object)Reader.LocalName == (object)_id59_group && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Particle = Read44_XmlSchemaGroupRef(false, true);
                        paramsRead[10] = true;
                    }
                    else if (!paramsRead[10] && ((object)Reader.LocalName == (object)_id185_sequence && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Particle = Read53_XmlSchemaSequence(false, true);
                        paramsRead[10] = true;
                    }
                    else if (!paramsRead[10] && ((object)Reader.LocalName == (object)_id186_choice && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Particle = Read54_XmlSchemaChoice(false, true);
                        paramsRead[10] = true;
                    }
                    else if (!paramsRead[10] && ((object)Reader.LocalName == (object)_id187_all && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Particle = Read55_XmlSchemaAll(false, true);
                        paramsRead[10] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id109_attribute && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_11) == null) Reader.Skip(); else a_11.Add(Read36_XmlSchemaAttribute(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id110_attributeGroup && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_11) == null) Reader.Skip(); else a_11.Add(Read37_XmlSchemaAttributeGroupRef(false, true));
                    }
                    else if (!paramsRead[12] && ((object)Reader.LocalName == (object)_id112_anyAttribute && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@AnyAttribute = Read39_XmlSchemaAnyAttribute(false, true);
                        paramsRead[12] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:complexContent, http://www.w3.org/2001/XMLSchema:simpleContent, http://www.w3.org/2001/XMLSchema:group, http://www.w3.org/2001/XMLSchema:sequence, http://www.w3.org/2001/XMLSchema:choice, http://www.w3.org/2001/XMLSchema:all, http://www.w3.org/2001/XMLSchema:attribute, http://www.w3.org/2001/XMLSchema:attributeGroup, http://www.w3.org/2001/XMLSchema:anyAttribute");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:complexContent, http://www.w3.org/2001/XMLSchema:simpleContent, http://www.w3.org/2001/XMLSchema:group, http://www.w3.org/2001/XMLSchema:sequence, http://www.w3.org/2001/XMLSchema:choice, http://www.w3.org/2001/XMLSchema:all, http://www.w3.org/2001/XMLSchema:attribute, http://www.w3.org/2001/XMLSchema:attributeGroup, http://www.w3.org/2001/XMLSchema:anyAttribute");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations70, ref readerCount70);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaAll Read55_XmlSchemaAll(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id188_XmlSchemaAll && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaAll o;
            o = new Microsoft.Xml.Schema.XmlSchemaAll();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_6 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Items;
            bool[] paramsRead = new bool[7];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id164_minOccurs && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@MinOccursString = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id165_maxOccurs && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@MaxOccursString = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations71 = 0;
            int readerCount71 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id92_element && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read52_XmlSchemaElement(false, true));
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:element");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:element");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations71, ref readerCount71);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaChoice Read54_XmlSchemaChoice(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id189_XmlSchemaChoice && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaChoice o;
            o = new Microsoft.Xml.Schema.XmlSchemaChoice();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_6 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Items;
            bool[] paramsRead = new bool[7];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id164_minOccurs && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@MinOccursString = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id165_maxOccurs && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@MaxOccursString = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations72 = 0;
            int readerCount72 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id190_any && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read46_XmlSchemaAny(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id186_choice && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read54_XmlSchemaChoice(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id185_sequence && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read53_XmlSchemaSequence(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id92_element && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read52_XmlSchemaElement(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id59_group && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read44_XmlSchemaGroupRef(false, true));
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:any, http://www.w3.org/2001/XMLSchema:choice, http://www.w3.org/2001/XMLSchema:sequence, http://www.w3.org/2001/XMLSchema:element, http://www.w3.org/2001/XMLSchema:group");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:any, http://www.w3.org/2001/XMLSchema:choice, http://www.w3.org/2001/XMLSchema:sequence, http://www.w3.org/2001/XMLSchema:element, http://www.w3.org/2001/XMLSchema:group");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations72, ref readerCount72);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaGroupRef Read44_XmlSchemaGroupRef(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id191_XmlSchemaGroupRef && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaGroupRef o;
            o = new Microsoft.Xml.Schema.XmlSchemaGroupRef();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[7];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id164_minOccurs && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@MinOccursString = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id165_maxOccurs && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@MaxOccursString = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id123_ref && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@RefName = ToXmlQualifiedName(Reader.Value);
                    paramsRead[6] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations73 = 0;
            int readerCount73 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations73, ref readerCount73);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaSequence Read53_XmlSchemaSequence(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id192_XmlSchemaSequence && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaSequence o;
            o = new Microsoft.Xml.Schema.XmlSchemaSequence();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_6 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Items;
            bool[] paramsRead = new bool[7];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id164_minOccurs && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@MinOccursString = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id165_maxOccurs && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@MaxOccursString = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations74 = 0;
            int readerCount74 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id92_element && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read52_XmlSchemaElement(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id185_sequence && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read53_XmlSchemaSequence(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id190_any && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read46_XmlSchemaAny(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id186_choice && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read54_XmlSchemaChoice(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id59_group && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read44_XmlSchemaGroupRef(false, true));
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:element, http://www.w3.org/2001/XMLSchema:sequence, http://www.w3.org/2001/XMLSchema:any, http://www.w3.org/2001/XMLSchema:choice, http://www.w3.org/2001/XMLSchema:group");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:element, http://www.w3.org/2001/XMLSchema:sequence, http://www.w3.org/2001/XMLSchema:any, http://www.w3.org/2001/XMLSchema:choice, http://www.w3.org/2001/XMLSchema:group");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations74, ref readerCount74);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaAny Read46_XmlSchemaAny(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id193_XmlSchemaAny && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaAny o;
            o = new Microsoft.Xml.Schema.XmlSchemaAny();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[8];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id164_minOccurs && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@MinOccursString = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id165_maxOccurs && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@MaxOccursString = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id36_namespace && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Namespace = Reader.Value;
                    paramsRead[6] = true;
                }
                else if (!paramsRead[7] && ((object)Reader.LocalName == (object)_id114_processContents && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@ProcessContents = Read38_XmlSchemaContentProcessing(Reader.Value);
                    paramsRead[7] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations75 = 0;
            int readerCount75 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations75, ref readerCount75);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaSimpleContent Read61_XmlSchemaSimpleContent(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id194_XmlSchemaSimpleContent && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaSimpleContent o;
            o = new Microsoft.Xml.Schema.XmlSchemaSimpleContent();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[5];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations76 = 0;
            int readerCount76 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id131_restriction && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Content = Read59_Item(false, true);
                        paramsRead[4] = true;
                    }
                    else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id195_extension && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Content = Read60_Item(false, true);
                        paramsRead[4] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:restriction, http://www.w3.org/2001/XMLSchema:extension");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:restriction, http://www.w3.org/2001/XMLSchema:extension");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations76, ref readerCount76);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaSimpleContentExtension Read60_Item(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id196_Item && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaSimpleContentExtension o;
            o = new Microsoft.Xml.Schema.XmlSchemaSimpleContentExtension();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_5 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Attributes;
            bool[] paramsRead = new bool[7];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id136_base && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@BaseTypeName = ToXmlQualifiedName(Reader.Value);
                    paramsRead[4] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations77 = 0;
            int readerCount77 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id110_attributeGroup && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_5) == null) Reader.Skip(); else a_5.Add(Read37_XmlSchemaAttributeGroupRef(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id109_attribute && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_5) == null) Reader.Skip(); else a_5.Add(Read36_XmlSchemaAttribute(false, true));
                    }
                    else if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id112_anyAttribute && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@AnyAttribute = Read39_XmlSchemaAnyAttribute(false, true);
                        paramsRead[6] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:attributeGroup, http://www.w3.org/2001/XMLSchema:attribute, http://www.w3.org/2001/XMLSchema:anyAttribute");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:attributeGroup, http://www.w3.org/2001/XMLSchema:attribute, http://www.w3.org/2001/XMLSchema:anyAttribute");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations77, ref readerCount77);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaSimpleContentRestriction Read59_Item(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id197_Item && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaSimpleContentRestriction o;
            o = new Microsoft.Xml.Schema.XmlSchemaSimpleContentRestriction();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_6 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Facets;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_7 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Attributes;
            bool[] paramsRead = new bool[9];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id136_base && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@BaseTypeName = ToXmlQualifiedName(Reader.Value);
                    paramsRead[4] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations78 = 0;
            int readerCount78 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id105_simpleType && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@BaseType = Read34_XmlSchemaSimpleType(false, true);
                        paramsRead[5] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id138_minInclusive && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read21_XmlSchemaMinInclusiveFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id144_maxExclusive && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read28_XmlSchemaMaxExclusiveFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id145_whiteSpace && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read29_XmlSchemaWhiteSpaceFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id147_minLength && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read31_XmlSchemaMinLengthFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id62_pattern && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read25_XmlSchemaPatternFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id142_enumeration && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read26_XmlSchemaEnumerationFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id143_maxInclusive && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read27_XmlSchemaMaxInclusiveFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id140_length && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read23_XmlSchemaLengthFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id139_maxLength && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read22_XmlSchemaMaxLengthFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id146_minExclusive && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read30_XmlSchemaMinExclusiveFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id141_totalDigits && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read24_XmlSchemaTotalDigitsFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id137_fractionDigits && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read20_XmlSchemaFractionDigitsFacet(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id110_attributeGroup && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_7) == null) Reader.Skip(); else a_7.Add(Read37_XmlSchemaAttributeGroupRef(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id109_attribute && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_7) == null) Reader.Skip(); else a_7.Add(Read36_XmlSchemaAttribute(false, true));
                    }
                    else if (!paramsRead[8] && ((object)Reader.LocalName == (object)_id112_anyAttribute && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@AnyAttribute = Read39_XmlSchemaAnyAttribute(false, true);
                        paramsRead[8] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:simpleType, http://www.w3.org/2001/XMLSchema:minInclusive, http://www.w3.org/2001/XMLSchema:maxExclusive, http://www.w3.org/2001/XMLSchema:whiteSpace, http://www.w3.org/2001/XMLSchema:minLength, http://www.w3.org/2001/XMLSchema:pattern, http://www.w3.org/2001/XMLSchema:enumeration, http://www.w3.org/2001/XMLSchema:maxInclusive, http://www.w3.org/2001/XMLSchema:length, http://www.w3.org/2001/XMLSchema:maxLength, http://www.w3.org/2001/XMLSchema:minExclusive, http://www.w3.org/2001/XMLSchema:totalDigits, http://www.w3.org/2001/XMLSchema:fractionDigits, http://www.w3.org/2001/XMLSchema:attributeGroup, http://www.w3.org/2001/XMLSchema:attribute, http://www.w3.org/2001/XMLSchema:anyAttribute");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:simpleType, http://www.w3.org/2001/XMLSchema:minInclusive, http://www.w3.org/2001/XMLSchema:maxExclusive, http://www.w3.org/2001/XMLSchema:whiteSpace, http://www.w3.org/2001/XMLSchema:minLength, http://www.w3.org/2001/XMLSchema:pattern, http://www.w3.org/2001/XMLSchema:enumeration, http://www.w3.org/2001/XMLSchema:maxInclusive, http://www.w3.org/2001/XMLSchema:length, http://www.w3.org/2001/XMLSchema:maxLength, http://www.w3.org/2001/XMLSchema:minExclusive, http://www.w3.org/2001/XMLSchema:totalDigits, http://www.w3.org/2001/XMLSchema:fractionDigits, http://www.w3.org/2001/XMLSchema:attributeGroup, http://www.w3.org/2001/XMLSchema:attribute, http://www.w3.org/2001/XMLSchema:anyAttribute");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations78, ref readerCount78);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaComplexContent Read58_XmlSchemaComplexContent(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id198_XmlSchemaComplexContent && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaComplexContent o;
            o = new Microsoft.Xml.Schema.XmlSchemaComplexContent();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id182_mixed && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@IsMixed = Microsoft.Xml.XmlConvert.ToBoolean(Reader.Value);
                    paramsRead[4] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations79 = 0;
            int readerCount79 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id195_extension && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Content = Read56_Item(false, true);
                        paramsRead[5] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id131_restriction && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Content = Read57_Item(false, true);
                        paramsRead[5] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:extension, http://www.w3.org/2001/XMLSchema:restriction");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:extension, http://www.w3.org/2001/XMLSchema:restriction");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations79, ref readerCount79);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaComplexContentRestriction Read57_Item(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id199_Item && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaComplexContentRestriction o;
            o = new Microsoft.Xml.Schema.XmlSchemaComplexContentRestriction();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_6 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Attributes;
            bool[] paramsRead = new bool[8];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id136_base && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@BaseTypeName = ToXmlQualifiedName(Reader.Value);
                    paramsRead[4] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations80 = 0;
            int readerCount80 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id186_choice && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Particle = Read54_XmlSchemaChoice(false, true);
                        paramsRead[5] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id59_group && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Particle = Read44_XmlSchemaGroupRef(false, true);
                        paramsRead[5] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id187_all && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Particle = Read55_XmlSchemaAll(false, true);
                        paramsRead[5] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id185_sequence && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Particle = Read53_XmlSchemaSequence(false, true);
                        paramsRead[5] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id110_attributeGroup && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read37_XmlSchemaAttributeGroupRef(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id109_attribute && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read36_XmlSchemaAttribute(false, true));
                    }
                    else if (!paramsRead[7] && ((object)Reader.LocalName == (object)_id112_anyAttribute && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@AnyAttribute = Read39_XmlSchemaAnyAttribute(false, true);
                        paramsRead[7] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:choice, http://www.w3.org/2001/XMLSchema:group, http://www.w3.org/2001/XMLSchema:all, http://www.w3.org/2001/XMLSchema:sequence, http://www.w3.org/2001/XMLSchema:attributeGroup, http://www.w3.org/2001/XMLSchema:attribute, http://www.w3.org/2001/XMLSchema:anyAttribute");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:choice, http://www.w3.org/2001/XMLSchema:group, http://www.w3.org/2001/XMLSchema:all, http://www.w3.org/2001/XMLSchema:sequence, http://www.w3.org/2001/XMLSchema:attributeGroup, http://www.w3.org/2001/XMLSchema:attribute, http://www.w3.org/2001/XMLSchema:anyAttribute");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations80, ref readerCount80);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaComplexContentExtension Read56_Item(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id200_Item && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaComplexContentExtension o;
            o = new Microsoft.Xml.Schema.XmlSchemaComplexContentExtension();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_6 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Attributes;
            bool[] paramsRead = new bool[8];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id136_base && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@BaseTypeName = ToXmlQualifiedName(Reader.Value);
                    paramsRead[4] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations81 = 0;
            int readerCount81 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id59_group && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Particle = Read44_XmlSchemaGroupRef(false, true);
                        paramsRead[5] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id186_choice && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Particle = Read54_XmlSchemaChoice(false, true);
                        paramsRead[5] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id187_all && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Particle = Read55_XmlSchemaAll(false, true);
                        paramsRead[5] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id185_sequence && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Particle = Read53_XmlSchemaSequence(false, true);
                        paramsRead[5] = true;
                    }
                    else if (((object)Reader.LocalName == (object)_id110_attributeGroup && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read37_XmlSchemaAttributeGroupRef(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id109_attribute && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_6) == null) Reader.Skip(); else a_6.Add(Read36_XmlSchemaAttribute(false, true));
                    }
                    else if (!paramsRead[7] && ((object)Reader.LocalName == (object)_id112_anyAttribute && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@AnyAttribute = Read39_XmlSchemaAnyAttribute(false, true);
                        paramsRead[7] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:group, http://www.w3.org/2001/XMLSchema:choice, http://www.w3.org/2001/XMLSchema:all, http://www.w3.org/2001/XMLSchema:sequence, http://www.w3.org/2001/XMLSchema:attributeGroup, http://www.w3.org/2001/XMLSchema:attribute, http://www.w3.org/2001/XMLSchema:anyAttribute");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:group, http://www.w3.org/2001/XMLSchema:choice, http://www.w3.org/2001/XMLSchema:all, http://www.w3.org/2001/XMLSchema:sequence, http://www.w3.org/2001/XMLSchema:attributeGroup, http://www.w3.org/2001/XMLSchema:attribute, http://www.w3.org/2001/XMLSchema:anyAttribute");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations81, ref readerCount81);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaGroup Read63_XmlSchemaGroup(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id201_XmlSchemaGroup && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaGroup o;
            o = new Microsoft.Xml.Schema.XmlSchemaGroup();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations82 = 0;
            int readerCount82 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id185_sequence && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Particle = Read53_XmlSchemaSequence(false, true);
                        paramsRead[5] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id186_choice && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Particle = Read54_XmlSchemaChoice(false, true);
                        paramsRead[5] = true;
                    }
                    else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id187_all && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Particle = Read55_XmlSchemaAll(false, true);
                        paramsRead[5] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:sequence, http://www.w3.org/2001/XMLSchema:choice, http://www.w3.org/2001/XMLSchema:all");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:sequence, http://www.w3.org/2001/XMLSchema:choice, http://www.w3.org/2001/XMLSchema:all");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations82, ref readerCount82);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaNotation Read65_XmlSchemaNotation(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id202_XmlSchemaNotation && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaNotation o;
            o = new Microsoft.Xml.Schema.XmlSchemaNotation();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[7];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id4_name && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Name = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id203_public && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Public = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (!paramsRead[6] && ((object)Reader.LocalName == (object)_id204_system && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@System = Reader.Value;
                    paramsRead[6] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations83 = 0;
            int readerCount83 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[2] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations83, ref readerCount83);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaRedefine Read64_XmlSchemaRedefine(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id205_XmlSchemaRedefine && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaRedefine o;
            o = new Microsoft.Xml.Schema.XmlSchemaRedefine();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            Microsoft.Xml.Schema.XmlSchemaObjectCollection a_4 = (Microsoft.Xml.Schema.XmlSchemaObjectCollection)o.@Items;
            bool[] paramsRead = new bool[5];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id206_schemaLocation && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@SchemaLocation = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[2] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations84 = 0;
            int readerCount84 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (((object)Reader.LocalName == (object)_id110_attributeGroup && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read40_XmlSchemaAttributeGroup(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id106_complexType && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read62_XmlSchemaComplexType(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id105_simpleType && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read34_XmlSchemaSimpleType(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read11_XmlSchemaAnnotation(false, true));
                    }
                    else if (((object)Reader.LocalName == (object)_id59_group && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        if ((object)(a_4) == null) Reader.Skip(); else a_4.Add(Read63_XmlSchemaGroup(false, true));
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:attributeGroup, http://www.w3.org/2001/XMLSchema:complexType, http://www.w3.org/2001/XMLSchema:simpleType, http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:group");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:attributeGroup, http://www.w3.org/2001/XMLSchema:complexType, http://www.w3.org/2001/XMLSchema:simpleType, http://www.w3.org/2001/XMLSchema:annotation, http://www.w3.org/2001/XMLSchema:group");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations84, ref readerCount84);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaImport Read13_XmlSchemaImport(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id207_XmlSchemaImport && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaImport o;
            o = new Microsoft.Xml.Schema.XmlSchemaImport();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id206_schemaLocation && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@SchemaLocation = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[2] = true;
                }
                else if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id36_namespace && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Namespace = CollapseWhitespace(Reader.Value);
                    paramsRead[4] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations85 = 0;
            int readerCount85 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[5] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations85, ref readerCount85);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private Microsoft.Xml.Schema.XmlSchemaInclude Read12_XmlSchemaInclude(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id208_XmlSchemaInclude && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id95_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            DecodeName = false;
            Microsoft.Xml.Schema.XmlSchemaInclude o;
            o = new Microsoft.Xml.Schema.XmlSchemaInclude();
            Microsoft.Xml.XmlAttribute[] a_3 = null;
            int ca_3 = 0;
            bool[] paramsRead = new bool[5];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[1] && ((object)Reader.LocalName == (object)_id206_schemaLocation && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@SchemaLocation = CollapseWhitespace(Reader.Value);
                    paramsRead[1] = true;
                }
                else if (!paramsRead[2] && ((object)Reader.LocalName == (object)_id102_id && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Id = CollapseWhitespace(Reader.Value);
                    paramsRead[2] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_3 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute)); a_3[ca_3++] = attr;
                }
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations86 = 0;
            int readerCount86 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id107_annotation && (object)Reader.NamespaceURI == (object)_id95_Item))
                    {
                        o.@Annotation = Read11_XmlSchemaAnnotation(false, true);
                        paramsRead[4] = true;
                    }
                    else
                    {
                        UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://www.w3.org/2001/XMLSchema:annotation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations86, ref readerCount86);
            }
            o.@UnhandledAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_3, ca_3, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        private global::System.Web.Services.Description.Import Read4_Import(bool isNullable, bool checkType)
        {
            Microsoft.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType)
            {
                if (xsiType == null || ((object)((Microsoft.Xml.XmlQualifiedName)xsiType).Name == (object)_id209_Import && (object)((Microsoft.Xml.XmlQualifiedName)xsiType).Namespace == (object)_id2_Item))
                {
                }
                else
                    throw CreateUnknownTypeException((Microsoft.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::System.Web.Services.Description.Import o;
            o = new global::System.Web.Services.Description.Import();
            Microsoft.Xml.XmlAttribute[] a_1 = null;
            int ca_1 = 0;
            global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection a_3 = (global::System.Web.Services.Description.ServiceDescriptionFormatExtensionCollection)o.@Extensions;
            bool[] paramsRead = new bool[6];
            while (Reader.MoveToNextAttribute())
            {
                if (!paramsRead[4] && ((object)Reader.LocalName == (object)_id36_namespace && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Namespace = Reader.Value;
                    paramsRead[4] = true;
                }
                else if (!paramsRead[5] && ((object)Reader.LocalName == (object)_id23_location && (object)Reader.NamespaceURI == (object)_id5_Item))
                {
                    o.@Location = Reader.Value;
                    paramsRead[5] = true;
                }
                else if (IsXmlnsAttribute(Reader.Name))
                {
                    if (o.@Namespaces == null) o.@Namespaces = new Microsoft.Xml.Serialization.XmlSerializerNamespaces();
                    ((Microsoft.Xml.Serialization.XmlSerializerNamespaces)o.@Namespaces).Add(Reader.Name.Length == 5 ? "" : Reader.LocalName, Reader.Value);
                }
                else
                {
                    Microsoft.Xml.XmlAttribute attr = (Microsoft.Xml.XmlAttribute)Document.ReadNode(Reader);
                    ParseWsdlArrayType(attr);
                    a_1 = (Microsoft.Xml.XmlAttribute[])EnsureArrayIndex(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute)); a_1[ca_1++] = attr;
                }
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations87 = 0;
            int readerCount87 = ReaderCount;
            while (Reader.NodeType != Microsoft.Xml.XmlNodeType.EndElement && Reader.NodeType != Microsoft.Xml.XmlNodeType.None)
            {
                if (Reader.NodeType == Microsoft.Xml.XmlNodeType.Element)
                {
                    if (!paramsRead[0] && ((object)Reader.LocalName == (object)_id7_documentation && (object)Reader.NamespaceURI == (object)_id2_Item))
                    {
                        o.@DocumentationElement = (Microsoft.Xml.XmlElement)ReadXmlNode(false);
                        paramsRead[0] = true;
                    }
                    else
                    {
                        a_3.Add((Microsoft.Xml.XmlElement)ReadXmlNode(false));
                    }
                }
                else
                {
                    UnknownNode((object)o, @"http://schemas.xmlsoap.org/wsdl/:documentation");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations87, ref readerCount87);
            }
            o.@ExtensibleAttributes = (Microsoft.Xml.XmlAttribute[])ShrinkArray(a_1, ca_1, typeof(Microsoft.Xml.XmlAttribute), true);
            ReadEndElement();
            return o;
        }

        protected override void InitCallbacks()
        {
        }

        private string _id133_XmlSchemaSimpleTypeUnion;
        private string _id143_maxInclusive;
        private string _id46_body;
        private string _id190_any;
        private string _id88_OperationOutput;
        private string _id6_targetNamespace;
        private string _id158_XmlSchemaMaxLengthFacet;
        private string _id11_portType;
        private string _id182_mixed;
        private string _id172_keyref;
        private string _id187_all;
        private string _id162_itemType;
        private string _id68_InputBinding;
        private string _id25_HttpAddressBinding;
        private string _id82_HttpBinding;
        private string _id17_address;
        private string _id3_ServiceDescription;
        private string _id38_SoapFaultBinding;
        private string _id123_ref;
        private string _id198_XmlSchemaComplexContent;
        private string _id53_parts;
        private string _id35_use;
        private string _id157_XmlSchemaLengthFacet;
        private string _id207_XmlSchemaImport;
        private string _id44_text;
        private string _id117_XmlSchemaAppInfo;
        private string _id203_public;
        private string _id69_urlEncoded;
        private string _id7_documentation;
        private string _id19_Item;
        private string _id129_final;
        private string _id163_XmlSchemaElement;
        private string _id60_capture;
        private string _id37_encodingStyle;
        private string _id185_sequence;
        private string _id166_abstract;
        private string _id23_location;
        private string _id111_XmlSchemaAttributeGroup;
        private string _id192_XmlSchemaSequence;
        private string _id33_FaultBinding;
        private string _id153_XmlSchemaMaxInclusiveFacet;
        private string _id201_XmlSchemaGroup;
        private string _id43_multipartRelated;
        private string _id168_nillable;
        private string _id149_value;
        private string _id64_MimeMultipartRelatedBinding;
        private string _id193_XmlSchemaAny;
        private string _id191_XmlSchemaGroupRef;
        private string _id74_soapAction;
        private string _id63_ignoreCase;
        private string _id101_version;
        private string _id47_header;
        private string _id195_extension;
        private string _id48_Soap12HeaderBinding;
        private string _id134_memberTypes;
        private string _id121_Item;
        private string _id146_minExclusive;
        private string _id84_PortType;
        private string _id42_mimeXml;
        private string _id138_minInclusive;
        private string _id118_source;
        private string _id73_Soap12OperationBinding;
        private string _id131_restriction;
        private string _id152_XmlSchemaMaxExclusiveFacet;
        private string _id135_XmlSchemaSimpleTypeRestriction;
        private string _id188_XmlSchemaAll;
        private string _id116_appinfo;
        private string _id86_parameterOrder;
        private string _id147_minLength;
        private string _id78_HttpOperationBinding;
        private string _id161_XmlSchemaSimpleTypeList;
        private string _id205_XmlSchemaRedefine;
        private string _id194_XmlSchemaSimpleContent;
        private string _id91_MessagePart;
        private string _id92_element;
        private string _id114_processContents;
        private string _id18_Item;
        private string _id50_headerfault;
        private string _id154_XmlSchemaEnumerationFacet;
        private string _id96_XmlSchema;
        private string _id127_form;
        private string _id176_field;
        private string _id49_part;
        private string _id5_Item;
        private string _id57_match;
        private string _id52_Soap12BodyBinding;
        private string _id104_redefine;
        private string _id20_Item;
        private string _id21_Soap12AddressBinding;
        private string _id142_enumeration;
        private string _id24_SoapAddressBinding;
        private string _id103_include;
        private string _id139_maxLength;
        private string _id165_maxOccurs;
        private string _id65_MimePart;
        private string _id102_id;
        private string _id196_Item;
        private string _id140_length;
        private string _id27_type;
        private string _id106_complexType;
        private string _id31_output;
        private string _id1_definitions;
        private string _id4_name;
        private string _id132_union;
        private string _id29_OperationBinding;
        private string _id170_key;
        private string _id45_Item;
        private string _id95_Item;
        private string _id169_substitutionGroup;
        private string _id178_xpath;
        private string _id9_types;
        private string _id97_attributeFormDefault;
        private string _id62_pattern;
        private string _id58_MimeTextMatch;
        private string _id180_XmlSchemaKey;
        private string _id10_message;
        private string _id8_import;
        private string _id148_XmlSchemaMinLengthFacet;
        private string _id105_simpleType;
        private string _id181_XmlSchemaComplexType;
        private string _id164_minOccurs;
        private string _id144_maxExclusive;
        private string _id160_XmlSchemaFractionDigitsFacet;
        private string _id124_XmlSchemaAttribute;
        private string _id209_Import;
        private string _id206_schemaLocation;
        private string _id179_XmlSchemaUnique;
        private string _id75_style;
        private string _id119_XmlSchemaDocumentation;
        private string _id136_base;
        private string _id66_MimeXmlBinding;
        private string _id30_input;
        private string _id40_content;
        private string _id93_Types;
        private string _id94_schema;
        private string _id200_Item;
        private string _id67_MimeContentBinding;
        private string _id59_group;
        private string _id32_fault;
        private string _id80_transport;
        private string _id98_blockDefault;
        private string _id13_service;
        private string _id54_SoapHeaderBinding;
        private string _id204_system;
        private string _id16_Port;
        private string _id108_notation;
        private string _id186_choice;
        private string _id110_attributeGroup;
        private string _id79_Soap12Binding;
        private string _id77_SoapOperationBinding;
        private string _id115_XmlSchemaAnnotation;
        private string _id83_verb;
        private string _id72_HttpUrlEncodedBinding;
        private string _id39_OutputBinding;
        private string _id183_complexContent;
        private string _id202_XmlSchemaNotation;
        private string _id81_SoapBinding;
        private string _id199_Item;
        private string _id28_operation;
        private string _id122_XmlSchemaAttributeGroupRef;
        private string _id155_XmlSchemaPatternFacet;
        private string _id76_soapActionRequired;
        private string _id90_Message;
        private string _id159_XmlSchemaMinInclusiveFacet;
        private string _id208_XmlSchemaInclude;
        private string _id85_Operation;
        private string _id130_list;
        private string _id14_Service;
        private string _id22_required;
        private string _id174_refer;
        private string _id71_HttpUrlReplacementBinding;
        private string _id56_MimeTextBinding;
        private string _id87_OperationFault;
        private string _id125_default;
        private string _id15_port;
        private string _id51_SoapHeaderFaultBinding;
        private string _id128_XmlSchemaSimpleType;
        private string _id36_namespace;
        private string _id175_selector;
        private string _id150_XmlSchemaMinExclusiveFacet;
        private string _id100_elementFormDefault;
        private string _id26_Binding;
        private string _id197_Item;
        private string _id126_fixed;
        private string _id107_annotation;
        private string _id99_finalDefault;
        private string _id137_fractionDigits;
        private string _id70_urlReplacement;
        private string _id189_XmlSchemaChoice;
        private string _id2_Item;
        private string _id112_anyAttribute;
        private string _id89_OperationInput;
        private string _id141_totalDigits;
        private string _id61_repeats;
        private string _id184_simpleContent;
        private string _id55_SoapBodyBinding;
        private string _id145_whiteSpace;
        private string _id167_block;
        private string _id151_XmlSchemaWhiteSpaceFacet;
        private string _id12_binding;
        private string _id109_attribute;
        private string _id171_unique;
        private string _id120_lang;
        private string _id173_XmlSchemaKeyref;
        private string _id177_XmlSchemaXPath;
        private string _id34_Soap12FaultBinding;
        private string _id41_Item;
        private string _id156_XmlSchemaTotalDigitsFacet;
        private string _id113_XmlSchemaAnyAttribute;

        protected override void InitIDs()
        {
            _id133_XmlSchemaSimpleTypeUnion = Reader.NameTable.Add(@"XmlSchemaSimpleTypeUnion");
            _id143_maxInclusive = Reader.NameTable.Add(@"maxInclusive");
            _id46_body = Reader.NameTable.Add(@"body");
            _id190_any = Reader.NameTable.Add(@"any");
            _id88_OperationOutput = Reader.NameTable.Add(@"OperationOutput");
            _id6_targetNamespace = Reader.NameTable.Add(@"targetNamespace");
            _id158_XmlSchemaMaxLengthFacet = Reader.NameTable.Add(@"XmlSchemaMaxLengthFacet");
            _id11_portType = Reader.NameTable.Add(@"portType");
            _id182_mixed = Reader.NameTable.Add(@"mixed");
            _id172_keyref = Reader.NameTable.Add(@"keyref");
            _id187_all = Reader.NameTable.Add(@"all");
            _id162_itemType = Reader.NameTable.Add(@"itemType");
            _id68_InputBinding = Reader.NameTable.Add(@"InputBinding");
            _id25_HttpAddressBinding = Reader.NameTable.Add(@"HttpAddressBinding");
            _id82_HttpBinding = Reader.NameTable.Add(@"HttpBinding");
            _id17_address = Reader.NameTable.Add(@"address");
            _id3_ServiceDescription = Reader.NameTable.Add(@"ServiceDescription");
            _id38_SoapFaultBinding = Reader.NameTable.Add(@"SoapFaultBinding");
            _id123_ref = Reader.NameTable.Add(@"ref");
            _id198_XmlSchemaComplexContent = Reader.NameTable.Add(@"XmlSchemaComplexContent");
            _id53_parts = Reader.NameTable.Add(@"parts");
            _id35_use = Reader.NameTable.Add(@"use");
            _id157_XmlSchemaLengthFacet = Reader.NameTable.Add(@"XmlSchemaLengthFacet");
            _id207_XmlSchemaImport = Reader.NameTable.Add(@"XmlSchemaImport");
            _id44_text = Reader.NameTable.Add(@"text");
            _id117_XmlSchemaAppInfo = Reader.NameTable.Add(@"XmlSchemaAppInfo");
            _id203_public = Reader.NameTable.Add(@"public");
            _id69_urlEncoded = Reader.NameTable.Add(@"urlEncoded");
            _id7_documentation = Reader.NameTable.Add(@"documentation");
            _id19_Item = Reader.NameTable.Add(@"http://schemas.xmlsoap.org/wsdl/soap/");
            _id129_final = Reader.NameTable.Add(@"final");
            _id163_XmlSchemaElement = Reader.NameTable.Add(@"XmlSchemaElement");
            _id60_capture = Reader.NameTable.Add(@"capture");
            _id37_encodingStyle = Reader.NameTable.Add(@"encodingStyle");
            _id185_sequence = Reader.NameTable.Add(@"sequence");
            _id166_abstract = Reader.NameTable.Add(@"abstract");
            _id23_location = Reader.NameTable.Add(@"location");
            _id111_XmlSchemaAttributeGroup = Reader.NameTable.Add(@"XmlSchemaAttributeGroup");
            _id192_XmlSchemaSequence = Reader.NameTable.Add(@"XmlSchemaSequence");
            _id33_FaultBinding = Reader.NameTable.Add(@"FaultBinding");
            _id153_XmlSchemaMaxInclusiveFacet = Reader.NameTable.Add(@"XmlSchemaMaxInclusiveFacet");
            _id201_XmlSchemaGroup = Reader.NameTable.Add(@"XmlSchemaGroup");
            _id43_multipartRelated = Reader.NameTable.Add(@"multipartRelated");
            _id168_nillable = Reader.NameTable.Add(@"nillable");
            _id149_value = Reader.NameTable.Add(@"value");
            _id64_MimeMultipartRelatedBinding = Reader.NameTable.Add(@"MimeMultipartRelatedBinding");
            _id193_XmlSchemaAny = Reader.NameTable.Add(@"XmlSchemaAny");
            _id191_XmlSchemaGroupRef = Reader.NameTable.Add(@"XmlSchemaGroupRef");
            _id74_soapAction = Reader.NameTable.Add(@"soapAction");
            _id63_ignoreCase = Reader.NameTable.Add(@"ignoreCase");
            _id101_version = Reader.NameTable.Add(@"version");
            _id47_header = Reader.NameTable.Add(@"header");
            _id195_extension = Reader.NameTable.Add(@"extension");
            _id48_Soap12HeaderBinding = Reader.NameTable.Add(@"Soap12HeaderBinding");
            _id134_memberTypes = Reader.NameTable.Add(@"memberTypes");
            _id121_Item = Reader.NameTable.Add(@"http://www.w3.org/XML/1998/namespace");
            _id146_minExclusive = Reader.NameTable.Add(@"minExclusive");
            _id84_PortType = Reader.NameTable.Add(@"PortType");
            _id42_mimeXml = Reader.NameTable.Add(@"mimeXml");
            _id138_minInclusive = Reader.NameTable.Add(@"minInclusive");
            _id118_source = Reader.NameTable.Add(@"source");
            _id73_Soap12OperationBinding = Reader.NameTable.Add(@"Soap12OperationBinding");
            _id131_restriction = Reader.NameTable.Add(@"restriction");
            _id152_XmlSchemaMaxExclusiveFacet = Reader.NameTable.Add(@"XmlSchemaMaxExclusiveFacet");
            _id135_XmlSchemaSimpleTypeRestriction = Reader.NameTable.Add(@"XmlSchemaSimpleTypeRestriction");
            _id188_XmlSchemaAll = Reader.NameTable.Add(@"XmlSchemaAll");
            _id116_appinfo = Reader.NameTable.Add(@"appinfo");
            _id86_parameterOrder = Reader.NameTable.Add(@"parameterOrder");
            _id147_minLength = Reader.NameTable.Add(@"minLength");
            _id78_HttpOperationBinding = Reader.NameTable.Add(@"HttpOperationBinding");
            _id161_XmlSchemaSimpleTypeList = Reader.NameTable.Add(@"XmlSchemaSimpleTypeList");
            _id205_XmlSchemaRedefine = Reader.NameTable.Add(@"XmlSchemaRedefine");
            _id194_XmlSchemaSimpleContent = Reader.NameTable.Add(@"XmlSchemaSimpleContent");
            _id91_MessagePart = Reader.NameTable.Add(@"MessagePart");
            _id92_element = Reader.NameTable.Add(@"element");
            _id114_processContents = Reader.NameTable.Add(@"processContents");
            _id18_Item = Reader.NameTable.Add(@"http://schemas.xmlsoap.org/wsdl/http/");
            _id50_headerfault = Reader.NameTable.Add(@"headerfault");
            _id154_XmlSchemaEnumerationFacet = Reader.NameTable.Add(@"XmlSchemaEnumerationFacet");
            _id96_XmlSchema = Reader.NameTable.Add(@"XmlSchema");
            _id127_form = Reader.NameTable.Add(@"form");
            _id176_field = Reader.NameTable.Add(@"field");
            _id49_part = Reader.NameTable.Add(@"part");
            _id5_Item = Reader.NameTable.Add(@"");
            _id57_match = Reader.NameTable.Add(@"match");
            _id52_Soap12BodyBinding = Reader.NameTable.Add(@"Soap12BodyBinding");
            _id104_redefine = Reader.NameTable.Add(@"redefine");
            _id20_Item = Reader.NameTable.Add(@"http://schemas.xmlsoap.org/wsdl/soap12/");
            _id21_Soap12AddressBinding = Reader.NameTable.Add(@"Soap12AddressBinding");
            _id142_enumeration = Reader.NameTable.Add(@"enumeration");
            _id24_SoapAddressBinding = Reader.NameTable.Add(@"SoapAddressBinding");
            _id103_include = Reader.NameTable.Add(@"include");
            _id139_maxLength = Reader.NameTable.Add(@"maxLength");
            _id165_maxOccurs = Reader.NameTable.Add(@"maxOccurs");
            _id65_MimePart = Reader.NameTable.Add(@"MimePart");
            _id102_id = Reader.NameTable.Add(@"id");
            _id196_Item = Reader.NameTable.Add(@"XmlSchemaSimpleContentExtension");
            _id140_length = Reader.NameTable.Add(@"length");
            _id27_type = Reader.NameTable.Add(@"type");
            _id106_complexType = Reader.NameTable.Add(@"complexType");
            _id31_output = Reader.NameTable.Add(@"output");
            _id1_definitions = Reader.NameTable.Add(@"definitions");
            _id4_name = Reader.NameTable.Add(@"name");
            _id132_union = Reader.NameTable.Add(@"union");
            _id29_OperationBinding = Reader.NameTable.Add(@"OperationBinding");
            _id170_key = Reader.NameTable.Add(@"key");
            _id45_Item = Reader.NameTable.Add(@"http://microsoft.com/wsdl/mime/textMatching/");
            _id95_Item = Reader.NameTable.Add(@"http://www.w3.org/2001/XMLSchema");
            _id169_substitutionGroup = Reader.NameTable.Add(@"substitutionGroup");
            _id178_xpath = Reader.NameTable.Add(@"xpath");
            _id9_types = Reader.NameTable.Add(@"types");
            _id97_attributeFormDefault = Reader.NameTable.Add(@"attributeFormDefault");
            _id62_pattern = Reader.NameTable.Add(@"pattern");
            _id58_MimeTextMatch = Reader.NameTable.Add(@"MimeTextMatch");
            _id180_XmlSchemaKey = Reader.NameTable.Add(@"XmlSchemaKey");
            _id10_message = Reader.NameTable.Add(@"message");
            _id8_import = Reader.NameTable.Add(@"import");
            _id148_XmlSchemaMinLengthFacet = Reader.NameTable.Add(@"XmlSchemaMinLengthFacet");
            _id105_simpleType = Reader.NameTable.Add(@"simpleType");
            _id181_XmlSchemaComplexType = Reader.NameTable.Add(@"XmlSchemaComplexType");
            _id164_minOccurs = Reader.NameTable.Add(@"minOccurs");
            _id144_maxExclusive = Reader.NameTable.Add(@"maxExclusive");
            _id160_XmlSchemaFractionDigitsFacet = Reader.NameTable.Add(@"XmlSchemaFractionDigitsFacet");
            _id124_XmlSchemaAttribute = Reader.NameTable.Add(@"XmlSchemaAttribute");
            _id209_Import = Reader.NameTable.Add(@"Import");
            _id206_schemaLocation = Reader.NameTable.Add(@"schemaLocation");
            _id179_XmlSchemaUnique = Reader.NameTable.Add(@"XmlSchemaUnique");
            _id75_style = Reader.NameTable.Add(@"style");
            _id119_XmlSchemaDocumentation = Reader.NameTable.Add(@"XmlSchemaDocumentation");
            _id136_base = Reader.NameTable.Add(@"base");
            _id66_MimeXmlBinding = Reader.NameTable.Add(@"MimeXmlBinding");
            _id30_input = Reader.NameTable.Add(@"input");
            _id40_content = Reader.NameTable.Add(@"content");
            _id93_Types = Reader.NameTable.Add(@"Types");
            _id94_schema = Reader.NameTable.Add(@"schema");
            _id200_Item = Reader.NameTable.Add(@"XmlSchemaComplexContentExtension");
            _id67_MimeContentBinding = Reader.NameTable.Add(@"MimeContentBinding");
            _id59_group = Reader.NameTable.Add(@"group");
            _id32_fault = Reader.NameTable.Add(@"fault");
            _id80_transport = Reader.NameTable.Add(@"transport");
            _id98_blockDefault = Reader.NameTable.Add(@"blockDefault");
            _id13_service = Reader.NameTable.Add(@"service");
            _id54_SoapHeaderBinding = Reader.NameTable.Add(@"SoapHeaderBinding");
            _id204_system = Reader.NameTable.Add(@"system");
            _id16_Port = Reader.NameTable.Add(@"Port");
            _id108_notation = Reader.NameTable.Add(@"notation");
            _id186_choice = Reader.NameTable.Add(@"choice");
            _id110_attributeGroup = Reader.NameTable.Add(@"attributeGroup");
            _id79_Soap12Binding = Reader.NameTable.Add(@"Soap12Binding");
            _id77_SoapOperationBinding = Reader.NameTable.Add(@"SoapOperationBinding");
            _id115_XmlSchemaAnnotation = Reader.NameTable.Add(@"XmlSchemaAnnotation");
            _id83_verb = Reader.NameTable.Add(@"verb");
            _id72_HttpUrlEncodedBinding = Reader.NameTable.Add(@"HttpUrlEncodedBinding");
            _id39_OutputBinding = Reader.NameTable.Add(@"OutputBinding");
            _id183_complexContent = Reader.NameTable.Add(@"complexContent");
            _id202_XmlSchemaNotation = Reader.NameTable.Add(@"XmlSchemaNotation");
            _id81_SoapBinding = Reader.NameTable.Add(@"SoapBinding");
            _id199_Item = Reader.NameTable.Add(@"XmlSchemaComplexContentRestriction");
            _id28_operation = Reader.NameTable.Add(@"operation");
            _id122_XmlSchemaAttributeGroupRef = Reader.NameTable.Add(@"XmlSchemaAttributeGroupRef");
            _id155_XmlSchemaPatternFacet = Reader.NameTable.Add(@"XmlSchemaPatternFacet");
            _id76_soapActionRequired = Reader.NameTable.Add(@"soapActionRequired");
            _id90_Message = Reader.NameTable.Add(@"Message");
            _id159_XmlSchemaMinInclusiveFacet = Reader.NameTable.Add(@"XmlSchemaMinInclusiveFacet");
            _id208_XmlSchemaInclude = Reader.NameTable.Add(@"XmlSchemaInclude");
            _id85_Operation = Reader.NameTable.Add(@"Operation");
            _id130_list = Reader.NameTable.Add(@"list");
            _id14_Service = Reader.NameTable.Add(@"Service");
            _id22_required = Reader.NameTable.Add(@"required");
            _id174_refer = Reader.NameTable.Add(@"refer");
            _id71_HttpUrlReplacementBinding = Reader.NameTable.Add(@"HttpUrlReplacementBinding");
            _id56_MimeTextBinding = Reader.NameTable.Add(@"MimeTextBinding");
            _id87_OperationFault = Reader.NameTable.Add(@"OperationFault");
            _id125_default = Reader.NameTable.Add(@"default");
            _id15_port = Reader.NameTable.Add(@"port");
            _id51_SoapHeaderFaultBinding = Reader.NameTable.Add(@"SoapHeaderFaultBinding");
            _id128_XmlSchemaSimpleType = Reader.NameTable.Add(@"XmlSchemaSimpleType");
            _id36_namespace = Reader.NameTable.Add(@"namespace");
            _id175_selector = Reader.NameTable.Add(@"selector");
            _id150_XmlSchemaMinExclusiveFacet = Reader.NameTable.Add(@"XmlSchemaMinExclusiveFacet");
            _id100_elementFormDefault = Reader.NameTable.Add(@"elementFormDefault");
            _id26_Binding = Reader.NameTable.Add(@"Binding");
            _id197_Item = Reader.NameTable.Add(@"XmlSchemaSimpleContentRestriction");
            _id126_fixed = Reader.NameTable.Add(@"fixed");
            _id107_annotation = Reader.NameTable.Add(@"annotation");
            _id99_finalDefault = Reader.NameTable.Add(@"finalDefault");
            _id137_fractionDigits = Reader.NameTable.Add(@"fractionDigits");
            _id70_urlReplacement = Reader.NameTable.Add(@"urlReplacement");
            _id189_XmlSchemaChoice = Reader.NameTable.Add(@"XmlSchemaChoice");
            _id2_Item = Reader.NameTable.Add(@"http://schemas.xmlsoap.org/wsdl/");
            _id112_anyAttribute = Reader.NameTable.Add(@"anyAttribute");
            _id89_OperationInput = Reader.NameTable.Add(@"OperationInput");
            _id141_totalDigits = Reader.NameTable.Add(@"totalDigits");
            _id61_repeats = Reader.NameTable.Add(@"repeats");
            _id184_simpleContent = Reader.NameTable.Add(@"simpleContent");
            _id55_SoapBodyBinding = Reader.NameTable.Add(@"SoapBodyBinding");
            _id145_whiteSpace = Reader.NameTable.Add(@"whiteSpace");
            _id167_block = Reader.NameTable.Add(@"block");
            _id151_XmlSchemaWhiteSpaceFacet = Reader.NameTable.Add(@"XmlSchemaWhiteSpaceFacet");
            _id12_binding = Reader.NameTable.Add(@"binding");
            _id109_attribute = Reader.NameTable.Add(@"attribute");
            _id171_unique = Reader.NameTable.Add(@"unique");
            _id120_lang = Reader.NameTable.Add(@"lang");
            _id173_XmlSchemaKeyref = Reader.NameTable.Add(@"XmlSchemaKeyref");
            _id177_XmlSchemaXPath = Reader.NameTable.Add(@"XmlSchemaXPath");
            _id34_Soap12FaultBinding = Reader.NameTable.Add(@"Soap12FaultBinding");
            _id41_Item = Reader.NameTable.Add(@"http://schemas.xmlsoap.org/wsdl/mime/");
            _id156_XmlSchemaTotalDigitsFacet = Reader.NameTable.Add(@"XmlSchemaTotalDigitsFacet");
            _id113_XmlSchemaAnyAttribute = Reader.NameTable.Add(@"XmlSchemaAnyAttribute");
        }
    }
}

