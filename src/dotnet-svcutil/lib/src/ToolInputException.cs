//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    class ToolInputException : ToolArgumentException
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

