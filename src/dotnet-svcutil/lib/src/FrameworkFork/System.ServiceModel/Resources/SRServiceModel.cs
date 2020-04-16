// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
