//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace Microsoft.Tools.ServiceModel.SvcUtil
{
    enum ToolExitCodes : int
    {
        Success = 0,

        ValidationError = 1,

        ArgumentError = 2,
        InputError = 3,
        RuntimeError = 4,
        Unknown = 9
    }
}
