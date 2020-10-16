// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
    internal class MetadataFileNameManager
    {
        private const string DataContractXsdBaseNamespace = "http://schemas.datacontract.org/2004/07/";

        private List<string> _files = new List<string>();
        public IEnumerable<string> Files
        {
            get { return _files; }
        }

        public string AddFileName(string basePath, string fileName, string extension)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(basePath))
            {
                basePath = Directory.GetCurrentDirectory();
            }

            var filePath = GetFilePath(basePath, fileName, extension);

            for (uint i = 1; i < uint.MaxValue; i++)
            {
                if (!_files.Any((f) => StringComparer.OrdinalIgnoreCase.Compare(f, filePath) == 0))
                {
                    _files.Add(filePath);
                    return filePath;
                }

                string uniqueFileName = fileName + i.ToString(NumberFormatInfo.InvariantInfo);
                string uniqueFileNameWithExtension = GetFileNameWithExtension(uniqueFileName, extension);
                filePath = Path.IsPathRooted(uniqueFileNameWithExtension) ? uniqueFileNameWithExtension : Path.Combine(basePath, uniqueFileNameWithExtension);
            }

            throw new InvalidOperationException(string.Format(MetadataResources.Culture, MetadataResources.ErrUnableToCreateUniqueFileNameFormat, GetFileNameWithExtension(fileName, extension)));
        }

        public string AddFromNamespace(string basePath, string schemaNamespace, string extension)
        {
            var filename = FileNameFromNS(schemaNamespace);
            return AddFileName(basePath, filename, extension);
        }

        public static string GetFilePath(string basePath, string fileName, string extension)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(basePath))
            {
                basePath = Directory.GetCurrentDirectory();
            }

            var fileNameWithExtension = GetFileNameWithExtension(fileName, extension);
            var filePath = Path.IsPathRooted(fileNameWithExtension) ? fileNameWithExtension : Path.Combine(basePath, fileNameWithExtension);
            return filePath;
        }

        public static string GetFilePathFromNamespace(string basePath, string schemaNs, string extension)
        {
            var fileName = FileNameFromNS(schemaNs);
            var filePath = GetFilePath(basePath, fileName, extension);
            return filePath;
        }

        private static string FileNameFromNS(string ns)
        {
            StringBuilder fileNameBuilder = new StringBuilder();

            if (string.IsNullOrWhiteSpace(ns))
            {
                FileNameFromNS_Add(fileNameBuilder, "noNamespace");
            }
            else
            {
                if (Uri.TryCreate(ns, UriKind.RelativeOrAbsolute, out Uri nsUri))
                {
                    if (nsUri.IsAbsoluteUri)
                    {
                        string absoluteUriString = nsUri.AbsoluteUri;
                        if (absoluteUriString.StartsWith(DataContractXsdBaseNamespace, StringComparison.Ordinal))
                        {
                            int length = absoluteUriString.Length - DataContractXsdBaseNamespace.Length;
                            if (absoluteUriString.EndsWith("/", StringComparison.Ordinal))
                            {
                                length--;
                            }
                            if (length > 0)
                            {
                                FileNameFromNS_Add(fileNameBuilder, absoluteUriString.Substring(DataContractXsdBaseNamespace.Length, length).Replace('/', '.'));
                            }
                            else
                            {
                                FileNameFromNS_Add(fileNameBuilder, "schema");
                            }
                        }
                        else
                        {
                            FileNameFromNS_Add(fileNameBuilder, nsUri.Host);
                            string absolutePath = nsUri.AbsolutePath;
                            if (absolutePath.EndsWith("/", StringComparison.Ordinal))
                            {
                                absolutePath = absolutePath.Substring(0, absolutePath.Length - 1);
                            }
                            FileNameFromNS_Add(fileNameBuilder, absolutePath.Replace('/', '.'));
                        }
                    }
                    else
                    {
                        FileNameFromNS_Add(fileNameBuilder, nsUri.OriginalString.Replace('/', '.'));
                    }
                }
                else
                {
                    FileNameFromNS_Add(fileNameBuilder, "noNamespace");
                }
            }

            return fileNameBuilder.ToString();
        }

        private static void FileNameFromNS_Add(StringBuilder path, string segment)
        {
            if (segment != null)
            {
                char[] chars = segment.ToCharArray();
                for (int i = 0; i < chars.Length; i++)
                {
                    char c = chars[i];
                    if (Array.IndexOf(Path.GetInvalidFileNameChars(), c) == -1)
                    {
                        path.Append(c);
                    }
                }
            }
        }

        private static string GetFileNameWithExtension(string outputFile, string extension)
        {
            string outputFileWithExtension;
            if (!string.IsNullOrEmpty(extension) && (!Path.HasExtension(outputFile) || !Path.GetExtension(outputFile).Equals(extension, StringComparison.OrdinalIgnoreCase)))
            {
                outputFileWithExtension = outputFile + extension;
            }
            else
            {
                outputFileWithExtension = outputFile;
            }
            return outputFileWithExtension;
        }

        // Case insensitive search for Linux, macOS.
        public static bool FileExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                return true;
            }

            var dirPath = Path.GetDirectoryName(filePath);
            if (Directory.Exists(dirPath))
            {
                var files = Directory.GetFiles(dirPath);
                return files.Any((f) => UriEqual(f, filePath));
            }

            return false;
        }

        public static bool TryCreateUri(string filePath1, out Uri fileUri)
        {
            if (Uri.TryCreate(filePath1, UriKind.Absolute, out fileUri) || Uri.TryCreate(filePath1, UriKind.Relative, out fileUri))
            {
                return fileUri.IsFile;
            }
            return false;
        }


        public static bool UriEqual(string filePath1, string filePath2)
        {
            Uri uri1, uri2;
            return Uri.TryCreate(filePath1, UriKind.Absolute, out uri1) && Uri.TryCreate(filePath2, UriKind.Absolute, out uri2) && StringComparer.OrdinalIgnoreCase.Compare(uri1.AbsoluteUri, uri2.AbsoluteUri) == 0 ||
                   Uri.TryCreate(filePath1, UriKind.Relative, out uri1) && Uri.TryCreate(filePath2, UriKind.Relative, out uri2) && StringComparer.OrdinalIgnoreCase.Compare(uri1.ToString(), uri2.ToString()) == 0;
        }

        public static string GetComposedUri(string baseUrl, string relUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                return relUrl;
            }
            if (string.IsNullOrEmpty(relUrl))
            {
                relUrl = string.Empty;
            }
            var baseUri = new Uri(baseUrl, UriKind.RelativeOrAbsolute);
            if (!baseUri.IsAbsoluteUri)
            {
                return relUrl;
            }
            return (new Uri(baseUri, relUrl)).GetComponents(UriComponents.AbsoluteUri, UriFormat.SafeUnescaped);
        }

        //Finds files that match a path (includes wildcards)
        public static FileInfo[] ResolveFiles(string path)
        {
            // Figure out the directory part
            string dirPath = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(dirPath))
            {
                dirPath = Directory.GetCurrentDirectory();
            }

            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);

            // Verify that the directory exists
            if (!dirInfo.Exists)
            {
                throw new DirectoryNotFoundException(string.Format(CultureInfo.CurrentCulture, MetadataResources.ErrDirectoryNotFoundFormat, dirInfo.FullName));
            }

            // Verify that the File exists
            string filename = Path.GetFileName(path);
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, MetadataResources.ErrDirectoryInsteadOfFileFormat, path));
            }

            FileInfo[] fileInfoList = dirInfo.GetFiles(filename);
            if (fileInfoList.Length == 0)
            {
                throw new FileNotFoundException(string.Format(CultureInfo.CurrentCulture, MetadataResources.ErrNoFilesFoundFormat, path));
            }

            return fileInfoList;
        }

        public static bool TryResolveFiles(string path, out FileInfo[] fileInfoList)
        {
            fileInfoList = null;
            try
            {
                fileInfoList = ResolveFiles(path);
            }
            catch
            {
            }
            return fileInfoList != null;
        }
    }
}
