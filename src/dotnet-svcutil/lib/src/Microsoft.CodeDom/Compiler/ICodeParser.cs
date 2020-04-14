// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.CodeDom.Compiler
{
    using System.Diagnostics;
    using System.IO;

    /// <devdoc>
    ///    <para>
    ///       Provides a code parsing interface.
    ///    </para>
    /// </devdoc>
    public interface ICodeParser
    {
        /// <devdoc>
        ///    <para>
        ///       Compiles the given text stream into a CodeCompile unit.  
        ///    </para>
        /// </devdoc>
        CodeCompileUnit Parse(TextReader codeStream);
    }
}
