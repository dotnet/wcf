//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace Microsoft.Tools.ServiceModel.Svcutil
{
    enum ToolExitCode : int
    {
        Success = 0,
        ValidationError = 1,
        ArgumentError = 2,
        InputError = 3,
        RuntimeError = 4,
        BootstrapError = 5,
        ValidationErrorTurnedWarning = 6,
        Unknown = 9
    }
}
