// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
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
        private static HashSet<int> s_includeIssuesHash = DetectIssuesToInclude();

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
            // If we are deliberately including this issue in the test run,
            // return "don't skip" response unconditionally.
            if (ShouldIncludeThisIssue(Issue))
            {
                return null;
            }

            string repositoryAndIssue = String.Format("{0} #{1}", Repository, Issue);

            if (Framework.MatchesCurrent())
            {
                // Don't skip if we are running tests with issues.
                // But match the Framework first so we don't run tests 
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

            // If no specific OS or Framework filters, it applies to all.
            if (OS == OSID.None && Framework == FrameworkID.None)
            {
                return String.Format("{0}",
                    repositoryAndIssue);
            }

            // Null means "don't skip"
            return null;
        }

        // Returns a HashSet<int> containing all the test issue numbers to include
        // during a test run (where "include" means "don't skip").  The meaning of
        // this HashSet is:
        //  null:       [default] don't include any tests with [Issue] (i.e. skip them all)
        // count == 0:  Include all tests with [Issue] (i.e. don't skip any)
        // count > 0:   Include only those tests with an issue in the HashSet 
        private static HashSet<int> DetectIssuesToInclude()
        {
            string includeTestsWithIssues = TestProperties.GetProperty(TestProperties.IncludeTestsWithIssues_PropertyName);

            // Empty string means don't include any tests with issues.
            // In other words, all tests with [Issue] will be skipped.
            if (String.IsNullOrEmpty(includeTestsWithIssues))
            {
                return null;
            }

            // The special value 'true' means include all tests with [Issue].
            // The special value 'false' means skip all tests with [Issue].
            bool propertyAsBool = false;
            if (bool.TryParse(includeTestsWithIssues, out propertyAsBool))
            {
                return propertyAsBool ? new HashSet<int>() : null;
            }

            // Anything else is interpreted as a semicolon or comma separated list of
            // issue numbers to include (i.e. not skip).
            string[] issues = includeTestsWithIssues.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            HashSet<int> hashSet = new HashSet<int>();
            foreach (string issue in issues)
            {
                int issueAsInt = 0;
                if (!int.TryParse(issue, out issueAsInt))
                {
                    Console.WriteLine(String.Format("Warning: The number '{0}' in IncludeTestsWithIssues is not a valid integer and will be ignored.", issue));
                    continue;
                }

                hashSet.Add(issueAsInt);
            }

            return hashSet;
        }

        // Returns 'true' if a test marked with the given issue should be included
        // (i.e. not skipped)
        private static bool ShouldIncludeThisIssue(int issue)
        {
            // A null HashSet (default) means no test with [Issue] is included,
            // meaning all of them should be skipped
            if (s_includeIssuesHash == null)
            {
                return false;
            }

            // An empty HashSet<> means "include all"
            if (s_includeIssuesHash.Count == 0)
            {
                return true;
            }

            // A non-empty HashSet contains issue numbers to include
            return s_includeIssuesHash.Contains(issue);
        }
    }
}
