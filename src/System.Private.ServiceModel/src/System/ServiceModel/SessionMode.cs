// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel
{
    public enum SessionMode
    {
        Allowed,
        Required,
        NotAllowed,
    }

    internal static class SessionModeHelper
    {
        public static bool IsDefined(SessionMode sessionMode)
        {
            return (sessionMode == SessionMode.NotAllowed ||
                    sessionMode == SessionMode.Allowed ||
                    sessionMode == SessionMode.Required);
        }
    }
}
