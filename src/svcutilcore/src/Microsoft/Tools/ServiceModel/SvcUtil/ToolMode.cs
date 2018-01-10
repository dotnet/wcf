// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.SvcUtil
{
    using System;

    [Flags]
    internal enum ToolMode
    {
        DisplayHelp = 0x01,
        MetadataFromAssembly = 0x02,
        ProxyGeneration = 0x04,
        WSMetadataExchange = 0x08,
        Validate = 0x10,
        DataContractImport = 0x20,
        DataContractExport = 0x40,
        XmlSerializerGeneration = 0x80,
        ServiceContractGeneration = 0x100,

        //Do not remove
        None = 0x00,
        Any = 0x1FF,
    }

    internal class InvalidToolModeException : InvalidOperationException { }
}
