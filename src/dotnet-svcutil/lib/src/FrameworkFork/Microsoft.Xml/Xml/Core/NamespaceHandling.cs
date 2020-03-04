// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml {
				using System;
				
    
    //
    // NamespaceHandling speficies how should the XmlWriter handle namespaces.
    //  

    [Flags]
    public enum NamespaceHandling {
        
        //
        // Default behavior
        //
        Default             = 0x0,

        //
        // Duplicate namespace declarations will be removed
        //
        OmitDuplicates      = 0x1,
    }
}
