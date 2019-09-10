//------------------------------------------------------------------------------
// <copyright file="IXPathNavigable.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
// <owner current="true" primary="true">sdub</owner>
//------------------------------------------------------------------------------

namespace Microsoft.Xml.XPath {

    public interface IXPathNavigable {
        XPathNavigator CreateNavigator();
    }
}