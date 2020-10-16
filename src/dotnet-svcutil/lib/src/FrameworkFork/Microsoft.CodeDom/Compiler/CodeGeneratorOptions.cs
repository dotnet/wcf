// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeDom.Compiler
{
    using System;
    using Microsoft.CodeDom;
    using System.Collections;
    using System.Collections.Specialized;



    /// <devdoc>
    ///    <para>
    ///       Represents options used in code generation
    ///    </para>
    /// </devdoc>


    public class CodeGeneratorOptions
    {
        private IDictionary _options = new ListDictionary();

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeGeneratorOptions()
        {
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public object this[string index]
        {
            get
            {
                return _options[index];
            }
            set
            {
                _options[index] = value;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string IndentString
        {
            get
            {
                object o = _options["IndentString"];
                return ((o == null) ? "    " : (string)o);
            }
            set
            {
                _options["IndentString"] = value;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string BracingStyle
        {
            get
            {
                object o = _options["BracingStyle"];
                return ((o == null) ? "Block" : (string)o);
            }
            set
            {
                _options["BracingStyle"] = value;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool ElseOnClosing
        {
            get
            {
                object o = _options["ElseOnClosing"];
                return ((o == null) ? false : (bool)o);
            }
            set
            {
                _options["ElseOnClosing"] = value;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool BlankLinesBetweenMembers
        {
            get
            {
                object o = _options["BlankLinesBetweenMembers"];
                return ((o == null) ? true : (bool)o);
            }
            set
            {
                _options["BlankLinesBetweenMembers"] = value;
            }
        }

        [System.Runtime.InteropServices.ComVisible(false)]
        public bool VerbatimOrder
        {
            get
            {
                object o = _options["VerbatimOrder"];
                return ((o == null) ? false : (bool)o);
            }
            set
            {
                _options["VerbatimOrder"] = value;
            }
        }
    }
}
