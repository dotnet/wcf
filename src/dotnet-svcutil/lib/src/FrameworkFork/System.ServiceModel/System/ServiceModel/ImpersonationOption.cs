// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
