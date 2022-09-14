// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


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
