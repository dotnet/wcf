//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.Tools.ServiceModel.SvcUtil
{
    using System;
    using System.Runtime.Serialization;
    
    [Serializable] 
    class ToolRuntimeException : ApplicationException 
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

        internal ToolRuntimeException(String message, Exception innerException) : base (message, innerException) 
        {
        }

        protected ToolRuntimeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }

}

