// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace Infrastructure.Common
{
    // Helper class for FrameworkID, an enum created to match
    // RuntimeInformation.FrameworkDescription
    public static class FrameworkHelper
    {
        // These literals must match those returned from RuntimeInformation.FrameworkDescription.
        private const string NetNativeFrameworkName = ".NET Native";
        private const string NetFrameworkFrameworkName = ".NET Framework";
        private const string NetCoreFrameworkName = ".NET Core";
        private const string Net5PlusFrameworkName = ".NET ";

        private static bool _detectedFrameworkID = false;
        private static FrameworkID _currentFrameworkID = 0;
        private static string _currentFrameworkDescription;

        private static string CurrentFrameworkDescription
        {
            get
            {
                if (_currentFrameworkDescription == null)
                {
                    _currentFrameworkDescription = RuntimeInformation.FrameworkDescription;
                }

                return _currentFrameworkDescription;
            }
        }

        public static FrameworkID Current
        {
            get
            {
                if (!_detectedFrameworkID)
                {
                    _currentFrameworkID = DetectCurrentFramework();
                    _detectedFrameworkID = true;

                    // Log this to the console so that lab run artifacts will show what we detected.
                    Console.WriteLine(String.Format("Detected current FrameworkID as \"{0}\" from description \"{1}\"",
                                     _currentFrameworkID.Name(), CurrentFrameworkDescription));
                }

                return _currentFrameworkID;
            }
        }

        // Extension method that returns 'true' if the given FrameworkID
        // includes the FrameworkID on which this code is currently executing.
        public static bool MatchesCurrent(this FrameworkID frameworkID)
        {
            return ((Current & frameworkID) != 0);
        }

        // Extension method for FrameworkID to provide name.
        // "G" formatting will expand to enum's name or collection if multiple.
        public static string Name(this FrameworkID frameworkID)
        {
            return frameworkID.ToString("G");
        }

        // Detects the FrameworkID associated with the framework
        // on which this code is running.
        private static FrameworkID DetectCurrentFramework()
        {
            string frameworkName = CurrentFrameworkDescription;
            if (frameworkName.IndexOf(NetNativeFrameworkName, StringComparison.Ordinal) >= 0)
            {
                return FrameworkID.NetNative;
            }

            if (frameworkName.IndexOf(NetFrameworkFrameworkName, StringComparison.Ordinal) >= 0)
            {
                return FrameworkID.NetFramework;
            }

            if (frameworkName.IndexOf(NetCoreFrameworkName, StringComparison.Ordinal) >= 0)
            {
                return FrameworkID.NetCore;
            }

            if (frameworkName.IndexOf(Net5PlusFrameworkName, StringComparison.Ordinal) >= 0
                && Char.IsDigit(frameworkName[5]))
            {
                return FrameworkID.NetCore;
            }

            // We use "None" when we cannot determine the FrameworkID and rely on
            // tests to verify detection succeeded.
            return FrameworkID.None;
        }
    }
}
