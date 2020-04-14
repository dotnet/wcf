// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using Microsoft.Xml;
using Microsoft.Xml.Schema;
using System.Diagnostics;
using System.Collections;

using System.Threading.Tasks;

namespace Microsoft.Xml
{
    using System;


    internal partial class XmlWrappingReader : XmlReader, IXmlLineInfo
    {
        public override Task<string> GetValueAsync()
        {
            return reader.GetValueAsync();
        }

        public override Task<bool> ReadAsync()
        {
            return reader.ReadAsync();
        }

        public override Task SkipAsync()
        {
            return reader.SkipAsync();
        }
    }
}

