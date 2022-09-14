// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Security.Tokens
{
    public static class ServiceModelSecurityTokenTypes
    {
        private const string Namespace = "http://schemas.microsoft.com/ws/2006/05/servicemodel/tokens";
        public static string Spnego => Namespace + "/Spnego";
        public static string MutualSslnego => Namespace + "/MutualSslnego";
        public static string AnonymousSslnego => Namespace + "/AnonymousSslnego";
        public static string SecurityContext => Namespace + "/SecurityContextToken";
        public static string SecureConversation => Namespace + "/SecureConversation";
        public static string SspiCredential => Namespace + "/SspiCredential";
    }
}
