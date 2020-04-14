// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal enum ToolExitCode : int
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
