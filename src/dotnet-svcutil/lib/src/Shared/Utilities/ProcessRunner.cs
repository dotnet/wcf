// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class ProcessRunner
    {
        public class ProcessException : Exception
        {
            public int ExitCode { get; private set; }
            public ProcessException(string message, int exitCode) : base(message) { this.ExitCode = exitCode; }
        }

        public class ProcessResult
        {
            public int ExitCode { get; private set; }
            public string OutputText { get; private set; }
            public ProcessResult(int exitCode, string outputText) { this.ExitCode = exitCode; this.OutputText = outputText; }
            public override string ToString() { return ExitCode.ToString(CultureInfo.InvariantCulture); }
        }

        public static async Task<ProcessResult> RunAsync(string processName, string processArgs, string currentDir, ILogger logger, CancellationToken cancellationToken)
        {
            return await RunAsync(processName, processArgs, currentDir, redirectOutput: true, throwOnError: true, logger: logger, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public static async Task<ProcessResult> RunAsync(string processName, string processArgs, string currentDir, bool redirectOutput, ILogger logger, CancellationToken cancellationToken)
        {
            return await RunAsync(processName, processArgs, currentDir, redirectOutput: redirectOutput, throwOnError: true, logger: logger, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public static async Task<ProcessResult> TryRunAsync(string processName, string processArgs, string currentDir, ILogger logger, CancellationToken cancellationToken)
        {
            return await RunAsync(processName, processArgs, currentDir, redirectOutput: true, throwOnError: false, logger: logger, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        public static async Task<ProcessResult> RunAsync(string processName, string processArgs, string currentDir, bool redirectOutput, bool throwOnError, ILogger logger, CancellationToken cancellationToken)
        {
            var emptyVars = new Dictionary<string, string>();
            return await RunAsync(processName, processArgs, currentDir, redirectOutput, throwOnError, emptyVars, logger, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<ProcessResult> RunAsync(string processName, string processArgs, string currentDir, bool redirectOutput, bool throwOnError, IDictionary<string, string> environmentVariables, ILogger logger, CancellationToken cancellationToken)
        {
            bool isErrorLogged = false;
            var errorTextBldr = new StringBuilder();
            var outputTextBldr = new StringBuilder();

            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(currentDir))
            {
                currentDir = Directory.GetCurrentDirectory();
            }

            using (var safeLogger = await SafeLogger.WriteStartOperationAsync(logger, $"Executing command [\"{currentDir}\"]{Environment.NewLine}>{processName} {processArgs}").ConfigureAwait(false))
            {
                using (var proc = new Process())
                {
                    proc.StartInfo.WorkingDirectory = Path.GetFullPath(currentDir);
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = redirectOutput;
                    proc.StartInfo.RedirectStandardError = redirectOutput;
                    proc.StartInfo.RedirectStandardOutput = redirectOutput;
                    proc.StartInfo.FileName = processName;
                    proc.StartInfo.Arguments = processArgs;
                    proc.EnableRaisingEvents = true;

                    foreach (var environmentVar in environmentVariables.Where(e => !string.IsNullOrWhiteSpace(e.Key)))
                    {
                        proc.StartInfo.Environment.Add(environmentVar);
                    }

                    if (redirectOutput)
                    {
                        // The default encoding might not work while redirecting non-ANSI characters.
                        // Standard error encoding is only supported when standard error is redirected.
                        proc.StartInfo.StandardErrorEncoding = Encoding.UTF8;
                        proc.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                    }

                    proc.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data))
                        {
                            errorTextBldr.AppendLine(e.Data);
                            safeLogger.WriteErrorAsync(e.Data, false).ConfigureAwait(false);
                            isErrorLogged = true;
                        }
                    };

                    proc.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e)
                    {
                        outputTextBldr.AppendLine(e.Data);
                        safeLogger.WriteMessageAsync(e.Data, false).ConfigureAwait(false);
                    };

                    proc.Start();
#if DEBUG
                    if (DebugUtils.SvcutilDebug == 1)
                    {
                        try
                        {
                            Console.WriteLine($"Starting process in the background: {Path.GetFileName(proc.ProcessName)}, ID: {proc.Id}.");
                            Console.WriteLine($"{Path.GetFileName(currentDir)}>{processName} {processArgs}{Environment.NewLine}");
                        }
                        catch
                        {
                        }
                    }
#endif
                    if (redirectOutput)
                    {
                        proc.BeginErrorReadLine();
                        proc.BeginOutputReadLine();
                    }

                    await AsyncHelper.RunAsync(() => proc.WaitForExit(), () => { try { proc.Kill(); } catch { } }, cancellationToken).ConfigureAwait(false);

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        // allow for processing message packets a few more times as they can keep coming after the process has finished.
                        int waitCount = 3;
                        while (waitCount-- > 0)
                        {
                            proc.WaitForExit();
                            await Task.Delay(100);
                        }
                    }
                    cancellationToken.ThrowIfCancellationRequested();

                    var outputText = outputTextBldr.ToString().Trim();
                    var errorText = errorTextBldr.ToString().Trim();

                    await safeLogger.WriteMessageAsync($"Exit code: {proc.ExitCode}", false).ConfigureAwait(false);

                    if (throwOnError && (isErrorLogged || proc.ExitCode != 0))
                    {
                        // avoid reporting a foreign tool's exit code.
                        var exitCode = Path.GetFileName(processName) == Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName) ? proc.ExitCode : -1;
                        throw new ProcessException(string.IsNullOrWhiteSpace(errorText) ? outputText : errorText, exitCode);
                    }
                    else if (string.IsNullOrWhiteSpace(outputText))
                    {
                        outputText = errorText;
                    }

                    return new ProcessResult(proc.ExitCode, outputText);
                }
            }
        }
    }
}
