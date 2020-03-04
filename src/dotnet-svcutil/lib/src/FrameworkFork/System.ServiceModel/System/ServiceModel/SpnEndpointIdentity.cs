// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel
{
    using System;
// Not needed in dotnet-svcutil scenario. 
//     using System.DirectoryServices;

    using System.IdentityModel.Claims;
    using Microsoft.Xml;

    public class SpnEndpointIdentity : EndpointIdentity
    {
        private static TimeSpan s_spnLookupTime = TimeSpan.FromMinutes(1);
// Not needed in dotnet-svcutil scenario. 
//         private SecurityIdentifier _spnSid;
// 
//         // Double-checked locking pattern requires volatile for read/write synchronization
//         private static volatile DirectoryEntry _directoryEntry;

        // Double-checked locking pattern requires volatile for read/write synchronization
        private volatile bool hasSpnSidBeenComputed;

        private Object _thisLock = new Object();

        private static Object s_typeLock = new Object();

        public static TimeSpan SpnLookupTime
        {
            get
            {
                return s_spnLookupTime;
            }
            set
            {
                if (value.Ticks < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value.Ticks, SRServiceModel.ValueMustBeNonNegative));
                }
                s_spnLookupTime = value;
            }
        }

        public SpnEndpointIdentity(string spnName)
        {
            if (spnName == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("spnName");

            base.Initialize(Claim.CreateSpnClaim(spnName));
        }

        public SpnEndpointIdentity(Claim identity)
        {
            if (identity == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("identity");

            // PreSharp Bug: Parameter 'identity.ResourceType' to this public method must be validated: A null-dereference can occur here.
#pragma warning disable 56506 // Claim.ClaimType will never return null
            if (!identity.ClaimType.Equals(ClaimTypes.Spn))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRServiceModel.Format(SRServiceModel.UnrecognizedClaimTypeForIdentity, identity.ClaimType, ClaimTypes.Spn));

            base.Initialize(identity);
        }

        internal override void WriteContentsTo(XmlDictionaryWriter writer)
        {
            if (writer == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");

            writer.WriteElementString(XD.AddressingDictionary.Spn, XD.AddressingDictionary.IdentityExtensionNamespace, (string)this.IdentityClaim.Resource);
        }
// Not needed in dotnet-svcutil scenario. 
//         internal SecurityIdentifier GetSpnSid()
//         {
//             Fx.Assert(ClaimTypes.Spn.Equals(this.IdentityClaim.ClaimType) || ClaimTypes.Dns.Equals(this.IdentityClaim.ClaimType), "");
//             if (!hasSpnSidBeenComputed)
//             {
//                 lock (thisLock)
//                 {
//                     if (!hasSpnSidBeenComputed)
//                     {
//                         string spn = null;
//                         try
//                         {
// 
//                             if (ClaimTypes.Dns.Equals(this.IdentityClaim.ClaimType))
//                             {
//                                 spn = "host/" + (string)this.IdentityClaim.Resource;
//                             }
//                             else
//                             {
//                                 spn = (string)this.IdentityClaim.Resource;
//                             }
//                             // canonicalize SPN for use in LDAP filter following RFC 1960:
//                             if (spn != null)
//                             {
//                                 spn = spn.Replace("*", @"\*").Replace("(", @"\(").Replace(")", @"\)");
//                             }
// 
//                             DirectoryEntry de = GetDirectoryEntry();
//                             using (DirectorySearcher searcher = new DirectorySearcher(de))
//                             {
//                                 searcher.CacheResults = true;
//                                 searcher.ClientTimeout = SpnLookupTime;
//                                 searcher.Filter = "(&(objectCategory=Computer)(objectClass=computer)(servicePrincipalName=" + spn + "))";
//                                 searcher.PropertiesToLoad.Add("objectSid");
//                                 SearchResult result = searcher.FindOne();
//                                 if (result != null)
//                                 {
//                                     byte[] sidBinaryForm = (byte[])result.Properties["objectSid"][0];
//                                     this._spnSid = new SecurityIdentifier(sidBinaryForm, 0);
//                                 }
//                                 else
//                                 {
//                                     SecurityTraceRecordHelper.TraceSpnToSidMappingFailure(spn, null);
//                                 }
//                             }
//                         }
// #pragma warning suppress 56500 // covered by FxCOP
//                         catch (Exception e)
//                         {
//                             // Always immediately rethrow fatal exceptions.
//                             if (Fx.IsFatal(e)) throw;
// 
//                             if (e is NullReferenceException || e is SEHException)
//                                 throw;
// 
//                             SecurityTraceRecordHelper.TraceSpnToSidMappingFailure(spn, e);
//                         }
//                         finally
//                         {
//                             hasSpnSidBeenComputed = true;
//                         }
//                     }
//                 }
//             }
//             return this._spnSid;
//         }
// 
//         static DirectoryEntry GetDirectoryEntry()
//         {
//             if (_directoryEntry == null)
//             {
//                 lock (typeLock)
//                 {
//                     if (_directoryEntry == null)
//                     {
//                         DirectoryEntry tmp = new DirectoryEntry(@"LDAP://" + SecurityUtils.GetPrimaryDomain());
//                         tmp.RefreshCache(new string[] { "name" });
//                         _directoryEntry = tmp;
//                     }
//                 }
//             }
//             return _directoryEntry;
//         }

    }
}
