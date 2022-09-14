// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.IdentityModel.Claims
{
    internal static class Rights
    {
        private const string rightNamespace = XsiConstants.Namespace + "/right";

        private const string identity = rightNamespace + "/identity";
        private const string possessProperty = rightNamespace + "/possessproperty";

        static public string Identity { get { return identity; } }
        static public string PossessProperty { get { return possessProperty; } }
    }

    internal static class XsiConstants
    {
        public const string Namespace = "http://schemas.xmlsoap.org/ws/2005/05/identity";
        public const string System = "System";
    }
}
