// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit.Abstractions;

namespace Infrastructure.Common
{
    // Abstract base class for attributes that can be used in
    // combination with [WcfFact] or [WcfTheory] to cause a test
    // to be skipped
    public abstract class WcfSkippableAttribute : Attribute
    {
        // Return a string describing why the test should be skipped.
        // Null means it should not be skipped.
        public abstract string GetSkipReason(ITestMethod testMethod);
    }
}
