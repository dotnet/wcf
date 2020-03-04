// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using Microsoft.Tools.ServiceModel.Svcutil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SvcutilTest
{
    public class FileUtil
    {
        public static void CopyDirectory(string sourceDirectory, string destinationDirectory, bool overwrite = false)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirectory);
            Assert.IsTrue(dir.Exists, "Can't copy nonexistant directory.");

            DirectoryInfo[] dirs = dir.GetDirectories();
            if (overwrite && Directory.Exists(destinationDirectory))
            {
                FileUtil.TryDeleteDirectory(destinationDirectory);
            }

            Directory.CreateDirectory(destinationDirectory);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destinationDirectory, file.Name);
                file.CopyTo(temppath, true);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destinationDirectory, subdir.Name);
                CopyDirectory(subdir.FullName, temppath);
            }
        }

        public static void TryDeleteDirectory(string directoryPath)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    Directory.Delete(directoryPath, true);
                }
            }
            catch
            {
                // Swallow the exception. 
            }
        }
    }
}
