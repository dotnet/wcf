// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    public enum InstanceContextMode
    {
        PerSession,
        PerCall,
        Single,
    }

    internal static class InstanceContextModeHelper
    {
        static public bool IsDefined(InstanceContextMode x)
        {
            return
                x == InstanceContextMode.PerCall ||
                x == InstanceContextMode.PerSession ||
                x == InstanceContextMode.Single ||
                false;
        }
    }
}
