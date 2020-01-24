// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.SvcUtil.XmlSerializer
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;
    using System.Net;
    using System.Threading;
    using System.Runtime.InteropServices;

    internal partial class InputModule
    {
        private static readonly XmlReaderSettings s_xmlReaderSettings;
        private List<Assembly> _assemblies;
        private const long MaxReceivedMexMessageSize = (long)(64 * 1024 * 1024); //64MB
        private const int MaxNameTableCharCount = 1024 * 1024; //1MB

        internal List<Assembly> Assemblies
        {
            get
            {
                if (_assemblies == null)
                    _assemblies = new List<Assembly>();
                return _assemblies;
            }
        }

        static InputModule()
        {
            s_xmlReaderSettings = new XmlReaderSettings();
            s_xmlReaderSettings.XmlResolver = null;
        }

        private InputModule()
        {
        }

        internal static InputModule LoadInputs(Options options)
        {
            if (options.InputParameters.Count == 0)
                throw new ToolArgumentException(SR.Format(SR.ErrNoValidInputFilesSpecified));

            InputModule inputs = new InputModule();
            InputModuleLoader loader = new InputModuleLoader(inputs, options);
            loader.LoadInputs();

            return inputs;
        }

        private partial class InputModuleLoader
        {
            private readonly InputModule _newInputModule;
            private readonly Options _options;

            internal InputModuleLoader(InputModule newInputModule, Options options)
            {
                _options = options;
                _newInputModule = newInputModule;
            }

            internal void LoadInputs()
            {
                //Load Input Parameters
                //-------------------------------------------------------------------------------------------
                foreach (string path in _options.InputParameters)
                {
                    LoadInputItem(path);
                }
            }

            private void LoadInputItem(string path)
            {
                Uri serviceUri;
                FileInfo[] inputFiles;

                if (Uri.TryCreate(path, UriKind.Absolute, out serviceUri) && !serviceUri.IsFile)
                {
                    // LoadInputItem_AsUri(serviceUri);
                }
                else if (TryFindFiles(path, out inputFiles))
                {
                    try
                    {
                        LoadInputItem_AsFilePath(inputFiles, path);
                    }
#pragma warning suppress 56500 // covered by FxCOP
                    catch (Exception e)
                    {
                        if (Tool.IsFatal(e))
                            throw;

                        throw new ToolInputException(SR.Format(SR.ErrUnableToLoadFile, path), e);
                    }
                }
                else
                {
                    throw new ToolInputException(SR.Format(SR.ErrInvalidInputPath, path));
                }
            }

            private void LoadInputItem_AsFilePath(FileInfo[] inputFiles, string path)
            {
                ToolMode? mode = _options.GetToolMode();
                foreach (FileInfo fileInfo in inputFiles)
                {
                    if (!LoadFile(fileInfo, mode))
                    {
                        throw new ToolInputException(SR.Format(SR.ErrInputFileNotAssemblyOrMetadata, fileInfo.FullName, path));
                    }
                }
            }

            private bool LoadFile(FileInfo fileInfo, ToolMode? mode)
            {
                if (mode == ToolMode.XmlSerializerGeneration)
                {
                    return LoadFileAsAssembly(fileInfo);
                }

                throw new PlatformNotSupportedException();
            }

            private bool LoadFileAsAssembly(FileInfo fileInfo)
            {
                Assembly assembly;
                try
                {
                    assembly = LoadAssembly(fileInfo.FullName);
                }
#pragma warning suppress 56500 // covered by FxCOP
                catch (ToolInputException tie)
                {
                    Exception ie = tie.InnerException;
                    if (ie is BadImageFormatException
                        && Marshal.GetHRForException(ie) == ToolInputException.COR_E_ASSEMBLYEXPECTED)
                        return false;

                    throw;
                }

                _newInputModule.Assemblies.Add(assembly);

                return true;
            }

            //Finds files that match a path (includes wildcards)
            private static bool TryFindFiles(string path, out FileInfo[] fileInfos)
            {
                char[] invalidPathChars = Path.GetInvalidPathChars();
                if (path.IndexOfAny(invalidPathChars) != -1)
                {
                    fileInfos = null;
                    return false;
                }

                // Figure out the directory part
                string dirPath = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(dirPath))
                    dirPath = "." + Path.DirectorySeparatorChar;
                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);

                // Verify that the directory exists
                if (!dirInfo.Exists)
                    throw new ToolInputException(SR.Format(SR.ErrDirectoryNotFound, dirInfo.FullName));

                // Verify that the File exists
                string filename = Path.GetFileName(path);
                if (string.IsNullOrEmpty(filename))
                    throw new ToolInputException(SR.Format(SR.ErrDirectoryInsteadOfFile, path));

                FileInfo[] files = dirInfo.GetFiles(filename);
                if (files.Length == 0)
                    throw new ToolInputException(SR.Format(SR.ErrNoFilesFound, path));

                fileInfos = files;
                return true;
            }

            private ToolArgumentException CreateInputException(string inputSrc)
            {
                if (_options.ModeSettingOption != null)
                {
                    string modeSettingStr = _options.GetCommandLineString(_options.ModeSettingOption, _options.ModeSettingValue);
                    if (_options.ModeSettingOption == Options.Cmd.Target)
                    {
                        return new ToolInputException(SR.Format(SR.ErrInputConflictsWithTarget, Options.Cmd.Target, _options.ModeSettingValue, inputSrc));
                    }
                    else
                    {
                        return new ToolInputException(SR.Format(SR.ErrInputConflictsWithOption, modeSettingStr, inputSrc));
                    }
                }
                else
                {
                    if (_options.ModeSettingValue != null)
                    {
                        return new ToolInputException(SR.Format(SR.ErrConflictingInputs, _options.ModeSettingValue, inputSrc));
                    }
                    else
                    {
                        return new ToolInputException(SR.Format(SR.ErrInputConflictsWithMode, inputSrc));
                    }
                }
            }
        }


        static public Assembly LoadAssembly(string path)
        {
            try
            {
                return Assembly.LoadFrom(path);
            }
#pragma warning suppress 56500 // covered by FxCOP
            catch (Exception e)
            {
                if (Tool.IsFatal(e))
                    throw;

                throw new ToolInputException(SR.Format(SR.ErrAssemblyLoadFailed, path), e);
            }
        }

        static public Type[] LoadTypes(Assembly assembly)
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException rtle)
            {
                ToolConsole.WriteWarning(SR.Format(SR.WrnCouldNotLoadTypesFromReferenceAssemblyAt, assembly.Location));
                foreach (Exception e in rtle.LoaderExceptions)
                {
                    ToolConsole.WriteLine("  " + e.Message, 2);
                }

                types = Array.FindAll<Type>(rtle.Types, delegate (Type t) { return t != null; });
                if (types.Length == 0)
                {
                    throw new ToolInputException(SR.Format(SR.ErrCouldNotLoadTypesFromAssemblyAt, assembly.Location));
                }
            }
            return types;
        }
    }
}
