// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml
{
    using System;

    // An enumeration for the xml:space scope used in XmlReader and XmlWriter.
    public enum XmlSpace
    {
        // xml:space scope has not been specified.
        None = 0,

        // The xml:space scope is "default".
        Default = 1,

        // The xml:space scope is "preserve".
        Preserve = 2
    }
}
