// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System
{
    internal partial class SRServiceModel
    {
        internal static string Format(string resourceFormat, params object[] args)
        {
            if (args != null)
            {
                return string.Format(resourceFormat, args);
            }

            return resourceFormat;
        }
    }
}
