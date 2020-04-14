// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.


using System;
using Microsoft.Xml;

namespace Microsoft.Xml
{
    using System;


    internal partial interface IDtdParser
    {
        IDtdInfo ParseInternalDtd(IDtdParserAdapter adapter, bool saveInternalSubset);
        IDtdInfo ParseFreeFloatingDtd(string baseUri, string docTypeName, string publicId, string systemId, string internalSubset, IDtdParserAdapter adapter);
    }
}
