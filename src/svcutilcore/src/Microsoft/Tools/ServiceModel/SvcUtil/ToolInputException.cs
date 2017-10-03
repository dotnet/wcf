//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.Tools.ServiceModel.SvcUtil
{
    using System;
    using System.Runtime.Serialization;
    
    [Serializable] 
    class ToolInputException : ToolArgumentException
    {
        internal const int COR_E_ASSEMBLYEXPECTED = -2147024885;

        internal override ToolExitCodes ExitCode
        {
            get
            {
                return ToolExitCodes.InputError;
            }
        }

        internal ToolInputException() : base()
        {
        }

        internal ToolInputException(String message) : base(message)
        {
        }

        internal ToolInputException(String message, Exception innerException) : base (message, innerException) 
        {
        }

    }

}

