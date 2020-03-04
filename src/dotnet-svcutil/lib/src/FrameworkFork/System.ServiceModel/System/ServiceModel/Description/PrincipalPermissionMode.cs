// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Description
{
    public enum PrincipalPermissionMode
    {
        None,
        UseWindowsGroups,
        UseAspNetRoles,
        Custom,
        Always
    }

    internal static class PrincipalPermissionModeHelper
    {
        public static bool IsDefined(PrincipalPermissionMode principalPermissionMode)
        {
            return Enum.IsDefined(typeof(PrincipalPermissionMode), principalPermissionMode);
        }
    }
}
