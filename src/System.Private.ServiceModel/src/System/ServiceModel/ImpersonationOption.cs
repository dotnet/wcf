// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
