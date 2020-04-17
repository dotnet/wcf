// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeDom.Compiler
{
    using System.Text;
    using System.Diagnostics;
    using System;
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;
    using System.IO;
    using System.Collections;
    using System.Security;
    using System.Reflection;
    using Microsoft.CodeDom;
    using System.Globalization;
    using System.Runtime.Versioning;

    /// <devdoc>
    /// <para>Provides a
    /// base
    /// class for code compilers.</para>
    /// </devdoc>


    public abstract class CodeCompiler : CodeGenerator, ICodeCompiler
    {
        /// <internalonly/>
        CompilerResults ICodeCompiler.CompileAssemblyFromDom(CompilerParameters options, CodeCompileUnit e)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            try
            {
                return FromDom(options, e);
            }
            finally
            {
                options.TempFiles.SafeDelete();
            }
        }

        /// <internalonly/>


        CompilerResults ICodeCompiler.CompileAssemblyFromFile(CompilerParameters options, string fileName)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            try
            {
                return FromFile(options, fileName);
            }
            finally
            {
                options.TempFiles.SafeDelete();
            }
        }

        /// <internalonly/>
        CompilerResults ICodeCompiler.CompileAssemblyFromSource(CompilerParameters options, string source)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            try
            {
                return FromSource(options, source);
            }
            finally
            {
                options.TempFiles.SafeDelete();
            }
        }

        /// <internalonly/>
        CompilerResults ICodeCompiler.CompileAssemblyFromSourceBatch(CompilerParameters options, string[] sources)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            try
            {
                return FromSourceBatch(options, sources);
            }
            finally
            {
                options.TempFiles.SafeDelete();
            }
        }

        /// <internalonly/>


        CompilerResults ICodeCompiler.CompileAssemblyFromFileBatch(CompilerParameters options, string[] fileNames)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            if (fileNames == null)
                throw new ArgumentNullException("fileNames");

            try
            {
                // Try opening the files to make sure they exists.  This will throw an exception
                // if it doesn't
                foreach (string fileName in fileNames)
                {
                    using (Stream str = File.OpenRead(fileName)) { }
                }

                return FromFileBatch(options, fileNames);
            }
            finally
            {
                options.TempFiles.SafeDelete();
            }
        }

        /// <internalonly/>
        CompilerResults ICodeCompiler.CompileAssemblyFromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            try
            {
                return FromDomBatch(options, ea);
            }
            finally
            {
                options.TempFiles.SafeDelete();
            }
        }

        /// <devdoc>
        /// <para>
        /// Gets
        /// or sets the file extension to use for source files.
        /// </para>
        /// </devdoc>
        protected abstract string FileExtension
        {
            get;
        }

        /// <devdoc>
        /// <para>Gets or
        /// sets the name of the compiler executable.</para>
        /// </devdoc>
        protected abstract string CompilerName
        {
            get;
        }




        internal void Compile(CompilerParameters options, string compilerDirectory, string compilerExe, string arguments, ref string outputFile, ref int nativeReturnValue, string trueArgs)
        {
            throw new NotImplementedException();
        }

        /// <devdoc>
        /// <para>
        /// Compiles the specified compile unit and options, and returns the results
        /// from the compilation.
        /// </para>
        /// </devdoc>
        protected virtual CompilerResults FromDom(CompilerParameters options, CodeCompileUnit e)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            CodeCompileUnit[] units = new CodeCompileUnit[1];
            units[0] = e;
            return FromDomBatch(options, units);
        }

        /// <devdoc>
        /// <para>
        /// Compiles the specified file using the specified options, and returns the
        /// results from the compilation.
        /// </para>
        /// </devdoc>


        protected virtual CompilerResults FromFile(CompilerParameters options, string fileName)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            // Try opening the file to make sure it exists.  This will throw an exception
            // if it doesn't
            using (Stream str = File.OpenRead(fileName)) { }

            string[] filenames = new string[1];
            filenames[0] = fileName;
            return FromFileBatch(options, filenames);
        }

        /// <devdoc>
        /// <para>
        /// Compiles the specified source code using the specified options, and
        /// returns the results from the compilation.
        /// </para>
        /// </devdoc>
        protected virtual CompilerResults FromSource(CompilerParameters options, string source)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            string[] sources = new string[1];
            sources[0] = source;

            return FromSourceBatch(options, sources);
        }

        /// <devdoc>
        /// <para>
        /// Compiles the specified compile units and
        /// options, and returns the results from the compilation.
        /// </para>
        /// </devdoc>


        protected virtual CompilerResults FromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            if (ea == null)
                throw new ArgumentNullException("ea");

            string[] filenames = new string[ea.Length];

            CompilerResults results = null;

            for (int i = 0; i < ea.Length; i++)
            {
                if (ea[i] == null)
                    continue;       // the other two batch methods just work if one element is null, so we'll match that. 

                ResolveReferencedAssemblies(options, ea[i]);
                filenames[i] = options.TempFiles.AddExtension(i + FileExtension);
                Stream temp = new FileStream(filenames[i], FileMode.Create, FileAccess.Write, FileShare.Read);
                try
                {
                    using (StreamWriter sw = new StreamWriter(temp, Encoding.UTF8))
                    {
                        ((ICodeGenerator)this).GenerateCodeFromCompileUnit(ea[i], sw, Options);
                        sw.Flush();
                    }
                }
                finally
                {
                    temp.Dispose();
                }
            }

            results = FromFileBatch(options, filenames);
            return results;
        }

        /// <devdoc>
        ///    <para>
        ///       Because CodeCompileUnit and CompilerParameters both have a referenced assemblies 
        ///       property, they must be reconciled. However, because you can compile multiple
        ///       compile units with one set of options, it will simply merge them.
        ///    </para>
        /// </devdoc>
        private void ResolveReferencedAssemblies(CompilerParameters options, CodeCompileUnit e)
        {
            if (e.ReferencedAssemblies.Count > 0)
            {
                foreach (string assemblyName in e.ReferencedAssemblies)
                {
                    if (!options.ReferencedAssemblies.Contains(assemblyName))
                    {
                        options.ReferencedAssemblies.Add(assemblyName);
                    }
                }
            }
        }

        /// <devdoc>
        /// <para>
        /// Compiles the specified files using the specified options, and returns the
        /// results from the compilation.
        /// </para>
        /// </devdoc>
        protected virtual CompilerResults FromFileBatch(CompilerParameters options, string[] fileNames)
        {
            throw new NotImplementedException();
        }

        /// <devdoc>
        /// <para>Processes the specified line from the specified <see cref='Microsoft.CodeDom.Compiler.CompilerResults'/> .</para>
        /// </devdoc>
        protected abstract void ProcessCompilerOutputLine(CompilerResults results, string line);

        /// <devdoc>
        /// <para>
        /// Gets the command arguments from the specified <see cref='Microsoft.CodeDom.Compiler.CompilerParameters'/>.
        /// </para>
        /// </devdoc>
        protected abstract string CmdArgsFromParameters(CompilerParameters options);

        /// <devdoc>
        /// <para>[To be supplied.]</para>
        /// </devdoc>


        protected virtual string GetResponseFileCmdArgs(CompilerParameters options, string cmdArgs)
        {
            string responseFileName = options.TempFiles.AddExtension("cmdline");

            Stream temp = new FileStream(responseFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            try
            {
                using (StreamWriter sw = new StreamWriter(temp, Encoding.UTF8))
                {
                    sw.Write(cmdArgs);
                    sw.Flush();
                }
            }
            finally
            {
                temp.Dispose();
            }

            return "@\"" + responseFileName + "\"";
        }

        /// <devdoc>
        /// <para>
        /// Compiles the specified source code strings using the specified options, and
        /// returns the results from the compilation.
        /// </para>
        /// </devdoc>


        protected virtual CompilerResults FromSourceBatch(CompilerParameters options, string[] sources)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            if (sources == null)
                throw new ArgumentNullException("sources");

            string[] filenames = new string[sources.Length];

            CompilerResults results = null;
            for (int i = 0; i < sources.Length; i++)
            {
                string name = options.TempFiles.AddExtension(i + FileExtension);
                Stream temp = new FileStream(name, FileMode.Create, FileAccess.Write, FileShare.Read);
                try
                {
                    using (StreamWriter sw = new StreamWriter(temp, Encoding.UTF8))
                    {
                        sw.Write(sources[i]);
                        sw.Flush();
                    }
                }
                finally
                {
                    temp.Dispose();
                }
                filenames[i] = name;
            }
            results = FromFileBatch(options, filenames);

            return results;
        }

        /// <devdoc>
        /// <para>Joins the specified string arrays.</para>
        /// </devdoc>
        protected static string JoinStringArray(string[] sa, string separator)
        {
            if (sa == null || sa.Length == 0)
                return String.Empty;

            if (sa.Length == 1)
            {
                return "\"" + sa[0] + "\"";
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < sa.Length - 1; i++)
            {
                sb.Append("\"");
                sb.Append(sa[i]);
                sb.Append("\"");
                sb.Append(separator);
            }
            sb.Append("\"");
            sb.Append(sa[sa.Length - 1]);
            sb.Append("\"");

            return sb.ToString();
        }
    }
}
