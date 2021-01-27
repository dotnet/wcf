// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeDom.Compiler
{
    using System.Text;

    using System.Diagnostics;
    using System;
    using Microsoft.Win32;
    using System.IO;
    using System.Collections;
    using System.Reflection;
    using Microsoft.CodeDom;


    /// <devdoc>
    ///    <para>
    ///       Provides a code parsing abstract base class.
    ///    </para>
    /// </devdoc>


    public abstract class CodeParser : ICodeParser
    {
        /// <devdoc>
        ///    <para>
        ///       Compiles the given text stream into a CodeCompile unit.  
        ///    </para>
        /// </devdoc>
        public abstract CodeCompileUnit Parse(TextReader codeStream);
    }
}
