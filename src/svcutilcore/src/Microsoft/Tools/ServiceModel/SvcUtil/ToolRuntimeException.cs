// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.SvcUtil.XmlSerializer
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal class ToolRuntimeException : ApplicationException
    {
        internal virtual ToolExitCodes ExitCode
        {
            get
            {
                return ToolExitCodes.RuntimeError;
            }
        }

        internal ToolRuntimeException() : base()
        {
        }

        internal ToolRuntimeException(string message) : base(message)
        {
        }

        internal ToolRuntimeException(String message, Exception innerException) : base(message, innerException)
        {
        }

#pragma warning disable SYSLIB0051
        protected ToolRuntimeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#pragma warning restore SYSLIB0051
    }
}

