// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.SvcUtil.XmlSerializer
{
    using System;
    using System.ServiceModel.Channels;
    using System.Configuration;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.ServiceModel;


    internal partial class Options
    {
        private ToolMode? _defaultMode;
        private ToolMode _validModes = ToolMode.Any;
        private string _modeSettingOption;
        private string _modeSettingValue;

        private string _targetValue;

        private string _outputFileArg;
        private string _directoryArg;
        private string _configFileArg;

        private SerializerMode _serializer;
        private bool _asyncMethods;
        private bool _internalTypes;
        private bool _serializableAttribute;
        private bool _importXmlTypes;
        private bool _typedMessages;
        private bool _noLogo;
        private bool _noConfig;
        private bool _mergeConfig;
        private bool _dataContractOnly;

        private bool _enableDataBinding;
        private string _serviceName;
        private List<string> _inputParameters;
        private List<Type> _referencedTypes;
        private List<Assembly> _referencedAssemblies;
        private List<Type> _referencedCollectionTypes;
        private Dictionary<string, Type> _excludedTypes;
        private bool _nostdlib;
        private Dictionary<string, string> _namespaceMappings;
        private TypeResolver _typeResolver;
        private TargetClientVersionMode _targetClientVersion;
        private bool _useSerializerForFaults;
        private bool _wrapped;
        private bool _serviceContractGeneration;
        private bool _syncMethodOnly;

        internal string OutputFileArg { get { return _outputFileArg; } }
        internal string DirectoryArg { get { return _directoryArg; } }
        internal string ConfigFileArg { get { return _configFileArg; } }
        internal bool AsyncMethods { get { return _asyncMethods; } }
        internal bool InternalTypes { get { return _internalTypes; } }
        internal bool SerializableAttribute { get { return _serializableAttribute; } }
        internal SerializerMode Serializer { get { return _serializer; } }
        internal bool ImportXmlTypes { get { return _importXmlTypes; } }
        internal bool TypedMessages { get { return _typedMessages; } }
        internal bool NoLogo { get { return _noLogo; } }
        internal bool NoConfig { get { return _noConfig || _dataContractOnly; } }
        internal bool MergeConfig { get { return _mergeConfig; } }
        internal string ServiceName { get { return _serviceName; } }
        internal bool EnableDataBinding { get { return _enableDataBinding; } }

        internal List<string> InputParameters { get { return _inputParameters; } }
        internal List<Type> ReferencedTypes { get { return _referencedTypes; } }
        internal List<Assembly> ReferencedAssemblies { get { return _referencedAssemblies; } }
        internal List<Type> ReferencedCollectionTypes { get { return _referencedCollectionTypes; } }
        internal bool Nostdlib { get { return _nostdlib; } }
        internal Dictionary<string, string> NamespaceMappings { get { return _namespaceMappings; } }

        internal TypeResolver TypeResolver { get { return _typeResolver; } }

        internal string ModeSettingOption { get { return _modeSettingOption; } }
        internal string ModeSettingValue { get { return _modeSettingValue; } }
        internal TargetClientVersionMode TargetClientVersion { get { return _targetClientVersion; } }
        internal bool UseSerializerForFaults { get { return _useSerializerForFaults; } }
        internal bool Wrapped { get { return _wrapped; } }
        internal bool ServiceContractGeneration { get { return _serviceContractGeneration; } }
        internal bool SyncMethodOnly { get { return _syncMethodOnly; } }

        private Options(ArgumentDictionary arguments)
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
                throw new ToolOptionException(ae.Message);
            }
            return new Options(arguments);
        }

        internal void SetAllowedModes(ToolMode newDefaultMode, ToolMode validModes, string option, string value)
        {
            Tool.Assert(validModes != ToolMode.None, "validModes should never be set to None!");
            Tool.Assert(newDefaultMode != ToolMode.None, "newDefaultMode should never be set to None!");
            Tool.Assert((validModes & newDefaultMode) != ToolMode.None, "newDefaultMode must be a validMode!");
            Tool.Assert(IsSingleBit(newDefaultMode), "newDefaultMode must Always represent a single mode!");

            //update/filter list of valid modes
            _validModes &= validModes;
            if (_validModes == ToolMode.None)
                throw new InvalidToolModeException();


            bool currentDefaultIsValid = (_defaultMode.HasValue && (_defaultMode & _validModes) != ToolMode.None);
            bool newDefaultIsValid = (newDefaultMode & _validModes) != ToolMode.None;
            if (!currentDefaultIsValid)
            {
                if (newDefaultIsValid)
                    _defaultMode = newDefaultMode;
                else
                    _defaultMode = null;
            }

            //If this is true, then this is an explicit mode setting
            if (IsSingleBit(validModes))
            {
                _modeSettingOption = option;
                _modeSettingValue = value;
            }
        }

        internal ToolMode? GetToolMode()
        {
            if (IsSingleBit(_validModes))
                return _validModes;
            return _defaultMode;
        }

        internal string GetCommandLineString(string option, string value)
        {
            return (value == null) ? option : option + ":" + value;
        }

        private static bool IsSingleBit(ToolMode mode)
        {
            //figures out if the mode has a single bit set ( is a power of 2)
            int x = (int)mode;
            return (x != 0) && ((x & (x + ~0)) == 0);
        }

        internal bool IsTypeExcluded(Type type)
        {
            return OptionProcessingHelper.IsTypeSpecified(type, _excludedTypes, Options.Cmd.ExcludeType);
        }

        private class OptionProcessingHelper
        {
            private Options _parent;
            private ArgumentDictionary _arguments;


            private static Type s_typeOfDateTimeOffset = typeof(DateTimeOffset);

            internal OptionProcessingHelper(Options options, ArgumentDictionary arguments)
            {
                _parent = options;
                _arguments = arguments;
            }

            internal void ProcessArguments()
            {
                CheckForBasicOptions();

                if (CheckForHelpOption())
                    return;

                //We're Checking these values first because they are explicit statements about tool mode.
                CheckForTargetOrValidateOptions();

                ProcessDirectoryOption();
                ProcessOutputOption();
                ProcessServiceNameOption();

                ReadInputArguments();

                ParseMiscCodeGenOptions();

                ParseConfigOption();

                ParseNamespaceMappings();

                ParseReferenceAssemblies();
            }

            private void ParseServiceContractOption()
            {
                _parent._serviceContractGeneration = _arguments.ContainsArgument(Options.Cmd.ServiceContract);
                if (_parent._serviceContractGeneration)
                {
                    SetAllowedModesFromOption(ToolMode.ServiceContractGeneration, ToolMode.ServiceContractGeneration, Options.Cmd.ServiceContract, null);
                }
            }

            private bool CheckForHelpOption()
            {
                if (_arguments.ContainsArgument(Options.Cmd.Help) || _arguments.Count == 0)
                {
                    _parent.SetAllowedModes(ToolMode.DisplayHelp, ToolMode.DisplayHelp, Options.Cmd.Help, null);
                    return true;
                }
                return false;
            }

            private void CheckForTargetOrValidateOptions()
            {
                if (_arguments.ContainsArgument(Options.Cmd.Target))
                {
                    ParseTargetOption(_arguments.GetArgument(Options.Cmd.Target));
                }

                if (_arguments.ContainsArgument(Options.Cmd.Validate))
                {
                    try
                    {
                        _parent.SetAllowedModes(ToolMode.Validate, ToolMode.Validate, Options.Cmd.Validate, null);
                    }
                    catch (InvalidToolModeException)
                    {
                        throw new ToolOptionException(SR.Format(SR.ErrValidateInvalidUse, Options.Cmd.Validate, Options.Cmd.Target));
                    }

                    if (!_arguments.ContainsArgument(Options.Cmd.ServiceName))
                    {
                        throw new ToolOptionException(SR.Format(SR.ErrValidateRequiresServiceName, Options.Cmd.ServiceName));
                    }
                }
            }

            private void ParseTargetOption(string targetValue)
            {
                try
                {
                    if (String.Equals(targetValue, Options.Targets.Metadata, StringComparison.OrdinalIgnoreCase))
                    {
                        _parent.SetAllowedModes(ToolMode.MetadataFromAssembly, ToolMode.MetadataFromAssembly | ToolMode.DataContractExport | ToolMode.WSMetadataExchange, Options.Cmd.Target, targetValue);
                    }
                    else if (String.Equals(targetValue, Options.Targets.Code, StringComparison.OrdinalIgnoreCase))
                    {
                        _parent.SetAllowedModes(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.Target, targetValue);
                    }
                    else if (String.Equals(targetValue, Options.Targets.XmlSerializer, StringComparison.OrdinalIgnoreCase))
                    {
                        _parent.SetAllowedModes(ToolMode.XmlSerializerGeneration, ToolMode.XmlSerializerGeneration, Options.Cmd.Target, targetValue);
                    }
                    else
                    {
                        throw new ToolOptionException(SR.Format(SR.ErrInvalidTarget, Options.Cmd.Target, targetValue, Options.Targets.SupportedTargets));
                    }
                    _parent._targetValue = targetValue;
                }
                catch (InvalidToolModeException)
                {
                    Tool.Assert(true, "This should have been the first check and shouldn't ever be called");
                }
            }

            private void CheckForBasicOptions()
            {
                _parent._noLogo = _arguments.ContainsArgument(Options.Cmd.NoLogo);
#if DEBUG
                ToolConsole.SetOptions(_arguments.ContainsArgument(Options.Cmd.Debug));
#endif
            }

            private void ProcessDirectoryOption()
            {
                // Directory
                //---------------------------------------------------------------------------------------------------------
                if (_arguments.ContainsArgument(Options.Cmd.Directory))
                {
                    string directoryArgValue = _arguments.GetArgument(Options.Cmd.Directory);

                    try
                    {
                        ValidateIsDirectoryPathOnly(Options.Cmd.Directory, directoryArgValue);

                        if (!directoryArgValue.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                            directoryArgValue += Path.DirectorySeparatorChar;

                        _parent._directoryArg = Path.GetFullPath(directoryArgValue);
                    }
                    catch (ToolOptionException)
                    {
                        throw;
                    }
#pragma warning suppress 56500 // covered by FxCOP
                    catch (Exception e)
                    {
                        if (Tool.IsFatal(e))
                            throw;

                        throw new ToolArgumentException(SR.Format(SR.ErrInvalidPath, directoryArgValue, Options.Cmd.Directory), e);
                    }
                }
                else
                {
                    _parent._directoryArg = null;
                }
            }

            private static void ValidateIsDirectoryPathOnly(string arg, string value)
            {
                ValidatePath(arg, value);
                FileInfo fileInfo = new FileInfo(value);
                if (fileInfo.Exists)
                    throw new ToolOptionException(SR.Format(SR.ErrDirectoryPointsToAFile, arg, value));
            }

            private static void ValidatePath(string arg, string value)
            {
                int invalidCharacterIndex = value.IndexOfAny(Path.GetInvalidPathChars());

                if (invalidCharacterIndex != -1)
                {
                    string invalidCharacter = value[invalidCharacterIndex].ToString();
                    throw new ToolOptionException(SR.Format(SR.ErrDirectoryContainsInvalidCharacters, arg, value, invalidCharacter));
                }
            }

            private void ProcessOutputOption()
            {
                if (_arguments.ContainsArgument(Options.Cmd.Out))
                {
                    _parent._outputFileArg = _arguments.GetArgument(Options.Cmd.Out);

                    if (_parent._outputFileArg != string.Empty)
                    {
                        SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.XmlSerializerGeneration | ToolMode.ServiceContractGeneration, Options.Cmd.Out, "");

                        ValidatePath(Options.Cmd.Out, _parent._outputFileArg);
                    }
                }
                else
                {
                    _parent._outputFileArg = null;
                }
            }

            private void ProcessServiceNameOption()
            {
                if (_arguments.ContainsArgument(Options.Cmd.ServiceName))
                {
                    _parent._serviceName = _arguments.GetArgument(Options.Cmd.ServiceName);

                    if (_parent._serviceName != string.Empty)
                    {
                        SetAllowedModesFromOption(ToolMode.MetadataFromAssembly, ToolMode.MetadataFromAssembly | ToolMode.Validate, Options.Cmd.ServiceName, "");
                    }
                }
                else
                {
                    _parent._serviceName = null;
                }
            }

            private void ReadInputArguments()
            {
                _parent._inputParameters = new List<string>(_arguments.GetArguments(String.Empty));
            }

            private void ParseMiscCodeGenOptions()
            {
                ParseSerializerOption();

                ParseDCOnly();

                ParseServiceContractOption();

                ParseTargetClientVersionOption();

                _parent._wrapped = _arguments.ContainsArgument(Options.Cmd.Wrapped);
                if (_parent._wrapped)
                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.Wrapped, null);

                _parent._useSerializerForFaults = _arguments.ContainsArgument(Options.Cmd.UseSerializerForFaults);
                if (_parent._useSerializerForFaults)
                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.UseSerializerForFaults, null);

                _parent._importXmlTypes = _arguments.ContainsArgument(Options.Cmd.ImportXmlTypes);
                if (_parent._importXmlTypes)
                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.ImportXmlTypes, null);

                _parent._noConfig = _arguments.ContainsArgument(Options.Cmd.NoConfig);
                if (_parent._noConfig)
                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration, Options.Cmd.NoConfig, null);

                _parent._internalTypes = _arguments.ContainsArgument(Options.Cmd.Internal);
                if (_parent._internalTypes)
                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.Internal, null);

                _parent._serializableAttribute = _arguments.ContainsArgument(Options.Cmd.Serializable);
                if (_parent._serializableAttribute)
                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.Serializable, null);

                _parent._typedMessages = _arguments.ContainsArgument(Options.Cmd.MessageContract);
                if (_parent._typedMessages)
                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.ServiceContractGeneration, Options.Cmd.MessageContract, null);

                _parent._enableDataBinding = _arguments.ContainsArgument(Options.Cmd.EnableDataBinding);
                if (_parent._enableDataBinding)
                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.EnableDataBinding, null);

                _parent._asyncMethods = _arguments.ContainsArgument(Options.Cmd.Async);
                if (_parent._asyncMethods)
                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.ServiceContractGeneration, Options.Cmd.Async, null);

                _parent._syncMethodOnly = _arguments.ContainsArgument(Options.Cmd.SyncOnly);
                if (_parent._syncMethodOnly)
                {
                    if (_parent._asyncMethods)
                    {
                        throw new ToolOptionException(SR.Format(SR.ErrExclusiveOptionsSpecified, Options.Cmd.SyncOnly, Options.Cmd.Async));
                    }
                    else
                    {
                        SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.ServiceContractGeneration, Options.Cmd.SyncOnly, null);
                    }
                }
            }

            private void ParseDCOnly()
            {
                _parent._dataContractOnly = _arguments.ContainsArgument(Options.Cmd.DataContractOnly);

                if (_parent._dataContractOnly)
                    SetAllowedModesFromOption(ToolMode.DataContractImport, ToolMode.DataContractImport | ToolMode.DataContractExport, Options.Cmd.DataContractOnly, null);
            }

            private void ParseSerializerOption()
            {
                if (_arguments.ContainsArgument(Options.Cmd.Serializer))
                {
                    string serializerValue = _arguments.GetArgument(Options.Cmd.Serializer);
                    try
                    {
                        _parent._serializer = (SerializerMode)Enum.Parse(typeof(SerializerMode), serializerValue, true);
                    }
                    catch (ArgumentException)
                    {
                        throw new ToolOptionException(SR.Format(SR.ErrInvalidSerializer, Options.Cmd.Serializer, serializerValue, Options.s_supportedSerializers));
                    }
                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.Serializer, null);
                }
                else
                {
                    _parent._serializer = SerializerMode.Default;
                }
            }

            private void ParseTargetClientVersionOption()
            {
                if (_arguments.ContainsArgument(Options.Cmd.TargetClientVersion))
                {
                    string targetClientVersionValue = _arguments.GetArgument(Options.Cmd.TargetClientVersion);
                    try
                    {
                        _parent._targetClientVersion = (TargetClientVersionMode)Enum.Parse(typeof(TargetClientVersionMode), targetClientVersionValue, true);
                    }
                    catch (ArgumentException)
                    {
                        throw new ToolOptionException(SR.Format(SR.ErrInvalidTargetClientVersion, Options.Cmd.TargetClientVersion, targetClientVersionValue, Options.s_supportedTargetClientVersions));
                    }
                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.ServiceContractGeneration, Options.Cmd.TargetClientVersion, null);
                }
                else
                {
                    _parent._targetClientVersion = TargetClientVersionMode.Version30;
                }
            }

            private void ParseConfigOption()
            {
                // Parse config
                //-----------------------------------------------------------------------------------------------------
                if (_arguments.ContainsArgument(Options.Cmd.Config))
                {
                    if (_parent._noConfig)
                        throw new ToolOptionException(SR.Format(SR.ErrExclusiveOptionsSpecified, Options.Cmd.NoConfig, Options.Cmd.Config));

                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.ProxyGeneration, Options.Cmd.Config, null);

                    _parent._configFileArg = _arguments.GetArgument(Options.Cmd.Config);

                    if (_arguments.ContainsArgument(Options.Cmd.MergeConfig))
                        _parent._mergeConfig = true;
                    else
                        _parent._mergeConfig = false;

                    if (_parent._configFileArg != null)
                        ValidatePath(Options.Cmd.Config, _parent._configFileArg);
                }
                else
                {
                    if (_arguments.ContainsArgument(Options.Cmd.MergeConfig))
                        throw new ToolOptionException(SR.Format(SR.ErrMergeConfigUsedWithoutConfig, Options.Cmd.MergeConfig, Options.Cmd.Config));

                    _parent._configFileArg = null;
                    _parent._mergeConfig = false;
                }
            }

            private void ParseNamespaceMappings()
            {
                IList<string> namespaceMappingsArgs = _arguments.GetArguments(Options.Cmd.Namespace);
                _parent._namespaceMappings = new Dictionary<string, string>(namespaceMappingsArgs.Count);

                foreach (string namespaceMapping in namespaceMappingsArgs)
                {
                    string[] parts = namespaceMapping.Split(',');

                    if (parts == null || parts.Length != 2)
                        throw new ToolOptionException(SR.Format(SR.ErrInvalidNamespaceArgument, Options.Cmd.Namespace, namespaceMapping));

                    string targetNamespace = parts[0].Trim();
                    string clrNamespace = parts[1].Trim();

                    if (_parent._namespaceMappings.ContainsKey(targetNamespace))
                    {
                        string prevClrNamespace = _parent._namespaceMappings[targetNamespace];
                        if (prevClrNamespace != clrNamespace)
                            throw new ToolOptionException(SR.Format(SR.ErrCannotSpecifyMultipleMappingsForNamespace,
                                Options.Cmd.Namespace, targetNamespace, prevClrNamespace, clrNamespace));
                    }
                    else
                    {
                        _parent._namespaceMappings.Add(targetNamespace, clrNamespace);
                    }
                }
            }

            private void ParseReferenceAssemblies()
            {
                IList<string> referencedAssembliesArgs = _arguments.GetArguments(Options.Cmd.Reference);
                IList<string> excludeTypesArgs = _arguments.GetArguments(Options.Cmd.ExcludeType);
                IList<string> referencedCollectionTypesArgs = _arguments.GetArguments(Options.Cmd.CollectionType);
                bool nostdlib = _arguments.ContainsArgument(Options.Cmd.Nostdlib);

                if (excludeTypesArgs != null && excludeTypesArgs.Count > 0)
                    SetAllowedModesFromOption(ToolMode.ProxyGeneration, ToolMode.MetadataFromAssembly | ToolMode.ProxyGeneration | ToolMode.DataContractImport | ToolMode.XmlSerializerGeneration | ToolMode.ServiceContractGeneration, Options.Cmd.ExcludeType, null);

                AddReferencedTypes(referencedAssembliesArgs, excludeTypesArgs, referencedCollectionTypesArgs, nostdlib);
                _parent._typeResolver = CreateTypeResolver(_parent);
            }

            private void SetAllowedModesFromOption(ToolMode newDefaultMode, ToolMode allowedModes, string option, string value)
            {
                try
                {
                    _parent.SetAllowedModes(newDefaultMode, allowedModes, option, value);
                }
                catch (InvalidToolModeException)
                {
                    string optionStr = _parent.GetCommandLineString(option, value);
                    if (_parent._modeSettingOption != null)
                    {
                        if (_parent._modeSettingOption == Options.Cmd.Target)
                        {
                            throw new ToolOptionException(SR.Format(SR.ErrOptionConflictsWithTarget, Options.Cmd.Target, _parent.ModeSettingValue, optionStr));
                        }
                        else
                        {
                            string modeSettingStr = _parent.GetCommandLineString(_parent._modeSettingOption, _parent._modeSettingValue);
                            throw new ToolOptionException(SR.Format(SR.ErrOptionModeConflict, optionStr, modeSettingStr));
                        }
                    }
                    else
                    {
                        throw new ToolOptionException(SR.Format(SR.ErrAmbiguousOptionModeConflict, optionStr));
                    }
                }
            }

            private void AddReferencedTypes(IList<string> referenceArgs, IList<string> excludedTypeArgs, IList<string> collectionTypesArgs, bool nostdlib)
            {
                _parent._referencedTypes = new List<Type>();
                _parent._referencedAssemblies = new List<Assembly>(referenceArgs.Count);
                _parent._referencedCollectionTypes = new List<Type>();
                _parent._nostdlib = nostdlib;
                _parent._excludedTypes = AddSpecifiedTypesToDictionary(excludedTypeArgs, Options.Cmd.ExcludeType);

                //Add the DateTimeOffset type to excluded types if the target client version is 3.0.
                //Ensures that the DateTimeOffset type is not referenced even if mscorlib is referenced from a non-3.0 machine.

                switch (_parent._targetClientVersion)
                {
                    case TargetClientVersionMode.Version35:
                        break;
                    default:
                        if (!_parent.IsTypeExcluded(s_typeOfDateTimeOffset))
                        {
                            _parent._excludedTypes.Add(s_typeOfDateTimeOffset.FullName, s_typeOfDateTimeOffset);
                        }
                        break;
                }


                Dictionary<string, Type> foundCollectionTypes = AddSpecifiedTypesToDictionary(collectionTypesArgs, Options.Cmd.CollectionType);

                LoadReferencedAssemblies(referenceArgs);

                foreach (Assembly assembly in _parent._referencedAssemblies)
                {
                    AddReferencedTypesFromAssembly(assembly, foundCollectionTypes);
                }

                if (!nostdlib)
                {
                    AddMscorlib(foundCollectionTypes);
                    AddServiceModelLib(foundCollectionTypes);
                }
                AddReferencedCollectionTypes(collectionTypesArgs, foundCollectionTypes);
            }

            private void LoadReferencedAssemblies(IList<string> referenceArgs)
            {
                foreach (string path in referenceArgs)
                {
                    Assembly assembly;
                    try
                    {
                        assembly = InputModule.LoadAssembly(path);
                        if (!_parent._referencedAssemblies.Contains(assembly))
                        {
                            _parent._referencedAssemblies.Add(assembly);
                        }
                        else
                        {
                            throw new ToolOptionException(SR.Format(SR.ErrDuplicateReferenceValues, Options.Cmd.Reference, assembly.Location));
                        }
                    }
#pragma warning suppress 56500 // covered by FxCOP
                    catch (Exception e)
                    {
                        if (Tool.IsFatal(e))
                            throw;

                        throw new ToolOptionException(SR.Format(SR.ErrCouldNotLoadReferenceAssemblyAt, path), e);
                    }
                }
            }

            private Dictionary<string, Type> AddSpecifiedTypesToDictionary(IList<string> typeArgs, string cmd)
            {
                Dictionary<string, Type> specifiedTypes = new Dictionary<string, Type>(typeArgs.Count);
                foreach (string typeArg in typeArgs)
                {
                    if (specifiedTypes.ContainsKey(typeArg))
                        throw new ToolOptionException(SR.Format(SR.ErrDuplicateValuePassedToTypeArg, cmd, typeArg));
                    specifiedTypes.Add(typeArg, null);
                }
                return specifiedTypes;
            }

            private void AddReferencedTypesFromAssembly(Assembly assembly, Dictionary<string, Type> foundCollectionTypes)
            {
                foreach (Type type in InputModule.LoadTypes(assembly))
                {
                    if (type.IsPublic || type.IsNestedPublic)
                    {
                        if (!_parent.IsTypeExcluded(type))
                            _parent._referencedTypes.Add(type);
                        else if (IsTypeSpecified(type, foundCollectionTypes, Options.Cmd.CollectionType))
                            _parent._referencedCollectionTypes.Add(type);
                    }
                }
            }

            private void AddMscorlib(Dictionary<string, Type> foundCollectionTypes)
            {
                Assembly mscorlib = typeof(int).Assembly;
                if (!_parent._referencedAssemblies.Contains(mscorlib))
                {
                    AddReferencedTypesFromAssembly(mscorlib, foundCollectionTypes);
                }
            }

            private void AddServiceModelLib(Dictionary<string, Type> foundCollectionTypes)
            {
                Assembly serviceModelLib = typeof(ChannelFactory).Assembly;
                if (!_parent._referencedAssemblies.Contains(serviceModelLib))
                {
                    AddReferencedTypesFromAssembly(serviceModelLib, foundCollectionTypes);
                }
            }

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
                        throw new ToolOptionException(SR.Format(SR.ErrCannotDisambiguateSpecifiedTypes,
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
                        foreach (Assembly assembly in _parent._referencedAssemblies)
                        {
                            foundType = assembly.GetType(collectionType);
                            if (foundType != null)
                                break;
                        }
                        foundType = foundType ?? Type.GetType(collectionType);
                        if (foundType == null)
                            throw new ToolOptionException(SR.Format(SR.ErrCannotLoadSpecifiedType, Options.Cmd.CollectionType,
                                collectionType, Options.Cmd.Reference));
                        else
                            _parent._referencedCollectionTypes.Add(foundType);
                    }
                }
            }

            private static TypeResolver CreateTypeResolver(Options options)
            {
                TypeResolver typeResolver = new TypeResolver(options);
                AppDomain.CurrentDomain.TypeResolve += new ResolveEventHandler(typeResolver.ResolveType);
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(typeResolver.ResolveAssembly);

                return typeResolver;
            }
        }
    }
}
