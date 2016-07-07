// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// 

using System;
using System.Net;

namespace Infrastructure.Common
{
    internal static class BridgeClientAuthenticationManager
    {
        private const string AuthenticationResourceName = "WcfService.TestResources.AuthenticationResource";
        private const string UsernameKeyName = "authUsername";
        private const string PasswordKeyName = "authPassword";
        private static readonly object s_authLock = new object();
        private static NetworkCredential s_networkCredential;

        public static NetworkCredential NetworkCredential
        {
            get
            {
                if (s_networkCredential == null)
                {
                    lock (s_authLock)
                    {
                        if (s_networkCredential == null)
                        {
                            var response = BridgeClient.MakeResourceGetRequest(AuthenticationResourceName, null);
                            var credential = new NetworkCredential();
                            credential.UserName = response[UsernameKeyName];
                            credential.Password = response[PasswordKeyName];
                            if (string.IsNullOrEmpty(credential.UserName))
                            {
                                throw new NullReferenceException(UsernameKeyName);
                            }

                            if (string.IsNullOrEmpty(credential.Password))
                            {
                                throw new NullReferenceException(PasswordKeyName);
                            }

                            s_networkCredential = credential;
                        }
                    }
                }

                return s_networkCredential;
            }
        }
    }
}
