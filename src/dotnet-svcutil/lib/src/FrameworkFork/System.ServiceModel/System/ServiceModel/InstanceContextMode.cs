// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
