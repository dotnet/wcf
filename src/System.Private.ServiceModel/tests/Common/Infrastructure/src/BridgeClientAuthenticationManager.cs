// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 

using System;
using System.Net;

namespace Infrastructure.Common
{
    public static class BridgeClientAuthenticationManager
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
