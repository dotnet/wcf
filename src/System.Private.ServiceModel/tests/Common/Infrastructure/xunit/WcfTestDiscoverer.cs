// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Infrastructure.Common
{
    // Internal helper class for code common to conditional test discovery through
    // [WcfFact] and [WcfTheory].  It reflects back onto the test method's attributes
    // to determine whether the test should be skipped.
    internal class WcfTestDiscoverer
    {
        internal static IEnumerable<IXunitTestCase> Discover(
                                                        ITestFrameworkDiscoveryOptions discoveryOptions,
                                                        IMessageSink diagnosticMessageSink,
                                                        ITestMethod testMethod,
                                                        IEnumerable<IXunitTestCase> testCases,
                                                        bool isTheory = false)
        {
            MethodInfo testMethodInfo = testMethod.Method.ToRuntimeMethod();
 
            // Evaluate any [Issue] attributes on this method.
            // We do this first so that tests we know won't be run will avoid calling
            // their ConditionalFact conditions (below) which could do unnecessary work
            // and affect other tests that will be run.
            IssueAttribute[] issues = testMethodInfo.GetCustomAttributes<IssueAttribute>().ToArray();
            if (issues.Length > 0)
            {
                List<string> issueSkipList = new List<string>();
                foreach (IssueAttribute issue in issues)
                {
                    string skipReason = issue.GetSkipReason(testMethod);
                    if (skipReason != null)
                    {
                        issueSkipList.Add(skipReason);
                    }
                }

                if (issueSkipList.Count > 0)
                {
                    string skippedReason = string.Format("Active issue(s): {0}", string.Join(", ", issueSkipList));
                    return testCases.Select(tc => new WcfTestCase((XunitTestCase)tc,
                                                                  discoveryOptions.MethodDisplayOrDefault(),
                                                                  Timeout.InfiniteTimeSpan,
                                                                  skippedReason,
                                                                  isTheory,
                                                                  diagnosticMessageSink));
                }
            }

            // Evaluate all the [Condition] attributes.  These will execute code
            // that determines whether this test should be run or skipped.
            ConditionAttribute[] conditions = testMethodInfo.GetCustomAttributes<ConditionAttribute>().ToArray();
            if (conditions.Length > 0)
            {
                List<string> skipReasons = new List<string>(conditions.Length);

                foreach (ConditionAttribute conditionAttribute in conditions)
                {
                    string skipReason = conditionAttribute.GetSkipReason(testMethod);
                    if (skipReason != null)
                    {
                        skipReasons.Add(skipReason);
                    }
                }

                // Compose a summary of all conditions that returned false.
                if (skipReasons.Count > 0)
                {
                    string skippedReason = string.Format("Condition(s) not met: {0}", string.Join(", ", skipReasons));
                    return testCases.Select(tc => new WcfTestCase((XunitTestCase)tc,
                                                                  discoveryOptions.MethodDisplayOrDefault(),
                                                                  Timeout.InfiniteTimeSpan,
                                                                  skippedReason,
                                                                  isTheory,
                                                                  diagnosticMessageSink));
                }
            }

            // Look for FailFastAfterAttribute applied to the test method
            FailFastAfterAttribute failFastAttribute = testMethodInfo.GetCustomAttribute<FailFastAfterAttribute>();
            if (failFastAttribute == null)
            {
                // If it's not found on the method, check the class
                failFastAttribute = testMethodInfo.DeclaringType.GetCustomAttribute<FailFastAfterAttribute>();
            }

            if (failFastAttribute == null)
            {
                // If it's not found on the method or class, check the assembly
                failFastAttribute = testMethodInfo.DeclaringType.Assembly.GetCustomAttribute<FailFastAfterAttribute>();
            }

            TimeSpan failFastDuration = Timeout.InfiniteTimeSpan;
            if (failFastAttribute != null)
            {
                failFastDuration = failFastAttribute.FailTime;
            }

            // If we get this far, we have decided to run the test.
            // Still wrap it in a WcfTestCase with a null skip message
            // so that other WcfTestCase customizations are used.
            return testCases.Select(tc => new WcfTestCase(testCase: (XunitTestCase)tc,
                                                          defaultMethodDisplay: discoveryOptions.MethodDisplayOrDefault(),
                                                          failFastDuration: failFastDuration,
                                                          skippedReason: null,
                                                          isTheory: isTheory,
                                                          diagnosticMessageSink: diagnosticMessageSink));
        }
    }
}
