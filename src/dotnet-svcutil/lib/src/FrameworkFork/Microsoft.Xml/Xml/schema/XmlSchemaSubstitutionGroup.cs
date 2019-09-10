//------------------------------------------------------------------------------
// <copyright file="XmlSchemaSubstitutionGroup.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">priyal</owner>                                                                 
//------------------------------------------------------------------------------

namespace Microsoft.Xml.Schema {
				using System;
				using Microsoft.Xml;


    using System.Collections;
    using Microsoft.Xml.Serialization;

    internal class XmlSchemaSubstitutionGroup : XmlSchemaObject {
        ArrayList membersList = new ArrayList();
        XmlQualifiedName examplar = XmlQualifiedName.Empty;

        [XmlIgnore]
        internal ArrayList Members {
            get { return membersList; }
        } 

        [XmlIgnore]
        internal XmlQualifiedName Examplar {
            get { return examplar; }
            set { examplar = value; }
        }
    }

    internal class XmlSchemaSubstitutionGroupV1Compat : XmlSchemaSubstitutionGroup {
        XmlSchemaChoice choice = new XmlSchemaChoice();

        [XmlIgnore]
        internal XmlSchemaChoice Choice {
            get { return choice; }
        }          

    }
}
