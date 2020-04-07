// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.CodeDom.Compiler {
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
    public class CompilerResults {
        private CompilerErrorCollection errors = new CompilerErrorCollection();
        private StringCollection output = new StringCollection();
        private Assembly compiledAssembly;
        private string pathToAssembly;
        private int nativeCompilerReturnValue;
        private TempFileCollection tempFiles;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='Microsoft.CodeDom.Compiler.CompilerResults'/>
        ///       that uses the specified
        ///       temporary files.
        ///    </para>
        /// </devdoc>
        public CompilerResults(TempFileCollection tempFiles) {
            this.tempFiles = tempFiles;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the temporary files to use.
        ///    </para>
        /// </devdoc>
        public TempFileCollection TempFiles {
            get {
                return tempFiles;
            }

            set {
                tempFiles = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       The compiled assembly.
        ///    </para>
        /// </devdoc>
        public Assembly CompiledAssembly {
            get {
                if (compiledAssembly == null && pathToAssembly != null) {
                    AssemblyName assemName = new AssemblyName();
                    // assemName.CodeBase = pathToAssembly;
                    compiledAssembly = Assembly.Load(assemName);
                }
                return compiledAssembly;
            }

            
            set {
                compiledAssembly = value;
            }
        }
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the collection of compiler errors.
        ///    </para>
        /// </devdoc>
        public CompilerErrorCollection Errors {
            get {
                return errors;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the compiler output messages.
        ///    </para>
        /// </devdoc>
        public StringCollection Output {
            
            get {
                return output;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the path to the assembly.
        ///    </para>
        /// </devdoc>
        public string PathToAssembly {
            
            
            get {
                return pathToAssembly;
            }

            
            
            set {
                pathToAssembly = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the compiler's return value.
        ///    </para>
        /// </devdoc>
        public int NativeCompilerReturnValue {
            get {
                return nativeCompilerReturnValue;
            }

            
            set {
                nativeCompilerReturnValue = value;
            }
        }
    }
}
