// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.


namespace Microsoft.Xml
{
    using System;

    // Specifies how entities are handled in XmlTextReader and XmlValidatingReader.
    public enum EntityHandling
    {
        // Expand all entities. This is the default in XmlValidatingReader. No nodes with NodeType EntityReference will be returned. 
        // The entity text is expanded in place of the entity references.
        ExpandEntities = 1,

        // Expand character entities only and return general entities as nodes (NodeType=XmlNodeType.EntityReference, Name=the name of the entity).
        // Default in XmlTextReader. You must call ResolveEntity to see what the general entity expands to.
        ExpandCharEntities = 2,
    }
}
