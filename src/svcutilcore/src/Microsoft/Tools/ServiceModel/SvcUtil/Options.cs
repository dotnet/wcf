// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.SvcUtil.XmlSerializer
{
    using System;
    using System.Configuration;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;


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

        private bool _noLogo;
        private bool _quiet;
        private List<string> _inputParameters;
        private List<Type> _referencedTypes;
        private List<Assembly> _referencedAssemblies;
        private Dictionary<string, Type> _excludedTypes;
        private bool _nostdlib;
        private Dictionary<string, string> _namespaceMappings;

        internal string OutputFileArg { get { return _outputFileArg; } }
        internal string DirectoryArg { get { return _directoryArg; } }
        internal bool NoLogo { get { return _noLogo; } }
        internal bool Quiet { get { return _quiet; } }

        internal List<string> InputParameters { get { return _inputParameters; } }
        internal List<Type> ReferencedTypes { get { return _referencedTypes; } }
        internal List<Assembly> ReferencedAssemblies { get { return _referencedAssemblies; } }
        internal bool Nostdlib { get { return _nostdlib; } }
        internal Dictionary<string, string> NamespaceMappings { get { return _namespaceMappings; } }
        private TypeResolver _typeResolver;

        internal string ModeSettingOption { get { return _modeSettingOption; } }
        internal string ModeSettingValue { get { return _modeSettingValue; } }

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

                if (CheckForHelpOption() || !CheckForQuietOption())
                    return;

                LoadSMReferenceAssembly();
                ProcessDirectoryOption();
                ProcessOutputOption();
                ReadInputArguments();
                ParseNamespaceMappings();
                ParseReferenceAssemblies();
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

            private bool CheckForQuietOption()
            {
                _parent._quiet = _arguments.ContainsArgument(Options.Cmd.Quiet);
                return _parent._quiet;
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
                        SetAllowedModesFromOption(ToolMode.XmlSerializerGeneration, ToolMode.XmlSerializerGeneration, Options.Cmd.Out, "");

                        ValidatePath(Options.Cmd.Out, _parent._outputFileArg);
                    }
                }
                else
                {
                    _parent._outputFileArg = null;
                }
            }

            private void ReadInputArguments()
            {
                _parent._inputParameters = new List<string>(_arguments.GetArguments(String.Empty));
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
                    SetAllowedModesFromOption(ToolMode.XmlSerializerGeneration, ToolMode.XmlSerializerGeneration, Options.Cmd.ExcludeType, null);

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
                _parent._nostdlib = nostdlib;
                _parent._excludedTypes = AddSpecifiedTypesToDictionary(excludedTypeArgs, Options.Cmd.ExcludeType);


                Dictionary<string, Type> foundCollectionTypes = AddSpecifiedTypesToDictionary(collectionTypesArgs, Options.Cmd.CollectionType);

                LoadReferencedAssemblies(referenceArgs);

                foreach (Assembly assembly in _parent._referencedAssemblies)
                {
                    AddReferencedTypesFromAssembly(assembly, foundCollectionTypes);
                }

                if (!nostdlib)
                {
                    AddMscorlib(foundCollectionTypes);
                }
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

                        ToolConsole.WriteWarning(SR.Format(SR.ErrCouldNotLoadReferenceAssemblyAt, path));
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

            private void LoadSMReferenceAssembly()
            {
                string smReferenceArg = _arguments.GetArgument(Options.Cmd.SMReference);
                IList<string> referencedAssembliesArgs = smReferenceArg.Split(';').ToList();
                if (referencedAssembliesArgs != null && referencedAssembliesArgs.Count > 0)
                {
                    string smassembly = "";
                    string smpassembly = "";
                    foreach (string path in referencedAssembliesArgs)
                    {
                        var file = new FileInfo(path);

                        if (file.Name.Equals("System.ServiceModel.Primitives.dll", StringComparison.OrdinalIgnoreCase))
                        {
                            smassembly = path;
                        }

                        if (file.Name.Equals("System.Private.ServiceModel.dll", StringComparison.OrdinalIgnoreCase))
                        {
                            smpassembly = path;
                        }
                    }
                    if (string.IsNullOrEmpty(smassembly))
                    {
                        ToolConsole.WriteError("Missing System.ServiceModel.Primitives");
                        throw new ArgumentException("Invalid smreference value");
                    }

                    try
                    {
                        ToolConsole.WriteLine("Load Assembly From " + smassembly);
                        Tool.SMAssembly = InputModule.LoadAssembly(smassembly);
                        ToolConsole.WriteLine($"Successfully Load {smassembly}");
                    }
                    catch (Exception e)
                    {
                        ToolConsole.WriteError(string.Format("Fail to load the assembly {0} with the error {1}", smassembly, e.Message));
                        throw;
                    }

                    if (!string.IsNullOrEmpty(smpassembly))
                    {
                        try
                        {
                            ToolConsole.WriteLine("Load Assembly From " + smpassembly);
                            Tool.SMAssembly = InputModule.LoadAssembly(smpassembly);
                            ToolConsole.WriteLine($"Successfully Load {smpassembly}");
                        }
                        catch (Exception e)
                        {
                            ToolConsole.WriteError(string.Format("Fail to load the assembly {0} with the error {1}", smpassembly, e.Message));
                            throw;
                        }
                    }
                }
                else
                {
                    ToolConsole.WriteError("Need to pass the System.ServiceModel.Primitives.dll path through the 'smreference' parameter.");
                    throw new ArgumentException("Need to pass the System.ServiceModel.Primitives.dll path through the 'smreference' parameter.");
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

            static TypeResolver CreateTypeResolver(Options options)
            {
                TypeResolver typeResolver = new TypeResolver(options);
                AppDomain.CurrentDomain.TypeResolve += new ResolveEventHandler(typeResolver.ResolveType);
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(typeResolver.ResolveAssembly);

                return typeResolver;
            }
        }
    }
}
