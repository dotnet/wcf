// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.IdentityModel.Claims
{
    public static class Rights
    {
        private const string rightNamespace = XsiConstants.Namespace + "/right";

        private const string identity = rightNamespace + "/identity";
        private const string possessProperty = rightNamespace + "/possessproperty";

        static public string Identity { get { return identity; } }
        static public string PossessProperty { get { return possessProperty; } }
    }
}
