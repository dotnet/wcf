// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.SvcUtil.XmlSerializer
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal class ToolOptionException : ToolArgumentException
    {
        internal ToolOptionException(String message)
            : base(message)
        {
        }

        internal ToolOptionException(string message, Exception e) : base(message, e) { }
    }
}

