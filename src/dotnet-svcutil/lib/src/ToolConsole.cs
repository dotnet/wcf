// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.ServiceModel.Description;
using System.Text;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal static class ToolConsole
    {
        public const string Space = " ";

        public static ToolExitCode ExitCode { get; set; } = ToolExitCode.Success;

        public static Verbosity Verbosity { get; set; }

        /// <summary>
        /// Enable this option would append the addition markup in the front of each output console message. It's for 
        /// the purpose to retrive more information while these messages had been redirected to rich UI. This option 
        /// would also enable passing telemetry data through console pipeline so it's hidden from command line help to
        /// prevent unexpected usage.
        /// </summary>
        public static bool IsMarkupEnabled { get; set; }

        public static OperationalContext ToolModeLevel { get; set; }

        public static void Init(CommandProcessorOptions options)
        {
            Verbosity = options.Verbosity.Value;
            IsMarkupEnabled = options.EnableLoggingMarkup == true;
            ToolModeLevel = options.ToolContext.Value;
        }

        internal static void WriteToolError(Exception ex)
        {
            StringBuilder toolError = new StringBuilder();
            toolError.AppendLine(Utils.GetExceptionMessage(ex));

            if (ex is ToolMexException me)
            {
                toolError.AppendLine(string.Format(SR.WrnWSMExFailedFormat, me.ServiceUri?.AbsoluteUri));
                toolError.AppendLine();
                toolError.AppendLine(Utils.GetExceptionMessage(me.WSMexException));
            }

            if (ToolConsole.Verbosity > Verbosity.Minimal && ToolConsole.ToolModeLevel != OperationalContext.Infrastructure)
            {
                toolError.AppendLine();
                toolError.AppendLine(string.Format(SR.MoreHelpFormat, CommandProcessorOptions.Switches.Help.Abbreviation));
            }

            MarkupTelemetryHelper.TelemetryPostFault(ex);
            WriteError(toolError.ToString());
        }

        internal static void WriteError(string errMsg, bool isTrace = false)
        {
            WriteError(errMsg, SR.ErrorPrefix, isTrace);
        }

        internal static void WriteError(Exception e)
        {
            WriteError(Utils.GetExceptionMessage(e), SR.ErrorPrefix);
        }

        internal static void WriteError(Exception e, string prefix)
        {
            MarkupTelemetryHelper.TelemetryPostFault(e);
            WriteError(Utils.GetExceptionMessage(e), prefix);
        }

        internal static void WriteError(string errMsg, string prefix, bool isTrace = false)
        {
            errMsg = errMsg?.Trim();

            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                if (!string.IsNullOrWhiteSpace(prefix))
                {
                    errMsg = string.Format(CultureInfo.CurrentCulture, "{0} {1}", prefix, errMsg);
                }

                WriteLine(errMsg, isTrace ? LogTag.LogMessage : LogTag.Error);
            }
        }

        internal static void WriteWarning(string message, bool isTrace = false)
        {
            if (message != null && !message.StartsWith(SR.WarningPrefix, StringComparison.CurrentCulture))
            {
                message = string.Format(CultureInfo.CurrentCulture, "{0} {1}", SR.WarningPrefix, message);
            }

            WriteLine(message, isTrace ? LogTag.LogMessage : LogTag.Warning);
        }

        internal static void WriteLine(string str)
        {
            WriteLine(str, LogTag.Information, LogTag.NewLine);
        }

        internal static void WriteLine(string str, LogTag logTag)
        {
            WriteLine(str, logTag, LogTag.NewLine);
        }

        internal static void WriteLineIf(bool condition, string str)
        {
            WriteLineIf(condition, str, LogTag.Information);
        }

        internal static void WriteLineIf(bool condition, string str, LogTag logTag)
        {
            if (condition)
            {
                WriteLine(str, logTag, LogTag.NewLine);
            }
        }

        internal static void WriteLine(string str, LogTag logTag, string newLineReplacement)
        {
            if (str != null)
            {
                str = str.Replace("\\r\\n", Environment.NewLine);

                if (ToolConsole.Verbosity > Verbosity.Minimal || logTag == LogTag.Error || logTag == LogTag.Important ||
                   (ToolConsole.Verbosity == Verbosity.Minimal && logTag == LogTag.Warning) || LogTag.IsTrace(logTag))
                {
                    if (IsMarkupEnabled)
                    {
                        str = logTag + str.Replace(Environment.NewLine, newLineReplacement);
                    }

                    Console.WriteLine(str);
                }
            }
        }

        internal static void WriteConversionError(MetadataConversionError conversionError)
        {
            if (conversionError?.Message != null)
            {
                char[] trimLFNL = new char[] { '\r', '\n' };
                if (conversionError.IsWarning)
                {
                    WriteWarning(conversionError.Message.Replace("\r\n", Environment.NewLine).Trim(trimLFNL));
                }
                else
                {
                    WriteError(conversionError.Message.Replace("\r\n", Environment.NewLine).Trim(trimLFNL));
                }
            }
        }

        internal static void WriteConversionErrors(Collection<MetadataConversionError> errors)
        {
            if (ToolConsole.Verbosity == Verbosity.Silent)
            {
                // filter out warnings if there are no errors.
                bool isError = false;

                foreach (var err in errors)
                {
                    if (!err.IsWarning)
                    {
                        isError = true;
                        break;
                    }
                }

                if (!isError)
                {
                    for (int idx = errors.Count - 1; idx >= 0; idx--)
                    {
                        if (errors[idx].IsWarning)
                        {
                            errors.RemoveAt(idx);
                        }
                    }
                }
            }

            if (errors.Count > 0)
            {
                foreach (MetadataConversionError conversionError in errors)
                {
                    if (!string.IsNullOrWhiteSpace(conversionError.Message))
                    {
                        ToolConsole.WriteConversionError(conversionError);
                    }
                }
            }
        }

        internal static void WriteHeaderIf(bool condition)
        {
            if (condition && ToolConsole.Verbosity > Verbosity.Minimal && ToolConsole.ToolModeLevel != OperationalContext.Infrastructure)
            {
                ToolConsole.WriteLine(string.Format(SR.LogoFormat, Tool.ToolName, Tool.PackageVersion, SR.Microsoft_Copyright_CommandLine_Logo), LogTag.Important);

                if (AppInsightsTelemetryClient.IsUserOptedIn)
                {
                    ToolConsole.WriteLine(SR.TelemetryEnabled, LogTag.Information);
                }
            }
        }

        internal static void WriteHelp()
        {
            if (ToolConsole.Verbosity > Verbosity.Silent && ToolConsole.ToolModeLevel != OperationalContext.Infrastructure)
            {
                ToolConsole.WriteLine(HelpGenerator.GenerateHelpText(), LogTag.Important);
            }
        }
    }
}
