// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeDom.Compiler
{
    using System;
    using Microsoft.CodeDom;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Security;
    using System.Runtime.Versioning;
    using System.IO;


    /// <devdoc>
    ///    <para>
    ///       Represents the results
    ///       of compilation from the compiler.
    ///    </para>
    /// </devdoc>
    // [Serializable()]
    public class CompilerResults
    {
        private CompilerErrorCollection _errors = new CompilerErrorCollection();
        private StringCollection _output = new StringCollection();
        private Assembly _compiledAssembly;
        private string _pathToAssembly;
        private int _nativeCompilerReturnValue;
        private TempFileCollection _tempFiles;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.Compiler.CompilerResults'/>
        ///       that uses the specified
        ///       temporary files.
        ///    </para>
        /// </devdoc>
        public CompilerResults(TempFileCollection tempFiles)
        {
            _tempFiles = tempFiles;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the temporary files to use.
        ///    </para>
        /// </devdoc>
        public TempFileCollection TempFiles
        {
            get
            {
                return _tempFiles;
            }

            set
            {
                _tempFiles = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       The compiled assembly.
        ///    </para>
        /// </devdoc>
        public Assembly CompiledAssembly
        {
            get
            {
                if (_compiledAssembly == null && _pathToAssembly != null)
                {
                    AssemblyName assemName = new AssemblyName();
                    // assemName.CodeBase = pathToAssembly;
                    _compiledAssembly = Assembly.Load(assemName);
                }
                return _compiledAssembly;
            }


            set
            {
                _compiledAssembly = value;
            }
        }
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the collection of compiler errors.
        ///    </para>
        /// </devdoc>
        public CompilerErrorCollection Errors
        {
            get
            {
                return _errors;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the compiler output messages.
        ///    </para>
        /// </devdoc>
        public StringCollection Output
        {
            get
            {
                return _output;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the path to the assembly.
        ///    </para>
        /// </devdoc>
        public string PathToAssembly
        {
            get
            {
                return _pathToAssembly;
            }



            set
            {
                _pathToAssembly = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the compiler's return value.
        ///    </para>
        /// </devdoc>
        public int NativeCompilerReturnValue
        {
            get
            {
                return _nativeCompilerReturnValue;
            }


            set
            {
                _nativeCompilerReturnValue = value;
            }
        }
    }
}
