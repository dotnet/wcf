// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace System.ServiceModel
{
    public class FaultImportOptions
    {
        /* use the current message formatter for faults.*/
        private bool _useMessageFormat = false;

        public bool UseMessageFormat
        {
            get { return _useMessageFormat; }
            set { _useMessageFormat = value; }
        }
    }
}
