// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal static class OutputPathHelper
    {
        private const string DataContractXsdBaseNamespace = "http://schemas.datacontract.org/2004/07/";
        private static readonly int DataContractXsdBaseNamespaceLength = DataContractXsdBaseNamespace.Length;

        public static string BuildFilePath(string defaultFileName, string directoryPath, string filePath, string extension, string option)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(filePath), "filePath must have a valid value!");
            if (string.IsNullOrWhiteSpace(filePath))
            {
                filePath = defaultFileName;
            }

            directoryPath = TryGetDirectoryPath(directoryPath);

            string outputFileWithExtension = GetFilePathWithExtension(filePath, extension);
            string combinedPath = Path.Combine(directoryPath, outputFileWithExtension);

            return TryGetFullPath(combinedPath, option);
        }

        private static string GetFilePathWithExtension(string outputFile, string extension)
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

        public static string TryGetDirectoryPath(string directory)
        {

            if (directory == null || directory.Length == 0)
            {
                return Directory.GetCurrentDirectory();
            }
            else
            {
                if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                {
                    return TryGetFullPath(directory + Path.DirectorySeparatorChar, null);
                }
                else
                {
                    return TryGetFullPath(directory, null);
                }
            }
        }

        internal static void CreateDirectoryIfNeeded(string path)
        {
            string directoryPath = Path.GetDirectoryName(path);

            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }
            catch (Exception e)
            {
                if (Utils.IsFatalOrUnexpected(e)) throw;
                throw new ToolRuntimeException(SR.GetString(SR.ErrCannotCreateDirectoryFormat, directoryPath), e);
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
                    throw new ToolArgumentException(SR.GetString(SR.ErrPathTooLongFormat, path, CommandProcessorOptions.Switches.OutputDirectory.Name, option), ptle);
                }
                else
                {
                    throw new ToolArgumentException(SR.GetString(SR.ErrPathTooLongDirOnlyFormat, path, CommandProcessorOptions.Switches.OutputDirectory.Name), ptle);
                }
            }
            catch (Exception e)
            {
                if (Utils.IsFatalOrUnexpected(e)) throw;
                throw new ToolArgumentException(SR.GetString(SR.ErrInvalidPathFormat, path, option), e);
            }
        }

        internal static string FilenameFromUri(string ns)
        {
            StringBuilder fileNameBuilder = new StringBuilder();

            if (string.IsNullOrEmpty(ns))
                FilenameFromUri_Add(fileNameBuilder, "noNamespace");
            else
            {
                if (Uri.TryCreate(ns, UriKind.RelativeOrAbsolute, out Uri nsUri))
                {

                    if (nsUri.IsAbsoluteUri)
                    {

                        string absoluteUriString = nsUri.AbsoluteUri;
                        if (absoluteUriString.StartsWith(DataContractXsdBaseNamespace, StringComparison.Ordinal))
                        {
                            int length = absoluteUriString.Length - DataContractXsdBaseNamespaceLength;
                            if (absoluteUriString.EndsWith("/", StringComparison.Ordinal))
                                length--;
                            if (length > 0)
                            {
                                FilenameFromUri_Add(fileNameBuilder,
                                    absoluteUriString.Substring(DataContractXsdBaseNamespaceLength, length).Replace('/', '.'));
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

        static void FilenameFromUri_Add(StringBuilder path, string segment)
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
