// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal partial class DebugUtils
    {
        public const string SvcutilDebugVariableEnvVar = "SVCUTIL_DEBUG";
        public const string SvcutilKeepBootstrapDirEnvVar = "SVCUTIL_KEEP_BOOTSTRAPDIR";

        public static bool KeepTemporaryDirs
        {
#if DEBUG
            set { Environment.SetEnvironmentVariable(SvcutilKeepBootstrapDirEnvVar, value ? "1" : string.Empty); }
            get { return Int32.TryParse(Environment.GetEnvironmentVariable(SvcutilKeepBootstrapDirEnvVar), out int ret) ? (ret > 0 ? true : false) : false; }
#else
            set { }
            get { return false; }
#endif
        }

#if DEBUG
        public static int SvcutilDebug
        {
            set { Environment.SetEnvironmentVariable(SvcutilDebugVariableEnvVar, value.ToString(CultureInfo.InvariantCulture)); }
            get { return Int32.TryParse(Environment.GetEnvironmentVariable(SvcutilDebugVariableEnvVar), out int ret) ? ret : 0; }
        }

        public static void SetupDebugging()
        {
            int flag = SvcutilDebug;
            if (flag == 1)
            {
                // System.Diagnostics.Deubgger.Launch() does not return for .net core projects.
                // for it to work a debugger needs to be attached to the process directly as opposed to starting the debugger 
                // from the debug window, then the debug window can be closed; but sometimes the process dies.
                var process = Process.GetCurrentProcess();
                Console.WriteLine("Waiting for debugger to attach. Press ENTER to continue");
                Console.WriteLine("Process ID: {0}, Name:{1}", process.Id, process.ProcessName);
                Console.ReadLine();
            }
            else if (flag == 2)
            {
                Debugger.Launch();
            }
        }
#endif
    }
}
