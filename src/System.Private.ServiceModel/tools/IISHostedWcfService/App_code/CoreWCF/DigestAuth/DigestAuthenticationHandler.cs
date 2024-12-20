#if NET
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace WcfService
{
    public class DigestAuthenticationHandler : AuthenticationHandler<DigestAuthenticationOptions>
    {
        public DigestAuthenticationHandler(IOptionsMonitor<DigestAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder) { }

        private const string DigestUsernameHeaderName = "DigestUsername";
        private const string DigestPasswordHeaderName = "DigestPassword";
        private const string DigestRealmHeaderName = "DigestRealm";
        private const string DigestAuthenticationMechanism = "Digest";
        private const string AuthenticationChallengeHeaderName = "WWW-Authenticate";
        private const string AuthorizationHeaderName = "Authorization";

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorizationHeader = Request.Headers[AuthorizationHeaderName].ToString();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith(DigestAuthenticationMechanism, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var digestState = new DigestAuthenticationState(authorizationHeader, GetRealm());
            string password;

            if (!GetPassword(digestState.Username, out password))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Username or Password"));
            }
            digestState.Password = password;

            if (!digestState.IsValid())
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Username or Password"));
            }

            var claimsPrincipal = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, digestState.Username) }, DigestAuthenticationDefaults.AuthenticationScheme));

            Context.User = claimsPrincipal;

            var ticket = new AuthenticationTicket(claimsPrincipal, DigestAuthenticationDefaults.AuthenticationScheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var nonce = Guid.NewGuid().ToString("N");
            var realm = GetRealm();

            var challenge = $"Digest realm=\"{realm}\", nonce=\"{nonce}\", opaque=\"0000000000000000\", stale=false, algorithm=MD5, qop=\"auth\"";

            Response.Headers[AuthenticationChallengeHeaderName] = challenge;
            Response.StatusCode = 401;

            return Task.CompletedTask;
        }

        public bool GetPassword(string username, out string password)
        {
            string sentUsername = Request.Headers[DigestUsernameHeaderName];
            if (username.Equals(sentUsername))
            {
                password = Request.Headers[DigestPasswordHeaderName];
                return true;
            }

            password = null;
            return false;
        }

        public string GetRealm()
        {
            return Request.Headers[DigestRealmHeaderName];
        }
    }
}
#endif
