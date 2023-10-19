// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.SvcUtil.XmlSerializer
{
    using System;
    using System.Text;
    using System.Globalization;
    using System.Diagnostics;
    using DcNS = System.Runtime.Serialization;
    using System.Threading;
    using System.Runtime.ConstrainedExecution;
    using System.Reflection;

    public static class Tool
    {
        internal static Assembly SMAssembly;
        public static int Main(string[] args)
        {
            try
            {
                // ValidateUICulture() makes sure that this command-line tool can run on RightToLeft systems.
                ValidateUICulture();

                Options options = Options.ParseArguments(args);
                ToolRuntime runtime = new ToolRuntime(options);
                return (int)runtime.Run();
            }
            catch (ToolArgumentException ae)
            {
                ToolConsole.WriteHeader();
                ToolConsole.WriteToolError(ae);
                return (int)ae.ExitCode;
            }
            catch (ToolRuntimeException re)
            {
                ToolConsole.WriteToolError(re);
                return (int)re.ExitCode;
            }
#pragma warning suppress 56500 // covered by FxCOP
            catch (Exception e)
            {
                if (Tool.IsFatal(e))
                    throw;

                ToolConsole.WriteUnexpectedError(e);
                Tool.FailFast(e.ToString());
                return (int)ToolExitCodes.Unknown; // unreachable code;
            }
        }

        internal static void Assert(bool condition, string message)
        {
            if (!condition)
            {
#if DEBUG
                ToolConsole.WriteError("Please file a bug or report the following issue with this tool:");
                StackTrace st = new StackTrace(true);
                ToolConsole.WriteLine(st.ToString());
#endif
                ToolConsole.WriteUnexpectedError(message);
                Tool.FailFast(message);
            }
        }

        internal static void FailFast(string message)
        {
            System.Environment.Exit((int)ToolExitCodes.Unknown);
        }

#pragma warning disable SYSLIB0004 // Type or member is obsolete
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
#pragma warning restore SYSLIB0004 // Type or member is obsolete
        internal static bool IsFatal(Exception exception)
        {
            return exception != null && (
                exception is OutOfMemoryException ||
                exception is ThreadAbortException ||
                exception is StackOverflowException ||
                exception is AccessViolationException);
        }

        // The following is needed for any command-line tool to run on RightToLeft systems.
        // Console applications, which employ the text user interface of the operating system 
        // console, do not provide RightToLeft support. Consequently, if a developer localizes 
        // a console application to Arabic or Hebrew, the application will display unreadable 
        // text on the console screen. We have to use the GetConsoleFallbackUICulture method 
        // to retrieve a neutral culture suitable for a console application user interface, 
        // because svcutil.exe displays (localized) errors thrown by Indigo and .Net Framework.
        // There are some console fallback cultures that still use code pages incompatible with
        // the console.  Catch those cases and fall back to English because ASCII is widely
        // accepted in OEM code pages.
        private static void ValidateUICulture()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture.GetConsoleFallbackUICulture();


            if ((System.Console.OutputEncoding.CodePage != Encoding.UTF8.CodePage) &&
                (System.Console.OutputEncoding.CodePage != Thread.CurrentThread.CurrentUICulture.TextInfo.OEMCodePage))
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            }
        }
    }
}

