// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Uncomment the define RUN_WCF_SKIP_TESTS below to run the tests
// in this file that are skipped based on their attributes.
// They are normally disabled to prevent spurious numbers of "skip" results.
// But they are presented here as samples for test skipping semantics.

// #define RUN_WCF_SKIP_TESTS

using System;
using System.Runtime.InteropServices;
using Infrastructure.Common;
using Xunit;

public class WcfFactTests
{
    // Tests that do not skip are allowed to run to test [WcfFact]
    // This also triggers lazy initialization of all the skippable
    // attributes.
    
    [WcfFact]
    [Condition(nameof(Some_True_Condition))]
    [OuterLoop]
    public static void Run_On_True_Condition()
    {
    }

#if RUN_WCF_SKIP_TESTS
 
    [WcfFact]
    [Condition(nameof(Some_False_Condition))]
    [OuterLoop]
    public static void Skip_False_Condition()
    {
        Assert.True(false, "Should have been skipped");
    }
    
    [WcfFact]
    [Issue(999)]
    [OuterLoop]
    public static void Skip_Issue_All_Platforms()
    {
        Assert.True(false, "Should have been skipped");
    }
    
    [WcfFact]
    [Issue(999, Framework = FrameworkID.NetCore)]
    [OuterLoop]
    public static void Skip_Issue_Framework_NET_Core()
    {
        Assert.False(FrameworkID.NetCore.MatchesCurrent(), "Test should have been skipped on NetCore framework");
    }

    [WcfFact]
    [Issue(999, OS = OSID.AnyUbuntu)]
    [OuterLoop]
    public static void Skip_Issue_OS_AnyUbuntu()
    {
        Assert.False(OSID.AnyUbuntu.MatchesCurrent(), "Test should have been skipped on any Ubuntu OS");
    }
    
    [WcfFact]
    [Issue(999, OS = OSID.Ubuntu_14_04)]
    [OuterLoop]
    public static void Skip_Issue_OS_Ubuntu_14_04()
    {
        Assert.False(OSID.Ubuntu_14_04.MatchesCurrent(), "Test should have been skipped on Ubuntu 14.04");
    }

    [WcfFact]
    [Issue(999, Framework = FrameworkID.Any ^ FrameworkID.NetFramework)]
    [OuterLoop]
    public static void Skip_Issue_All_Frameworks_Except_Full_Framework()
    {
        Assert.False(FrameworkID.NetFramework.MatchesCurrent(), "Test should have been skipped on all frameworks but the full NET framework");
    }

    [WcfFact]
    [Issue(999, OS = OSID.Any ^ OSID.Ubuntu_14_04)]
    [OuterLoop]
    public static void Skip_Issue_All_OSes_Except_Ubuntu_14_04()
    {
        Assert.False(OSID.Ubuntu_14_04.MatchesCurrent(), "Test should have been skipped on all OSes but Ubuntu_14_04");
    }

    [WcfFact]
    [Issue(888, Framework = FrameworkID.NetCore)]
    [Issue(777, OS = OSID.AnyWindows)]
    [OuterLoop]
    public static void Skip_Issue_Platform_Framework_And_OS()
    {
        Assert.False(FrameworkID.NetCore.MatchesCurrent(), "Test should have been skipped on NetCore framework");
        Assert.False(OSID.AnyWindows.MatchesCurrent(), "Test should have been skipped on Windows OS");
    }
    
    [WcfFact]
    [Issue(999, OS = OSID.AnyWindows)]
    [Condition(nameof(Some_True_Condition))]
    [OuterLoop]
    public static void Skip_Isssue_With_True_Condition()
    {
        Assert.True(false, "Should have been skipped");
    }
    
    public static bool Some_False_Condition()
    {
        return false;
    }
    
#endif // RUN_WCF_SKIP_TESTS
    
    public static bool Some_True_Condition()
    {
        return true;
    }
}
