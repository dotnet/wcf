// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class BootstrapException : Exception
    {
        public int ExitCode { get; private set; } = (int)ToolExitCode.BootstrapError;

        public BootstrapException(string message) : base(message) { }
    }
}
