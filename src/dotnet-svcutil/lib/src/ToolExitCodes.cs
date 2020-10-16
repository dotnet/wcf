// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
