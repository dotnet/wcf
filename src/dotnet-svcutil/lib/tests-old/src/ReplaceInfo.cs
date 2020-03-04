// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Text;

namespace SvcutilTest
{
    public class ReplaceInfo
    {
        public string OriginalValue
        {
            get; private set;
        }

        public string NewValue
        {
            get; private set;
        }

        public bool IgnoreCase
        {
            get; set;
        }

        public bool UseRegex
        {
            get; set;
        }

        public ReplaceInfo(string originalValue, string newValue, bool ignoreCase = true)
        {
            this.OriginalValue = originalValue;
            this.NewValue = newValue;
            this.IgnoreCase = ignoreCase;
        }

        public override string ToString()
        {
            return $"{this.OriginalValue} -> {this.NewValue}";
        }
    }
}
