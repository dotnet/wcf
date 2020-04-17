// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

            typeName = "Microsoft.CSharp.CSharpCodeProvider";
            compilerInfo = new CompilerInfo(compilerParameters, typeName);
            compilerInfo._compilerLanguages = new string[] { "c#", "cs", "csharp" };
            compilerInfo._compilerExtensions = new string[] { ".cs", "cs" };
            compilerInfo._providerOptions = new Dictionary<string, string>();
            compilerInfo._providerOptions[RedistVersionInfo.NameTag] = RedistVersionInfo.DefaultVersion;
            AddCompilerInfo(compilerInfo);
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
