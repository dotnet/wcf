// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    // Minimal subset of JsonGlobals constants used by the Web HTTP encoder factories.
    // Mirrors CoreWCF.Runtime.JsonGlobals.
    internal static class JsonGlobals
    {
        public const string ApplicationJsonMediaType = "application/json";
        public const string TextJsonMediaType = "text/json";
    }
}
