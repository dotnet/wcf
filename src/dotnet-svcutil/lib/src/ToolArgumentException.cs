//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    class ToolArgumentException : ArgumentException 
    {

        internal virtual ToolExitCode ExitCode
        {
            get
            {
                return ToolExitCode.ArgumentError;
            }
        }

        internal ToolArgumentException() : base()
        {
        }

        internal ToolArgumentException(String message) : base(message)
        {
        }

        internal ToolArgumentException(String message, Exception innerException) : base (message, innerException) 
        {
        }
    }

}

