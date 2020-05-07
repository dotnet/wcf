// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    using System;
    internal static class AppSettings
    {
        internal static bool EnableSoapEncoding = false;
        internal static bool EnableMessageHeader = false;

        internal static void Initialize(FrameworkInfo frameworkInfo)
        {
            // NOTE: this method assumes the passed in FrameworkInfo has been validated.

            if (!frameworkInfo.IsDnx || frameworkInfo.Version >= new Version("2.0"))
            {
                EnableSoapEncoding = true;
                EnableMessageHeader = true;
            }
        }
    }
}
