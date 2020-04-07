// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class SvcutilBootstrapper : IDisposable
    {
        private const string ProjectName = "SvcutilBootstrapper.csproj";
        private static readonly string BootstrapperParamsFileName = $"{Tool.ToolName}-bootstrapper.params.json";

        private MSBuildProj MSBuildProj { get; set; }

        private SvcutilOptions Options { get; set; }

        public SvcutilBootstrapper(SvcutilOptions options)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.Options.ProviderId = $"{Tool.ToolName}-bootstrap";
            this.Options.Version = Tool.PackageVersion;

            // reset options that don't apply to the bootstrapper and that prevent bootstrapping recursion.
            this.Options.NoBootstrapping = true;
            this.Options.NoLogo = true;
            this.Options.NoProjectUpdates = true;

            // the operational context has some control over what messages to display on the UI.
            if (this.Options.ToolContext.HasValue && this.Options.ToolContext.Value <= OperationalContext.Global)
            {
                this.Options.ToolContext = OperationalContext.Bootstrapper;
            }

            ProjectDependency.RemoveRedundantReferences(this.Options.References);
        }

        internal static bool RequiresBootstrapping(FrameworkInfo targetFramework, IEnumerable<ProjectDependency> references)
        {
            // Bootstrapping is required for type reuse when targetting a supported .NET Core platform and when there are project references 
            // different form the .NET Core and WCF ones.
            return targetFramework.IsDnx && references.Where(r => !r.IsFramework).Except(TargetFrameworkHelper.ServiceModelPackages).Count() > 0;
        }

        internal async Task<ProcessRunner.ProcessResult> BoostrapSvcutilAsync(bool keepBootstrapperDir, ILogger logger, CancellationToken cancellationToken)
        {
            ProcessRunner.ProcessResult result = null;

            using (await SafeLogger.WriteStartOperationAsync(logger, "Bootstrapping svcutil ...").ConfigureAwait(false))
            {
                // guard against bootstrapping recursion.
                if (this.Options.NoBootstrapping != true)
                {
                    Debug.Fail($"The NoBootstrapping property is not set, this would cause infinite bootstrapping recursion!");
                    return null;
                }

                if (this.Options.Project != null && StringComparer.OrdinalIgnoreCase.Compare(this.Options.Project.FileName, SvcutilBootstrapper.ProjectName) == 0)
                {
                    Debug.Fail("Bootstrapping is enabled for the bootstrapper! This would cause an infinite bootstrapping recursion!");
                    return null;
                }

                // When in Infrastructure mode (WCF CS) it is assumed the initial progress message is to be presented by the calling tool.
                ToolConsole.WriteLineIf(ToolConsole.ToolModeLevel != OperationalContext.Infrastructure, Resource.BootstrappingApplicationMsg);

                await GenerateProjectAsync(keepBootstrapperDir, logger, cancellationToken).ConfigureAwait(false);
                await GenerateProgramFileAsync(logger, cancellationToken).ConfigureAwait(false);

                var paramsFilePath = await GenerateParamsFileAsync(logger, cancellationToken).ConfigureAwait(false);

                await AddNugetConfigAsync(logger, cancellationToken).ConfigureAwait(false);
                await BuildBootstrapProjectAsync(logger, cancellationToken).ConfigureAwait(false);

                ToolConsole.WriteLineIf(ToolConsole.Verbosity >= Verbosity.Verbose, Resource.InvokingProjectMsg);
                result = await ProcessRunner.RunAsync("dotnet", $"run \"{paramsFilePath}\"", this.MSBuildProj.DirectoryPath, /*redirectOutput*/ false, logger, cancellationToken).ConfigureAwait(false);
                MarkupTelemetryHelper.TelemetryPostOperation(result.ExitCode == 0, "Invoke svcutil bootstrapper");
            }

            return result;
        }

        private async Task GenerateProjectAsync(bool keepBootstrapperDir, ILogger logger, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var projectFullPath = Path.Combine(this.Options.BootstrapPath.FullName, nameof(SvcutilBootstrapper), SvcutilBootstrapper.ProjectName);

            if (keepBootstrapperDir)
            {
                // creating the directory prevents the bootstrapper project from deleting it as it doesn't own it, files will not be deleted either.
                Directory.CreateDirectory(Path.GetDirectoryName(projectFullPath));
            }

            // If the target framework was provided, it was validated already when processing params.
            // Bootstrapping is enabled only for netcoreapp or netstandard TFM, let's check.

            bool isSupportedTFM = TargetFrameworkHelper.IsSupportedFramework(this.Options.TargetFramework.FullName, out var frameworkInfo);
            Debug.Assert(frameworkInfo.IsDnx, "The bootstrapper has been enabled for a non-DNX platform!");

            ToolConsole.WriteLineIf(ToolConsole.Verbosity >= Verbosity.Verbose, Resource.CreatingProjectMsg);

            using (await SafeLogger.WriteStartOperationAsync(logger, $"Creating project file: \"{projectFullPath}\"").ConfigureAwait(false))
            {
                var svcutilPkgRef = ProjectDependency.FromPackage(Tool.AssemblyName, Tool.PackageVersion);

                this.MSBuildProj = await MSBuildProj.DotNetNewAsync(projectFullPath, logger, cancellationToken).ConfigureAwait(false);
                this.MSBuildProj.AddDependency(svcutilPkgRef);

                // NOTE: If post v2.0 NetStandard ships a different version from NetCore the table below needs to be updated!
                var targetFramework = frameworkInfo.FullName;
                if (isSupportedTFM && frameworkInfo.IsKnownDnx)
                {
                    if (frameworkInfo.Name == FrameworkInfo.Netstandard)
                    {
                        targetFramework = FrameworkInfo.Netcoreapp + TargetFrameworkHelper.NetStandardToNetCoreVersionMap[frameworkInfo.Version];
                    }
                    this.MSBuildProj.TargetFramework = targetFramework;
                }
                // else
                // The TFM is unknown: either, it was not provided or it is a version not yet known to the tool,
                // we will use the default TF from the generated project.
            }

            foreach (ProjectDependency dependency in this.Options.References)
            {
                this.MSBuildProj.AddDependency(dependency);
            }

            if (!string.IsNullOrEmpty(this.Options.RuntimeIdentifier))
            {
                this.MSBuildProj.RuntimeIdentifier = this.Options.RuntimeIdentifier;
            }

            // Don't treat warnings as errors so the bootstrapper will succeed as often as possible.
            this.MSBuildProj.ClearWarningsAsErrors();

            await this.MSBuildProj.SaveAsync(logger, cancellationToken).ConfigureAwait(false);
        }

        private static readonly string ProgramClass =
        @"using System
namespace SvcutilBootstrap {
    public class Program {
        public static int Main(string[] args) {
            return Microsoft.Tools.ServiceModel.Svcutil.Tool.Main(args);
        }
    }
}";

        private async Task GenerateProgramFileAsync(ILogger logger, CancellationToken cancellationToken)
        {
            using (var safeLogger = await SafeLogger.WriteStartOperationAsync(logger, "Generating Program.cs ...").ConfigureAwait(false))
            {
                string programFilePath = Path.Combine(this.MSBuildProj.DirectoryPath, "Program.cs");
                File.WriteAllText(programFilePath, ProgramClass);
            }
        }

        private async Task<string> GenerateParamsFileAsync(ILogger logger, CancellationToken cancellationToken)
        {
            var paramsFilePath = Path.Combine(this.MSBuildProj.DirectoryPath, BootstrapperParamsFileName);
            using (await SafeLogger.WriteStartOperationAsync(logger, $"Generating {paramsFilePath} params file ...").ConfigureAwait(false))
            {
                await AsyncHelper.RunAsync(() => this.Options.Save(paramsFilePath), cancellationToken).ConfigureAwait(false);
                return paramsFilePath;
            }
        }

        private async Task AddNugetConfigAsync(ILogger logger, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var safeLogger = await SafeLogger.WriteStartOperationAsync(logger, "Generating nuget.config ...").ConfigureAwait(false))
            {
                var svcutilPkgName = $"{Tool.AssemblyName}.{Tool.PackageVersion}.nupkg";
                var svcutilPkgDir = Path.GetDirectoryName(Tool.FullPath);

                // This is the case with an installed NuGet package where the package itself is at the root
                // of the extracted directory and the executing assembly is under the lib subfolder.
                var svcutilPkgPath = await PathHelper.TryFindFileAsync(svcutilPkgName, svcutilPkgDir, logger, cancellationToken).ConfigureAwait(false);

                if (File.Exists(svcutilPkgPath))
                {
                    var nugetConfigFilePath = Path.Combine(this.MSBuildProj.DirectoryPath, "nuget.config");
                    var packageFeed = Path.GetDirectoryName(svcutilPkgPath);

                    if (!await RuntimeEnvironmentHelper.TryAddSvcutilNuGetFeedAsync(nugetConfigFilePath, packageFeed, logger, cancellationToken).ConfigureAwait(false))
                    {
                        string nugetConfigText = string.Format(CultureInfo.InvariantCulture, RuntimeEnvironmentHelper.NugetConfigFormat, Path.GetDirectoryName(svcutilPkgPath));
                        File.WriteAllText(nugetConfigFilePath, nugetConfigText);
                    }
                }
                else
                {
                    await safeLogger.WriteWarningAsync(string.Format(CultureInfo.CurrentCulture, Resource.ErrCannotFindNuGetPackageFormat, Path.Combine(svcutilPkgDir, svcutilPkgName)), logToUI: true).ConfigureAwait(false);
                }
            }
        }

        private async Task BuildBootstrapProjectAsync(ILogger logger, CancellationToken cancellationToken)
        {
            string outputText;
            string errorMessage = null;

            try
            {
                ToolConsole.WriteLineIf(ToolConsole.Verbosity >= Verbosity.Verbose, Resource.RestoringNuGetPackagesMsg);
                var restoreResult = await this.MSBuildProj.RestoreAsync(logger, cancellationToken).ConfigureAwait(false);
                MarkupTelemetryHelper.TelemetryPostOperation(restoreResult.ExitCode == 0, "Restore bootstrapper");
                if (restoreResult.ExitCode != 0)
                {
                    ToolConsole.WriteWarning(restoreResult.OutputText);
                }

                ToolConsole.WriteLineIf(ToolConsole.Verbosity >= Verbosity.Verbose, Resource.BuildingProjectMsg);
                var buildResult = await this.MSBuildProj.BuildAsync(logger, cancellationToken).ConfigureAwait(false);
                MarkupTelemetryHelper.TelemetryPostOperation(buildResult.ExitCode == 0, "Build bootstrapper");
            }
            catch (ProcessRunner.ProcessException exception)
            {
                throw new BootstrapException(string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", exception.Message, Environment.NewLine, Resource.BootstrapErrorDisableReferences));
            }
        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.MSBuildProj?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
