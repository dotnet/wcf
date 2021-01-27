// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

#if NETCORE
#if !NETCORE10
using System.Runtime.Loader;
#endif
#endif

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class ProjectPropertyResolver
    {
        public async Task<Dictionary<string, string>> EvaluateProjectPropertiesAsync(string projectPath, string targetFramework, IEnumerable<string> propertyNames, IDictionary<string, string> globalProperties, ILogger logger, CancellationToken cancellationToken)
        {
            var propertyTable = new Dictionary<string, string>();

            using (var safeLogger = await SafeLogger.WriteStartOperationAsync(logger, $"Resolving {propertyNames.Count()} project properties ...").ConfigureAwait(false))
            {
                ValidatePropertyNames(propertyNames);
                ValidatePropertyNames(globalProperties);

                var workingDirectory = Path.GetDirectoryName(projectPath);
                var sdkVersion = await GetSdkVersionAsync(workingDirectory, logger, cancellationToken).ConfigureAwait(false);
                var sdkPath = await GetSdkPathAsync(workingDirectory, logger, cancellationToken).ConfigureAwait(false);

                try
                {
#if NETCORE
                    var propertiesResolved = false;

                    try
                    {
                        // In order for the MSBuild project evaluation API to work in .NET Core, the code must be executed directly from the .NET Core SDK assemblies.
                        // MSBuild libraries need to be explicitly loaded from the right SDK path (the one that corresponds to the runtime executing the project) as
                        // dependencies must be loaded from the executing runtime. This is not always possible, a newer SDK can load an older runtime; also, msbuild
                        // scripts from a newer SDK may not be supported by an older SDKs.
                        // Consider: A project created with the command 'dotnet new console' will target the right platform for the current SDK.

                        Assembly msbuildAssembly = await LoadMSBuildAssembliesAsync(sdkPath, logger, cancellationToken).ConfigureAwait(false);
                        if (msbuildAssembly != null)
                        {
                            var projType = msbuildAssembly.GetType("Microsoft.Build.Evaluation.Project", true, false);
                            var projInstance = Activator.CreateInstance(projType, new object[] { projectPath, globalProperties, /*toolsVersion*/ null });
                            var getPropertyValue = projType.GetMethod("GetPropertyValue");

                            if (getPropertyValue != null)
                            {
                                foreach (var propertyName in propertyNames)
                                {
                                    var propertyValue = getPropertyValue.Invoke(projInstance, new object[] { propertyName }).ToString();
                                    propertyTable[propertyName] = propertyValue;
                                    await safeLogger.WriteMessageAsync($"Evaluated '{propertyName}={propertyValue}'", logToUI: false).ConfigureAwait(false);
                                }
                                propertiesResolved = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await safeLogger.WriteMessageAsync(ex.Message, logToUI: false).ConfigureAwait(false);
                    }

                    if (!propertiesResolved)
                    {
                        foreach (var propertyName in propertyNames)
                        {
                            var propertyValue = GetDefaultPropertyValue(projectPath, targetFramework, propertyName);
                            propertyTable[propertyName] = propertyValue;
                            await safeLogger.WriteMessageAsync($"Resolved '{propertyName}={propertyValue}'", logToUI: false).ConfigureAwait(false);
                        }
                    }
#else
                    // don't use GlobalProjectCollection as once a project is loaded into memory changes to the project file won't take effect until the solution is reloaded.
                    var projCollection = new Microsoft.Build.Evaluation.ProjectCollection(globalProperties);
                    var project = projCollection.LoadProject(projectPath);

                    foreach (var propertyName in propertyNames)
                    {
                        var propertyValue = project.GetPropertyValue(propertyName);
                        propertyTable[propertyName] = propertyValue;
                        await safeLogger.WriteMessageAsync($"Evaluated '{propertyName}={propertyValue}'", logToUI: false).ConfigureAwait(false);
                    }
#endif // NETCORE
                }
                catch (Exception ex)
                {
                    if (Utils.IsFatalOrUnexpected(ex)) throw;
                    await safeLogger.WriteErrorAsync($"{ex.Message}{Environment.NewLine}{ex.StackTrace}", logToUI: false);
                }
                finally
                {
                    // Ensure the dictionary is populated in any case, the client needs to validate the values but not the collection.
                    foreach (var propertyName in propertyNames)
                    {
                        if (!propertyTable.ContainsKey(propertyName))
                        {
                            propertyTable[propertyName] = string.Empty;
                        }
                    }
                }
            }

            return propertyTable;
        }

        private static string s_sdkVersion;
        public static async Task<string> GetSdkVersionAsync(string workingDirectory, ILogger logger, CancellationToken cancellationToken)
        {
            if (s_sdkVersion == null)
            {
                using (var safeLogger = await SafeLogger.WriteStartOperationAsync(logger, "Resolving dotnet sdk version ...").ConfigureAwait(false))
                {
                    var procResult = await ProcessRunner.TryRunAsync("dotnet", "--version", workingDirectory, logger, cancellationToken).ConfigureAwait(false);

                    if (procResult.ExitCode == 0)
                    {
                        s_sdkVersion = procResult.OutputText.Trim();
                    }

                    await safeLogger.WriteMessageAsync($"dotnet sdk version:{s_sdkVersion}", logToUI: false).ConfigureAwait(false);
                }
            }
            return s_sdkVersion;
        }

        private static string s_sdkPath;
        public static async Task<string> GetSdkPathAsync(string workingDirectory, ILogger logger, CancellationToken cancellationToken)
        {
            if (s_sdkPath == null)
            {
                using (var safeLogger = await SafeLogger.WriteStartOperationAsync(logger, "Resolving .NETCore SDK path ...").ConfigureAwait(false))
                {
#if NETCORE10
                    var dotnetDir = Path.GetDirectoryName(typeof(int).GetTypeInfo().Assembly.Location);
#else
                    var dotnetDir = Path.GetDirectoryName(typeof(int).Assembly.Location);
#endif

                    while (dotnetDir != null && !(File.Exists(Path.Combine(dotnetDir, "dotnet")) || File.Exists(Path.Combine(dotnetDir, "dotnet.exe"))))
                    {
                        dotnetDir = Path.GetDirectoryName(dotnetDir);
                    }

                    if (dotnetDir != null)
                    {
                        var sdkVersion = await GetSdkVersionAsync(workingDirectory, logger, cancellationToken).ConfigureAwait(false);
                        if (!string.IsNullOrEmpty(sdkVersion))
                        {
                            s_sdkPath = Path.Combine(dotnetDir, "sdk", sdkVersion);
                        }
                    }

                    await safeLogger.WriteMessageAsync($"SDK path: \"{s_sdkPath}\"", logToUI: false).ConfigureAwait(false);
                }
            }

            return s_sdkPath;
        }

#if NETCORE
        private async Task<Assembly> LoadMSBuildAssembliesAsync(string sdkPath, ILogger logger, CancellationToken cancellationToken)
        {
#if !NETCORE10
            Assembly msbuildAssembly = null;

            using (var safeLogger = await SafeLogger.WriteStartOperationAsync(logger, "Loading MSBuild assemblies ...").ConfigureAwait(false))
            {
                if (Directory.Exists(sdkPath))
                {
                    foreach (var assemblyPath in Directory.GetFiles(sdkPath, "Microsoft.Build.*", SearchOption.TopDirectoryOnly))
                    {
                        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                        if (Path.GetFileNameWithoutExtension(assemblyPath) == "Microsoft.Build")
                        {
                            msbuildAssembly = assembly;
                        }
                    }
                }
            }
            return msbuildAssembly;
#else
            return await Task.FromResult<Assembly>(null);
#endif
        }

        private string GetDefaultPropertyValue(string projectPath, string targetFramework, string propertyName)
        {
            string value = string.Empty;

            if (StringComparer.OrdinalIgnoreCase.Compare("OutputPath", propertyName) == 0)
            {
                var projectDir = Path.GetDirectoryName(projectPath);

                // we can only support standard output paths under bin/ folder, try to determine what configuration to use (bin/debug).
                var depsFiles = Directory.GetFiles(projectDir, $"{Path.GetFileNameWithoutExtension(projectPath)}.deps.json", SearchOption.AllDirectories)
                    .Where(f => PathHelper.PathHasFolder(f, new string[] { targetFramework }, projectDir))
                    .Select(f => new FileInfo(f));
                var depsFileInfo = depsFiles.OrderBy(f => f.CreationTimeUtc).LastOrDefault();

                if (depsFileInfo != null)
                {
                    value = depsFileInfo.DirectoryName;
                }
            }
            else if (StringComparer.OrdinalIgnoreCase.Compare("TargetPath", propertyName) == 0)
            {
                var projName = Path.GetFileNameWithoutExtension(projectPath);
                value = $"{projName}.dll";
            }

            return value;
        }
#endif // NETCORE

        private void ValidatePropertyNames(IDictionary<string, string> propertyTable)
        {
            if (propertyTable == null)
            {
                throw new ArgumentNullException(nameof(propertyTable));
            }

            ValidatePropertyNames(propertyTable.Keys);
        }

        private void ValidatePropertyNames(IEnumerable<string> propertyNames)
        {
            if (propertyNames == null)
            {
                throw new ArgumentNullException(nameof(propertyNames));
            }

            var chars = Path.GetInvalidFileNameChars();

            foreach (var propertyName in propertyNames)
            {
                if (string.IsNullOrWhiteSpace(propertyName) || propertyName.Any(c => chars.Contains(c) || !char.IsLetterOrDigit(c)))
                {
                    throw new ArgumentException(nameof(propertyName));
                }
            }
        }
    }
}
