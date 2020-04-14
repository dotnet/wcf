// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using Microsoft.Xml.Schema;

namespace Microsoft.Xml
{
    using System;

    internal interface IValidationEventHandling
    {
        // This is a ValidationEventHandler, but it is not strongly typed due to dependencies on Microsoft.Xml.Schema
        object EventHandler { get; }

        // The exception is XmlSchemaException, but it is not strongly typed due to dependencies on Microsoft.Xml.Schema
        void SendEvent(Exception exception, XmlSeverityType severity);
    }
}
