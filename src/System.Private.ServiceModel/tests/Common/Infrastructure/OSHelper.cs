// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Infrastructure.Common
{
    // Helper class for OSID, an enum created to match
    // RuntimeInformation.OSDescription
    public static class OSHelper
    {
        // These literals must match those returned from RuntimeInformation.OSDescription.
        private static string WindowsOsName = "Microsoft Windows"; // netcore50 || win8
        private static string WindowsPhoneOsName = "Microsoft Windows Phone";    // wpa81

        private static bool _detectedOSID = false;
        private static OSID _currentOSID = 0;
        private static string _currentOSDescription;

        // Test runtimes are specified via the $(TestNugetRuntimeId) property.
        // This list associates names with their corresponding OSID.
        // It falls back to "any" for partial matches so that future versions
        // choose at least the correct OS category.

        private static List<Tuple<string, OSID>> _runtimeToOSID = new List<Tuple<string, OSID>>
        {
            new Tuple<string, OSID>("centos.7.1", OSID.CentOS_7_1),
            new Tuple<string, OSID>("centos.7", OSID.CentOS_7),
            new Tuple<string, OSID>("centos.", OSID.AnyCentOS),

            new Tuple<string, OSID>("debian.8.2", OSID.Debian_8_2),
            new Tuple<string, OSID>("debian.8", OSID.Debian_8),
            new Tuple<string, OSID>("debian", OSID.AnyDebian),

            new Tuple<string, OSID>("fedora_23", OSID.Fedora_23),
            new Tuple<string, OSID>("fedora", OSID.AnyFedora),

            new Tuple<string, OSID>("opensuse.13.2", OSID.OpenSUSE_13_2),
            new Tuple<string, OSID>("opensuse", OSID.AnyOpenSUSE),

            new Tuple<string, OSID>("osx.10.10", OSID.OSX_10_10),
            new Tuple<string, OSID>("osx.10.11", OSID.OSX_10_11),
            new Tuple<string, OSID>("osx", OSID.AnyOSX),

            new Tuple<string, OSID>("rhel.7", OSID.RHEL_7),
            new Tuple<string, OSID>("rhel", OSID.AnyRHEL),

            new Tuple<string, OSID>("ubuntu.14.04", OSID.Ubuntu_14_04),
            new Tuple<string, OSID>("ubuntu.16.04", OSID.Ubuntu_16_04),
            new Tuple<string, OSID>("ubuntu", OSID.AnyUbuntu),

            // Currently, win7 is used for the windows runtime, regardless
            // which version of Windows is actually used to run the test.
            // So we can't determine the OS from the runtime in this list.
        };

        private static string CurrentOSDescription
        {
            get
            {
                if (_currentOSDescription == null)
                {
                    _currentOSDescription = RuntimeInformation.OSDescription;
                }

                return _currentOSDescription;
            }
        }

        public static OSID Current
        {
            get
            {
                if (!_detectedOSID)
                {
                    _currentOSID = DetectCurrentOS();
                    _detectedOSID = true;

                    // Log this to the console so that lab run artifacts will show what we detected.
                    Console.WriteLine(String.Format("Detected current OSID as \"{0}\" from test runtime \"{1}\" and description \"{2}\"",
                                                     _currentOSID.Name(), 
                                                     TestProperties.GetProperty(TestProperties.TestNugetRuntimeId_PropertyName),
                                                     CurrentOSDescription));
                }

                return _currentOSID;
            }
        }

        // Extension method that returns 'true' if the given OSID
        // includes the OSID on which this is currently executing.
        public static bool MatchesCurrent(this OSID id)
        {
            return ((Current & id) != 0);
        }

        // Extension method for OSID to provide name.
        // "G" formatting will expand to enum's name or collection if multiple.
        public static string Name(this OSID id)
        {
            return id.ToString("G");
        }

        private static OSID DetectCurrentOS()
        {
            // Attempt to map from the test runtime
            OSID osid = OSIDfromTestRuntime();
            if (osid != OSID.None)
            {
                return osid;
            }

            string osName = CurrentOSDescription;
            if (osName.IndexOf(WindowsOsName, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                // We cannot distinguish further from RuntimeInformation.OSDescription alone
                return OSID.AnyWindows;
            }

            if (osName.IndexOf(WindowsPhoneOsName, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return OSID.WindowsPhone;
            }

            // We use "None" when we cannot determine the OSID and rely on
            // tests to verify detection succeeded.
            return OSID.None;
        }

        // Maps from the $(TestNugetRuntimeId) property to corresponding OSID.
        // Returns OSID.None if cannot determine the OSID.
        private static OSID OSIDfromTestRuntime()
        {
            string testRuntime = TestProperties.GetProperty(TestProperties.TestNugetRuntimeId_PropertyName);
            if (string.IsNullOrEmpty(testRuntime))
            {
                return OSID.None;
            }

            foreach (var pair in _runtimeToOSID)
            {
                string runtime = pair.Item1;
                if (testRuntime.IndexOf(runtime, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return pair.Item2;
                }
            }
            return OSID.None;
        }
    }
}
