// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.SvcUtil.XmlSerializer
{
    using System;

    [Flags]
    internal enum ToolMode
    {
        DisplayHelp = 0x01,
        XmlSerializerGeneration = 0x80,

        //Do not remove
        None = 0x00,
        Any = 0x1FF,
    }

    internal class InvalidToolModeException : InvalidOperationException { }
}
