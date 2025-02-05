#if NET
using Microsoft.AspNetCore.Authentication;

namespace WcfService
{
    public class DigestAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Realm { get; set; } = "realm";
    }
}
#endif
