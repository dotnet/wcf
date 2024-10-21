// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeDom.Compiler;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal partial class CommandProcessorOptions : SvcutilOptions
    {
        #region Options-related properties
        public const string UpdateServiceReferenceKey = "update";

        public string UpdateServiceReferenceFolder { get { return GetValue<string>(UpdateServiceReferenceKey); } set { SetValue(UpdateServiceReferenceKey, value); } }
        public override string Json { get { return Serialize<CommandProcessorOptions, OptionsSerializer<CommandProcessorOptions>>(); } }

        /// <summary>
        /// Wrapper around TypeReuseMode. 
        /// A flag makes more sense on the command line: 'true/false' map to 'None/All' and 
        /// if any reference specified in the command line maps to 'Specified'.
        /// </summary>
        public bool NoTypeReuse
        {
            get { return this.TypeReuseMode == Svcutil.TypeReuseMode.None; }
            set { SetValue(TypeReuseModeKey, ParseNoTypeReuseOptionValue(value)); }
        }
        #endregion

        #region Properties
        public static CommandSwitches Switches = new CommandSwitches();
        public List<Type> ReferencedTypes { get; private set; }
        public List<Assembly> ReferencedAssemblies { get; private set; }
        public List<Type> ReferencedCollectionTypes { get; private set; }
        public ILogger Logger { get; private set; }
        public CodeDomProvider CodeProvider { get; private set; }
        public bool OwnsBootstrapDir { get; private set; }
        public bool KeepBootstrapDir { get { return this.Verbosity == Svcutil.Verbosity.Debug || DebugUtils.KeepTemporaryDirs; } }

        // See Update option processing.
        public bool IsUpdateOperation { get { return this.UpdateServiceReferenceFolder != null; } }

        // See NoBootstrapping option processing.
        public bool RequiresBoostrapping { get; private set; }
        #endregion

        #region Constants
        internal const string SvcutilParamsFileName = "dotnet-svcutil.params.json";
        internal const string WCFCSParamsFileName = "ConnectedService.json";
        internal const string BaseServiceReferenceName = "ServiceReference";

        private static readonly List<string> s_cmdLineOverwriteSwitches = new List<string> { Switches.NoLogo.Name, Switches.Verbosity.Name, Switches.ToolContext.Name, Switches.ProjectFile.Name, Switches.AcceptCertificate.Name, Switches.ServiceContract.Name };

        internal class CommandSwitches
        {
            public readonly CommandSwitch BootstrapDir = new CommandSwitch(BootstrapPathKey, "bd", SwitchType.SingletonValue, OperationalContext.Infrastructure);
            public readonly CommandSwitch CollectionType = new CommandSwitch(CollectionTypesKey, "ct", SwitchType.ValueList);
            public readonly CommandSwitch CultureName = new CommandSwitch(CultureInfoKey, "cn", SwitchType.SingletonValue, OperationalContext.Infrastructure);
            public readonly CommandSwitch EnableDataBinding = new CommandSwitch(EnableDataBindingKey, "edb", SwitchType.Flag);
            public readonly CommandSwitch EnableLoggingMarkup = new CommandSwitch(EnableLoggingMarkupKey, "elm", SwitchType.Flag, OperationalContext.Infrastructure);
            public readonly CommandSwitch ExcludeType = new CommandSwitch(ExcludeTypesKey, "et", SwitchType.ValueList);
            public readonly CommandSwitch Help = new CommandSwitch(HelpKey, "h", SwitchType.Flag);
            public readonly CommandSwitch Internal = new CommandSwitch(InternalTypeAccessKey, "i", SwitchType.Flag);
            public readonly CommandSwitch MessageContract = new CommandSwitch(MessageContractKey, "mc", SwitchType.Flag);
            public readonly CommandSwitch Namespace = new CommandSwitch(NamespaceMappingsKey, "n", SwitchType.ValueList);
            public readonly CommandSwitch NoBootstraping = new CommandSwitch(NoBootstrappingKey, "nb", SwitchType.Flag, OperationalContext.Infrastructure);
            public readonly CommandSwitch NoLogo = new CommandSwitch(NoLogoKey, "nl", SwitchType.Flag);
            public readonly CommandSwitch NoProjectUpdates = new CommandSwitch(NoProjectUpdatesKey, "npu", SwitchType.Flag, OperationalContext.Infrastructure);
            public readonly CommandSwitch NoTelemetry = new CommandSwitch(NoTelemetryKey, "nm", SwitchType.Flag, OperationalContext.Infrastructure);
            public readonly CommandSwitch NoTypeReuse = new CommandSwitch("noTypeReuse", "ntr", SwitchType.Flag, OperationalContext.Project); // this maps to TypeReuseMode, for the command line a flag makes more sense.
            public readonly CommandSwitch NoStdlib = new CommandSwitch(NoStandardLibraryKey, "nsl", SwitchType.Flag);
            public readonly CommandSwitch OutputDirectory = new CommandSwitch(OutputDirKey, "d", SwitchType.SingletonValue, OperationalContext.Global);
            public readonly CommandSwitch OutputFile = new CommandSwitch(OutputFileKey, "o", SwitchType.SingletonValue, OperationalContext.Global);
            public readonly CommandSwitch ProjectFile = new CommandSwitch(ProjectFileKey, "pf", SwitchType.SingletonValue, OperationalContext.Global);
            public readonly CommandSwitch Reference = new CommandSwitch(ReferencesKey, "r", SwitchType.ValueList);
            public readonly CommandSwitch RuntimeIdentifier = new CommandSwitch(RuntimeIdentifierKey, "ri", SwitchType.SingletonValue, OperationalContext.Global);
            public readonly CommandSwitch Serializer = new CommandSwitch(SerializerModeKey, "ser", SwitchType.SingletonValue);
            public readonly CommandSwitch Sync = new CommandSwitch(SyncKey, "syn", SwitchType.Flag);
            public readonly CommandSwitch TargetFramework = new CommandSwitch(TargetFrameworkKey, "tf", SwitchType.SingletonValue, OperationalContext.Global);
            public readonly CommandSwitch ToolContext = new CommandSwitch(ToolContextKey, "tc", SwitchType.SingletonValue, OperationalContext.Infrastructure);
            public readonly CommandSwitch Update = new CommandSwitch(UpdateServiceReferenceKey, "u", SwitchType.FlagOrSingletonValue, OperationalContext.Project);
            public readonly CommandSwitch Verbosity = new CommandSwitch(VerbosityKey, "v", SwitchType.SingletonValue);
            public readonly CommandSwitch Wrapped = new CommandSwitch(WrappedKey, "wr", SwitchType.Flag);
            public readonly CommandSwitch AcceptCertificate = new CommandSwitch(AccecptCertificateKey, "ac", SwitchType.Flag);
            public readonly CommandSwitch ServiceContract = new CommandSwitch(ServiceContractKey, "sc", SwitchType.Flag);

            public void Init() { } // provided as a way to get the static class Switches loaded early.
        }
        #endregion

        public CommandProcessorOptions()
        {
            this.ReferencedTypes = new List<Type>();
            this.ReferencedAssemblies = new List<Assembly>();
            this.ReferencedCollectionTypes = new List<Type>();

            RegisterOptions(
                new SingleValueOption<string>(UpdateServiceReferenceKey) { CanSerialize = false });

            var typeReuseModeOption = this.GetOption(TypeReuseModeKey);
            typeReuseModeOption.Aliases.Add(Switches.NoTypeReuse.Name);
            typeReuseModeOption.ValueChanging += (s, e) => e.Value = ParseNoTypeReuseOptionValue(e.Value);

            Switches.Init();
        }

        public static new CommandProcessorOptions FromFile(string filePath, bool throwOnError = true)
        {
            return FromFile<CommandProcessorOptions>(filePath, throwOnError);
        }

        public static bool TryFromFile(string filePath, out CommandProcessorOptions options)
        {
            options = null;
            try
            {
                options = FromFile<CommandProcessorOptions>(filePath, throwOnError: false);
            }
            catch
            {
            }
            return options?.Errors.Count() == 0;
        }

        #region option processing methods
        internal static async Task<CommandProcessorOptions> ParseArgumentsAsync(string[] args, ILogger logger, CancellationToken cancellationToken)
        {
            CommandProcessorOptions cmdOptions = new CommandProcessorOptions();

            try
            {
                cmdOptions = CommandParser.ParseCommand(args);

                // Try to load parameters from input file.
                if (cmdOptions.Errors.Count() == 0 && cmdOptions.Inputs.Count == 1)
                {
                    if (PathHelper.IsFile(cmdOptions.Inputs[0], Directory.GetCurrentDirectory(), out var fileUri) &&
                        TryFromFile(fileUri.LocalPath, out var fileOptions) && fileOptions.GetOptions().Count() > 0)
                    {
                        // user switches are disallowed when a params file is provided.
                        var options = cmdOptions.GetOptions().ToList();
                        var disallowedSwitchesOnParamsFilesProvided = CommandSwitch.All
                            .Where(s => !s_cmdLineOverwriteSwitches.Contains(s.Name) && s.SwitchLevel <= OperationalContext.Global && options.Any(o =>
                            {
                                if (o.HasSameId(s.Name))
                                {
                                    o.Value = null;
                                    return true;
                                }
                                return false;
                            }));

                        // warn about disallowed options when params file has been provided and clear them.
                        if (disallowedSwitchesOnParamsFilesProvided.Count() > 0)
                        {
                            fileOptions.AddWarning(string.Format(SR.WrnExtraParamsOnInputFileParamIgnoredFormat, disallowedSwitchesOnParamsFilesProvided.Select(s => $"'{s.Name}'").Aggregate((msg, n) => $"{msg}, '{n}'")), 0);
                        }

                        fileOptions.ResolveFullPathsFrom(new DirectoryInfo(Path.GetDirectoryName(fileUri.LocalPath)));

                        // ensure inputs are clear as the params file is the input.
                        cmdOptions.Inputs.Clear();
                        cmdOptions.CopyTo(fileOptions);
                        cmdOptions = fileOptions;
                    }
                }
            }
            catch (ArgumentException ae)
            {
                cmdOptions.AddError(ae);
            }
            catch (FileLoadException fle)
            {
                cmdOptions.AddError(fle);
            }
            catch (FormatException fe)
            {
                cmdOptions.AddError(fe);
            }

            await cmdOptions.ProcessBasicOptionsAsync(logger, cancellationToken);

            return cmdOptions;
        }

        public async Task ResolveAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (this.Help != true && this.Errors.Count() == 0)
                {
                    ProcessLanguageOption();

                    ProcessSerializerOption();

                    // process project file first as it can define the working directory.
                    await ProcessProjectFileOptionAsync(cancellationToken).ConfigureAwait(false);

                    // next update option as the options may change.
                    await ProcessUpdateOptionAsync(cancellationToken).ConfigureAwait(false);

                    // next output directory and output file, they have a circular dependency resolved with the working directory.
                    await ProcessOutputDirOptionAsync(this.Project?.DirectoryPath, cancellationToken).ConfigureAwait(false);

                    await ProcessOutputFileOptionAsync(this.OutputDir.FullName, cancellationToken);

                    // target framework option depends on the boostrapping option (a temporary project may be needed).
                    await ProcessBootstrapDirOptionAsync(cancellationToken).ConfigureAwait(false);

                    // namespace mappings depends on the project and outputdir options to compute default namespace.
                    await ProcessNamespaceMappingsOptionAsync(cancellationToken).ConfigureAwait(false);

                    // inputs depends on the update option in case the real inputs come from the update params file.
                    await ProcessInputsAsync(cancellationToken).ConfigureAwait(false);

                    // target framework is needed by the references option.
                    await ProcessTargetFrameworkOptionAsync(cancellationToken).ConfigureAwait(false);

                    // type reuse is needed by the references option.
                    ProcessTypeReuseModeOption();

                    await ProcessReferencesOptionAsync(cancellationToken).ConfigureAwait(false);

                    // bootstrapping option deteremines whether referenced assemblies(next) should be processed now or by the bootstrapper.
                    ProcessBootstrappingOption();

                    await ProcessReferenceAssembliesAsync(cancellationToken).ConfigureAwait(false);
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (Exception ex)
            {
                if (Utils.IsFatalOrUnexpected(ex)) throw;
                this.AddError(ex);
            }
        }

        internal async Task ProcessBasicOptionsAsync(ILogger logger, CancellationToken cancellation)
        {
            var userOptions = this.GetOptions().ToList();

            if (userOptions.Count == 0)
            {
                // no options provided in the command line.
                this.Help = true;
            }

            if (!this.ToolContext.HasValue)
            {
                this.ToolContext = CommandSwitch.DefaultSwitchLevel;
            }

            if (!this.Verbosity.HasValue)
            {
                this.Verbosity = Svcutil.Verbosity.Normal;
            }

            if (!this.AcceptCert.HasValue)
            {
                this.AcceptCert = false;
            }

            if (!this.ServiceContract.HasValue)
            {
                this.ServiceContract = false;
            }

            this.Logger = logger ?? new DebugLogger();

            if (this.Logger is DebugLogger debugLooger)
            {
                debugLooger.EnableTracing = this.EnableLoggingMarkup == true || this.Verbosity == Svcutil.Verbosity.Debug;
            }

            if (this.Help != true)
            {
                using (SafeLogger safeLogger = await SafeLogger.WriteStartOperationAsync(this.Logger, "Validating options ...").ConfigureAwait(false))
                {
                    await safeLogger.WriteMessageAsync($"Tool context: {this.ToolContext}", logToUI: false).ConfigureAwait(false);

                    var disallowedContextSwitches = CommandSwitch.All.Where(s => s != Switches.ToolContext && s.SwitchLevel > this.ToolContext && userOptions.Any(o => o.HasSameId(s.Name)));
                    foreach (var cmdSwitch in disallowedContextSwitches)
                    {
                        this.AddWarning(string.Format(SR.WrnUnexpectedArgumentFormat, cmdSwitch.Name), 0);
                    }

                    if (IsUpdateOperation)
                    {
                        s_cmdLineOverwriteSwitches.Add(Switches.Update.Name);
                        var disallowedUserOptionsOnUpdateOperation = this.GetOptions().Where(o => !s_cmdLineOverwriteSwitches.Any(n => o.HasSameId(n)));

                        // special-case inputs as there's no switch for them.
                        if (this.Inputs.Count > 0)
                        {
                            this.AddWarning(string.Format(SR.WrnUnexpectedInputsFormat, this.Inputs.Select(i => $"{i}''").Aggregate((msg, i) => $"{msg}, {i}")));
                            await safeLogger.WriteMessageAsync($"Resetting unexpected option '{InputsKey}' ...", logToUI: false).ConfigureAwait(false);
                            this.Inputs.Clear();
                        }

                        foreach (var option in disallowedUserOptionsOnUpdateOperation)
                        {
                            this.AddWarning(string.Format(SR.WrnUnexpectedArgumentFormat, option.Name));
                            await safeLogger.WriteMessageAsync($"Resetting unexpected option '{option.Name}' ...", logToUI: false).ConfigureAwait(false);
                            option.Value = null; // this will exclude the invalid option from processing/serializing.
                        }
                    }
                }
            }

            Debug.Assert(this.ToolContext.HasValue, $"{nameof(ToolContext)} is not initialized!");
            Debug.Assert(this.Verbosity.HasValue, $"{nameof(Verbosity)} is not initialized!");
        }

        private async Task ProcessProjectFileOptionAsync(CancellationToken cancellationToken)
        {
            var projectFile = this.Project?.FullPath;

            if (projectFile == null)
            {
                using (SafeLogger logger = await SafeLogger.WriteStartOperationAsync(this.Logger, $"Resolving {ProjectFileKey} option ...").ConfigureAwait(false))
                {
                    // Resolve the project in the current directory.

                    var workingDirectory = Directory.GetCurrentDirectory();
                    var projects = Directory.GetFiles(workingDirectory, "*.csproj", SearchOption.TopDirectoryOnly);

                    if (projects.Length == 1)
                    {
                        projectFile = projects[0];
                    }
                    else if (projects.Length == 0)
                    {
                        if (this.ToolContext == OperationalContext.Project)
                        {
                            throw new ToolArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ErrInvalidOperationNoProjectFileFoundUnderFolderFormat, workingDirectory));
                        }
                    }
                    else if (projects.Length > 1)
                    {
                        var moreThanOneProjectMsg = string.Format(CultureInfo.CurrentCulture, SR.ErrMoreThanOneProjectFoundFormat, workingDirectory);
                        if (this.ToolContext != OperationalContext.Project)
                        {
                            var projectItems = projects.Aggregate((projectMsg, projectItem) => $"{projectMsg}, {projectItem}").Trim(',').Trim();
                            var useProjectOptions = string.Format(CultureInfo.CurrentCulture, SR.UseProjectFileOptionOnMultipleFilesMsgFormat, Switches.ProjectFile.Name, projectItems);
                            throw new ToolArgumentException($"{moreThanOneProjectMsg}{Environment.NewLine}{useProjectOptions}");
                        }
                        else
                        {
                            throw new ToolArgumentException(moreThanOneProjectMsg);
                        }
                    }

                    await logger.WriteMessageAsync($"{ProjectFileKey}:\"{projectFile}\"", logToUI: false).ConfigureAwait(false);
                }
            }


            if (this.Project == null && projectFile != null)
            {
                this.Project = await MSBuildProj.FromPathAsync(projectFile, this.Logger, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task ProcessOutputDirOptionAsync(string workingDirectory, CancellationToken cancellationToken)
        {
            if (this.OutputDir == null)
            {
                using (SafeLogger logger = await SafeLogger.WriteStartOperationAsync(this.Logger, $"Resolving {OutputDirKey} option ...").ConfigureAwait(false))
                {
                    if (string.IsNullOrEmpty(workingDirectory))
                    {
                        // First check the output file and use its directory if fully qualified, second try to use the project's directory if available,
                        // use the current directory otherwise.
                        var defaultDir = this.Project?.DirectoryPath ?? Directory.GetCurrentDirectory();
                        await ProcessOutputFileOptionAsync(defaultDir, cancellationToken).ConfigureAwait(false);

                        workingDirectory = Path.IsPathRooted(this.OutputFile.OriginalPath()) ?
                                Path.GetDirectoryName(this.OutputFile.FullName) : Path.GetDirectoryName(Path.Combine(Directory.GetCurrentDirectory(), this.OutputFile.OriginalPath()));
                    }

                    // Guard against infinite recursion as reentrancy can happen when resolving the output file above as it checks the output dir as well.
                    if (this.OutputDir == null)
                    {
                        this.OutputDir = IsUpdateOperation ?
                            new DirectoryInfo(Path.Combine(workingDirectory, this.UpdateServiceReferenceFolder)) :
                            PathHelper.CreateUniqueDirectoryName(BaseServiceReferenceName, new DirectoryInfo(workingDirectory));
                    }

                    await logger.WriteMessageAsync($"{OutputDirKey}:\"{this.OutputDir}\"", logToUI: false).ConfigureAwait(false);
                }
            }
            else
            {
                var originalDirSpec = this.OutputDir.ToString(); // ToString provides the original value of the DirectoryInfo.
                if (!Path.IsPathRooted(originalDirSpec))
                {
                    workingDirectory = workingDirectory ?? Directory.GetCurrentDirectory();
                    this.OutputDir = new DirectoryInfo(Path.Combine(workingDirectory, originalDirSpec));
                }
            }
        }

        private async Task ProcessOutputFileOptionAsync(string workingDirectory, CancellationToken cancellationToken)
        {
            using (SafeLogger logger = await SafeLogger.WriteStartOperationAsync(this.Logger, $"Resolving {OutputFileKey} option ...").ConfigureAwait(false))
            {
                var outputFile = this.OutputFile?.OriginalPath();
                if (outputFile == null)
                {
                    outputFile = "Reference.cs";
                }

                if (!outputFile.EndsWith(this.CodeProvider.FileExtension, RuntimeEnvironmentHelper.FileStringComparison))
                {
                    outputFile += $".{this.CodeProvider.FileExtension}";
                }

                // Ensure the ouput directory has been resolved first by using the specified directory as the default if not null, 
                // the project file directory if available, the current application directory otherwise.
                var defaultDir = workingDirectory ?? this.Project?.DirectoryPath ?? Directory.GetCurrentDirectory();
                await ProcessOutputDirOptionAsync(defaultDir, cancellationToken);

                if (PathHelper.IsUnderDirectory(outputFile, this.OutputDir, out var filePath, out var relPath))
                {
                    // if adding a new service reference fail if the output file already exists.
                    // notice that if bootstrapping, the bootstrapper doesn't understand the update opertion, it just knows to add the reference file.
                    if (!IsUpdateOperation && this.ToolContext <= OperationalContext.Global && File.Exists(filePath))
                    {
                        throw new ToolArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ErrOutputFileAlreadyExistsFormat, filePath, Switches.OutputFile.Name));
                    }
                    outputFile = filePath;
                }
                else
                {
                    throw new ToolArgumentException(
                        string.Format(CultureInfo.CurrentCulture, SR.ErrOutputFileNotUnderOutputDirFormat, Switches.OutputFile.Name, outputFile, this.OutputDir, Switches.OutputDirectory.Name));
                }

                if (this.ToolContext == OperationalContext.Project && this.Project != null &&
                    !PathHelper.IsUnderDirectory(outputFile, new DirectoryInfo(this.Project.DirectoryPath), out filePath, out relPath))
                {
                    this.AddWarning(string.Format(CultureInfo.CurrentCulture, SR.WrnSpecifiedFilePathNotUndeProjectDirFormat, Switches.OutputFile.Name, outputFile, this.Project.DirectoryPath));
                }

                this.OutputFile = new FileInfo(outputFile);

                await logger.WriteMessageAsync($"{OutputFileKey}:\"{filePath}\"", logToUI: false).ConfigureAwait(false);
            }
        }

        private async Task ProcessUpdateOptionAsync(CancellationToken cancellation)
        {
            if (IsUpdateOperation)
            {
                using (SafeLogger logger = await SafeLogger.WriteStartOperationAsync(this.Logger, $"Processing {UpdateServiceReferenceKey} option ...").ConfigureAwait(false))
                {
                    var projectDir = this.Project?.DirectoryPath;
                    if (projectDir == null)
                    {
                        if (this.ToolContext == OperationalContext.Project)
                        {
                            throw new ToolArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ErrInvalidOperationNoProjectFileFoundUnderFolderFormat, Directory.GetCurrentDirectory()));
                        }
                        else
                        {
                            throw new ToolArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ErrProjectToUpdateNotFoundFormat, Switches.Update.Name, Switches.ProjectFile.Name));
                        }
                    }

                    var paramsFilePath = string.Empty;
                    var fileRelPath = string.Empty;

                    // check whether the params file was passed instead of the folder name, this is not expected but let's deal with it.
                    var updateFileName = Path.GetFileName(this.UpdateServiceReferenceFolder);
                    if (updateFileName.Equals(CommandProcessorOptions.SvcutilParamsFileName, RuntimeEnvironmentHelper.FileStringComparison) ||
                        updateFileName.Equals(CommandProcessorOptions.WCFCSParamsFileName, RuntimeEnvironmentHelper.FileStringComparison))
                    {
                        // if the resolved path is empty, we will try to find the params file next.
                        this.UpdateServiceReferenceFolder = Path.GetDirectoryName(this.UpdateServiceReferenceFolder);
                    }

                    if (this.UpdateServiceReferenceFolder == string.Empty)
                    {
                        // param passed as flag, there must be only one service under the project.

                        var excludeJsonFiles = Directory.GetFiles(projectDir, "*.json", SearchOption.TopDirectoryOnly); // update json files must be under a reference folder, exclude any top json files.
                        var jsonFiles = Directory.GetFiles(projectDir, "*.json", SearchOption.AllDirectories);
                        var paramsFiles = jsonFiles.Except(excludeJsonFiles).Where(fn => Path.GetFileName(fn).Equals(CommandProcessorOptions.SvcutilParamsFileName, RuntimeEnvironmentHelper.FileStringComparison) ||
                                                                                         Path.GetFileName(fn).Equals(CommandProcessorOptions.WCFCSParamsFileName, RuntimeEnvironmentHelper.FileStringComparison));

                        if (paramsFiles.Count() == 1)
                        {
                            paramsFilePath = paramsFiles.First();
                        }
                        else if (paramsFiles.Count() == 0)
                        {
                            throw new ToolArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ErrNoUpdateParamsFileFoundFormat, this.Project.FullPath));
                        }

                        // no else here, this check applies to the inner block above as well.
                        if (paramsFiles.Count() > 1)
                        {
                            var svcRefNames = paramsFiles.Select(pf => { PathHelper.GetRelativePath(Path.GetDirectoryName(pf), new DirectoryInfo(projectDir), out var relPath); return relPath; })
                                                         .Select(f => $"'{f}'").Aggregate((files, f) => $"{files}, {f}");
                            throw new ToolArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ErrMoreThanOneUpdateParamsFilesFoundFormat, this.Project.FullPath, Switches.Update.Name, svcRefNames));
                        }

                        PathHelper.GetRelativePath(paramsFilePath, new DirectoryInfo(projectDir), out fileRelPath);
                    }
                    else
                    {
                        var projectDirInfo = new DirectoryInfo(projectDir);
                        var svcutilParmasFile = Path.Combine(projectDir, this.UpdateServiceReferenceFolder, CommandProcessorOptions.SvcutilParamsFileName);
                        if (!PathHelper.IsUnderDirectory(svcutilParmasFile, projectDirInfo, out paramsFilePath, out fileRelPath) || !File.Exists(paramsFilePath))
                        {
                            var wcfcsParamsFile = Path.Combine(projectDir, this.UpdateServiceReferenceFolder, CommandProcessorOptions.WCFCSParamsFileName);
                            if (!PathHelper.IsUnderDirectory(wcfcsParamsFile, projectDirInfo, out paramsFilePath, out fileRelPath) || !File.Exists(paramsFilePath))
                            {
                                throw new ToolArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ErrServiceReferenceNotFoundUnderProjectFormat, this.UpdateServiceReferenceFolder, this.Project.FullPath));
                            }
                        }
                    }

                    var relDir = Path.GetDirectoryName(fileRelPath);
                    if (string.IsNullOrEmpty(relDir))
                    {
                        throw new ToolArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ErrNoUpdateParamsFileFoundFormat, this.Project.FullPath));
                    }

                    this.UpdateServiceReferenceFolder = relDir;

                    UpdateOptions updateOptions = null;
                    if (Path.GetFileName(paramsFilePath).Equals(CommandProcessorOptions.WCFCSParamsFileName))
                    {
                        var wcfOptions = WCFCSUpdateOptions.FromFile(paramsFilePath);
                        updateOptions = wcfOptions.CloneAs<UpdateOptions>();
                    }
                    else
                    {
                        updateOptions = UpdateOptions.FromFile(paramsFilePath);
                    }

                    updateOptions.ResolveFullPathsFrom(new DirectoryInfo(Path.GetDirectoryName(paramsFilePath)));

                    // merge/overwrite options.
                    updateOptions.CopyTo(this);

                    await logger.WriteMessageAsync($"Update option read from \"{paramsFilePath}\" ...", logToUI: false).ConfigureAwait(false);
                }
            }
        }

        private async Task ProcessBootstrapDirOptionAsync(CancellationToken cancellationToken)
        {
            // NOTE: The bootstrapping directory is not only used for the svcutil bootstrapper but also for other temporary projects 
            // like the one generated to get the target framework. The svcutil bootstrapper is created under this directory.

            using (SafeLogger logger = await SafeLogger.WriteStartOperationAsync(this.Logger, $"Processing {BootstrapPathKey} option ...").ConfigureAwait(false))
            {
                if (this.BootstrapPath == null)
                {
                    var tempDir = Path.GetTempPath();
                    var baseDirName = $"{Tool.AssemblyName}_Temp";
                    var sessionDirName = DateTime.Now.ToString("yyyy_MMM_dd_HH_mm_ss", CultureInfo.InvariantCulture);

                    this.BootstrapPath = new DirectoryInfo(Path.Combine(tempDir, baseDirName, sessionDirName));
                }

                // delay creating the bootstrapping directory until needed.

                await logger.WriteMessageAsync($"{BootstrapPathKey}:\"{this.BootstrapPath}\"", logToUI: false).ConfigureAwait(false);
            }
        }

        private void ProcessBootstrappingOption()
        {
            if (this.NoBootstrapping != true) // value not set or set to false, check whether we need the boostrapper or not.
            {
                // bootstrapping is required for type reuse when targetting a supported .NET Core platform and when there are project references 
                // different form the .NET Core and WCF ones.
                this.RequiresBoostrapping = SvcutilBootstrapper.RequiresBootstrapping(this.TargetFramework, this.References);
            }
        }

        private void ProcessTypeReuseModeOption()
        {
            if (!this.TypeReuseMode.HasValue)
            {
                this.TypeReuseMode = this.References.Count == 0 ? Svcutil.TypeReuseMode.All : Svcutil.TypeReuseMode.Specified;
            }
        }

        private async Task ProcessReferencesOptionAsync(CancellationToken cancellationToken)
        {
            // references are resolved in order to reuse types from referenced assemblies for the proxy code generation, supported on DNX frameworks only.
            // resolve project references when the type reuse option or the bootstrapping option (which is meant for processing external references) have not been disabled 
            // and either no specific references have been provided or the service reference is being updated. In the latter case if type reuse is enabled for all
            // assemblies, we need to resolve references regardless because the user could have changed the project refernces since the web service reference was added.

            bool resolveReferences = this.Project != null && this.TargetFramework.IsDnx && this.NoBootstrapping != true && this.TypeReuseMode != Svcutil.TypeReuseMode.None &&
                                     (this.IsUpdateOperation || this.TypeReuseMode == Svcutil.TypeReuseMode.All);

            if (resolveReferences)
            {
                using (var logger = await SafeLogger.WriteStartOperationAsync(this.Logger, $"Processing {nameof(this.References)}, count: {this.References.Count}. Reference resolution enabled: {resolveReferences}").ConfigureAwait(false))
                {
                    await logger.WriteMessageAsync(Shared.Resources.ResolvingProjectReferences, logToUI: this.ToolContext <= OperationalContext.Global).ConfigureAwait(false);

                    var references = await this.Project.ResolveProjectReferencesAsync(ProjectDependency.IgnorableDependencies, logger, cancellationToken).ConfigureAwait(false);

                    if (this.TypeReuseMode == Svcutil.TypeReuseMode.All)
                    {
                        this.References.Clear();
                        this.References.AddRange(references);
                    }
                    else // Update operation: remove any reference no longer in the project!
                    {
                        for (int idx = this.References.Count - 1; idx >= 0; idx--)
                        {
                            if (!references.Contains(this.References[idx]))
                            {
                                this.References.RemoveAt(idx);
                            }
                        }
                    }
                }
            }

            this.References.Sort();
        }

        private async Task ProcessNamespaceMappingsOptionAsync(CancellationToken cancellationToken)
        {
            using (var logger = await SafeLogger.WriteStartOperationAsync(this.Logger, $"Processing {nameof(this.NamespaceMappings)}, count: {this.NamespaceMappings.Count}").ConfigureAwait(false))
            {
                if (this.NamespaceMappings.Count == 0)
                {
                    // try to add default namespace.
                    if (this.Project != null && PathHelper.GetRelativePath(this.OutputDir.FullName, new DirectoryInfo(this.Project.DirectoryPath), out var relPath))
                    {
                        var clrNamespace = CodeDomHelpers.GetValidValueTypeIdentifier(relPath);
                        this.NamespaceMappings.Add(new KeyValuePair<string, string>("*", clrNamespace));
                    }
                }
                else
                {
                    // validate provided namespace values.
                    var invalidNamespaces = this.NamespaceMappings.Where(nm => !CodeDomHelpers.IsValidNameSpace(nm.Value));
                    if (invalidNamespaces.Count() > 0)
                    {
                        throw new ToolArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ErrInvalidNamespaceFormat,
                            invalidNamespaces.Select(n => $"'{n.Key},{n.Value}'").Aggregate((msg, n) => $"{msg}, {n}")));
                    }
                }
            }
        }

        private async Task ProcessTargetFrameworkOptionAsync(CancellationToken cancellationToken)
        {
            if (this.TargetFramework == null)
            {
                var targetFrameworkMoniker = string.Empty;

                if (this.Project != null)
                {
                    targetFrameworkMoniker = this.Project.TargetFramework;
                }
                else
                {
                    Debug.Assert(this.BootstrapPath != null, $"{nameof(this.BootstrapPath)} is not initialized!");

                    using (SafeLogger logger = await SafeLogger.WriteStartOperationAsync(this.Logger, $"Resolving {TargetFrameworkKey} option ...").ConfigureAwait(false))
                    {
                        var projectFullPath = Path.Combine(this.BootstrapPath.FullName, "TFMResolver", "TFMResolver.csproj");

                        if (File.Exists(projectFullPath))
                        {
                            // this is not expected unless the boostrapping directory is reused (as in testing)
                            // as we don't know what SDK version was used to create the temporary project we better clean up.
                            Directory.Delete(Path.GetDirectoryName(projectFullPath));
                        }

                        await SetupBootstrappingDirectoryAsync(logger, cancellationToken).ConfigureAwait(false);

                        using (var proj = await MSBuildProj.DotNetNewAsync(projectFullPath, this.Logger, cancellationToken).ConfigureAwait(false))
                        {
                            targetFrameworkMoniker = proj.TargetFramework;
                        }
                    }
                }

                ProcessToolArg(() => this.TargetFramework = TargetFrameworkHelper.GetValidFrameworkInfo(targetFrameworkMoniker));
            }

            AppSettings.Initialize(this.TargetFramework);
        }

        private async Task ProcessInputsAsync(CancellationToken cancellationToken)
        {
            if (this.Inputs.Count == 0)
            {
                throw new ToolInputException(SR.ErrNoValidInputSpecified);
            }

            using (var safeLogger = await SafeLogger.WriteStartOperationAsync(this.Logger, $"Processing inputs, count: {this.Inputs.Count} ...").ConfigureAwait(false))
            {
                for (int idx = 0; idx < this.Inputs.Count; idx++)
                {
                    if (PathHelper.IsFile(this.Inputs[idx], Directory.GetCurrentDirectory(), out Uri metadataUri))
                    {
                        this.Inputs.RemoveAt(idx);
                        var inputFiles = Metadata.MetadataFileNameManager.ResolveFiles(metadataUri.LocalPath).Select(f => f.FullName);
                        await safeLogger.WriteMessageAsync($"resolved inputs: {inputFiles.Count()}", logToUI: false).ConfigureAwait(false);
                        foreach (var file in inputFiles)
                        {
                            this.Inputs.Insert(idx, new Uri(file));
                        }
                    }
                }
            }
        }

        private void ProcessSerializerOption()
        {
            if (!this.SerializerMode.HasValue)
            {
                this.SerializerMode = Svcutil.SerializerMode.Default;
            }
        }

        private void ProcessLanguageOption()
        {
            if (this.CodeProvider == null)
            {
                this.CodeProvider = CodeDomProvider.CreateProvider("csharp");
            }
        }
        #endregion

        #region serialization methods
        private object ParseNoTypeReuseOptionValue(object value)
        {
            object typeReuseMode = this.GetOption(TypeReuseModeKey).DefaultValue;

            if (value != null)
            {
                var stringValue = value.ToString();

                if (bool.TryParse(stringValue, out var notTypeReuse))
                {
                    typeReuseMode = notTypeReuse ? (object)Svcutil.TypeReuseMode.None : null;
                }
                else
                {
                    typeReuseMode = OptionValueParser.ParseEnum<TypeReuseMode>(stringValue, this.GetOption(TypeReuseModeKey));
                }
            }

            return typeReuseMode;
        }

        protected override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            if (this.TypeReuseMode == Svcutil.TypeReuseMode.All)
            {
                // no need to serialize references as they will have to be resolved again if service reference is updated.
                this.References.Clear();
            }
        }
        #endregion

        #region helper methods
        private async Task ProcessReferenceAssembliesAsync(CancellationToken cancellationToken)
        {
            // Reference processing is done when no bootstrapping is needed or by the boostrapper!

            if (!this.RequiresBoostrapping)
            {
                using (var logger = await SafeLogger.WriteStartOperationAsync(this.Logger, "Processing reference assemblies ...").ConfigureAwait(false))
                {
                    var foundCollectionTypes = AddSpecifiedTypesToDictionary(this.CollectionTypes, Switches.CollectionType.Name);
                    var excludedTypes = AddSpecifiedTypesToDictionary(this.ExcludeTypes, Switches.ExcludeType.Name);

                    LoadReferencedAssemblies();

                    foreach (Assembly assembly in this.ReferencedAssemblies)
                    {
                        AddReferencedTypesFromAssembly(assembly, foundCollectionTypes, excludedTypes);
                    }

                    if (this.NoStandardLibrary != true)
                    {
                        AddStdLibraries(foundCollectionTypes, excludedTypes);
                    }

                    AddReferencedCollectionTypes(this.CollectionTypes, foundCollectionTypes);
                }
            }
        }

        private void LoadReferencedAssemblies()
        {
            // we should not load the ServiceModel assemblies as types will clash with the private code types.
            var loadableReferences = this.References.Where(r => !TargetFrameworkHelper.ServiceModelPackages.Any(s => s.Name == r.Name));
            foreach (ProjectDependency reference in loadableReferences)
            {
                Assembly assembly = null;

                if (this.ToolContext == OperationalContext.Infrastructure)
                {
                    string projFolder = Path.Combine(this.BootstrapPath.FullName, nameof(SvcutilBootstrapper));
                    DirectoryInfo directoryInfo = new DirectoryInfo(projFolder);
                    FileInfo assemblyFile = directoryInfo.GetFiles(reference.AssemblyName + ".*", SearchOption.AllDirectories).FirstOrDefault();
                    if (assemblyFile != null)
                    {
                        assembly = Assembly.LoadFrom(assemblyFile.FullName);
                    }
                }
                else
                {
                    assembly = TypeLoader.LoadAssembly(reference.AssemblyName);
                }

                if (assembly != null)
                {
                    if (!this.ReferencedAssemblies.Contains(assembly))
                    {
                        this.ReferencedAssemblies.Add(assembly);
                    }
                }
            }
        }

        private static Dictionary<string, Type> AddSpecifiedTypesToDictionary(IList<string> typeArgs, string cmd)
        {
            Dictionary<string, Type> specifiedTypes = new Dictionary<string, Type>(typeArgs.Count);
            foreach (string typeArg in typeArgs)
            {
                if (specifiedTypes.ContainsKey(typeArg))
                {
                    throw new ToolArgumentException(string.Format(SR.ErrDuplicateValuePassedToTypeArgFormat, cmd, typeArg));
                }
                specifiedTypes.Add(typeArg, null);
            }
            return specifiedTypes;
        }

        private void AddReferencedTypesFromAssembly(Assembly assembly, Dictionary<string, Type> foundCollectionTypes, Dictionary<string, Type> excludedTypes)
        {
            foreach (Type type in TypeLoader.LoadTypes(assembly, this.Verbosity.Value))
            {
                TypeInfo info = type.GetTypeInfo();
                if (info.IsPublic || info.IsNestedPublic)
                {
                    if (!IsTypeSpecified(type, excludedTypes, Switches.ExcludeType.Name))
                    {
                        this.ReferencedTypes.Add(type);
                    }

                    if (IsTypeSpecified(type, foundCollectionTypes, Switches.CollectionType.Name))
                    {
                        this.ReferencedCollectionTypes.Add(type);
                    }
                }
            }
        }

        private void AddStdLibraries(Dictionary<string, Type> foundCollectionTypes, Dictionary<string, Type> excludedTypes)
        {
            List<Type> coreTypes = new List<Type>
            {
                typeof(int), // System.Runtime.dll
                typeof(System.ServiceModel.ChannelFactory), // System.ServiceModel (svcutil private code)
                typeof(System.Net.HttpStatusCode) // netstandard.dll, System.Net.Primitives.dll
            };

            foreach (var type in coreTypes)
            {
                Assembly stdLib = type.GetTypeInfo().Assembly;
                if (!this.ReferencedAssemblies.Contains(stdLib))
                {
                    AddReferencedTypesFromAssembly(stdLib, foundCollectionTypes, excludedTypes);
                }
            }
        }

        private static bool IsTypeSpecified(Type type, Dictionary<string, Type> specifiedTypes, string cmd)
        {
            string foundTypeName = null;

            // Search the Dictionary for the type
            // --------------------------------------------------------------------------------------------------------
            if (specifiedTypes.TryGetValue(type.FullName, out Type foundType))
            {
                foundTypeName = type.FullName;
            }
            else if (specifiedTypes.TryGetValue(type.AssemblyQualifiedName, out foundType))
            {
                foundTypeName = type.AssemblyQualifiedName;
            }

            // Throw appropriate error message if we found something and the entry value wasn't null
            // --------------------------------------------------------------------------------------------------------
            if (foundTypeName != null)
            {
                if (foundType != null && foundType != type)
                {
                    throw new ToolArgumentException(string.Format(SR.ErrCannotDisambiguateSpecifiedTypesFormat,
                        cmd, type.AssemblyQualifiedName, foundType.AssemblyQualifiedName));
                }
                else
                {
                    specifiedTypes[foundTypeName] = type;
                }
                return true;
            }

            return false;
        }

        private void AddReferencedCollectionTypes(IList<string> collectionTypesArgs, Dictionary<string, Type> foundCollectionTypes)
        {
            // Instantiated generics specified via /rct can only be added via assembly.GetType or Type.GetType
            foreach (string collectionType in collectionTypesArgs)
            {
                if (foundCollectionTypes[collectionType] == null)
                {
                    Type foundType = null;
                    foreach (Assembly assembly in this.ReferencedAssemblies)
                    {
                        try
                        {
                            foundType = assembly.GetType(collectionType);
                            foundCollectionTypes[collectionType] = foundType;
                        }
                        catch (Exception ex)
                        {
                            if (Utils.IsFatalOrUnexpected(ex)) throw;
                        }

                        if (foundType != null)
                            break;
                    }

                    try
                    {
                        foundType = foundType ?? Type.GetType(collectionType);
                    }
                    catch (Exception ex)
                    {
                        if (Utils.IsFatalOrUnexpected(ex)) throw;
                    }

                    if (foundType == null)
                    {
                        throw new ToolArgumentException(string.Format(SR.ErrCannotLoadSpecifiedTypeFormat, Switches.CollectionType.Name, collectionType, Switches.Reference.Name));
                    }
                    else
                    {
                        this.ReferencedCollectionTypes.Add(foundType);
                    }
                }
            }
        }

        private static void ProcessToolArg(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (Utils.IsFatalOrUnexpected(ex)) throw;
                throw new ToolArgumentException(ex.Message);
            }
        }

        internal async Task SetupBootstrappingDirectoryAsync(ILogger logger, CancellationToken cancellationToken)
        {
            var workingDirectory = this.Project?.DirectoryPath ?? Directory.GetCurrentDirectory();

            if (!Directory.Exists(this.BootstrapPath.FullName))
            {
                Directory.CreateDirectory(this.BootstrapPath.FullName);
                this.OwnsBootstrapDir = true;
            }

            await RuntimeEnvironmentHelper.TryCopyingConfigFiles(workingDirectory, this.BootstrapPath.FullName, logger, cancellationToken).ConfigureAwait(false);
        }
        #endregion

        internal void Cleanup()
        {
            try
            {
                this.Project?.Dispose();

                if (this.BootstrapPath != null && this.BootstrapPath.Exists)
                {
                    if (this.KeepBootstrapDir)
                    {
                        this.Logger?.WriteMessageAsync($"Bootstrap directory '{this.BootstrapPath}' ...", logToUI: false);
                    }
                    else if (this.OwnsBootstrapDir)
                    {
                        this.Logger?.WriteMessageAsync($"Deleting bootstrap directory '{this.BootstrapPath}' ...", logToUI: false);
                        this.BootstrapPath.Delete(recursive: true);
                    }
                }
            }
            catch
            {
            }
        }

        public string ToTelemetryString()
        {
            return GetOptions()
                .Where(o => o.CanSerialize)
                .Select(o => $"{o.Name}:[{OptionValueParser.GetTelemetryValue(o)}]")
                .Aggregate((num, s) => num + ", " + s).ToString();
        }
    }
}
