// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using System.Security.Cryptography;
using System.Text;

namespace WcfService
{
    public class DigestAuthenticationState
    {
        private const string DigestAuthenticationMechanism = "Digest";
        private const int DigestAuthenticationMechanismLength = 7; // DigestAuthenticationMechanism.Length;
        private const string UriAuthenticationParameter = "uri";
        private const string UsernameAuthenticationParameter = "username";
        private const string NonceAuthenticationParameter = "nonce";
        private const string ResponseAuthenticationParameter = "response";
        private readonly string _authorizationHeader;
        private readonly Dictionary<string, string> _authorizationParameters;
        private readonly string _method;
        private readonly string _realm;
        private readonly bool _useStrictUsername;
        private string _nonceString;
        private string _password;
        private bool _authorized;

        public DigestAuthenticationState(string authorizationHeader, string realm)
        {
            _useStrictUsername = true;
            _method = "POST";// Request.Method;
            _realm = realm;
            _password = null;
            _authorized = new bool();
            _authorizationHeader = authorizationHeader;
            if (!_authorizationHeader.StartsWith(DigestAuthenticationMechanism))
            {
                _authorized = false;
                _nonceString = string.Empty;
                _authorizationParameters = new Dictionary<string, string>();
                return;
            }

            string[] digestElements = _authorizationHeader.Substring(DigestAuthenticationMechanismLength).Split(',');
            _authorizationParameters = new Dictionary<string, string>(digestElements.Length);
            foreach (string element in digestElements)
            {
                string[] keyValue = element.Split(new[] { '=' }, 2);
                string key = keyValue[0].Trim(' ', '\"');
                string value = keyValue[1].Trim(' ', '\"');
                _authorizationParameters.Add(key, value);
            }

            _nonceString = _authorizationParameters[NonceAuthenticationParameter];
        }

        public string Nonce { get { return _nonceString; } }

        public Dictionary<string, string> Parameters { get { return _authorizationParameters; } }

        public string Password { set { _password = value; } }

        public string Uri { get { return Parameters[UriAuthenticationParameter]; } }

        public string Username
        {
            get
            {
                var username = Parameters[UsernameAuthenticationParameter];

                if (!_useStrictUsername)
                {
                    // On some platforms, the username is sent as realm\\username or realm\username. This should only happen when
                    // the user account is in a different realm/domain than the server but some implementations of HttpClient
                    // always send the username prefixed with the realm.

                    username = username.Replace(@"\\", @"\");
                    var realmPrefix = _realm + @"\";
                    if (username.StartsWith(realmPrefix))
                    {
                        username = username.Substring(realmPrefix.Length);
                    }
                }

                return username;
            }
        }

        private string RawUsername { get { return Parameters[UsernameAuthenticationParameter]; } }

        private string CalculateHash(string plaintext)
        {
            Encoding enc = Encoding.ASCII;
            MD5 md5 = MD5.Create();
            byte[] hashbytes = md5.ComputeHash(enc.GetBytes(plaintext));
            var hashStringBuilder = new StringBuilder();
            for (int i = 0; i < 16; i++)
            {
                hashStringBuilder.AppendFormat("{0:x02}", hashbytes[i]);
            }

            return hashStringBuilder.ToString();
        }

        public bool IsValid()
        {
            if (_realm == null || RawUsername == null || _password == null || _method == null)
            {
                throw new InvalidOperationException("Insufficient information to determine authorization");
            }

            // If username is realm\\username, convert the double slash to a single slash
            var username = RawUsername.Replace(@"\\", @"\");

            string SA1 = string.Concat(username, ":", _realm, ":", _password);
            string HA1 = CalculateHash(SA1);
            string SA2 = string.Concat(_method, ":", Uri);
            string HA2 = CalculateHash(SA2);

            string responseString;
            if (Parameters["qop"] != null)
            {
                responseString = string.Concat(
                    HA1, ":",
                    Nonce, ":",
                    Parameters["nc"], ":",
                    Parameters["cnonce"], ":",
                    Parameters["qop"], ":",
                    HA2);
            }
            else
            {
                responseString = string.Concat(HA1, ":", Nonce, ":", HA2);
            }

            string responseDigest = CalculateHash(responseString);
            _authorized = (Parameters[ResponseAuthenticationParameter] == responseDigest);
            return _authorized;
        }
    }
}
#endif
