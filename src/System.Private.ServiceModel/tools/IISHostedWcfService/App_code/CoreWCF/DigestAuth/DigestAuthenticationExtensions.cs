#if NET
using Microsoft.AspNetCore.Authentication;

namespace WcfService
{
    public static class DigestAuthenticationAppBuilderExtensions
    {
        public static AuthenticationBuilder AddDigest(this AuthenticationBuilder builder)
            => builder.AddDigest(DigestAuthenticationDefaults.AuthenticationScheme);

        public static AuthenticationBuilder AddDigest(this AuthenticationBuilder builder, string authenticationScheme)
            => builder.AddDigest(authenticationScheme, configureOptions: null);

        public static AuthenticationBuilder AddDigest(this AuthenticationBuilder builder, Action<DigestAuthenticationOptions> configureOptions)
            => builder.AddDigest(DigestAuthenticationDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddDigest(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            Action<DigestAuthenticationOptions> configureOptions)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddScheme<DigestAuthenticationOptions, DigestAuthenticationHandler>(authenticationScheme, configureOptions);
        }
    }
}
#endif
