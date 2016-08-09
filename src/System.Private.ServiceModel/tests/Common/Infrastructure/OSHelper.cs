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
        // Hard-coded string literals used by RuntimeInformation.OSDescription
        // for conditionally compiled code for specific OS versions.  Other
        // versions of Windows obtain their description at runtime.
        private const string MicrosoftWindowsName = "Microsoft Windows";
        private const string MicrosoftWindowsPhoneName = "Microsoft Windows Phone";

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

        // All Windows version currently use the runtime "win7" so cannot be distinguished by that.
        // However the windows major and minor versions are known and can be used.  Some versions are
        // shared by different OSes, so at this level we can only say it is all of them.
        // Applications that have not been manifested for Win 8.1 or Win 10 will return Win 8.
        // This lookup table assumes the test running application has been manifested.
        // This mapping is described at https://msdn.microsoft.com/en-us/library/windows/desktop/ms724832(v=vs.85).aspx
        private static List<Tuple<string, OSID>> _descriptionToOSID = new List<Tuple<string, OSID>>
        {
            new Tuple<string, OSID>("Microsoft Windows 6.0.", OSID.Windows_Server_2008),
            new Tuple<string, OSID>("Microsoft Windows 6.1.", OSID.Windows_7 | OSID.Windows_Server_2008_R2),
            new Tuple<string, OSID>("Microsoft Windows 6.2.", OSID.Windows_8 | OSID.Windows_Server_2012),
            new Tuple<string, OSID>("Microsoft Windows 6.3.", OSID.Windows_8_1 | OSID.Windows_Server_2012_R2),
            new Tuple<string, OSID>("Microsoft Windows 10.", OSID.Windows_10 | OSID.Windows_Server_2016),
            new Tuple<string, OSID>(MicrosoftWindowsPhoneName, OSID.WindowsPhone),
            new Tuple<string, OSID>(MicrosoftWindowsName, OSID.AnyWindows),  // reserved for "Don't know which version"
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
            // First attempt to map from the test runtime.
            // All the non-Windows OSes are mapped this way.
            OSID osid = OSIDfromTestRuntime();
            if (osid == OSID.None)
            {
                // The Windows OSes are mapped based on description
                // because they all share the same runtime
                osid = OSIDfromOSDescription();
            }

            return osid;
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

        // Detects the OSID based on the current OS description.
        // Currently this is used only for Windows because the non-Windows
        // descriptions vary widely in format.
        private static OSID OSIDfromOSDescription()
        {
            string osDescription = CurrentOSDescription;
            if (string.IsNullOrEmpty(osDescription))
            {
                return OSID.None;
            }

            foreach (var pair in _descriptionToOSID)
            {
                string description = pair.Item1;
                if (osDescription.IndexOf(description, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    OSID detectedID =  pair.Item2;

                    // A match of "AnyWindows" means we know it was Windows but don't know which version.
                    if (detectedID == OSID.AnyWindows)
                    {
                        // "Microsoft Windows" is hard-coded at compilation time by RuntimeInformation.OSDescription
                        // for win8 and netcore50 builds.  See if we had an exact match (not just prefix-match).
                        if (string.Equals(MicrosoftWindowsName, CurrentOSDescription, StringComparison.OrdinalIgnoreCase))
                        {
                            // Complete match means it is compile-time hard coded for NET Native or Win8.
                            // If this is NET Native, mask out any OSID's where we know UWP cannot execute.
                            // What remains is a bit mask indicating which Windows version it could be.
                            if (FrameworkID.NetNative.MatchesCurrent())
                            {
                                detectedID &= ~(OSID.Windows_Server_2008 |
                                                OSID.Windows_7 | OSID.Windows_Server_2008_R2 |
                                                OSID.Windows_8 | OSID.Windows_Server_2008 | OSID.Windows_Server_2012 |
                                                OSID.WindowsPhone);
                            }
                            else
                            {
                                // If this is not NET Native, Win8 is the only other OS for which
                                // this hard-coded literal is used.
                                detectedID = OSID.Windows_8;
                            }    
                        }
                        else
                        {
                            // If the OSDescription is more than the hard-coded literal but we did not
                            // match the version that followed it, it means we have a version of Windows
                            // that did not exist at the time this code was written.  Flag the result as
                            // OSID.None to force infrastructure functional tests to fail for investigation.
                            detectedID = OSID.None;
                        }
                    }

                    return detectedID;
                }
            }

            return OSID.None;
        }
    }
}
