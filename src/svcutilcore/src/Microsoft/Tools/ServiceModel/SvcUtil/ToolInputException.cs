// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.SvcUtil
{
    using System;

    [Serializable]
    internal class ToolInputException : ToolArgumentException
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

        internal ToolInputException(String message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

