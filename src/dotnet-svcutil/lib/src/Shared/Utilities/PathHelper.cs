// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal static class SystemInfoExtensions
    {
        // DirectoryInfo.ToString() returns the original value which may be a relative path.
        // Name/FullName compute the value from the current directory if not relative.

        public static string OriginalPath(this FileInfo fileInfo)
        {
            return fileInfo.ToString();
        }

        public static string OriginalPath(this DirectoryInfo directoryInfo)
        {
            return directoryInfo.ToString();
        }
    }

    internal static class PathHelper
    {
        public static bool PathEqual(string path1, string path2)
        {
            return path1 != null && path2 != null && path1.Equals(path2, RuntimeEnvironmentHelper.FileStringComparison);
        }

        public static bool PathHasFolder(string path, IEnumerable<string> folders, string workingDirectory)
        {
            bool contains = PathHelper.GetRelativePath(path, workingDirectory)
                    .Split(new char[] { '/', '\\' })
                    .Any(p => folders.Any(f => p.Equals(f, RuntimeEnvironmentHelper.FileStringComparison)));

            return contains;
        }

        internal static bool IsFile(string input, string basePath, out Uri fileUri)
        {
            fileUri = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(input))
                {
                    input = input.Trim(new char[] { '"' }).Trim();

                    var isUrl = input.StartsWith("http", StringComparison.OrdinalIgnoreCase) ||
                                input.StartsWith("net.tcp", StringComparison.OrdinalIgnoreCase) ||
                                input.StartsWith("net.pipe", StringComparison.OrdinalIgnoreCase);

                    if (!isUrl && (Uri.TryCreate(Path.Combine(basePath, input), UriKind.Absolute, out var uri)) && uri.Scheme == "file")
                    {
                        fileUri = uri;
                    }
                }
            }
            catch
            {
            }

            return fileUri != null;
        }

        internal static bool IsFile(Uri uri, string basePath, out Uri fileUri)
        {
            fileUri = uri;
            return fileUri.IsAbsoluteUri && fileUri.IsFile || IsFile(fileUri.ToString(), basePath, out fileUri);
        }

        public static bool IsUnderDirectory(string fileSpec, DirectoryInfo directory, out string filePath, out string relPath)
        {
            filePath = null;
            if (GetRelativePath(fileSpec, directory, out relPath))
            {
                filePath = Path.Combine(directory.FullName, relPath);
            }
            return filePath != null;
        }

        public static string GetRelativePath(string childPath, string parentPath)
        {
            return GetRelativePath(childPath, new DirectoryInfo(parentPath));
        }

        public static string GetRelativePath(string childPath, DirectoryInfo parentPath)
        {
            if (GetRelativePath(childPath, parentPath, out var relativePath))
            {
                return relativePath;
            }
            return childPath;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        public static bool GetRelativePath(string childPath, DirectoryInfo parentPath, out string relativePath)
        {
            // Path.GetRelativePath is not available in NET Core 1.x
            if (Path.IsPathRooted(childPath))
            {
                // if the path is a file that contains wildcards Path.GetFullPath will fail, let's use the path directory instead.
                var childFileName = Path.GetFileName(childPath);
                var childDirectory = childFileName.Contains("*") ? Path.GetDirectoryName(childPath) : childPath;
                var parentSegments = Path.GetFullPath(parentPath.FullName).Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                var childSegments = Path.GetFullPath(childDirectory).Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                relativePath = string.Empty;

                if (!parentSegments[0].Equals(childSegments[0], RuntimeEnvironmentHelper.FileStringComparison))
                {
                    // at least both paths must be in the same drive.
                    return false;
                }

                var builder = new StringBuilder();
                int idx = 0;

                while (idx < parentSegments.Length)
                {
                    if (idx >= childSegments.Length || !childSegments[idx].Equals(parentSegments[idx], RuntimeEnvironmentHelper.FileStringComparison))
                    {
                        for (int jdx = idx; jdx < parentSegments.Length; jdx++)
                        {
                            builder.Append($"..{Path.AltDirectorySeparatorChar}");
                        }
                        break;
                    }
                    idx++;
                }

                while (idx < childSegments.Length)
                {
                    builder.Append($"{childSegments[idx]}{Path.AltDirectorySeparatorChar}");
                    idx++;
                }

                if (childPath != childDirectory)
                {
                    builder.Append(childFileName);
                }

                relativePath = builder.ToString().Trim(Path.AltDirectorySeparatorChar);
            }
            else
            {
                relativePath = childPath;
            }
            return true;
        }

        public static string GetFolderName(string path)
        {
            var pathParts = GetPathParts(path);
            return pathParts.Length > 0 ? pathParts[pathParts.Length - 1] : string.Empty;
        }

        public static string[] GetPathParts(string path)
        {
            return path.Split(new char[] { '/', '\\', Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static DirectoryInfo CreateUniqueDirectoryName(string directoryName, string parentDirInfo)
        {
            return CreateUniqueDirectoryName(directoryName, new DirectoryInfo(parentDirInfo));
        }

        public static DirectoryInfo CreateUniqueDirectoryName(string directoryName, DirectoryInfo parentDirInfo)
        {
            var directory = new DirectoryInfo(Path.Combine(parentDirInfo.FullName, directoryName));
            if (directory.Exists)
            {
                int nextIndex = 1;
                foreach (var childDir in parentDirInfo.GetDirectories())
                {
                    if (childDir.Name.StartsWith(directoryName, RuntimeEnvironmentHelper.FileStringComparison))
                    {
                        string str = childDir.Name.Substring(directoryName.Length);

                        if (!string.IsNullOrEmpty(str) && int.TryParse(str, out var index))
                        {
                            if (index + 1 > nextIndex)
                            {
                                nextIndex = index + 1;
                            }
                        }
                    };
                }

                directory = new DirectoryInfo(Path.Combine(parentDirInfo.FullName, directoryName + nextIndex));
            }
            return directory;
        }

        public static async Task<string> TryCopyFileIfFoundAsync(string fileName, string workingDirectory, string destinationDir, ILogger logger, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var filePath = await TryFindFileAsync(fileName, workingDirectory, logger, cancellationToken).ConfigureAwait(false);

            if (File.Exists(filePath))
            {
                using (var safeLogger = await SafeLogger.WriteStartOperationAsync(logger, $"Copying {fileName} to {destinationDir} ...").ConfigureAwait(false))
                {
                    try
                    {
                        var dstFilePath = Path.Combine(destinationDir, fileName);
                        File.Copy(filePath, dstFilePath, overwrite: true);
                        return dstFilePath;
                    }
                    catch (Exception ex)
                    {
                        await safeLogger.WriteErrorAsync(ex.Message, logToUI: false).ConfigureAwait(false);
                    }
                }
            }
            return null;
        }

        public static async Task<string> TryFindFileAsync(string fileName, string workingDir, ILogger logger, CancellationToken cancellationToken)
        {
            return await TryFindItemAsync(Directory.EnumerateFiles, fileName, workingDir, logger, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<string> TryFindFolderAsync(string folderName, string workingDir, ILogger logger, CancellationToken cancellationToken)
        {
            return await TryFindItemAsync(Directory.EnumerateDirectories, folderName, workingDir, logger, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<string> TryFindItemAsync(Func<string, IEnumerable<string>> EnumerateItems, string itemName, string workingDir, ILogger logger, CancellationToken cancellationToken)
        {
            // Look up the file in the folder structure starting from the specified working directory.
            cancellationToken.ThrowIfCancellationRequested();

            using (var safeLogger = await SafeLogger.WriteStartOperationAsync(logger, $"Looking for '{itemName}' up from '{workingDir}' dir ...").ConfigureAwait(false))
            {
                var fullPath = string.Empty;

                try
                {
                    if (!string.IsNullOrEmpty(itemName) && !string.IsNullOrEmpty(workingDir) && Directory.Exists(workingDir))
                    {
                        while (string.IsNullOrEmpty(fullPath))
                        {
                            fullPath = EnumerateItems(workingDir).Where(item => Path.GetFileName(item).Equals(itemName, RuntimeEnvironmentHelper.FileStringComparison)).FirstOrDefault();
                            if (!string.IsNullOrEmpty(fullPath))
                            {
                                break;
                            }

                            workingDir = Path.GetDirectoryName(workingDir);
                            if (string.IsNullOrEmpty(workingDir))
                            {
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await safeLogger.WriteMessageAsync($"Error: {ex.Message}", logToUI: false).ConfigureAwait(false);
                }

                await safeLogger.WriteMessageAsync($"Item found: {!string.IsNullOrEmpty(fullPath)}", logToUI: false).ConfigureAwait(false);

                return fullPath;
            }
        }
    }
}
