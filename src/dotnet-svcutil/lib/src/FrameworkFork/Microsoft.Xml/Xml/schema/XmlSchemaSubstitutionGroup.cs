// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Schema
{
    using System;
    using Microsoft.Xml;


    using System.Collections;
    using Microsoft.Xml.Serialization;

    internal class XmlSchemaSubstitutionGroup : XmlSchemaObject
    {
        private ArrayList _membersList = new ArrayList();
        private XmlQualifiedName _examplar = XmlQualifiedName.Empty;

        [XmlIgnore]
        internal ArrayList Members
        {
            get { return _membersList; }
        }

        [XmlIgnore]
        internal XmlQualifiedName Examplar
        {
            get { return _examplar; }
            set { _examplar = value; }
        }
    }

    internal class XmlSchemaSubstitutionGroupV1Compat : XmlSchemaSubstitutionGroup
    {
        private XmlSchemaChoice _choice = new XmlSchemaChoice();

        [XmlIgnore]
        internal XmlSchemaChoice Choice
        {
            get { return _choice; }
        }
    }
}
