// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Linq;
using System.ServiceModel;

namespace WcfService
{
    public static class AuthenticationResourceHelper
    {
        private static string s_username;
        private static string s_password;
        private static string s_digestrealm;
        public const string usernameKeyName = "authUsername";
        public const string passwordKeyName = "authPassword";
        public const string digestRealmKeyName = "authDigestRealm";

        static AuthenticationResourceHelper()
        {
            s_username = RandomString(10);
            s_password = RandomString(20);
            s_digestrealm = RandomString(5);
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string Username { get { return s_username; } }
        public static string Password { get { return s_password; } }
        public static string DigestRealm { get { return s_digestrealm; } }

        public static void ConfigureServiceHostUseDigestAuth(ServiceHost serviceHost)
        {
            var authManager = new ResourceDigestServiceAuthorizationManager(s_digestrealm);
            serviceHost.Description.Behaviors.Add(authManager);
        }

        private class ResourceDigestServiceAuthorizationManager : DigestServiceAuthorizationManager
        {
            public ResourceDigestServiceAuthorizationManager(string realm) : base(realm) { }
            public override bool GetPassword(string username, out string password)
            {
                if (username.Equals(s_username))
                {
                    password = s_password;
                    return true;
                }

                password = null;
                return false;
            }
        }
    }
}
