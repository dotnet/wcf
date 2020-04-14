// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.Schema
{
    using System;
    using Microsoft.Xml;


    public sealed class XmlSchemaCompilationSettings
    {
        private bool _enableUpaCheck;

        public XmlSchemaCompilationSettings()
        {
            _enableUpaCheck = true;
        }

        public bool EnableUpaCheck
        {
            get
            {
                return _enableUpaCheck;
            }
            set
            {
                _enableUpaCheck = value;
            }
        }
    }
}
