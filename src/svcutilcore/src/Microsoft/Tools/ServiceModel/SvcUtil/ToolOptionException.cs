//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.Tools.ServiceModel.SvcUtil
{
    using System;
    using System.Runtime.Serialization;
    
    [Serializable] 
    class ToolOptionException : ToolArgumentException 
    {

        internal ToolOptionException(String message)
            : base(message)
        {
        }

        internal ToolOptionException(string message, Exception e) : base(message, e) { }

    }

}

