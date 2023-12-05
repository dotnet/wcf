// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using Infrastructure.Common;
using Xunit;

public class OSAndFrameworkTests
{
    [WcfFact]
    [OuterLoop]
    public static void FrameworkID_Was_Detected()
    {
        Assert.True(FrameworkHelper.Current != FrameworkID.None,
                    String.Format("FrameworkID was not properly detected from: RuntimeInformation.FrameworkDescription = \'{0}\"",
                                   RuntimeInformation.FrameworkDescription));
    }

    [WcfFact]
    [OuterLoop]
    public static void OSID_Was_Detected()
    {
        Assert.True(OSHelper.Current != OSID.None,
                String.Format("OSID was not properly detected:{0}  The RuntimeInformation.OSDescription is = \'{1}\" {2}  The RuntimeIdentifier is = {3}",
                               Environment.NewLine,
                               RuntimeInformation.OSDescription,
                               Environment.NewLine,
                               OSHelper.GetRuntimeIdentifier()));
    }

    [WcfFact]
    [OuterLoop]
    public static void FrameworkID_Name_Formats_Correctly()
    {
        FrameworkID id = FrameworkID.NetCore | FrameworkID.NetNative;
        string formatted = id.Name();

        Assert.True(formatted.Contains("NetCore") && formatted.Contains("NetNative"),
                    String.Format("FrameworkID.Name should have contained NetCore and NetNative, but actual was \"{0}\"", formatted));
    }

    [WcfFact]
    [OuterLoop]
    public static void OSID_Name_Formats_Correctly()
    {
        OSID id = OSID.Windows_7 | OSID.Ubuntu;
        string formatted = id.Name();

        Assert.True(formatted.Contains("Windows_7") && formatted.Contains("Ubuntu"),
                    String.Format("FrameworkID.Name should have contained Windows_7 and Ubuntu, but actual was \"{0}\"", formatted));
    }

    // Enable this test to get the RID string of lab machines.
    // New RIDs should be added to Infrastructure.Common OSHelper and OSID classes.
    [Issue(0000)]
    [WcfFact]
    [OuterLoop]
    public static void ListAllOSRIDs()
    {
        string testRuntime = OSHelper.GetRuntimeIdentifier();
        OSID id = OSHelper.OSIDfromRuntimeEnvironment();
        string runtimeOSDescription = RuntimeInformation.OSDescription;

        Assert.Fail(string.Format("Detected the current Runtime Identifier as: '{0}'\n" + 
                                         "Which maps to OSID: '{1}'\n" +
                                           "Detected the current runtimeOSDescription as: '{2}'", testRuntime, id.Name(), runtimeOSDescription));
    }
}
