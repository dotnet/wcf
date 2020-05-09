// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
