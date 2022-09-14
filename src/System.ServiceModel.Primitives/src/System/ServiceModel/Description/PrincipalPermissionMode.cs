// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


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
