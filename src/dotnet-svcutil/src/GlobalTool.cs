// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Linq;
using System.Collections.Generic;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    // The global tool is a thin wrapper that just references dotnet-svcutil-lib as a library and calls its Main method.
    //
    // Note: In the future we would like to turn dotnet-svcutil-lib into a true library so the WCF Connected Service uses it as a PackageReference instead of a DotNetCliToolReference.
    // Once this is done we should move any dotnet-svcutil tool specific logic into this project. For example, command line processesing and help display.
    class GlobalTool
    {
        static int Main(string[] args)
        {
            var arguments = new List<string>(args);

            if (!arguments.Contains("--toolContext", StringComparer.InvariantCultureIgnoreCase) && !arguments.Contains("-tc", StringComparer.InvariantCultureIgnoreCase))
            {
                arguments.Add("--toolContext");
                arguments.Add("Global");
            }

            return Tool.Main(arguments.ToArray());
        }
    }
}
