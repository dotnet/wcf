// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeDom
{
    using System.Diagnostics;
    using System;
    using Microsoft.Win32;
    using System.Collections;
    using System.Runtime.InteropServices;

    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeIfDirective : CodeDirective
    {
        private string _ifText;
        private CodeIfMode _ifMode;

        public CodeIfDirective()
        {
        }

        public CodeIfDirective(CodeIfMode ifMode, string ifText)
        {
            this.IfText = ifText;
            _ifMode = ifMode;
        }

        public string IfText
        {
            get
            {
                return (_ifText == null) ? string.Empty : _ifText;
            }
            set
            {
                _ifText = value;
            }
        }

        public CodeIfMode IfMode
        {
            get
            {
                return _ifMode;
            }
            set
            {
                _ifMode = value;
            }
        }
    }
}
