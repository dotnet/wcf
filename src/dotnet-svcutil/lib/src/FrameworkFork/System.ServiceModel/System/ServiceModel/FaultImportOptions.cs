// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
