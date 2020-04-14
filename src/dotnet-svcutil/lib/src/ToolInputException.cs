// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Runtime.Serialization;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class ToolInputException : ToolArgumentException
    {
        internal const int COR_E_ASSEMBLYEXPECTED = -2147024885;

        internal override ToolExitCode ExitCode
        {
            get
            {
                return ToolExitCode.InputError;
            }
        }

        internal ToolInputException() : base()
        {
        }

        internal ToolInputException(String message) : base(message)
        {
        }

        internal ToolInputException(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

