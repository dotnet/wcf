// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    public enum ImpersonationOption
    {
        NotAllowed,
        Allowed,
        Required,
    }

    internal static class ImpersonationOptionHelper
    {
        public static bool IsDefined(ImpersonationOption option)
        {
            return (option == ImpersonationOption.NotAllowed ||
                    option == ImpersonationOption.Allowed ||
                    option == ImpersonationOption.Required);
        }

        internal static bool AllowedOrRequired(ImpersonationOption option)
        {
            return (option == ImpersonationOption.Allowed ||
                    option == ImpersonationOption.Required);
        }
    }
}
