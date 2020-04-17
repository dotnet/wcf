// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
