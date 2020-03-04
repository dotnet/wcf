// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Schema {
				using System;
				using Microsoft.Xml;


    using Microsoft.Xml.Serialization;

    /// <include file='doc\XmlSchemaSimpleTypeList.uex' path='docs/doc[@for="XmlSchemaSimpleTypeList"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class XmlSchemaSimpleTypeList : XmlSchemaSimpleTypeContent {
        XmlQualifiedName itemTypeName = XmlQualifiedName.Empty; 
        XmlSchemaSimpleType itemType;
        XmlSchemaSimpleType baseItemType; //Compiled
        
        /// <include file='doc\XmlSchemaSimpleTypeList.uex' path='docs/doc[@for="XmlSchemaSimpleTypeList.ItemTypeName"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlAttribute("itemType")]
        public XmlQualifiedName ItemTypeName { 
            get { return itemTypeName; }
            set { itemTypeName = (value == null ? XmlQualifiedName.Empty : value); }
        }

        /// <include file='doc\XmlSchemaSimpleTypeList.uex' path='docs/doc[@for="XmlSchemaSimpleTypeList.BaseType"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
        public XmlSchemaSimpleType ItemType {
            get { return itemType; }
            set { itemType = value; }
        }
        
        //Compiled
        /// <include file='doc\XmlSchemaSimpleTypeList.uex' path='docs/doc[@for="XmlSchemaSimpleTypeList.BaseItemType"]/*' />
        [XmlIgnore]
        public XmlSchemaSimpleType BaseItemType {
            get { return baseItemType; }
            set { baseItemType = value; }
        }

        internal override XmlSchemaObject Clone() {
            XmlSchemaSimpleTypeList newList = (XmlSchemaSimpleTypeList)MemberwiseClone();
            newList.ItemTypeName = itemTypeName.Clone();
            return newList;
        }
    }

}
