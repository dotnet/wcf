// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeDom.Compiler
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using Microsoft.Win32;
    using System.Security;


    using System.ComponentModel;

    using System.Globalization;
    using System.Runtime.Versioning;

    /// <devdoc>
    ///    <para>Represents a collection of temporary file names that are all based on a
    ///       single base filename located in a temporary directory.</para>
    /// </devdoc>


    // [Serializable],
    public class TempFileCollection : ICollection, IDisposable
    {
        private string _basePath;
        private string _tempDir;
        private bool _keepFiles;
        private Hashtable _files;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public TempFileCollection() : this(null, false)
        {
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public TempFileCollection(string tempDir) : this(tempDir, false)
        {
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public TempFileCollection(string tempDir, bool keepFiles)
        {
            _keepFiles = keepFiles;
            _tempDir = tempDir;
#if !FEATURE_CASE_SENSITIVE_FILESYSTEM            
            _files = new Hashtable(StringComparer.OrdinalIgnoreCase);
#else
            files = new Hashtable();
#endif
        }

        /// <internalonly/>
        /// <devdoc>
        /// <para> To allow it's stuff to be cleaned up</para>
        /// </devdoc>
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // It is safe to call Delete from here even if Dispose is called from Finalizer
            // because the graph of objects is guaranteed to be there and
            // neither Hashtable nor String have a finalizer of their own that could 
            // be called before TempFileCollection Finalizer
            Delete();
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        ~TempFileCollection()
        {
            Dispose(false);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string AddExtension(string fileExtension)
        {
            return AddExtension(fileExtension, _keepFiles);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string AddExtension(string fileExtension, bool keepFile)
        {
            if (fileExtension == null || fileExtension.Length == 0)
                throw new ArgumentException(string.Format(SRCodeDom.InvalidNullEmptyArgument, "fileExtension"), "fileExtension");  // fileExtension not specified
            string fileName = BasePath + "." + fileExtension;
            AddFile(fileName, keepFile);
            return fileName;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public void AddFile(string fileName, bool keepFile)
        {
            if (fileName == null || fileName.Length == 0)
                throw new ArgumentException(string.Format(SRCodeDom.InvalidNullEmptyArgument, "fileName"), "fileName");  // fileName not specified

            if (_files[fileName] != null)
                throw new ArgumentException(string.Format(SRCodeDom.DuplicateFileName, fileName), "fileName");  // duplicate fileName
            _files.Add(fileName, (object)keepFile);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public IEnumerator GetEnumerator()
        {
            return _files.Keys.GetEnumerator();
        }

        /// <internalonly/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _files.Keys.GetEnumerator();
        }

        /// <internalonly/>
        void ICollection.CopyTo(Array array, int start)
        {
            _files.Keys.CopyTo(array, start);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public void CopyTo(string[] fileNames, int start)
        {
            _files.Keys.CopyTo(fileNames, start);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int Count
        {
            get
            {
                return _files.Count;
            }
        }

        /// <internalonly/>
        int ICollection.Count
        {
            get { return _files.Count; }
        }

        /// <internalonly/>
        object ICollection.SyncRoot
        {
            get { return null; }
        }

        /// <internalonly/>
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string TempDir
        {
            get { return _tempDir == null ? string.Empty : _tempDir; }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string BasePath
        {
            get
            {
                EnsureTempNameCreated();
                return _basePath;
            }
        }



        private void EnsureTempNameCreated()
        {
            if (_basePath == null)
            {
                string tempFileName = null;
                FileStream tempFileStream;
                bool uniqueFile = false;
                int retryCount = 5000;
                do
                {
                    try
                    {
                        _basePath = GetTempFileName(TempDir);

                        string full = Path.GetFullPath(_basePath);

                        // make sure the filename is unique. 
                        tempFileName = _basePath + ".tmp";
                        using (tempFileStream = new FileStream(tempFileName, FileMode.CreateNew, FileAccess.Write)) { }
                        uniqueFile = true;
                    }
                    catch (IOException e)
                    {
                        retryCount--;

                        uint HR_ERROR_FILE_EXISTS = unchecked(((uint)0x80070000) | NativeMethods.ERROR_FILE_EXISTS);
                        if (retryCount == 0 || Marshal.GetHRForException(e) != HR_ERROR_FILE_EXISTS)
                            throw;

                        uniqueFile = false;
                    }
                } while (!uniqueFile);
                _files.Add(tempFileName, _keepFiles);
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool KeepFiles
        {
            get { return _keepFiles; }
            set { _keepFiles = value; }
        }

        private bool KeepFile(string fileName)
        {
            object keep = _files[fileName];
            if (keep == null) return false;
            return (bool)keep;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>


        public void Delete()
        {
            if (_files != null && _files.Count > 0)
            {
                string[] fileNames = new string[_files.Count];
                _files.Keys.CopyTo(fileNames, 0);
                foreach (string fileName in fileNames)
                {
                    if (!KeepFile(fileName))
                    {
                        Delete(fileName);
                        _files.Remove(fileName);
                    }
                }
            }
        }

        // This function deletes files after reverting impersonation.
        internal void SafeDelete()
        {
            try
            {
                Delete();
            }
            finally
            {
            }
        }

        private void Delete(string fileName)
        {
            try
            {
                File.Delete(fileName);
            }
            catch
            {
                // Ignore all exceptions
            }
        }



        private static string GetTempFileName(string tempDir)
        {
            string fileName;
            if (String.IsNullOrEmpty(tempDir)) tempDir = Path.GetTempPath();

            string randomFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());

            if (tempDir.EndsWith("\\", StringComparison.Ordinal))
                fileName = tempDir + randomFileName;
            else
                fileName = tempDir + "\\" + randomFileName;

            return fileName;
        }
    }
}
