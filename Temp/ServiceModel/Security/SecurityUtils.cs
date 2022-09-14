// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Net;
using System.Runtime;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.Text;

namespace System.ServiceModel.Security
{
    //internal static partial class SecurityUtils
    //{
    //    // Copied from TransportDefaults
    //    public const int MaxSecurityFaultSize = 16384;
    //    public const string Identities = "Identities";

    //    internal static void ThrowIfNegotiationFault(Message message, EndpointAddress target)
    //    {
    //        if (message.IsFault)
    //        {
    //            MessageFault fault = MessageFault.CreateFault(message, MaxSecurityFaultSize);
    //            Exception faultException = new FaultException(fault, message.Headers.Action);
    //            if (fault.Code != null && fault.Code.IsReceiverFault && fault.Code.SubCode != null)
    //            {
    //                FaultCode subCode = fault.Code.SubCode;
    //                if (subCode.Name == DotNetSecurityStrings.SecurityServerTooBusyFault && subCode.Namespace == DotNetSecurityStrings.Namespace)
    //                {
    //                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ServerTooBusyException(SR.Format(SR.SecurityServerTooBusy, target), faultException));
    //                }
    //                else if (subCode.Name == AddressingStrings.EndpointUnavailable && subCode.Namespace == message.Version.Addressing.Namespace)
    //                {
    //                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new EndpointNotFoundException(SR.Format(SR.SecurityEndpointNotFound, target), faultException));
    //                }
    //            }

    //            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(faultException);
    //        }
    //    }

    //    internal static void FixNetworkCredential(ref NetworkCredential credential)
    //    {
    //        if (credential == null)
    //        {
    //            return;
    //        }

    //        string username = credential.UserName;
    //        string domain = credential.Domain;
    //        if (!string.IsNullOrEmpty(username) && string.IsNullOrEmpty(domain))
    //        {
    //            // do the splitting only if there is exactly 1 \ or exactly 1 @
    //            string[] partsWithSlashDelimiter = username.Split('\\');
    //            string[] partsWithAtDelimiter = username.Split('@');
    //            if (partsWithSlashDelimiter.Length == 2 && partsWithAtDelimiter.Length == 1)
    //            {
    //                if (!string.IsNullOrEmpty(partsWithSlashDelimiter[0]) && !string.IsNullOrEmpty(partsWithSlashDelimiter[1]))
    //                {
    //                    credential = new NetworkCredential(partsWithSlashDelimiter[1], credential.Password, partsWithSlashDelimiter[0]);
    //                }
    //            }
    //            else if (partsWithSlashDelimiter.Length == 1 && partsWithAtDelimiter.Length == 2)
    //            {
    //                if (!string.IsNullOrEmpty(partsWithAtDelimiter[0]) && !string.IsNullOrEmpty(partsWithAtDelimiter[1]))
    //                {
    //                    credential = new NetworkCredential(partsWithAtDelimiter[0], credential.Password, partsWithAtDelimiter[1]);
    //                }
    //            }
    //        }
    //    }

    //    internal static Claim GetPrimaryIdentityClaim(AuthorizationContext authContext)
    //    {
    //        if (authContext != null)
    //        {
    //            for (int i = 0; i < authContext.ClaimSets.Count; ++i)
    //            {
    //                ClaimSet claimSet = authContext.ClaimSets[i];
    //                foreach (Claim claim in claimSet.FindClaims(null, Rights.Identity))
    //                {
    //                    return claim;
    //                }
    //            }
    //        }

    //        return null;
    //    }

    //    internal static string GetSpnFromTarget(EndpointAddress target)
    //    {
    //        if (target == null)
    //        {
    //            throw Fx.AssertAndThrow("target should not be null - expecting an EndpointAddress");
    //        }

    //        return string.Format(CultureInfo.InvariantCulture, "host/{0}", target.Uri.DnsSafeHost);
    //    }

    //    internal static string GetIdentityNamesFromContext(AuthorizationContext authContext)
    //    {
    //        if (authContext == null)
    //        {
    //            return string.Empty;
    //        }

    //        StringBuilder str = new StringBuilder(256);
    //        for (int i = 0; i < authContext.ClaimSets.Count; ++i)
    //        {
    //            ClaimSet claimSet = authContext.ClaimSets[i];

    //            // X509
    //            X509CertificateClaimSet x509 = claimSet as X509CertificateClaimSet;
    //            if (x509 != null)
    //            {
    //                if (str.Length > 0)
    //                {
    //                    str.Append(", ");
    //                }

    //                AppendCertificateIdentityName(str, x509.X509Certificate);
    //            }
    //        }

    //        if (str.Length <= 0)
    //        {
    //            List<IIdentity> identities = null;
    //            object obj;
    //            if (authContext.Properties.TryGetValue(SecurityUtils.Identities, out obj))
    //            {
    //                identities = obj as List<IIdentity>;
    //            }
    //            if (identities != null)
    //            {
    //                for (int i = 0; i < identities.Count; ++i)
    //                {
    //                    IIdentity identity = identities[i];
    //                    if (identity != null)
    //                    {
    //                        if (str.Length > 0)
    //                        {
    //                            str.Append(", ");
    //                        }

    //                        AppendIdentityName(str, identity);
    //                    }
    //                }
    //            }
    //        }

    //        return str.Length <= 0 ? string.Empty : str.ToString();
    //    }

    //    internal static void AppendIdentityName(StringBuilder str, IIdentity identity)
    //    {
    //        string name = null;
    //        try
    //        {
    //            name = identity.Name;
    //        }
    //        catch (Exception e)
    //        {
    //            if (Fx.IsFatal(e))
    //            {
    //                throw;
    //            }
    //            // suppress exception, this is just info.
    //        }

    //        str.Append(string.IsNullOrEmpty(name) ? "<null>" : name);
    //        WindowsIdentity windows = identity as WindowsIdentity;
    //        if (windows != null)
    //        {
    //            if (windows.User != null)
    //            {
    //                str.Append("; ");
    //                str.Append(windows.User.ToString());
    //            }
    //        }
    //    }

    //    internal static void AppendCertificateIdentityName(StringBuilder str, X509Certificate2 certificate)
    //    {
    //        string value = certificate.SubjectName.Name;
    //        if (string.IsNullOrEmpty(value))
    //        {
    //            value = certificate.GetNameInfo(X509NameType.DnsName, false);
    //            if (string.IsNullOrEmpty(value))
    //            {
    //                value = certificate.GetNameInfo(X509NameType.SimpleName, false);
    //                if (string.IsNullOrEmpty(value))
    //                {
    //                    value = certificate.GetNameInfo(X509NameType.EmailName, false);
    //                    if (string.IsNullOrEmpty(value))
    //                    {
    //                        value = certificate.GetNameInfo(X509NameType.UpnName, false);
    //                    }
    //                }
    //            }
    //        }
    //        // Same format as X509Identity
    //        str.Append(string.IsNullOrEmpty(value) ? "<x509>" : value);
    //        str.Append("; ");
    //        str.Append(certificate.Thumbprint);
    //    }

    //    public static DateTime MaxUtcDateTime
    //    {
    //        get
    //        {
    //            // + and -  TimeSpan.TicksPerDay is to compensate the DateTime.ParseExact (to localtime) overflow.
    //            return new DateTime(DateTime.MaxValue.Ticks - TimeSpan.TicksPerDay, DateTimeKind.Utc);
    //        }
    //    }
    //}

    //internal static class SslProtocolsHelper
    //{
    //    internal static bool IsDefined(SslProtocols value)
    //    {
    //        SslProtocols allValues = SslProtocols.None;
    //        foreach (var protocol in Enum.GetValues(typeof(SslProtocols)))
    //        {
    //            allValues |= (SslProtocols)protocol;
    //        }

    //        return (value & allValues) == value;
    //    }

    //    internal static void Validate(SslProtocols value)
    //    {
    //        if (!IsDefined(value))
    //        {
    //            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException(nameof(value), (int)value,
    //                typeof(SslProtocols)));
    //        }
    //    }
    //}
}
