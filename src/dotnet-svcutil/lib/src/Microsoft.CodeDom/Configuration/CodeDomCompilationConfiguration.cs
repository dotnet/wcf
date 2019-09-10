namespace Microsoft.CodeDom.Compiler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class CodeDomCompilationConfiguration
    {
        internal Hashtable _compilerLanguages;
        internal ArrayList _allCompilerInfo;
        internal Hashtable _compilerExtensions;

        internal CodeDomCompilationConfiguration()
        {
            // First time initialization. This must be kept consistent with machine.config.comments in that it 
            // must initialize the config system as if that block was present.

            _compilerLanguages = new Hashtable(StringComparer.OrdinalIgnoreCase);
            _compilerExtensions = new Hashtable(StringComparer.OrdinalIgnoreCase);
            _allCompilerInfo = new ArrayList();

            CompilerInfo compilerInfo;
            CompilerParameters compilerParameters;
            String typeName;

            // C#
            compilerParameters = new CompilerParameters();
            compilerParameters.WarningLevel = 4;
#if disabled
            typeName = "Microsoft.CSharp.CSharpCodeProvider, System";
#endif
            typeName = "Microsoft.CSharp.CSharpCodeProvider";
            compilerInfo = new CompilerInfo(compilerParameters, typeName);
            compilerInfo._compilerLanguages = new string[] { "c#", "cs", "csharp" };
            compilerInfo._compilerExtensions = new string[] { ".cs", "cs" };
            compilerInfo._providerOptions = new Dictionary<string, string>();
            compilerInfo._providerOptions[RedistVersionInfo.NameTag] = RedistVersionInfo.DefaultVersion;
            AddCompilerInfo(compilerInfo);

#if disabled
            // VB
            compilerParameters = new CompilerParameters();
            compilerParameters.WarningLevel = 4;
            typeName = "Microsoft.VisualBasic.VBCodeProvider, System";
            compilerInfo = new CompilerInfo(compilerParameters, typeName);
            compilerInfo._compilerLanguages = new string[] { "vb", "vbs", "visualbasic", "vbscript" };
            compilerInfo._compilerExtensions = new string[] { ".vb", "vb" };
            compilerInfo._providerOptions = new Dictionary<string, string>();
            compilerInfo._providerOptions[RedistVersionInfo.NameTag] = RedistVersionInfo.DefaultVersion;
            AddCompilerInfo(compilerInfo);

            // JScript
            compilerParameters = new CompilerParameters();
            compilerParameters.WarningLevel = 4;
            typeName = "Microsoft.JScript.JScriptCodeProvider, MicrosoftJScript";
            compilerInfo = new CompilerInfo(compilerParameters, typeName);
            compilerInfo._compilerLanguages = new string[] { "js", "jscript", "javascript" };
            compilerInfo._compilerExtensions = new string[] { ".js", "js" };
            compilerInfo._providerOptions = new Dictionary<string, string>();
            AddCompilerInfo(compilerInfo);

            // C++
            compilerParameters = new CompilerParameters();
            compilerParameters.WarningLevel = 4;
            typeName = "Microsoft.VisualC.CppCodeProvider, MicrosoftVisualCCppCodeProvider";
            compilerInfo = new CompilerInfo(compilerParameters, typeName);
            compilerInfo._compilerLanguages = new string[] { "c++", "mc", "cpp" };
            compilerInfo._compilerExtensions = new string[] { ".h", "h" };
            compilerInfo._providerOptions = new Dictionary<string, string>();
            AddCompilerInfo(compilerInfo);
#endif
        }

        private void AddCompilerInfo(CompilerInfo compilerInfo)
        {

            foreach (string language in compilerInfo._compilerLanguages)
            {
                _compilerLanguages[language] = compilerInfo;
            }

            foreach (string extension in compilerInfo._compilerExtensions)
            {
                _compilerExtensions[extension] = compilerInfo;
            }

            _allCompilerInfo.Add(compilerInfo);
        }

    }
}