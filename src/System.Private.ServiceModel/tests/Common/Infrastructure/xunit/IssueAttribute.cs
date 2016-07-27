// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit.Abstractions;

namespace Infrastructure.Common
{
    // The [Issue] attribute can be applied to any test method that
    // is marked with [WcfFact] or [WcfTheory].  It indicates the open
    // issue number and optionally the framework(s), or
    // OS(s) to which it applies.
    // Examples: 
    //  [Issue(999)]
    //      means this issue applies to all OSes, all Frameworks
    //  [Issue(999, Framework=FrameworkID.NetNative)]
    //      means this issue only applies to NET Native
    //  [Issue(999, OS=OSID.Ubuntu_14_04)]
    //      means this issue only applies to Ubuntu 14.04

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class IssueAttribute : WcfSkippableAttribute
    {
        public int Issue { get; private set; }

        public FrameworkID Framework {get; set; }

        public OSID OS { get; set; }

        // Short name to repository where the issue exists.
        // It is not required to be a URL, and it won't be used to create a URL.
        public string Repository { get; set; }

        public IssueAttribute(int issue)
        {
            Issue = issue;
            Repository = "dotnet/wcf";

            // Default framework and OS to "none" so that they must
            // be explicitly specified to be more specific.
            Framework = FrameworkID.None;
            OS = OSID.None;
        }

        public override string GetSkipReason(ITestMethod testMethod)
        {
            string repositoryAndIssue = String.Format("{0} #{1}", Repository, Issue);

            if (Framework.MatchesCurrent())
            {
                return String.Format("{0} on framework \"{1}\" (filter is \"{2}\")",
                                    repositoryAndIssue,
                                    FrameworkHelper.Current.Name(),
                                    Framework.Name());
            }

            if (OS.MatchesCurrent())
            {
                return String.Format("{0} on OS \"{1}\" (filter is \"{2}\")",
                                    repositoryAndIssue,
                                    OSHelper.Current.Name(),
                                    OS.Name());
            }

            // If no specific OS or Framework filters, it applies to all
            if (OS == OSID.None && Framework == FrameworkID.None)
            {
                return String.Format("{0}",
                    repositoryAndIssue);
            }

            // Null means "don't skip"
            return null;
        }
    }
}
