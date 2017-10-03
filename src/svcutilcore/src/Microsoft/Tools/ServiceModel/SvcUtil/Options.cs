//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace Microsoft.Tools.ServiceModel.SvcUtil
{
    using System;
    using System.ServiceModel.Channels;
    using System.Configuration;
    using System.Collections.Generic;
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Reflection;
    //using System.ServiceModel.Configuration;
    using System.ServiceModel;


    partial class Options
    {

        ToolMode? defaultMode;
        ToolMode validModes = ToolMode.Any;
        string modeSettingOption;
        string modeSettingValue;

        string targetValue;

        string outputFileArg;
        string directoryArg;
        string configFileArg;
        //ServiceModelSectionGroup toolConfig;  // to use instead of svcutil.exe.config
        //CodeDomProvider codeProvider;

        //SerializerMode serializer;
        bool asyncMethods;
        bool internalTypes;
        bool serializableAttribute;
        bool importXmlTypes;
        bool typedMessages;
        bool noLogo;
        bool noConfig;
        bool mergeConfig;
        bool dataContractOnly;

        bool enableDataBinding;
        string serviceName;
        List<string> inputParameters;
        List<Type> referencedTypes;
        List<Assembly> referencedAssemblies;
        List<Type> referencedCollectionTypes;
        Dictionary<string, Type> excludedTypes;
        bool nostdlib;
        Dictionary<string, string> namespaceMappings;
        //TypeResolver typeResolver;
        //TargetClientVersionMode targetClientVersion;
        bool useSerializerForFaults;
        bool wrapped;
        bool serviceContractGeneration;
        bool syncMethodOnly;

        internal string OutputFileArg { get { return outputFileArg; } }
        internal string DirectoryArg { get { return directoryArg; } }
        internal string ConfigFileArg { get { return configFileArg; } }
        //internal CodeDomProvider CodeProvider { get { return codeProvider; } }
        internal bool AsyncMethods { get { return asyncMethods; } }
        internal bool InternalTypes { get { return internalTypes; } }
        internal bool SerializableAttribute { get { return serializableAttribute; } }
        //internal SerializerMode Serializer { get { return serializer; } }
        internal bool ImportXmlTypes { get { return importXmlTypes; } }
        internal bool TypedMessages { get { return typedMessages; } }
        internal bool NoLogo { get { return noLogo; } }
        internal bool NoConfig { get { return noConfig || dataContractOnly; } }
        internal bool MergeConfig { get { return mergeConfig; } }
        internal string ServiceName { get { return serviceName; } }
        internal bool EnableDataBinding { get { return enableDataBinding; } }

        internal List<string> InputParameters { get { return inputParameters; } }
        internal List<Type> ReferencedTypes { get { return referencedTypes; } }
        internal List<Assembly> ReferencedAssemblies { get { return referencedAssemblies; } }
        internal List<Type> ReferencedCollectionTypes { get { return referencedCollectionTypes; } }
        internal bool Nostdlib { get { return nostdlib; } }
        internal Dictionary<string, string> NamespaceMappings { get { return namespaceMappings; } }

        //internal TypeResolver TypeResolver { get { return typeResolver; } }

        internal string ModeSettingOption { get { return modeSettingOption; } }
        internal string ModeSettingValue { get { return modeSettingValue; } }
        //internal TargetClientVersionMode TargetClientVersion { get { return targetClientVersion; } }
        internal bool UseSerializerForFaults { get { return useSerializerForFaults; } }
        internal bool Wrapped { get { return wrapped; } }
        internal bool ServiceContractGeneration { get { return serviceContractGeneration; } }
        internal bool SyncMethodOnly { get { return syncMethodOnly; } }

        //internal ServiceModelSectionGroup ServiceModelSectionGroup
        //{
        //    get
        //    {
        //        return toolConfig;
        //    }
        //}

        Options(ArgumentDictionary arguments)
        {
            OptionProcessingHelper optionProcessor = new OptionProcessingHelper(this, arguments);
            optionProcessor.ProcessArguments();
        }

        internal static Options ParseArguments(string[] args)
        {
            ArgumentDictionary arguments;
            try
            {
                arguments = CommandParser.ParseCommand(args, Options.Switches.All);
            }
            catch (ArgumentException ae)
            {
                throw new InvalidOperationException();
                //throw new ToolOptionException(ae.Message);
            }
            return new Options(arguments);
        }

        internal void SetAllowedModes(ToolMode newDefaultMode, ToolMode validModes, string option, string value)
        {
            //Tool.Assert(validModes != ToolMode.None, "validModes should never be set to None!");
            //Tool.Assert(newDefaultMode != ToolMode.None, "newDefaultMode should never be set to None!");
            //Tool.Assert((validModes & newDefaultMode) != ToolMode.None, "newDefaultMode must be a validMode!");
            //Tool.Assert(IsSingleBit(newDefaultMode), "newDefaultMode must Always represent a single mode!");

            //update/filter list of valid modes
            this.validModes &= validModes;
            if (this.validModes == ToolMode.None)
                throw new InvalidToolModeException();


            bool currentDefaultIsValid = (this.defaultMode.HasValue && (this.defaultMode & this.validModes) != ToolMode.None);
            bool newDefaultIsValid = (newDefaultMode & this.validModes) != ToolMode.None;
            if (!currentDefaultIsValid)
            {
                if (newDefaultIsValid)
                    this.defaultMode = newDefaultMode;
                else
                    this.defaultMode = null;
            }

            //If this is true, then this is an explicit mode setting
            if (IsSingleBit(validModes))
            {
                this.modeSettingOption = option;
                this.modeSettingValue = value;
            }
        }

        internal ToolMode? GetToolMode()
        {
            if (IsSingleBit(this.validModes))
                return this.validModes;
            return this.defaultMode;
        }

        internal string GetCommandLineString(string option, string value)
        {
            return (value == null) ? option : option + ":" + value;
        }

        static bool IsSingleBit(ToolMode mode)
        {
            //figures out if the mode has a single bit set ( is a power of 2)
            int x = (int)mode;
            return (x != 0) && ((x & (x + ~0)) == 0);
        }

        internal bool IsTypeExcluded(Type type)
        {
            return OptionProcessingHelper.IsTypeSpecified(type, this.excludedTypes, Options.Cmd.ExcludeType);
        }

        class OptionProcessingHelper
        {
            Options parent;
            ArgumentDictionary arguments;


            static Type typeOfDateTimeOffset = typeof(DateTimeOffset);

            internal OptionProcessingHelper(Options options, ArgumentDictionary arguments)
            {
                this.parent = options;
                this.arguments = arguments;
            }

            internal void ProcessArguments()
            {
                //CheckForBasicOptions();

                //if (CheckForHelpOption())
                //    return;

                //We're Checking these values first because they are explicit statements about tool mode.
                //CheckForTargetOrValidateOptions();

                //ProcessDirectoryOption();
                //ProcessOutputOption();
                //ProcessServiceNameOption();

                //ReadInputArguments();

                //ParseMiscCodeGenOptions();

                //ParseLanguageOption();

                //ParseConfigOption();

                //ParseNamespaceMappings();

                ParseReferenceAssemblies();

                //ParseCustomConfigOption();
            }

            //void ParseServiceContractOption()
            //{
            //    parent.serviceContractGeneration = arguments.ContainsArgument(Options.Cmd.ServiceContract);
            //    if (parent.serviceContractGeneration)
            //    {
            //        SetAllowedModesFromOption(ToolMode.ServiceContractGeneration, ToolMode.ServiceContractGeneration, Options.Cmd.ServiceContract, null);
            //    }
            //}

            //bool CheckForHelpOption()
            //{
            //    if (arguments.ContainsArgument(Options.Cmd.Help) || arguments.Count == 0)
            //    {
            //        parent.SetAllowedModes(ToolMode.DisplayHelp, ToolMode.DisplayHelp, Options.Cmd.Help, null);
            //        return true;
            //    }
            //    return false;
            //}

            //            void CheckForTargetOrValidateOptions()
            //            {
            //                if (arguments.ContainsArgument(Options.Cmd.Target))
            //                {
            //                    ParseTargetOption(arguments.GetArgument(Options.Cmd.Target));
            //                }

            //                if (arguments.ContainsArgument(Options.Cmd.Validate))
            //                {
            //                    try
            //                    {
            //                        parent.SetAllowedModes(ToolMode.Validate, ToolMode.Validate, Options.Cmd.Validate, null);
            //                    }
            //                    catch (InvalidToolModeException)
            //                    {
            //                        throw new ToolOptionException(SR.GetString(SR.ErrValidateInvalidUse, Options.Cmd.Validate, Options.Cmd.Target));
            //                    }

            //                    if (!arguments.ContainsArgument(Options.Cmd.ServiceName))
            //                    {
            //                        throw new ToolOptionException(SR.GetString(SR.ErrValidateRequiresServiceName, Options.Cmd.ServiceName));
            //                    }
            //                }
            //            }

            //            void ParseTargetOption(string targetValue)
            //            {
            //                try
            //                {
            //                    if (String.Equals(targetValue, Options.Targets.Metadata, StringComparison.OrdinalIgnoreCase))
            //                    {
            //                        parent.SetAllowedModes(ToolMode.MetadataFromAssembly, ToolMode.MetadataFromAssembly | ToolMode.DataContractExport | ToolMode.WSMetadataExchange, Options.Cmd.Target, targetValue);
            //                    }
            //                    else if (String.Equals(targetValue, Options.Targets.Code, StringComparison.OrdinalIgnoreCase))
            //                    {
            //                        parent.SetAllowedModes(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.Target, targetValue);
            //                    }
            //                    else if (String.Equals(targetValue, Options.Targets.XmlSerializer, StringComparison.OrdinalIgnoreCase))
            //                    {
            //                        parent.SetAllowedModes(ToolMode.XmlSerializerGeneration, ToolMode.XmlSerializerGeneration, Options.Cmd.Target, targetValue);
            //                    }
            //                    else
            //                    {
            //                        throw new ToolOptionException(SR.GetString(SR.ErrInvalidTarget, Options.Cmd.Target, targetValue, Options.Targets.SupportedTargets));
            //                    }
            //                    parent.targetValue = targetValue;
            //                }
            //                catch (InvalidToolModeException)
            //                {
            //                    Tool.Assert(true, "This should have been the first check and shouldn't ever be called");
            //                }

            //            }

            //            void CheckForBasicOptions()
            //            {
            //                parent.noLogo = arguments.ContainsArgument(Options.Cmd.NoLogo);
            //#if DEBUG
            //                ToolConsole.SetOptions(arguments.ContainsArgument(Options.Cmd.Debug));
            //#endif
            //            }

            //            void ProcessDirectoryOption()
            //            {
            //                // Directory
            //                //---------------------------------------------------------------------------------------------------------
            //                if (arguments.ContainsArgument(Options.Cmd.Directory))
            //                {

            //                    string directoryArgValue = arguments.GetArgument(Options.Cmd.Directory);

            //                    try
            //                    {
            //                        ValidateIsDirectoryPathOnly(Options.Cmd.Directory, directoryArgValue);

            //                        if (!directoryArgValue.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            //                            directoryArgValue += Path.DirectorySeparatorChar;

            //                        parent.directoryArg = Path.GetFullPath(directoryArgValue);
            //                    }
            //                    catch (ToolOptionException)
            //                    {
            //                        throw;
            //                    }
            //#pragma warning suppress 56500 // covered by FxCOP
            //                    catch (Exception e)
            //                    {
            //                        if (Tool.IsFatal(e))
            //                            throw;

            //                        throw new ToolArgumentException(SR.GetString(SR.ErrInvalidPath, directoryArgValue, Options.Cmd.Directory), e);
            //                    }
            //                }
            //                else
            //                {
            //                    parent.directoryArg = null;
            //                }
            //            }

            //            static void ValidateIsDirectoryPathOnly(string arg, string value)
            //            {
            //                ValidatePath(arg, value);
            //                FileInfo fileInfo = new FileInfo(value);
            //                if (fileInfo.Exists)
            //                    throw new ToolOptionException(SR.GetString(SR.ErrDirectoryPointsToAFile, arg, value));
            //            }

            //            static void ValidatePath(string arg, string value)
            //            {
            //                int invalidCharacterIndex = value.IndexOfAny(Path.GetInvalidPathChars());

            //                if (invalidCharacterIndex != -1)
            //                {
            //                    string invalidCharacter = value[invalidCharacterIndex].ToString();
            //                    throw new ToolOptionException(SR.GetString(SR.ErrDirectoryContainsInvalidCharacters, arg, value, invalidCharacter));
            //                }
            //            }

            //            void ProcessOutputOption()
            //            {
            //                if (arguments.ContainsArgument(Options.Cmd.Out))
            //                {
            //                    parent.outputFileArg = arguments.GetArgument(Options.Cmd.Out);

            //                    if (parent.outputFileArg != string.Empty)
            //                    {
            //                        SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.XmlSerializerGeneration | ToolMode.ServiceContractGeneration, Options.Cmd.Out, "");

            //                        ValidatePath(Options.Cmd.Out, parent.outputFileArg);
            //                    }
            //                }
            //                else
            //                {
            //                    parent.outputFileArg = null;
            //                }
            //            }

            //            void ProcessServiceNameOption()
            //            {
            //                if (arguments.ContainsArgument(Options.Cmd.ServiceName))
            //                {
            //                    parent.serviceName = arguments.GetArgument(Options.Cmd.ServiceName);

            //                    if (parent.serviceName != string.Empty)
            //                    {
            //                        SetAllowedModesFromOption(ToolMode.MetadataFromAssembly, ToolMode.MetadataFromAssembly | ToolMode.Validate, Options.Cmd.ServiceName, "");
            //                    }
            //                }
            //                else
            //                {
            //                    parent.serviceName = null;
            //                }
            //            }

            //            void ReadInputArguments()
            //            {
            //                parent.inputParameters = new List<string>(arguments.GetArguments(String.Empty));
            //            }

            //            void ParseMiscCodeGenOptions()
            //            {
            //                ParseSerializerOption();

            //                ParseDCOnly();

            //                ParseServiceContractOption();

            //                ParseTargetClientVersionOption();

            //                parent.wrapped = arguments.ContainsArgument(Options.Cmd.Wrapped);
            //                if (parent.wrapped)
            //                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.Wrapped, null);

            //                parent.useSerializerForFaults = arguments.ContainsArgument(Options.Cmd.UseSerializerForFaults);
            //                if (parent.useSerializerForFaults)
            //                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.UseSerializerForFaults, null);

            //                parent.importXmlTypes = arguments.ContainsArgument(Options.Cmd.ImportXmlTypes);
            //                if (parent.importXmlTypes)
            //                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.ImportXmlTypes, null);

            //                parent.noConfig = arguments.ContainsArgument(Options.Cmd.NoConfig);
            //                if (parent.noConfig)
            //                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration, Options.Cmd.NoConfig, null);

            //                parent.internalTypes = arguments.ContainsArgument(Options.Cmd.Internal);
            //                if (parent.internalTypes)
            //                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.Internal, null);

            //                parent.serializableAttribute = arguments.ContainsArgument(Options.Cmd.Serializable);
            //                if (parent.serializableAttribute)
            //                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.Serializable, null);

            //                parent.typedMessages = arguments.ContainsArgument(Options.Cmd.MessageContract);
            //                if (parent.typedMessages)
            //                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.ServiceContractGeneration, Options.Cmd.MessageContract, null);

            //                parent.enableDataBinding = arguments.ContainsArgument(Options.Cmd.EnableDataBinding);
            //                if (parent.enableDataBinding)
            //                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.EnableDataBinding, null);

            //                parent.asyncMethods = arguments.ContainsArgument(Options.Cmd.Async);
            //                if (parent.asyncMethods)
            //                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.ServiceContractGeneration, Options.Cmd.Async, null);

            //                parent.syncMethodOnly = arguments.ContainsArgument(Options.Cmd.SyncOnly);
            //                if (parent.syncMethodOnly)
            //                {
            //                    if (parent.asyncMethods)
            //                    {
            //                        throw new ToolOptionException(SR.GetString(SR.ErrExclusiveOptionsSpecified, Options.Cmd.SyncOnly, Options.Cmd.Async));
            //                    }
            //                    else
            //                    {
            //                        SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.ServiceContractGeneration, Options.Cmd.SyncOnly, null);
            //                    }
            //                }
            //            }

            //            private void ParseDCOnly()
            //            {
            //                parent.dataContractOnly = arguments.ContainsArgument(Options.Cmd.DataContractOnly);

            //                if (parent.dataContractOnly)
            //                    SetAllowedModesFromOption(ToolMode.DataContractImport, ToolMode.DataContractImport | ToolMode.DataContractExport, Options.Cmd.DataContractOnly, null);

            //            }

            //            void ParseSerializerOption()
            //            {
            //                if (arguments.ContainsArgument(Options.Cmd.Serializer))
            //                {
            //                    string serializerValue = arguments.GetArgument(Options.Cmd.Serializer);
            //                    try
            //                    {
            //                        parent.serializer = (SerializerMode)Enum.Parse(typeof(SerializerMode), serializerValue, true);
            //                    }
            //                    catch (ArgumentException)
            //                    {
            //                        throw new ToolOptionException(SR.GetString(SR.ErrInvalidSerializer, Options.Cmd.Serializer, serializerValue, Options.SupportedSerializers));
            //                    }
            //                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.Serializer, null);
            //                }
            //                else
            //                {
            //                    parent.serializer = SerializerMode.Default;
            //                }
            //            }

            //            void ParseTargetClientVersionOption()
            //            {
            //                if (arguments.ContainsArgument(Options.Cmd.TargetClientVersion))
            //                {
            //                    string targetClientVersionValue = arguments.GetArgument(Options.Cmd.TargetClientVersion);
            //                    try
            //                    {
            //                        parent.targetClientVersion = (TargetClientVersionMode)Enum.Parse(typeof(TargetClientVersionMode), targetClientVersionValue, true);
            //                    }
            //                    catch (ArgumentException)
            //                    {
            //                        throw new ToolOptionException(SR.GetString(SR.ErrInvalidTargetClientVersion, Options.Cmd.TargetClientVersion, targetClientVersionValue, Options.SupportedTargetClientVersions));
            //                    }
            //                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.TargetClientVersion, null);
            //                }
            //                else
            //                {
            //                    parent.targetClientVersion = TargetClientVersionMode.Version30;
            //                }
            //            }

            //            void ParseLanguageOption()
            //            {
            //                if (arguments.ContainsArgument(Options.Cmd.Language))
            //                {
            //                    string langValue = arguments.GetArgument(Options.Cmd.Language);
            //                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.Language, langValue);

            //                    parent.codeProvider = CreateLanguageProvider(langValue);
            //                }
            //                else
            //                {
            //                    parent.codeProvider = CodeDomProvider.CreateProvider("csharp");
            //                }
            //            }

            //            void ParseCustomConfigOption()
            //            {
            //                // Parse custom confug /svsutilConfig:<file_name>
            //                //-----------------------------------------------------------------------------------------------------

            //                string toolConfigFileArg = null;
            //                if (arguments.ContainsArgument(Options.Cmd.ToolConfig))
            //                {
            //                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.WSMetadataExchange | ToolMode.ServiceContractGeneration, Options.Cmd.ToolConfig, null);
            //                    toolConfigFileArg = arguments.GetArgument(Options.Cmd.ToolConfig);
            //                    ValidatePath(Options.Cmd.ToolConfig, toolConfigFileArg);

            //                    if (!File.Exists(toolConfigFileArg))
            //                    {
            //                        throw new ToolOptionException(SR.GetString(SR.ErrToolConfigDoesNotExist, toolConfigFileArg, Options.Cmd.ToolConfig));
            //                    }
            //                }

            //                parent.toolConfig = LoadToolConfig(toolConfigFileArg);
            //            }

            //            ServiceModelSectionGroup LoadToolConfig(string customConfigFileArg)
            //            {
            //                try
            //                {
            //                    Configuration config;
            //                    if (customConfigFileArg != null)
            //                    {
            //                        ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            //                        fileMap.ExeConfigFilename = customConfigFileArg;
            //                        config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            //                    }
            //                    else
            //                    {
            //                        config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //                    }

            //                    Tool.Assert(config != null, "Configuration Object should Never be null");
            //                    return ServiceModelSectionGroup.GetSectionGroup(config);

            //                }
            //                catch (ConfigurationErrorsException e)
            //                {
            //                    string configPath = customConfigFileArg ?? AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            //                    throw new ToolInputException(SR.GetString(SR.ErrUnableToLoadInputConfig, configPath), e);
            //                }
            //            }


            //            void ParseConfigOption()
            //            {
            //                // Parse config
            //                //-----------------------------------------------------------------------------------------------------
            //                if (arguments.ContainsArgument(Options.Cmd.Config))
            //                {
            //                    if (parent.noConfig)
            //                        throw new ToolOptionException(SR.GetString(SR.ErrExclusiveOptionsSpecified, Options.Cmd.NoConfig, Options.Cmd.Config));

            //                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration, Options.Cmd.Config, null);

            //                    parent.configFileArg = arguments.GetArgument(Options.Cmd.Config);

            //                    if (arguments.ContainsArgument(Options.Cmd.MergeConfig))
            //                        parent.mergeConfig = true;
            //                    else
            //                        parent.mergeConfig = false;

            //                    if (parent.configFileArg != null)
            //                        ValidatePath(Options.Cmd.Config, parent.configFileArg);
            //                }
            //                else
            //                {
            //                    if (arguments.ContainsArgument(Options.Cmd.MergeConfig))
            //                        throw new ToolOptionException(SR.GetString(SR.ErrMergeConfigUsedWithoutConfig, Options.Cmd.MergeConfig, Options.Cmd.Config));

            //                    parent.configFileArg = null;
            //                    parent.mergeConfig = false;
            //                }
            //            }

            //            void ParseNamespaceMappings()
            //            {
            //                IList<string> namespaceMappingsArgs = arguments.GetArguments(Options.Cmd.Namespace);
            //                parent.namespaceMappings = new Dictionary<string, string>(namespaceMappingsArgs.Count);

            //                foreach (string namespaceMapping in namespaceMappingsArgs)
            //                {
            //                    string[] parts = namespaceMapping.Split(',');

            //                    if (parts == null || parts.Length != 2)
            //                        throw new ToolOptionException(SR.GetString(SR.ErrInvalidNamespaceArgument, Options.Cmd.Namespace, namespaceMapping));

            //                    string targetNamespace = parts[0].Trim();
            //                    string clrNamespace = parts[1].Trim();

            //                    if (parent.namespaceMappings.ContainsKey(targetNamespace))
            //                    {
            //                        string prevClrNamespace = parent.namespaceMappings[targetNamespace];
            //                        if (prevClrNamespace != clrNamespace)
            //                            throw new ToolOptionException(SR.GetString(SR.ErrCannotSpecifyMultipleMappingsForNamespace,
            //                                Options.Cmd.Namespace, targetNamespace, prevClrNamespace, clrNamespace));
            //                    }
            //                    else
            //                    {
            //                        parent.namespaceMappings.Add(targetNamespace, clrNamespace);
            //                    }

            //                }

            //            }

            void ParseReferenceAssemblies()
            {
                IList<string> referencedAssembliesArgs = arguments.GetArguments(Options.Cmd.Reference);
                IList<string> excludeTypesArgs = arguments.GetArguments(Options.Cmd.ExcludeType);
                IList<string> referencedCollectionTypesArgs = arguments.GetArguments(Options.Cmd.CollectionType);
                bool nostdlib = arguments.ContainsArgument(Options.Cmd.Nostdlib);

                //if (excludeTypesArgs != null && excludeTypesArgs.Count > 0)
                   // SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.MetadataFromAssembly | ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.XmlSerializerGeneration | ToolMode.ServiceContractGeneration, Options.Cmd.ExcludeType, null);

                AddReferencedTypes(referencedAssembliesArgs, excludeTypesArgs, referencedCollectionTypesArgs, nostdlib);
                //parent.typeResolver = CreateTypeResolver(parent);
            }

            //            void SetAllowedModesFromOption(ToolMode newDefaultMode, ToolMode allowedModes, string option, string value)
            //            {
            //                try
            //                {
            //                    parent.SetAllowedModes(newDefaultMode, allowedModes, option, value);
            //                }
            //                catch (InvalidToolModeException)
            //                {
            //                    string optionStr = parent.GetCommandLineString(option, value);
            //                    if (parent.modeSettingOption != null)
            //                    {
            //                        if (parent.modeSettingOption == Options.Cmd.Target)
            //                        {
            //                            throw new ToolOptionException(SR.GetString(SR.ErrOptionConflictsWithTarget, Options.Cmd.Target, parent.ModeSettingValue, optionStr));
            //                        }
            //                        else
            //                        {
            //                            string modeSettingStr = parent.GetCommandLineString(parent.modeSettingOption, parent.modeSettingValue);
            //                            throw new ToolOptionException(SR.GetString(SR.ErrOptionModeConflict, optionStr, modeSettingStr));
            //                        }
            //                    }
            //                    else
            //                    {

            //                        throw new ToolOptionException(SR.GetString(SR.ErrAmbiguousOptionModeConflict, optionStr));
            //                    }
            //                }

            //            }

            //            CodeDomProvider CreateLanguageProvider(string language)
            //            {

            //                if (CodeDomProvider.IsDefinedLanguage(language))
            //                {
            //                    try
            //                    {
            //                        return CodeDomProvider.CreateProvider(language);
            //                    }
            //#pragma warning suppress 56500 // covered by FxCOP
            //                    catch (Exception e)
            //                    {
            //                        if (Tool.IsFatal(e))
            //                            throw;

            //                        throw new ToolOptionException(SR.GetString(SR.ErrCouldNotCreateCodeProvider, language, Options.Cmd.Language), e);
            //                    }
            //                }
            //                else
            //                {
            //                    return CreateCustomLanguageProvider(language);
            //                }
            //            }

            //            CodeDomProvider CreateCustomLanguageProvider(string language)
            //            {
            //                //try to reflect a custom code generator
            //                //ignore case when reflecting; language argument must specify the namespace
            //                Type t = Type.GetType(language, false, true);

            //                if (t == null)
            //                    throw new ToolOptionException(SR.GetString(SR.ErrNotLanguageOrCodeDomType, language, Options.Cmd.Language));

            //                if (!t.IsSubclassOf(typeof(CodeDomProvider)))
            //                    throw new ToolOptionException(SR.GetString(SR.ErrNotCodeDomType, language, Options.Cmd.Language, typeof(CodeDomProvider).FullName));

            //                try
            //                {
            //                    return Activator.CreateInstance(t) as CodeDomProvider;
            //                }
            //#pragma warning suppress 56500 // covered by FxCOP
            //                catch (Exception e)
            //                {
            //                    if (Tool.IsFatal(e))
            //                        throw;

            //                    throw new ToolOptionException(SR.GetString(SR.ErrCouldNotCreateInstance, language, Options.Cmd.Language), e);
            //                }
            //            }

            void AddReferencedTypes(IList<string> referenceArgs, IList<string> excludedTypeArgs, IList<string> collectionTypesArgs, bool nostdlib)
            {
                parent.referencedTypes = new List<Type>();
                parent.referencedAssemblies = new List<Assembly>(referenceArgs.Count);
                parent.referencedCollectionTypes = new List<Type>();
                parent.nostdlib = nostdlib;
                parent.excludedTypes = AddSpecifiedTypesToDictionary(excludedTypeArgs, Options.Cmd.ExcludeType);

                //Add the DateTimeOffset type to excluded types if the target client version is 3.0.
                //Ensures that the DateTimeOffset type is not referenced even if mscorlib is referenced from a non-3.0 machine.

                //switch (parent.targetClientVersion)
                //{
                //    case TargetClientVersionMode.Version35:
                //        break;
                //    default:
                //        if (!parent.IsTypeExcluded(typeOfDateTimeOffset))
                //        {
                //            parent.excludedTypes.Add(typeOfDateTimeOffset.FullName, typeOfDateTimeOffset);
                //        }
                //        break;
                //}


                Dictionary<string, Type> foundCollectionTypes = AddSpecifiedTypesToDictionary(collectionTypesArgs, Options.Cmd.CollectionType);

                //LoadReferencedAssemblies(referenceArgs);

                foreach (Assembly assembly in parent.referencedAssemblies)
                {
                    //AddReferencedTypesFromAssembly(assembly, foundCollectionTypes);
                }

                if (!nostdlib)
                {
                    //AddMscorlib(foundCollectionTypes);
                    //AddServiceModelLib(foundCollectionTypes);
                }
                //AddReferencedCollectionTypes(collectionTypesArgs, foundCollectionTypes);
            }

            //            void LoadReferencedAssemblies(IList<string> referenceArgs)
            //            {
            //                foreach (string path in referenceArgs)
            //                {
            //                    Assembly assembly;
            //                    try
            //                    {
            //                        assembly = InputModule.LoadAssembly(path);
            //                        if (!parent.referencedAssemblies.Contains(assembly))
            //                        {
            //                            parent.referencedAssemblies.Add(assembly);
            //                        }
            //                        else
            //                        {
            //                            throw new ToolOptionException(SR.GetString(SR.ErrDuplicateReferenceValues, Options.Cmd.Reference, assembly.Location));
            //                        }
            //                    }
            //#pragma warning suppress 56500 // covered by FxCOP
            //                    catch (Exception e)
            //                    {
            //                        if (Tool.IsFatal(e))
            //                            throw;

            //                        throw new ToolOptionException(SR.GetString(SR.ErrCouldNotLoadReferenceAssemblyAt, path), e);
            //                    }
            //                }
            //            }

            Dictionary<string, Type> AddSpecifiedTypesToDictionary(IList<string> typeArgs, string cmd)
            {
                Dictionary<string, Type> specifiedTypes = new Dictionary<string, Type>(typeArgs.Count);
                foreach (string typeArg in typeArgs)
                {
                    if (specifiedTypes.ContainsKey(typeArg))
                        //throw new ToolOptionException(SR.GetString(SR.ErrDuplicateValuePassedToTypeArg, cmd, typeArg));
                    specifiedTypes.Add(typeArg, null);
                }
                return specifiedTypes;
            }

            //            void AddReferencedTypesFromAssembly(Assembly assembly, Dictionary<string, Type> foundCollectionTypes)
            //            {
            //                foreach (Type type in InputModule.LoadTypes(assembly))
            //                {
            //                    if (type.IsPublic || type.IsNestedPublic)
            //                    {
            //                        if (!parent.IsTypeExcluded(type))
            //                            parent.referencedTypes.Add(type);
            //                        else if (IsTypeSpecified(type, foundCollectionTypes, Options.Cmd.CollectionType))
            //                            parent.referencedCollectionTypes.Add(type);
            //                    }
            //                }

            //            }

            //            void AddMscorlib(Dictionary<string, Type> foundCollectionTypes)
            //            {
            //                Assembly mscorlib = typeof(int).Assembly;
            //                if (!parent.referencedAssemblies.Contains(mscorlib))
            //                {
            //                    AddReferencedTypesFromAssembly(mscorlib, foundCollectionTypes);
            //                }
            //            }

            //            void AddServiceModelLib(Dictionary<string, Type> foundCollectionTypes)
            //            {
            //                Assembly serviceModelLib = typeof(ServiceHost).Assembly;
            //                if (!parent.referencedAssemblies.Contains(serviceModelLib))
            //                {
            //                    AddReferencedTypesFromAssembly(serviceModelLib, foundCollectionTypes);
            //                }
            //            }

            internal static bool IsTypeSpecified(Type type, Dictionary<string, Type> specifiedTypes, string cmd)
            {
                Type foundType = null;
                string foundTypeName = null;

                // Search the Dictionary for the type
                // --------------------------------------------------------------------------------------------------------
                if (specifiedTypes.TryGetValue(type.FullName, out foundType))
                    foundTypeName = type.FullName;

                if (specifiedTypes.TryGetValue(type.AssemblyQualifiedName, out foundType))
                    foundTypeName = type.AssemblyQualifiedName;

                // Throw appropriate error message if we found something and the entry value wasn't null
                // --------------------------------------------------------------------------------------------------------
                if (foundTypeName != null)
                {
                    if (foundType != null && foundType != type)
                    {
                        //throw new ToolOptionException(SR.GetString(SR.ErrCannotDisambiguateSpecifiedTypes,
                        //    cmd, type.AssemblyQualifiedName, foundType.AssemblyQualifiedName));
                    }
                    else
                    {
                        specifiedTypes[foundTypeName] = type;
                    }
                    return true;
                }

                return false;
            }

            //            void AddReferencedCollectionTypes(IList<string> collectionTypesArgs, Dictionary<string, Type> foundCollectionTypes)
            //            {
            //                // Instantiated generics specified via /rct can only be added via assembly.GetType or Type.GetType
            //                foreach (string collectionType in collectionTypesArgs)
            //                {
            //                    if (foundCollectionTypes[collectionType] == null)
            //                    {
            //                        Type foundType = null;
            //                        foreach (Assembly assembly in parent.referencedAssemblies)
            //                        {
            //                            foundType = assembly.GetType(collectionType);
            //                            if (foundType != null)
            //                                break;
            //                        }
            //                        foundType = foundType ?? Type.GetType(collectionType);
            //                        if (foundType == null)
            //                            throw new ToolOptionException(SR.GetString(SR.ErrCannotLoadSpecifiedType, Options.Cmd.CollectionType,
            //                                collectionType, Options.Cmd.Reference));
            //                        else
            //                            parent.referencedCollectionTypes.Add(foundType);
            //                    }
            //                }
            //            }

            //            static TypeResolver CreateTypeResolver(Options options)
            //            {
            //                TypeResolver typeResolver = new TypeResolver(options);
            //                AppDomain.CurrentDomain.TypeResolve += new ResolveEventHandler(typeResolver.ResolveType);
            //                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(typeResolver.ResolveAssembly);

            //                return typeResolver;
            //            }

        }

    }

}
