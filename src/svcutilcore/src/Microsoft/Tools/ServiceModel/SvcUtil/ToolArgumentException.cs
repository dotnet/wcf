//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.Tools.ServiceModel.SvcUtil
{
    using System;
    using System.Runtime.Serialization;
    
    [Serializable] 
    class ToolArgumentException : ArgumentException 
    {

        internal virtual ToolExitCodes ExitCode
        {
            get
            {
                return ToolExitCodes.ArgumentError;
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

        protected ToolArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

    }

}

