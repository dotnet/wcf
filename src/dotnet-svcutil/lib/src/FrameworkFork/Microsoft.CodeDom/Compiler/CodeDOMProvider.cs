// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeDom.Compiler
{
    using System;
    using Microsoft.CodeDom;
    using System.ComponentModel;
    using System.IO;
    using System.Collections;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public abstract class CodeDomProvider : IDisposable
    {
        [ComVisible(false)]
        public static CodeDomProvider CreateProvider(String language, System.Collections.Generic.IDictionary<String, String> providerOptions)
        {
            CompilerInfo compilerInfo = GetCompilerInfo(language);
            return compilerInfo.CreateProvider(providerOptions);
        }

        [ComVisible(false)]
        public static CodeDomProvider CreateProvider(String language)
        {
            CompilerInfo compilerInfo = GetCompilerInfo(language);
            return compilerInfo.CreateProvider();
        }

        [ComVisible(false)]
        public static String GetLanguageFromExtension(String extension)
        {
            CompilerInfo compilerInfo = (CompilerInfo)GetCompilerInfoForExtensionNoThrow(extension);
            if (compilerInfo == null)
                throw new ConfigurationErrorsException(SRCodeDom.CodeDomProvider_NotDefined);
            return compilerInfo._compilerLanguages[0]; // Return the first language name. There has to be one.
        }
        [ComVisible(false)]
        public static bool IsDefinedLanguage(String language)
        {
            return (GetCompilerInfoForLanguageNoThrow(language) != null);
        }

        [ComVisible(false)]
        public static bool IsDefinedExtension(String extension)
        {
            return (GetCompilerInfoForExtensionNoThrow(extension) != null);
        }


        [ComVisible(false)]
        public static CompilerInfo GetCompilerInfo(String language)
        {
            CompilerInfo compilerInfo = GetCompilerInfoForLanguageNoThrow(language);
            if (compilerInfo == null)
                throw new ConfigurationErrorsException(SRCodeDom.CodeDomProvider_NotDefined);
            return compilerInfo;
        }


        // Do argument validation but don't throw if there's no compiler defined for a language.
        private static CompilerInfo GetCompilerInfoForLanguageNoThrow(String language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            CompilerInfo compilerInfo = (CompilerInfo)Config._compilerLanguages[language.Trim()];
            return compilerInfo;
        }

        // Do argument validation but don't throw if there's no compiler defined for a language.
        private static CompilerInfo GetCompilerInfoForExtensionNoThrow(String extension)
        {
            if (extension == null)
                throw new ArgumentNullException("extension");

            CompilerInfo compilerInfo = (CompilerInfo)Config._compilerExtensions[extension.Trim()];
            return compilerInfo;
        }

        [ComVisible(false)]
        public static CompilerInfo[] GetAllCompilerInfo()
        {
            ArrayList values = Config._allCompilerInfo;
            CompilerInfo[] compilerInfos = (CompilerInfo[])values.ToArray(typeof(CompilerInfo));
            return compilerInfos;
        }

        // Don't cache the configuration since things are different for asp.net scenarios.
        private static CodeDomCompilationConfiguration Config
        {
            get
            {
                CodeDomCompilationConfiguration _configuration = new CodeDomCompilationConfiguration();
                return _configuration;
            }
        }
        /// <devdoc>
        ///    <para>Retrieves the default extension to use when saving files using this code dom provider.</para>
        /// </devdoc>
        public virtual string FileExtension
        {
            get
            {
                return string.Empty;
            }
        }

        /// <devdoc>
        ///    <para>Returns flags representing language variations.</para>
        /// </devdoc>
        public virtual LanguageOptions LanguageOptions
        {
            get
            {
                return LanguageOptions.None;
            }
        }

        [Obsolete("Callers should not use the ICodeGenerator interface and should instead use the methods directly on the CodeDomProvider class. Those inheriting from CodeDomProvider must still implement this interface, and should exclude this warning or also obsolete this method.")]
        public abstract ICodeGenerator CreateGenerator();

#pragma warning disable 618
        public virtual ICodeGenerator CreateGenerator(TextWriter output)
        {
            return CreateGenerator();
        }

        public virtual ICodeGenerator CreateGenerator(string fileName)
        {
            return CreateGenerator();
        }
#pragma warning restore 618

        [Obsolete("Callers should not use the ICodeCompiler interface and should instead use the methods directly on the CodeDomProvider class. Those inheriting from CodeDomProvider must still implement this interface, and should exclude this warning or also obsolete this method.")]
        public abstract ICodeCompiler CreateCompiler();

        [Obsolete("Callers should not use the ICodeParser interface and should instead use the methods directly on the CodeDomProvider class. Those inheriting from CodeDomProvider must still implement this interface, and should exclude this warning or also obsolete this method.")]
        public virtual ICodeParser CreateParser()
        {
            return null;
        }

        /// <devdoc>
        ///     This method allows a code dom provider implementation to provide a different type converter
        ///     for a given data type.  At design time, a designer may pass data types through this
        ///     method to see if the code dom provider wants to provide an additional converter.  
        ///     As typical way this would be used is if the language this code dom provider implements
        ///     does not support all of the values of MemberAttributes enumeration, or if the language
        ///     uses different names (Protected instead of Family, for example).  The default 
        ///     implementation just calls TypeDescriptor.GetConverter for the given type.
        /// </devdoc>
        public virtual TypeConverter GetConverter(Type type)
        {
            return TypeDescriptor.GetConverter(type);
        }
        /// <devdoc>
        ///    <para>
        ///       Creates an assembly based on options, with the information from the compile units
        ///    </para>
        /// </devdoc>
        public virtual CompilerResults CompileAssemblyFromDom(CompilerParameters options, params CodeCompileUnit[] compilationUnits)
        {
            return CreateCompilerHelper().CompileAssemblyFromDomBatch(options, compilationUnits);
        }

        /// <devdoc>
        ///    <para>
        ///       Creates an assembly based on options, with the contents of the files
        ///    </para>
        /// </devdoc>
        public virtual CompilerResults CompileAssemblyFromFile(CompilerParameters options, params string[] fileNames)
        {
            return CreateCompilerHelper().CompileAssemblyFromFileBatch(options, fileNames);
        }

        /// <devdoc>
        ///    <para>
        ///       Creates an assembly based on options, with the information from sources
        ///    </para>
        /// </devdoc>
        public virtual CompilerResults CompileAssemblyFromSource(CompilerParameters options, params string[] sources)
        {
            return CreateCompilerHelper().CompileAssemblyFromSourceBatch(options, sources);
        }

        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether
        ///       the specified value is a valid identifier for this language.
        ///    </para>
        /// </devdoc>
        public virtual bool IsValidIdentifier(string value)
        {
            return CreateGeneratorHelper().IsValidIdentifier(value);
        }

        public virtual string CreateEscapedIdentifier(string value)
        {
            return CreateGeneratorHelper().CreateEscapedIdentifier(value);
        }

        public virtual string CreateValidIdentifier(string value)
        {
            return CreateGeneratorHelper().CreateValidIdentifier(value);
        }

        public virtual string GetTypeOutput(CodeTypeReference type)
        {
            return CreateGeneratorHelper().GetTypeOutput(type);
        }

        public virtual bool Supports(GeneratorSupport generatorSupport)
        {
            return CreateGeneratorHelper().Supports(generatorSupport);
        }

        /// <devdoc>
        ///    <para>
        ///       Generates code from the specified expression and
        ///       outputs it to the specified textwriter.
        ///    </para>
        /// </devdoc>
        public virtual void GenerateCodeFromExpression(CodeExpression expression, TextWriter writer, CodeGeneratorOptions options)
        {
            CreateGeneratorHelper().GenerateCodeFromExpression(expression, writer, options);
        }

        /// <devdoc>
        ///    <para>
        ///       Outputs the language specific representaion of the CodeDom tree
        ///       refered to by statement, into writer.
        ///    </para>
        /// </devdoc>
        public virtual void GenerateCodeFromStatement(CodeStatement statement, TextWriter writer, CodeGeneratorOptions options)
        {
            CreateGeneratorHelper().GenerateCodeFromStatement(statement, writer, options);
        }

        /// <devdoc>
        ///    <para>
        ///       Outputs the language specific representaion of the CodeDom tree
        ///       refered to by codeNamespace, into writer.
        ///    </para>
        /// </devdoc>
        public virtual void GenerateCodeFromNamespace(CodeNamespace codeNamespace, TextWriter writer, CodeGeneratorOptions options)
        {
            CreateGeneratorHelper().GenerateCodeFromNamespace(codeNamespace, writer, options);
        }

        /// <devdoc>
        ///    <para>
        ///       Outputs the language specific representaion of the CodeDom tree
        ///       refered to by compileUnit, into writer.
        ///    </para>
        /// </devdoc>
        public virtual void GenerateCodeFromCompileUnit(CodeCompileUnit compileUnit, TextWriter writer, CodeGeneratorOptions options)
        {
            CreateGeneratorHelper().GenerateCodeFromCompileUnit(compileUnit, writer, options);
        }

        /// <devdoc>
        ///    <para>
        ///       Outputs the language specific representaion of the CodeDom tree
        ///       refered to by codeType, into writer.
        ///    </para>
        /// </devdoc>
        public virtual void GenerateCodeFromType(CodeTypeDeclaration codeType, TextWriter writer, CodeGeneratorOptions options)
        {
            CreateGeneratorHelper().GenerateCodeFromType(codeType, writer, options);
        }

        public virtual void GenerateCodeFromMember(CodeTypeMember member, TextWriter writer, CodeGeneratorOptions options)
        {
            throw new NotImplementedException(SRCodeDom.NotSupported_CodeDomAPI);
        }

        /// <devdoc>
        ///    <para>
        ///       Compiles the given text stream into a CodeCompile unit.  
        ///    </para>
        /// </devdoc>
        public virtual CodeCompileUnit Parse(TextReader codeStream)
        {
            return CreateParserHelper().Parse(codeStream);
        }

#pragma warning disable 618
        private ICodeCompiler CreateCompilerHelper()
        {
            ICodeCompiler compiler = CreateCompiler();
            if (compiler == null)
            {
                throw new NotImplementedException(SRCodeDom.NotSupported_CodeDomAPI);
            }
            return compiler;
        }

        private ICodeGenerator CreateGeneratorHelper()
        {
            ICodeGenerator generator = CreateGenerator();
            if (generator == null)
            {
                throw new NotImplementedException(SRCodeDom.NotSupported_CodeDomAPI);
            }
            return generator;
        }

        private ICodeParser CreateParserHelper()
        {
            ICodeParser parser = CreateParser();
            if (parser == null)
            {
                throw new NotImplementedException(SRCodeDom.NotSupported_CodeDomAPI);
            }
            return parser;
        }

        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CodeDomProvider() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
#pragma warning restore 618
    }
}
