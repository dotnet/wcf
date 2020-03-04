// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Schema {
				using System;
				using Microsoft.Xml;


    public sealed class XmlSchemaCompilationSettings {

        bool enableUpaCheck;

        public XmlSchemaCompilationSettings() {
            enableUpaCheck = true;
        }

        public bool EnableUpaCheck {
            get {
                return enableUpaCheck;
            }
            set {
                enableUpaCheck = value;
            }
        }
    }
    
}
