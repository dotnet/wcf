// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
    public class CodeRegionDirective : CodeDirective
    {
        private string _regionText;
        private CodeRegionMode _regionMode;

        public CodeRegionDirective()
        {
        }

        public CodeRegionDirective(CodeRegionMode regionMode, string regionText)
        {
            this.RegionText = regionText;
            _regionMode = regionMode;
        }

        public string RegionText
        {
            get
            {
                return (_regionText == null) ? string.Empty : _regionText;
            }
            set
            {
                _regionText = value;
            }
        }

        public CodeRegionMode RegionMode
        {
            get
            {
                return _regionMode;
            }
            set
            {
                _regionMode = value;
            }
        }
    }
}
