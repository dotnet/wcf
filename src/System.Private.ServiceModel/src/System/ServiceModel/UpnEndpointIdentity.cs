// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using System.Security.Principal;
using System.IdentityModel.Claims;
using System.Runtime;
using System.Runtime.InteropServices;
using System.ServiceModel.Diagnostics;
using System.Text;
using System.Xml;

namespace System.ServiceModel
{
    public class UpnEndpointIdentity : EndpointIdentity
    {
#if SUPPORTS_WINDOWSIDENTITY
#pragma warning disable 0414 // We don't use this yet in the initial stubbing - remove this once we have references again. 
        private SecurityIdentifier _upnSid;
        private bool _hasUpnSidBeenComputed;
        private WindowsIdentity _windowsIdentity;
#pragma warning restore 0414
#endif // SUPPORTS_WINDOWSIDENTITY

        private Object _thisLock = new Object();

        public UpnEndpointIdentity(string upnName)
        {
            if (upnName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(upnName));
            }

            Initialize(Claim.CreateUpnClaim(upnName));
        }

        public UpnEndpointIdentity(Claim identity)
        {
            if (identity == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(identity));

            if (!identity.ClaimType.Equals(ClaimTypes.Upn))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.Format(SR.UnrecognizedClaimTypeForIdentity, identity.ClaimType, ClaimTypes.Upn));

            Initialize(identity);
        }

#if SUPPORTS_WINDOWSIDENTITY
        internal UpnEndpointIdentity(WindowsIdentity windowsIdentity)
        {
            if (windowsIdentity == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("windowsIdentity");

            _windowsIdentity = windowsIdentity;
            _upnSid = windowsIdentity.User;
            _hasUpnSidBeenComputed = true;
        }

        internal override void EnsureIdentityClaim()
        {
            if (_windowsIdentity != null)
            {
                lock (_thisLock)
                {
                    if (_windowsIdentity != null)
                    {
                        Initialize(Claim.CreateUpnClaim(GetUpnFromWindowsIdentity(_windowsIdentity)));
                        _windowsIdentity.Dispose();
                        _windowsIdentity = null;
                    }
                }
            }
        }

        string GetUpnFromWindowsIdentity(WindowsIdentity windowsIdentity)
        {
            string downlevelName = null;
            string upnName = null;
 
            try
            {
                downlevelName = windowsIdentity.Name;
 
                if (IsMachineJoinedToDomain())
                {
                    upnName = GetUpnFromDownlevelName(downlevelName);
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
             }
 
            // if the AD cannot be queried for the fully qualified domain name,
            // fall back to the downlevel UPN name
            return upnName ?? downlevelName;
        }

        bool IsMachineJoinedToDomain()
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        string GetUpnFromDownlevelName(string downlevelName)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
#endif // SUPPORTS_WINDOWSIDENTITY

        internal override void WriteContentsTo(XmlDictionaryWriter writer)
        {
            if (writer == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(writer));

            writer.WriteElementString(XD.AddressingDictionary.Upn, XD.AddressingDictionary.IdentityExtensionNamespace, (string)this.IdentityClaim.Resource);
        }

#if SUPPORTS_WINDOWSIDENTITY
        internal SecurityIdentifier GetUpnSid()
        {
            Fx.Assert(ClaimTypes.Upn.Equals(this.IdentityClaim.ClaimType), "");
            if (!_hasUpnSidBeenComputed)
            {
                lock (_thisLock)
                {
                    string upn = (string)this.IdentityClaim.Resource;
                    if (!_hasUpnSidBeenComputed)
                    {
                        try
                        {
                            NTAccount userAccount = new NTAccount(upn);
                            _upnSid = userAccount.Translate(typeof(SecurityIdentifier)) as SecurityIdentifier;
                        }
#pragma warning suppress 56500 // covered by FxCOP
                        catch (Exception e)
                        {
                            // Always immediately rethrow fatal exceptions.
                            if (Fx.IsFatal(e))
                            {
                                throw;
                            }

                            if (e is NullReferenceException)
                            {
                                throw;
                            }

                            SecurityTraceRecordHelper.TraceSpnToSidMappingFailure(upn, e);
                        }
                        finally
                        {
                            _hasUpnSidBeenComputed = true;
                        }
                    }
                }
            }
            return _upnSid;
        }
#endif
    }
}
