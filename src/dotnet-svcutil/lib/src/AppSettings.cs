//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
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
