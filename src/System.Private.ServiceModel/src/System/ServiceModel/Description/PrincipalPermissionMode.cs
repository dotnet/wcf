// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
