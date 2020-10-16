// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Dispatcher;
    using Microsoft.Xml;
    using Microsoft.Xml.Schema;
    using Microsoft.Xml.Serialization;
    using WsdlNS = System.Web.Services.Description;

    internal static class StockSchemas
    {
        internal static XmlSchema CreateWsdl()
        {
            return XmlSchema.Read(new StringReader(wsdl), null);
        }
        internal static XmlSchema CreateSoap()
        {
            return XmlSchema.Read(new StringReader(soap), null);
        }

        internal static XmlSchema CreateSoapEncoding()
        {
            return XmlSchema.Read(new StringReader(soapEncoding), null);
        }

        internal static XmlSchema CreateFakeSoapEncoding()
        {
            return XmlSchema.Read(new StringReader(fakeSoapEncoding), null);
        }

        internal static XmlSchema CreateFakeXsdSchema()
        {
            return XmlSchema.Read(new StringReader(fakeXsd), null);
        }

        internal static XmlSchema CreateFakeXmlSchema()
        {
            return XmlSchema.Read(new StringReader(fakeXmlSchema), null);
        }

        internal static bool IsKnownSchema(string ns)
        {
            return ns == XmlSchema.Namespace || ns == "http://schemas.xmlsoap.org/wsdl/soap/" || ns == "http://schemas.xmlsoap.org/soap/encoding/";
        }

        internal const string WsdlNamespace = "http://schemas.xmlsoap.org/wsdl/";
        internal const string SoapNamespace = "http://schemas.xmlsoap.org/wsdl/soap/";
        internal const string SoapEncodingNamespace = "http://schemas.xmlsoap.org/soap/encoding/";

        private const string wsdl = @"<?xml version='1.0' encoding='UTF-8' ?> 
<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'
           xmlns:wsdl='http://schemas.xmlsoap.org/wsdl/'
           targetNamespace='http://schemas.xmlsoap.org/wsdl/'
           elementFormDefault='qualified' >
   
  <xs:complexType mixed='true' name='tDocumentation' >
    <xs:sequence>
      <xs:any minOccurs='0' maxOccurs='unbounded' processContents='lax' />
    </xs:sequence>
  </xs:complexType>

  <xs:complexType name='tDocumented' >
    <xs:annotation>
      <xs:documentation>
      This type is extended by  component types to allow them to be documented
      </xs:documentation>
    </xs:annotation>
    <xs:sequence>
      <xs:element name='documentation' type='wsdl:tDocumentation' minOccurs='0' />
    </xs:sequence>
  </xs:complexType>
 <!-- allow extensibility via elements and attributes on all elements swa124 -->
 <xs:complexType name='tExtensibleAttributesDocumented' abstract='true' >
    <xs:complexContent>
      <xs:extension base='wsdl:tDocumented' >
        <xs:annotation>
          <xs:documentation>
          This type is extended by component types to allow attributes from other namespaces to be added.
          </xs:documentation>
        </xs:annotation>
        <xs:sequence>
          <xs:any namespace='##other' minOccurs='0' maxOccurs='unbounded' processContents='lax' />
        </xs:sequence>
        <xs:anyAttribute namespace='##other' processContents='lax' />   
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:complexType name='tExtensibleDocumented' abstract='true' >
    <xs:complexContent>
      <xs:extension base='wsdl:tDocumented' >
        <xs:annotation>
          <xs:documentation>
          This type is extended by component types to allow elements from other namespaces to be added.
          </xs:documentation>
        </xs:annotation>
        <xs:sequence>
          <xs:any namespace='##other' minOccurs='0' maxOccurs='unbounded' processContents='lax' />
        </xs:sequence>
        <xs:anyAttribute namespace='##other' processContents='lax' />   
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <!-- original wsdl removed as part of swa124 resolution
  <xs:complexType name='tExtensibleAttributesDocumented' abstract='true' >
    <xs:complexContent>
      <xs:extension base='wsdl:tDocumented' >
        <xs:annotation>
          <xs:documentation>
          This type is extended by component types to allow attributes from other namespaces to be added.
          </xs:documentation>
        </xs:annotation>
        <xs:anyAttribute namespace='##other' processContents='lax' />    
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name='tExtensibleDocumented' abstract='true' >
    <xs:complexContent>
      <xs:extension base='wsdl:tDocumented' >
        <xs:annotation>
          <xs:documentation>
          This type is extended by component types to allow elements from other namespaces to be added.
          </xs:documentation>
        </xs:annotation>
        <xs:sequence>
          <xs:any namespace='##other' minOccurs='0' maxOccurs='unbounded' processContents='lax' />
        </xs:sequence>
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
 -->
  <xs:element name='definitions' type='wsdl:tDefinitions' >
    <xs:key name='message' >
      <xs:selector xpath='wsdl:message' />
      <xs:field xpath='@name' />
    </xs:key>
    <xs:key name='portType' >
      <xs:selector xpath='wsdl:portType' />
      <xs:field xpath='@name' />
    </xs:key>
    <xs:key name='binding' >
      <xs:selector xpath='wsdl:binding' />
      <xs:field xpath='@name' />
    </xs:key>
    <xs:key name='service' >
      <xs:selector xpath='wsdl:service' />
      <xs:field xpath='@name' />
    </xs:key>
    <xs:key name='import' >
      <xs:selector xpath='wsdl:import' />
      <xs:field xpath='@namespace' />
    </xs:key>
  </xs:element>

  <xs:group name='anyTopLevelOptionalElement' >
    <xs:annotation>
      <xs:documentation>
      Any top level optional element allowed to appear more then once - any child of definitions element except wsdl:types. Any extensibility element is allowed in any place.
      </xs:documentation>
    </xs:annotation>
    <xs:choice>
      <xs:element name='import' type='wsdl:tImport' />
      <xs:element name='types' type='wsdl:tTypes' />                     
      <xs:element name='message'  type='wsdl:tMessage' >
        <xs:unique name='part' >
          <xs:selector xpath='wsdl:part' />
          <xs:field xpath='@name' />
        </xs:unique>
      </xs:element>
      <xs:element name='portType' type='wsdl:tPortType' />
      <xs:element name='binding'  type='wsdl:tBinding' />
      <xs:element name='service'  type='wsdl:tService' >
        <xs:unique name='port' >
          <xs:selector xpath='wsdl:port' />
          <xs:field xpath='@name' />
        </xs:unique>
      </xs:element>
    </xs:choice>
  </xs:group>

  <xs:complexType name='tDefinitions' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:sequence>
          <xs:group ref='wsdl:anyTopLevelOptionalElement'  minOccurs='0'   maxOccurs='unbounded' />
        </xs:sequence>
        <xs:attribute name='targetNamespace' type='xs:anyURI' use='optional' />
        <xs:attribute name='name' type='xs:NCName' use='optional' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
   
  <xs:complexType name='tImport' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleAttributesDocumented' >
        <xs:attribute name='namespace' type='xs:anyURI' use='required' />
        <xs:attribute name='location' type='xs:anyURI' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
   
  <xs:complexType name='tTypes' >
    <xs:complexContent>   
      <xs:extension base='wsdl:tExtensibleDocumented' />
    </xs:complexContent>   
  </xs:complexType>
     
  <xs:complexType name='tMessage' >
    <xs:complexContent>   
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:sequence>
          <xs:element name='part' type='wsdl:tPart' minOccurs='0' maxOccurs='unbounded' />
        </xs:sequence>
        <xs:attribute name='name' type='xs:NCName' use='required' />
      </xs:extension>
    </xs:complexContent>   
  </xs:complexType>

  <xs:complexType name='tPart' >
    <xs:complexContent>   
      <xs:extension base='wsdl:tExtensibleAttributesDocumented' >
        <xs:attribute name='name' type='xs:NCName' use='required' />
        <xs:attribute name='element' type='xs:QName' use='optional' />
        <xs:attribute name='type' type='xs:QName' use='optional' />    
      </xs:extension>
    </xs:complexContent>   
  </xs:complexType>

  <xs:complexType name='tPortType' >
    <xs:complexContent>   
      <xs:extension base='wsdl:tExtensibleAttributesDocumented' >
        <xs:sequence>
          <xs:element name='operation' type='wsdl:tOperation' minOccurs='0' maxOccurs='unbounded' />
        </xs:sequence>
        <xs:attribute name='name' type='xs:NCName' use='required' />
      </xs:extension>
    </xs:complexContent>   
  </xs:complexType>
   
  <xs:complexType name='tOperation' >
    <xs:complexContent>   
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:sequence>
          <xs:choice>
            <xs:group ref='wsdl:request-response-or-one-way-operation' />
            <xs:group ref='wsdl:solicit-response-or-notification-operation' />
          </xs:choice>
        </xs:sequence>
        <xs:attribute name='name' type='xs:NCName' use='required' />
        <xs:attribute name='parameterOrder' type='xs:NMTOKENS' use='optional' />
      </xs:extension>
    </xs:complexContent>   
  </xs:complexType>
    
  <xs:group name='request-response-or-one-way-operation' >
    <xs:sequence>
      <xs:element name='input' type='wsdl:tParam' />
      <xs:sequence minOccurs='0' >
        <xs:element name='output' type='wsdl:tParam' />
        <xs:element name='fault' type='wsdl:tFault' minOccurs='0' maxOccurs='unbounded' />
      </xs:sequence>
    </xs:sequence>
  </xs:group>

  <xs:group name='solicit-response-or-notification-operation' >
    <xs:sequence>
      <xs:element name='output' type='wsdl:tParam' />
      <xs:sequence minOccurs='0' >
        <xs:element name='input' type='wsdl:tParam' />
        <xs:element name='fault' type='wsdl:tFault' minOccurs='0' maxOccurs='unbounded' />
      </xs:sequence>
    </xs:sequence>
  </xs:group>
        
  <xs:complexType name='tParam' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleAttributesDocumented' >
        <xs:attribute name='name' type='xs:NCName' use='optional' />
        <xs:attribute name='message' type='xs:QName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name='tFault' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleAttributesDocumented' >
        <xs:attribute name='name' type='xs:NCName'  use='required' />
        <xs:attribute name='message' type='xs:QName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
     
  <xs:complexType name='tBinding' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:sequence>
          <xs:element name='operation' type='wsdl:tBindingOperation' minOccurs='0' maxOccurs='unbounded' />
        </xs:sequence>
        <xs:attribute name='name' type='xs:NCName' use='required' />
        <xs:attribute name='type' type='xs:QName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
    
  <xs:complexType name='tBindingOperationMessage' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:attribute name='name' type='xs:NCName' use='optional' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  
  <xs:complexType name='tBindingOperationFault' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:attribute name='name' type='xs:NCName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:complexType name='tBindingOperation' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:sequence>
          <xs:element name='input' type='wsdl:tBindingOperationMessage' minOccurs='0' />
          <xs:element name='output' type='wsdl:tBindingOperationMessage' minOccurs='0' />
          <xs:element name='fault' type='wsdl:tBindingOperationFault' minOccurs='0' maxOccurs='unbounded' />
        </xs:sequence>
        <xs:attribute name='name' type='xs:NCName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
     
  <xs:complexType name='tService' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:sequence>
          <xs:element name='port' type='wsdl:tPort' minOccurs='0' maxOccurs='unbounded' />
        </xs:sequence>
        <xs:attribute name='name' type='xs:NCName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
     
  <xs:complexType name='tPort' >
    <xs:complexContent>
      <xs:extension base='wsdl:tExtensibleDocumented' >
        <xs:attribute name='name' type='xs:NCName' use='required' />
        <xs:attribute name='binding' type='xs:QName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>

  <xs:attribute name='arrayType' type='xs:string' />
  <xs:attribute name='required' type='xs:boolean' />
  <xs:complexType name='tExtensibilityElement' abstract='true' >
    <xs:attribute ref='wsdl:required' use='optional' />
  </xs:complexType>

</xs:schema>";

        private const string soap = @"<?xml version='1.0' encoding='UTF-8' ?> 
<xs:schema xmlns:soap='http://schemas.xmlsoap.org/wsdl/soap/' xmlns:wsdl='http://schemas.xmlsoap.org/wsdl/' targetNamespace='http://schemas.xmlsoap.org/wsdl/soap/' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xs:import namespace='http://schemas.xmlsoap.org/wsdl/' />
  <xs:simpleType name='encodingStyle'>
    <xs:annotation>
      <xs:documentation>
      'encodingStyle' indicates any canonicalization conventions followed in the contents of the containing element.  For example, the value 'http://schemas.xmlsoap.org/soap/encoding/' indicates the pattern described in SOAP specification
      </xs:documentation>
    </xs:annotation>
    <xs:list itemType='xs:anyURI' />
  </xs:simpleType>
  <xs:element name='binding' type='soap:tBinding' />
  <xs:complexType name='tBinding'>
    <xs:complexContent mixed='false'>
      <xs:extension base='wsdl:tExtensibilityElement'>
        <xs:attribute name='transport' type='xs:anyURI' use='required' />
        <xs:attribute name='style' type='soap:tStyleChoice' use='optional' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:simpleType name='tStyleChoice'>
    <xs:restriction base='xs:string'>
      <xs:enumeration value='rpc' />
      <xs:enumeration value='document' />
    </xs:restriction>
  </xs:simpleType>
  <xs:element name='operation' type='soap:tOperation' />
  <xs:complexType name='tOperation'>
    <xs:complexContent mixed='false'>
      <xs:extension base='wsdl:tExtensibilityElement'>
        <xs:attribute name='soapAction' type='xs:anyURI' use='optional' />
        <xs:attribute name='style' type='soap:tStyleChoice' use='optional' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:element name='body' type='soap:tBody' />
  <xs:attributeGroup name='tBodyAttributes'>
    <xs:attribute name='encodingStyle' type='soap:encodingStyle' use='optional' />
    <xs:attribute name='use' type='soap:useChoice' use='optional' />
    <xs:attribute name='namespace' type='xs:anyURI' use='optional' />
  </xs:attributeGroup>
  <xs:complexType name='tBody'>
    <xs:complexContent mixed='false'>
      <xs:extension base='wsdl:tExtensibilityElement'>
        <xs:attribute name='parts' type='xs:NMTOKENS' use='optional' />
        <xs:attributeGroup ref='soap:tBodyAttributes' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:simpleType name='useChoice'>
    <xs:restriction base='xs:string'>
      <xs:enumeration value='literal' />
      <xs:enumeration value='encoded' />
    </xs:restriction>
  </xs:simpleType>
  <xs:element name='fault' type='soap:tFault' />
  <xs:complexType name='tFaultRes' abstract='true'>
    <xs:complexContent mixed='false'>
      <xs:restriction base='soap:tBody'>
        <xs:attribute ref='wsdl:required' use='optional' />
        <xs:attribute name='parts' type='xs:NMTOKENS' use='prohibited' />
        <xs:attributeGroup ref='soap:tBodyAttributes' />
      </xs:restriction>
    </xs:complexContent>
  </xs:complexType>
  <xs:complexType name='tFault'>
    <xs:complexContent mixed='false'>
      <xs:extension base='soap:tFaultRes'>
        <xs:attribute name='name' type='xs:NCName' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:element name='header' type='soap:tHeader' />
  <xs:attributeGroup name='tHeaderAttributes'>
    <xs:attribute name='message' type='xs:QName' use='required' />
    <xs:attribute name='part' type='xs:NMTOKEN' use='required' />
    <xs:attribute name='use' type='soap:useChoice' use='required' />
    <xs:attribute name='encodingStyle' type='soap:encodingStyle' use='optional' />
    <xs:attribute name='namespace' type='xs:anyURI' use='optional' />
  </xs:attributeGroup>
  <xs:complexType name='tHeader'>
    <xs:complexContent mixed='false'>
      <xs:extension base='wsdl:tExtensibilityElement'>
        <xs:sequence>
          <xs:element minOccurs='0' maxOccurs='unbounded' ref='soap:headerfault' />
        </xs:sequence>
        <xs:attributeGroup ref='soap:tHeaderAttributes' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
  <xs:element name='headerfault' type='soap:tHeaderFault' />
  <xs:complexType name='tHeaderFault'>
    <xs:attributeGroup ref='soap:tHeaderAttributes' />
  </xs:complexType>
  <xs:element name='address' type='soap:tAddress' />
  <xs:complexType name='tAddress'>
    <xs:complexContent mixed='false'>
      <xs:extension base='wsdl:tExtensibilityElement'>
        <xs:attribute name='location' type='xs:anyURI' use='required' />
      </xs:extension>
    </xs:complexContent>
  </xs:complexType>
</xs:schema>";
        private const string soapEncoding = @"<?xml version='1.0' encoding='UTF-8' ?>
<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'
           xmlns:tns='http://schemas.xmlsoap.org/soap/encoding/'
           targetNamespace='http://schemas.xmlsoap.org/soap/encoding/' >
        
 <xs:attribute name='root' >
   <xs:simpleType>
     <xs:restriction base='xs:boolean'>
       <xs:pattern value='0|1' />
     </xs:restriction>
   </xs:simpleType>
 </xs:attribute>

  <xs:attributeGroup name='commonAttributes' >
    <xs:attribute name='id' type='xs:ID' />
    <xs:attribute name='href' type='xs:anyURI' />
    <xs:anyAttribute namespace='##other' processContents='lax' />
  </xs:attributeGroup>
   
  <xs:simpleType name='arrayCoordinate' >
    <xs:restriction base='xs:string' />
  </xs:simpleType>
          
  <xs:attribute name='arrayType' type='xs:string' />
  <xs:attribute name='offset' type='tns:arrayCoordinate' />
  
  <xs:attributeGroup name='arrayAttributes' >
    <xs:attribute ref='tns:arrayType' />
    <xs:attribute ref='tns:offset' />
  </xs:attributeGroup>    
  
  <xs:attribute name='position' type='tns:arrayCoordinate' /> 
  
  <xs:attributeGroup name='arrayMemberAttributes' >
    <xs:attribute ref='tns:position' />
  </xs:attributeGroup>    

  <xs:group name='Array' >
    <xs:sequence>
      <xs:any namespace='##any' minOccurs='0' maxOccurs='unbounded' processContents='lax' />
    </xs:sequence>
  </xs:group>

  <xs:element name='Array' type='tns:Array' />
  <xs:complexType name='Array' >
    <xs:group ref='tns:Array' minOccurs='0' />
    <xs:attributeGroup ref='tns:arrayAttributes' />
    <xs:attributeGroup ref='tns:commonAttributes' />
  </xs:complexType> 
  <xs:element name='Struct' type='tns:Struct' />
  <xs:group name='Struct' >
    <xs:sequence>
      <xs:any namespace='##any' minOccurs='0' maxOccurs='unbounded' processContents='lax' />
    </xs:sequence>
  </xs:group>

  <xs:complexType name='Struct' >
    <xs:group ref='tns:Struct' minOccurs='0' />
    <xs:attributeGroup ref='tns:commonAttributes'/>
  </xs:complexType> 
  
  <xs:simpleType name='base64' >
    <xs:restriction base='xs:base64Binary' />
  </xs:simpleType>

  <xs:element name='duration' type='tns:duration' />
  <xs:complexType name='duration' >
    <xs:simpleContent>
      <xs:extension base='xs:duration' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='dateTime' type='tns:dateTime' />
  <xs:complexType name='dateTime' >
    <xs:simpleContent>
      <xs:extension base='xs:dateTime' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>



  <xs:element name='NOTATION' type='tns:NOTATION' />
  <xs:complexType name='NOTATION' >
    <xs:simpleContent>
      <xs:extension base='xs:QName' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>
  

  <xs:element name='time' type='tns:time' />
  <xs:complexType name='time' >
    <xs:simpleContent>
      <xs:extension base='xs:time' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='date' type='tns:date' />
  <xs:complexType name='date' >
    <xs:simpleContent>
      <xs:extension base='xs:date' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='gYearMonth' type='tns:gYearMonth' />
  <xs:complexType name='gYearMonth' >
    <xs:simpleContent>
      <xs:extension base='xs:gYearMonth' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='gYear' type='tns:gYear' />
  <xs:complexType name='gYear' >
    <xs:simpleContent>
      <xs:extension base='xs:gYear' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='gMonthDay' type='tns:gMonthDay' />
  <xs:complexType name='gMonthDay' >
    <xs:simpleContent>
      <xs:extension base='xs:gMonthDay' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='gDay' type='tns:gDay' />
  <xs:complexType name='gDay' >
    <xs:simpleContent>
      <xs:extension base='xs:gDay' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='gMonth' type='tns:gMonth' />
  <xs:complexType name='gMonth' >
    <xs:simpleContent>
      <xs:extension base='xs:gMonth' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>
  
  <xs:element name='boolean' type='tns:boolean' />
  <xs:complexType name='boolean' >
    <xs:simpleContent>
      <xs:extension base='xs:boolean' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='base64Binary' type='tns:base64Binary' />
  <xs:complexType name='base64Binary' >
    <xs:simpleContent>
      <xs:extension base='xs:base64Binary' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='hexBinary' type='tns:hexBinary' />
  <xs:complexType name='hexBinary' >
    <xs:simpleContent>
     <xs:extension base='xs:hexBinary' >
       <xs:attributeGroup ref='tns:commonAttributes' />
     </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='float' type='tns:float' />
  <xs:complexType name='float' >
    <xs:simpleContent>
      <xs:extension base='xs:float' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='double' type='tns:double' />
  <xs:complexType name='double' >
    <xs:simpleContent>
      <xs:extension base='xs:double' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='anyURI' type='tns:anyURI' />
  <xs:complexType name='anyURI' >
    <xs:simpleContent>
      <xs:extension base='xs:anyURI' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='QName' type='tns:QName' />
  <xs:complexType name='QName' >
    <xs:simpleContent>
      <xs:extension base='xs:QName' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  
  <xs:element name='string' type='tns:string' />
  <xs:complexType name='string' >
    <xs:simpleContent>
      <xs:extension base='xs:string' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='normalizedString' type='tns:normalizedString' />
  <xs:complexType name='normalizedString' >
    <xs:simpleContent>
      <xs:extension base='xs:normalizedString' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='token' type='tns:token' />
  <xs:complexType name='token' >
    <xs:simpleContent>
      <xs:extension base='xs:token' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='language' type='tns:language' />
  <xs:complexType name='language' >
    <xs:simpleContent>
      <xs:extension base='xs:language' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='Name' type='tns:Name' />
  <xs:complexType name='Name' >
    <xs:simpleContent>
      <xs:extension base='xs:Name' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='NMTOKEN' type='tns:NMTOKEN' />
  <xs:complexType name='NMTOKEN' >
    <xs:simpleContent>
      <xs:extension base='xs:NMTOKEN' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='NCName' type='tns:NCName' />
  <xs:complexType name='NCName' >
    <xs:simpleContent>
      <xs:extension base='xs:NCName' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='NMTOKENS' type='tns:NMTOKENS' />
  <xs:complexType name='NMTOKENS' >
    <xs:simpleContent>
      <xs:extension base='xs:NMTOKENS' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='ID' type='tns:ID' />
  <xs:complexType name='ID' >
    <xs:simpleContent>
      <xs:extension base='xs:ID' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='IDREF' type='tns:IDREF' />
  <xs:complexType name='IDREF' >
    <xs:simpleContent>
      <xs:extension base='xs:IDREF' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='ENTITY' type='tns:ENTITY' />
  <xs:complexType name='ENTITY' >
    <xs:simpleContent>
      <xs:extension base='xs:ENTITY' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='IDREFS' type='tns:IDREFS' />
  <xs:complexType name='IDREFS' >
    <xs:simpleContent>
      <xs:extension base='xs:IDREFS' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='ENTITIES' type='tns:ENTITIES' />
  <xs:complexType name='ENTITIES' >
    <xs:simpleContent>
      <xs:extension base='xs:ENTITIES' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='decimal' type='tns:decimal' />
  <xs:complexType name='decimal' >
    <xs:simpleContent>
      <xs:extension base='xs:decimal' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='integer' type='tns:integer' />
  <xs:complexType name='integer' >
    <xs:simpleContent>
      <xs:extension base='xs:integer' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='nonPositiveInteger' type='tns:nonPositiveInteger' />
  <xs:complexType name='nonPositiveInteger' >
    <xs:simpleContent>
      <xs:extension base='xs:nonPositiveInteger' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='negativeInteger' type='tns:negativeInteger' />
  <xs:complexType name='negativeInteger' >
    <xs:simpleContent>
      <xs:extension base='xs:negativeInteger' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='long' type='tns:long' />
  <xs:complexType name='long' >
    <xs:simpleContent>
      <xs:extension base='xs:long' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='int' type='tns:int' />
  <xs:complexType name='int' >
    <xs:simpleContent>
      <xs:extension base='xs:int' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='short' type='tns:short' />
  <xs:complexType name='short' >
    <xs:simpleContent>
      <xs:extension base='xs:short' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='byte' type='tns:byte' />
  <xs:complexType name='byte' >
    <xs:simpleContent>
      <xs:extension base='xs:byte' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='nonNegativeInteger' type='tns:nonNegativeInteger' />
  <xs:complexType name='nonNegativeInteger' >
    <xs:simpleContent>
      <xs:extension base='xs:nonNegativeInteger' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='unsignedLong' type='tns:unsignedLong' />
  <xs:complexType name='unsignedLong' >
    <xs:simpleContent>
      <xs:extension base='xs:unsignedLong' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='unsignedInt' type='tns:unsignedInt' />
  <xs:complexType name='unsignedInt' >
    <xs:simpleContent>
      <xs:extension base='xs:unsignedInt' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='unsignedShort' type='tns:unsignedShort' />
  <xs:complexType name='unsignedShort' >
    <xs:simpleContent>
      <xs:extension base='xs:unsignedShort' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='unsignedByte' type='tns:unsignedByte' />
  <xs:complexType name='unsignedByte' >
    <xs:simpleContent>
      <xs:extension base='xs:unsignedByte' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='positiveInteger' type='tns:positiveInteger' />
  <xs:complexType name='positiveInteger' >
    <xs:simpleContent>
      <xs:extension base='xs:positiveInteger' >
        <xs:attributeGroup ref='tns:commonAttributes' />
      </xs:extension>
    </xs:simpleContent>
  </xs:complexType>

  <xs:element name='anyType' />
</xs:schema>";

        private const string fakeXsd = @"<?xml version='1.0' encoding='UTF-8' ?> 
<xsd:schema targetNamespace=""http://www.w3.org/2001/XMLSchema"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
    <xsd:element name=""schema"">
    <xsd:complexType />
    </xsd:element>
</xsd:schema>";

        private const string fakeXmlSchema = @"<xs:schema targetNamespace='http://www.w3.org/XML/1998/namespace' xmlns:xs='http://www.w3.org/2001/XMLSchema' xml:lang='en'>
 <xs:attribute name='lang' type='xs:language'/>
 <xs:attribute name='space'>
  <xs:simpleType>
   <xs:restriction base='xs:NCName'>
    <xs:enumeration value='default'/>
    <xs:enumeration value='preserve'/>
   </xs:restriction>
  </xs:simpleType>
 </xs:attribute>
 <xs:attribute name='base' type='xs:anyURI'/>
 <xs:attribute name='id' type='xs:ID' />
 <xs:attributeGroup name='specialAttrs'>
  <xs:attribute ref='xml:base'/>
  <xs:attribute ref='xml:lang'/>
  <xs:attribute ref='xml:space'/>
 </xs:attributeGroup>
</xs:schema>";


        private const string fakeSoapEncoding = @"<?xml version='1.0' encoding='UTF-8' ?>
<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'
           xmlns:tns='http://schemas.xmlsoap.org/soap/encoding/'
           targetNamespace='http://schemas.xmlsoap.org/soap/encoding/' >
        
  <xs:attributeGroup name='commonAttributes' >
    <xs:attribute name='id' type='xs:ID' />
    <xs:attribute name='href' type='xs:anyURI' />
    <xs:anyAttribute namespace='##other' processContents='lax' />
  </xs:attributeGroup>
   
  <xs:simpleType name='arrayCoordinate' >
    <xs:restriction base='xs:string' />
  </xs:simpleType>
          
  <xs:attribute name='arrayType' type='xs:string' />
  <xs:attribute name='offset' type='tns:arrayCoordinate' />
  
  <xs:attributeGroup name='arrayAttributes' >
    <xs:attribute ref='tns:arrayType' />
    <xs:attribute ref='tns:offset' />
  </xs:attributeGroup>    

  <xs:group name='Array' >
    <xs:sequence>
      <xs:any namespace='##any' minOccurs='0' maxOccurs='unbounded' processContents='lax' />
    </xs:sequence>
  </xs:group>

  <xs:element name='Array' type='tns:Array' />
  <xs:complexType name='Array' >
    <xs:group ref='tns:Array' minOccurs='0' />
    <xs:attributeGroup ref='tns:arrayAttributes' />
    <xs:attributeGroup ref='tns:commonAttributes' />
  </xs:complexType> 
</xs:schema>";
    }
}
