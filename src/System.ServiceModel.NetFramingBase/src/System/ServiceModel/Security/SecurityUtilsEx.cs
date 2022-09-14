// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel.Security
{
    internal class SecurityUtilsEx
    {
        private static ClaimSet s_anonymousClaimSet;

        internal static ClaimSet AnonymousClaimSet
        {
            get
            {
                if (s_anonymousClaimSet == null)
                {
                    s_anonymousClaimSet = new DefaultClaimSet();
                }

                return s_anonymousClaimSet;
            }
        }

        internal static void AbortTokenProviderIfRequired(SecurityTokenProvider tokenProvider)
        {
            AbortCommunicationObject(tokenProvider);
        }

        internal static Task CloseTokenProviderIfRequiredAsync(SecurityTokenProvider tokenProvider, TimeSpan timeout)
        {
            return CloseCommunicationObjectAsync(tokenProvider, timeout);
        }

        internal static Task OpenTokenProviderIfRequiredAsync(SecurityTokenProvider tokenProvider, TimeSpan timeout)
        {
            return OpenCommunicationObjectAsync(tokenProvider as ICommunicationObject, timeout);
        }

        internal static void AbortTokenAuthenticatorIfRequired(SecurityTokenAuthenticator tokenAuthenticator)
        {
            AbortCommunicationObject(tokenAuthenticator);
        }

        internal static Task CloseTokenAuthenticatorIfRequiredAsync(SecurityTokenAuthenticator tokenAuthenticator, TimeSpan timeout)
        {
            return CloseCommunicationObjectAsync(tokenAuthenticator, timeout);
        }

        internal static Task OpenTokenAuthenticatorIfRequiredAsync(SecurityTokenAuthenticator tokenAuthenticator, TimeSpan timeout)
        {
            return OpenCommunicationObjectAsync(tokenAuthenticator as ICommunicationObject, timeout);
        }

        private static void AbortCommunicationObject(object obj)
        {
            if (obj != null)
            {
                ICommunicationObject co = obj as ICommunicationObject;
                if (co != null)
                {
                    try
                    {
                        co.Abort();
                    }
                    catch (CommunicationException)
                    {
                    }
                }
                else if (obj is IDisposable)
                {
                    ((IDisposable)obj).Dispose();
                }
            }
        }

        private static Task CloseCommunicationObjectAsync(object obj, TimeSpan timeout)
        {
            if (obj != null)
            {
                ICommunicationObject co = obj as ICommunicationObject;
                if (co != null)
                {
                    return Task.Factory.FromAsync(co.BeginClose, co.EndClose, timeout, null, TaskCreationOptions.None);
                }
                else if (obj is IDisposable)
                {
                    ((IDisposable)obj).Dispose();
                }
            }

            return Task.CompletedTask;
        }

        private static Task OpenCommunicationObjectAsync(ICommunicationObject obj, TimeSpan timeout)
        {
            if (obj != null)
            {
                return Task.Factory.FromAsync(obj.BeginOpen, obj.EndOpen, timeout, null);
            }

            return Task.CompletedTask;
        }

        internal static string GetIdentityNamesFromContext(AuthorizationContext authContext)
        {
            if (authContext == null)
                return String.Empty;

            StringBuilder str = new StringBuilder(256);
            for (int i = 0; i < authContext.ClaimSets.Count; ++i)
            {
                ClaimSet claimSet = authContext.ClaimSets[i];

                // X509
                X509CertificateClaimSet x509 = claimSet as X509CertificateClaimSet;
                if (x509 != null)
                {
                    if (str.Length > 0)
                        str.Append(", ");

                    IdentityModel.SecurityUtils.AppendCertificateIdentityName(str, x509.X509Certificate);
                }
            }

            if (str.Length <= 0)
            {
                List<IIdentity> identities = null;
                object obj;
                if (authContext.Properties.TryGetValue(SecurityUtils.Identities, out obj))
                {
                    identities = obj as List<IIdentity>;
                }
                if (identities != null)
                {
                    for (int i = 0; i < identities.Count; ++i)
                    {
                        IIdentity identity = identities[i];
                        if (identity != null)
                        {
                            if (str.Length > 0)
                                str.Append(", ");

                            AppendIdentityName(str, identity);
                        }
                    }
                }
            }
            return str.Length <= 0 ? String.Empty : str.ToString();
        }

        internal static void AppendIdentityName(StringBuilder str, IIdentity identity)
        {
            string name = null;
            try
            {
                name = identity.Name;
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                // suppress exception, this is just info.
            }

            str.Append(string.IsNullOrEmpty(name) ? "<null>" : name);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WindowsIdentity windows = identity as WindowsIdentity;
                if (windows != null)
                {
                    if (windows.User != null)
                    {
                        str.Append("; ");
                        str.Append(windows.User.ToString());
                    }
                }
            }
        }

        //internal static ReadOnlyCollection<IAuthorizationPolicy> CreatePrincipalNameAuthorizationPolicies(string principalName)
        //{
        //    if (principalName == null)
        //    {
        //        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(principalName));
        //    }

        //    Claim identityClaim;
        //    Claim primaryPrincipal;
        //    if (principalName.Contains("@") || principalName.Contains(@"\"))
        //    {
        //        identityClaim = new Claim(ClaimTypes.Upn, principalName, Rights.Identity);
        //        primaryPrincipal = Claim.CreateUpnClaim(principalName);
        //    }
        //    else
        //    {
        //        identityClaim = new Claim(ClaimTypes.Spn, principalName, Rights.Identity);
        //        primaryPrincipal = Claim.CreateSpnClaim(principalName);
        //    }

        //    List<Claim> claims = new List<Claim>(2);
        //    claims.Add(identityClaim);
        //    claims.Add(primaryPrincipal);

        //    List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>(1);
        //    policies.Add(new UnconditionalPolicy(SecurityUtils.CreateIdentity(principalName), new DefaultClaimSet(AnonymousClaimSet, claims)));
        //    return policies.AsReadOnly();
        //}

        //internal static string GetSpnFromIdentity(EndpointIdentity identity, EndpointAddress target)
        //{
        //    bool foundSpn = false;
        //    string spn = null;
        //    if (identity != null)
        //    {
        //        if (ClaimTypes.Spn.Equals(identity.IdentityClaim.ClaimType))
        //        {
        //            spn = (string)identity.IdentityClaim.Resource;
        //            foundSpn = true;
        //        }
        //        else if (ClaimTypes.Upn.Equals(identity.IdentityClaim.ClaimType))
        //        {
        //            spn = (string)identity.IdentityClaim.Resource;
        //            foundSpn = true;
        //        }
        //        else if (ClaimTypes.Dns.Equals(identity.IdentityClaim.ClaimType))
        //        {
        //            spn = string.Format(CultureInfo.InvariantCulture, "host/{0}", (string)identity.IdentityClaim.Resource);
        //            foundSpn = true;
        //        }
        //    }

        //    if (!foundSpn)
        //    {
        //        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SR.Format(SR.CannotDetermineSPNBasedOnAddress, target)));
        //    }

        //    return spn;
        //}

        //internal static IIdentity CreateIdentity(string name)
        //{
        //    return new GenericIdentity(name);
        //}

        //internal static class NetworkCredentialHelper
        //{
        //    //private static string s_currentUser = string.Empty;
        //    //private const string DefaultCurrentUser = "____CURRENTUSER_NOT_AVAILABLE____";
        //    //static internal bool IsNullOrEmpty(NetworkCredential credential)
        //    //{
        //    //    return credential == null ||
        //    //            (
        //    //                string.IsNullOrEmpty(credential.UserName) &&
        //    //                string.IsNullOrEmpty(credential.Domain) &&
        //    //                string.IsNullOrEmpty(credential.Password)
        //    //            );
        //    //}

        //    static internal bool IsDefault(NetworkCredential credential)
        //    {
        //        return CredentialCache.DefaultNetworkCredentials.Equals(credential);
        //    }

        //internal static string GetCurrentUserIdAsString(NetworkCredential credential)
        //{
        //    if (!string.IsNullOrEmpty(s_currentUser))
        //    {
        //        return s_currentUser;
        //    }

        //    // CurrentUser could be set muliple times
        //    // This is fine because it does not affect the value returned.
        //    try
        //    {
        //        using (WindowsIdentity self = WindowsIdentity.GetCurrent())
        //        {
        //            s_currentUser = self.User.Value;
        //        }
        //    }
        //    catch (PlatformNotSupportedException)
        //    {
        //        //WindowsIdentity is not supported on *NIX
        //        //so returning a username which is very unlikely to be a real username;
        //        s_currentUser = DefaultCurrentUser;
        //    }

        //    return s_currentUser;
        //}
    }
}

