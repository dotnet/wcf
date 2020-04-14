// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class ToolRuntimeException : Exception
    {
        internal virtual ToolExitCode ExitCode { get; } = ToolExitCode.RuntimeError;

        public ToolRuntimeException()
        {
        }

        public ToolRuntimeException(string message) : base(message)
        {
        }

        public ToolRuntimeException(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

