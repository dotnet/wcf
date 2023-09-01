// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Tools.ServiceModel.Svcutil.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DcNS = System.Runtime.Serialization;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    public static class Tool
    {
        #region properties
        internal static string ToolName = "Microsoft.Tools.ServiceModel.Svcutil";

        internal static string _assemblyName;
        internal static string AssemblyName
        {
            get
            {
                if (string.IsNullOrEmpty(_assemblyName))
                {
                    _assemblyName = typeof(Tool).GetTypeInfo().Assembly.GetName().Name;
                }
                return _assemblyName;
            }
        }

        private static string s_pkgVersion;
        internal static string PackageVersion
        {
            get
            {
                if (string.IsNullOrEmpty(s_pkgVersion))
                {
                    s_pkgVersion = typeof(Tool).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                        .InformationalVersion.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                }
                return s_pkgVersion;
            }
        }

        internal static string _fullPath;
        internal static string FullPath
        {
            get
            {
                if (string.IsNullOrEmpty(_fullPath))
                {
                    _fullPath = typeof(Tool).GetTypeInfo().Assembly.Location;
                }
                return _fullPath;
            }
        }
        #endregion

        public static int Main(string[] args)
        {
            var cancellationToken = CancellationToken.None;
#if DEBUG
            var cancellationSource = new CancellationTokenSource();
            cancellationToken = cancellationSource.Token;
#endif
            return MainAsync(args, null, cancellationToken).Result;
        }

        internal static async Task<int> MainAsync(string[] args, ILogger logger, CancellationToken cancellationToken)
        {
            int result = -1;
            CommandProcessorOptions options = null;

            WaitForDebugger();

            try
            {
                options = await CommandProcessorOptions.ParseArgumentsAsync(args, logger, cancellationToken);

                ValidateUICulture(options);

                if (options.NoTelemetry == true)
                {
                    AppInsightsTelemetryClient.IsUserOptedIn = false;
                }

                ToolConsole.Init(options);
                ToolConsole.WriteHeaderIf(options.NoLogo != true);

                ThrowOnValidationErrors(options);

                if (options.Help == true)
                {
                    ToolConsole.WriteHelp();
                    result = (int)ToolExitCode.Success;
                }
                else
                {
                    // Show early warnings
                    var earlyWarnings = options.Warnings;
                    foreach (string warning in earlyWarnings.Distinct())
                    {
                        ToolConsole.WriteWarning(warning);
                    }

                    var operationMessage = (options.IsUpdateOperation ? "Update" : "Add") + " web service reference operation started!";
                    using (var safeLogger = await SafeLogger.WriteStartOperationAsync(options.Logger, operationMessage).ConfigureAwait(false))
                    {
                        await options.ResolveAsync(cancellationToken).ConfigureAwait(false);
                        ThrowOnValidationErrors(options);

                        options.ProviderId = Tool.ToolName;
                        options.Version = Tool.PackageVersion;

                        foreach (string warning in options.Warnings.Distinct().Except(earlyWarnings))
                        {
                            ToolConsole.WriteWarning(warning);
                        }

                        if (options.RequiresBoostrapping)
                        {
                            using (var bootstrapper = new SvcutilBootstrapper(options.CloneAs<SvcutilOptions>()))
                            {
                                await options.SetupBootstrappingDirectoryAsync(options.Logger, cancellationToken).ConfigureAwait(false);

                                var bootstrapResult = await bootstrapper.BoostrapSvcutilAsync(options.KeepBootstrapDir, options.Logger, cancellationToken).ConfigureAwait(false);
                                ToolConsole.WriteLine(bootstrapResult.OutputText);
                                result = bootstrapResult.ExitCode;
                            }
                        }
                        else
                        {
                            result = (int)await RunAsync(options, cancellationToken).ConfigureAwait(false);
                        }

                        if (IsSuccess(result))
                        {
                            if (!File.Exists(options.OutputFile.FullName))
                            {
                                await safeLogger.WriteMessageAsync("The process completed successfully but no proxy file was found!", logToUI: false);
                                throw new ToolArgumentException(SR.ErrUnexpectedError);
                            }
                            else
                            {
                                await GenerateParamsFileAsync(options, options.Logger, cancellationToken);

                                if (CanAddProjectReferences(options) && !await AddProjectReferencesAsync(options.Project, options, cancellationToken).ConfigureAwait(false))
                                {
                                    result = (int)ToolExitCode.InputError;
                                }
                            }

                            // clean up only on success to allow to troubleshoot any problems on failure.
                            options?.Cleanup();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = await ProcessExceptionAsync(e, options);
            }
            finally
            {
                try
                {
                    // Don't log telemetry if we're running from bootstrapper or connected service.
                    if (options?.ToolContext <= OperationalContext.Global)
                    {
                        var properties = new Dictionary<string, string>()
                        {
                            { "IsUpdate", (options?.IsUpdateOperation).ToString() },
                            { "TargetFramework", options?.TargetFramework?.FullName },
                            { "Parameters", options?.ToTelemetryString() },
                            { "ExitCode", result.ToString() },
                            { "RequiresBootstrapping", options?.RequiresBoostrapping.ToString() },
                        };

                        var telemetryClient = await AppInsightsTelemetryClient.GetInstanceAsync(cancellationToken).ConfigureAwait(false);
                        telemetryClient.TrackEvent("ToolRun", properties);
                    }
                }
                catch
                {
                }
            }

            return result;
        }

        internal static async Task<ToolExitCode> RunAsync(CommandProcessorOptions options, CancellationToken cancellationToken)
        {
            ImportModule importModule = null;
            var credsProvider = new CmdCredentialsProvider();
            credsProvider.AcceptCert = options.AcceptCert.Value;

            ServiceDescriptor serviceDescriptor = options.Inputs.Count == 1 ?
                new ServiceDescriptor(options.Inputs[0].ToString(), credsProvider, credsProvider, credsProvider) :
                new ServiceDescriptor(options.Inputs.Select(i => i.ToString()).ToList(), credsProvider, credsProvider, credsProvider);

            // When in Infrastructure mode (WCF CS) it is assumed the metadata docs have been downloaded and passed in as wsdl files.
            if (options.ToolContext != OperationalContext.Infrastructure)
            {
                if (serviceDescriptor.MetadataUrl != null)
                {
                    ToolConsole.WriteLine(string.Format(SR.RetreivingMetadataMsgFormat, serviceDescriptor.MetadataUrl.AbsoluteUri));
                }
                else
                {
                    var displayUri = serviceDescriptor.MetadataFiles.Count() == 1 ? serviceDescriptor.MetadataFiles.First().LocalPath : SR.WsdlOrSchemaFilesMsg;
                    ToolConsole.WriteLine(string.Format(SR.ReadingMetadataMessageFormat, displayUri));
                }
            }

            using (await SafeLogger.WriteStartOperationAsync(options.Logger, "Importing metadata ...").ConfigureAwait(false))
            {
                try
                {
                    await serviceDescriptor.ImportMetadataAsync(
                        (wi) => importModule = new ImportModule(options, serviceDescriptor, wi),
                        (sd) => importModule.BeforeImportMetadata(sd),
                        (sd) => importModule.AfterImportMetadata(sd),
                        cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (Utils.IsFatalOrUnexpected(ex)) throw;
                    var exception = new ToolInputException(Utils.GetExceptionMessage(ex), ex);
                    if (serviceDescriptor.MetadataUrl != null)
                    {
                        exception = new ToolMexException(exception, serviceDescriptor.MetadataUrl);
                    }
                    throw exception;
                }
            }

            using (await SafeLogger.WriteStartOperationAsync(options.Logger, "Processing Code DOM ...").ConfigureAwait(false))
            {
                ToolConsole.WriteLine(SR.GeneratingFiles);

                CodeSerializer codeSerializer = new CodeSerializer(options, serviceDescriptor.MetadataDocuments);
                var filePath = codeSerializer.Save(importModule.CodeCompileUnit);

                // When in Infrastructure mode (WCF CS) it is assumed the output file path have been provided so no need to display it.
                ToolConsole.WriteLineIf(options.ToolContext != OperationalContext.Infrastructure, filePath, LogTag.Important);
            }

            return ToolConsole.ExitCode;
        }

        private static bool IsSuccess(int result)
        {
            return result == (int)ToolExitCode.Success || result == (int)ToolExitCode.ValidationErrorTurnedWarning;
        }

        private static bool CanAddProjectReferences(CommandProcessorOptions options)
        {
            // Project references are added at the end of the process, never but the bootstrapper which may not be even invoked.
            // ensure the output files goes under the project dir.
            return options.NoProjectUpdates != true && options.Project != null &&
                PathHelper.IsUnderDirectory(options.OutputFile.FullName, new DirectoryInfo(options.Project.DirectoryPath), out var filePath, out var relPath);
        }

        private static async Task<bool> AddProjectReferencesAsync(MSBuildProj project, CommandProcessorOptions options, CancellationToken cancellationToken)
        {
            try
            {
                var dependencies = TargetFrameworkHelper.GetWcfProjectReferences(project.TargetFramework);
                if (dependencies != null)
                {
                    bool needSave = false;
                    foreach (var dep in dependencies)
                    {
                        if (dep.Name.Contains("NetNamedPipe") && !project.TargetFrameworks.Any(t => t.ToLower().Contains("windows")))
                        {
                            continue;
                        }

                        needSave |= project.AddDependency(dep);
                    }

                    if (project.TargetFrameworks.Count() > 1 && project.TargetFrameworks.Any(t => TargetFrameworkHelper.IsSupportedFramework(t, out FrameworkInfo fxInfo) && !fxInfo.IsDnx))
                    {
                        FrameworkInfo fxInfo = null;
                        var tfx = project.TargetFrameworks.FirstOrDefault(t => TargetFrameworkHelper.IsSupportedFramework(t, out fxInfo) && fxInfo.IsDnx);
                        if (!string.IsNullOrEmpty(tfx) && fxInfo.Version.Major >= 6)
                        {
                            needSave |= project.AddDependency(TargetFrameworkHelper.FullFrameworkReferences.FirstOrDefault());
                        }
                    }

                    if (needSave)
                    {
                        await project.SaveAsync(options.Logger, cancellationToken).ConfigureAwait(false);
                    }
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        private static async Task GenerateParamsFileAsync(CommandProcessorOptions options, ILogger logger, CancellationToken cancellationToken)
        {
            // params file is generated on first run, never on update and never by the bootstrapper.

            if (!(options.IsUpdateOperation || options.NoProjectUpdates == true))
            {
                using (var safeLogger = await SafeLogger.WriteStartOperationAsync(logger, "Generating svcutil params file ...").ConfigureAwait(false))
                {
                    var updateOptions = options.CloneAs<UpdateOptions>();
                    updateOptions.ProviderId = Tool.ToolName;
                    updateOptions.Version = Tool.PackageVersion;

                    ProjectDependency.RemoveRedundantReferences(updateOptions.References);

                    updateOptions.MakePathsRelativeTo(options.OutputDir);

                    var paramsFile = Path.Combine(options.OutputDir.FullName, CommandProcessorOptions.SvcutilParamsFileName);
                    await AsyncHelper.RunAsync(() => updateOptions.Save(paramsFile), cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private static void ThrowOnValidationErrors(CommandProcessorOptions options)
        {
            if (options.Errors.Count() > 0)
            {
                var ex = new AggregateException(options.Errors.Select(e => e as ToolArgumentException ?? new ToolArgumentException(e.Message, e.InnerException)));
                throw ex;
            }
        }

        private static bool s_processingException = false;
        private static async Task<int> ProcessExceptionAsync(Exception ex, CommandProcessorOptions options)
        {
            int retVal = (int)ToolExitCode.Unknown;

            Debug.Assert(ex != null, "exception should not be null!");

            if (ex != null)
            {
                try
                {
                    s_processingException = true;

                    var agex = ex as AggregateException;

                    if (agex != null)
                    {
                        foreach (var ax in agex.InnerExceptions)
                        {
                            retVal = await ProcessExceptionAsync(ax, options);
                        }
                    }
                    else if (ex is BootstrapException bootstrapEx)
                    {
                        ToolConsole.WriteToolError(ex);
                        retVal = bootstrapEx.ExitCode;
                    }
                    else if (ex is ProcessRunner.ProcessException rpe)
                    {
                        ToolConsole.WriteError(rpe, null);
                        retVal = rpe.ExitCode;
                    }
                    else if (ex is ToolArgumentException ae)
                    {
                        ToolConsole.WriteToolError(ae);
                        retVal = (int)ae.ExitCode;
                    }
                    else if (ex is ToolRuntimeException rt)
                    {
                        ToolConsole.WriteToolError(rt);
                        retVal = (int)rt.ExitCode;
                    }
                    else if (ex is DcNS.InvalidDataContractException dce)
                    {
                        ToolConsole.WriteError(dce);
                        retVal = (int)ToolExitCode.RuntimeError;
                    }
                    else if (Utils.IsUnexpected(ex))
                    {
                        ToolConsole.WriteError(SR.ErrUnexpectedError);
                        retVal = (int)ToolExitCode.RuntimeError;
                    }
                    else // (Exception e)
                    {
                        ToolConsole.WriteError(ex);
                        retVal = (int)ToolExitCode.Unknown;
                    }

                    // don't log aggregate exceptions as the internal exceptions are already logged.
                    if (agex == null)
                    {
                        string exMsg = null;

                        if (options?.Logger != null)
                        {
                            exMsg = Utils.GetExceptionMessage(ex, true);
                            await options.Logger.WriteErrorAsync(exMsg, logToUI: false).ConfigureAwait(false);
                        }

                        // Don't log telemetry if we're running from bootstrapper or connected service.
                        // if options = null, it must be that a parsing exception occurred.
                        if (options == null || options.ToolContext <= OperationalContext.Global)
                        {
                            exMsg = exMsg ?? Utils.GetExceptionMessage(ex, true);
                            var telemetryClient = await AppInsightsTelemetryClient.GetInstanceAsync(CancellationToken.None).ConfigureAwait(false);
                            telemetryClient.TrackError("Exception", exMsg);
                        }
                    }
                }
                catch
                {
                }
            }

            return retVal;
        }

        internal static void WaitForDebugger()
        {
#if DEBUG
            DebugUtils.SetupDebugging();
#endif
        }

        private static void ValidateUICulture(CommandProcessorOptions options)
        {
            if (options.EnableLoggingMarkup == true)
            {
                // The default encoding might not work while redirecting non-ANSI characters
                // UTF8 would support non-ANSI character and this should be same encoding at WCF tool side
                Console.InputEncoding = Encoding.UTF8;
                Console.OutputEncoding = Encoding.UTF8;
            }

            if (options.CultureInfo != null)
            {
                CultureInfo.DefaultThreadCurrentUICulture = options.CultureInfo;
                CultureInfo.CurrentUICulture = options.CultureInfo;
            }
        }
    }
}

