// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Tools.ServiceModel.SvcUtil.XmlSerializer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Globalization;
    using System.Text;


    internal abstract class OutputModule
    {
        protected const string defaultFilename = "output";
        protected const string configFileExtension = ".config";

        private readonly string _directoryPath;

        protected OutputModule(Options options)
        {
            _directoryPath = PathHelper.TryGetDirectoryPath(options.DirectoryArg);
        }

        protected string BuildFilePath(string filepath, string extension, string option)
        {
            return PathHelper.BuildFilePath(_directoryPath, filepath, extension, option);
        }

        protected static class PathHelper
        {
            internal static string BuildFilePath(string directoryPath, string filepath, string extension, string option)
            {
                Tool.Assert(!string.IsNullOrEmpty(filepath), "filename must have a valid value");
                Tool.Assert(!string.IsNullOrEmpty(extension), "extension must have a valid value");

                string outputFileWithExtension = GetFilepathWithExtension(filepath, extension);

                string combinedPath = Path.Combine(directoryPath, outputFileWithExtension);

                return TryGetFullPath(combinedPath, option);
            }

            internal static string GetFilepathWithExtension(string outputFile, string extension)
            {
                string outputFileWithExtension;
                if (extension != null && (!Path.HasExtension(outputFile) || !Path.GetExtension(outputFile).Equals(extension, StringComparison.OrdinalIgnoreCase)))
                {
                    outputFileWithExtension = outputFile + extension;
                }
                else
                {
                    outputFileWithExtension = outputFile;
                }
                return outputFileWithExtension;
            }

            internal static string TryGetDirectoryPath(string directory)
            {
                if (directory == null || directory.Length == 0)
                    return "." + Path.DirectorySeparatorChar;
                else
                {
                    if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                        return TryGetFullPath(directory + Path.DirectorySeparatorChar, null);
                    else
                        return TryGetFullPath(directory, null);
                }
            }

            private static string TryGetFullPath(string path, string option)
            {
                try
                {
                    return Path.GetFullPath(path);
                }
                catch (PathTooLongException ptle)
                {
                    if (!string.IsNullOrEmpty(option))
                    {
                        throw new ToolArgumentException(SR.Format(SR.ErrPathTooLong, path, Options.Cmd.Directory, option), ptle);
                    }
                    else
                    {
                        throw new ToolArgumentException(SR.Format(SR.ErrPathTooLongDirOnly, path, Options.Cmd.Directory), ptle);
                    }
                }
#pragma warning suppress 56500 // covered by FxCOP
                catch (Exception e)
                {
                    if (Tool.IsFatal(e))
                        throw;

                    throw new ToolArgumentException(SR.Format(SR.ErrInvalidPath, path, option), e);
                }
            }
        }

        internal static void CreateDirectoryIfNeeded(string path)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(path);
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
            }
#pragma warning suppress 56500 // covered by FxCOP
            catch (Exception e)
            {
                if (Tool.IsFatal(e))
                    throw;

                throw new ToolRuntimeException(SR.Format(SR.ErrCannotCreateDirectory, path), e);
            }
        }

        internal static class FilenameHelper
        {
            private const string DataContractXsdBaseNamespace = "http://schemas.datacontract.org/2004/07/";
            private static int s_dataContractXsdBaseNamespaceLength = DataContractXsdBaseNamespace.Length;

            private static readonly List<string> s_existingFileNames = new List<string>();

            internal static string UniquifyFileName(string filename, string extension)
            {
                string fileNameWithExtension = PathHelper.GetFilepathWithExtension(filename, extension);
                if (!UniquifyFileName_NameExists(fileNameWithExtension))
                {
                    s_existingFileNames.Add(fileNameWithExtension);
                    return filename;
                }

                for (uint i = 1; i < uint.MaxValue; i++)
                {
                    string uniqueFileName = filename + i.ToString(NumberFormatInfo.InvariantInfo);
                    string uniqueFileNameWithExtension = PathHelper.GetFilepathWithExtension(uniqueFileName, extension);
                    if (!UniquifyFileName_NameExists(uniqueFileNameWithExtension))
                    {
                        s_existingFileNames.Add(uniqueFileNameWithExtension);
                        return uniqueFileName;
                    }
                }
                throw new ToolRuntimeException(SR.Format(SR.ErrUnableToUniquifyFilename, fileNameWithExtension), null);
            }

            private static bool UniquifyFileName_NameExists(string fileName)
            {
                for (int i = 0; i < s_existingFileNames.Count; i++)
                {
                    if (String.Compare(fileName, s_existingFileNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                        return true;
                }
                return false;
            }

            internal static string FilenameFromUri(string ns)
            {
                StringBuilder fileNameBuilder = new StringBuilder();

                if (string.IsNullOrEmpty(ns))
                    FilenameFromUri_Add(fileNameBuilder, "noNamespace");
                else
                {
                    Uri nsUri = null;
                    if (Uri.TryCreate(ns, UriKind.RelativeOrAbsolute, out nsUri))
                    {
                        if (nsUri.IsAbsoluteUri)
                        {
                            string absoluteUriString = nsUri.AbsoluteUri;
                            if (absoluteUriString.StartsWith(DataContractXsdBaseNamespace, StringComparison.Ordinal))
                            {
                                int length = absoluteUriString.Length - s_dataContractXsdBaseNamespaceLength;
                                if (absoluteUriString.EndsWith("/", StringComparison.Ordinal))
                                    length--;
                                if (length > 0)
                                {
                                    FilenameFromUri_Add(fileNameBuilder,
                                        absoluteUriString.Substring(s_dataContractXsdBaseNamespaceLength, length).Replace('/', '.'));
                                }
                                else
                                {
                                    FilenameFromUri_Add(fileNameBuilder, "schema");
                                }
                            }
                            else
                            {
                                FilenameFromUri_Add(fileNameBuilder, nsUri.Host);
                                string absolutePath = nsUri.AbsolutePath;
                                if (absolutePath.EndsWith("/", StringComparison.Ordinal))
                                    absolutePath = absolutePath.Substring(0, absolutePath.Length - 1);
                                FilenameFromUri_Add(fileNameBuilder, absolutePath.Replace('/', '.'));
                            }
                        }
                        else
                        {
                            FilenameFromUri_Add(fileNameBuilder, nsUri.OriginalString.Replace('/', '.'));
                        }
                    }
                }
                string filename = fileNameBuilder.ToString();

                return filename;
            }

            private static void FilenameFromUri_Add(StringBuilder path, string segment)
            {
                if (segment != null)
                {
                    char[] chars = segment.ToCharArray();
                    for (int i = 0; i < chars.Length; i++)
                    {
                        char c = chars[i];
                        if (Array.IndexOf(Path.GetInvalidFileNameChars(), c) == -1)
                            path.Append(c);
                    }
                }
            }
        }
    }
}
