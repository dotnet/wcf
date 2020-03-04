// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    /// <summary>
    /// Note: the order of the values matters as it is used as a context level.
    /// </summary>
    internal enum OperationalContext
    {
        // The tool works in the context of a project: DotNetCliToolReference.
        Project,
        // The-tool works as a CLI global tool.
        Global,
        // The tool works in either of the above modes but is invoked by an external tool like the WCF CS.
        Infrastructure,
        // The tool works in all modes when invoked by the bootstrapper.
        Bootstrapper
    }
}
