// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

