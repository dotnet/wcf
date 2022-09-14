// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;

namespace System.IdentityModel.Claims
{
    // Examples:
    //    ClaimType        ResourceValue     ResourceRight
    //    ---------------  ----------------  ------------------
    //    "File"           "boot.ini"        "Read"
    //    "HairColor"      "Brown"           "PossessProperty"
    //    "UserName"       "Mary"            "PossessProperty"
    //    "Service"        "MailService"     "Access"
    //    "Operation"      "ReadMail"        "Invoke"
    // ClaimType:
    //    DESC: The type of resource for which rights are granted
    //    XrML: ClaimSet/Resource
    //    SAML: SamlAttributeStatement/Attribute/@Name/..., SamlAuthorizationDecisionStatement/Action/@Namespace/...
    // ResourceValue:
    //    DESC: Value identifying the resource for which rights are granted
    //    XrML: ClaimSet/Resource/...
    //    SAML: SamlAttributeStatement/Attribute/..., SamlAuthorizationDecisionStatement/@Resource/...
    // Right:
    //    DESC: Rights expressed about a resource
    //    XRML: ClaimSet/Right
    //    SAML: SamlAttributeStatement (aka. "PossessProperty") or, SamlAuthorizationDecisionStatement/Action/...

    [DataContract(Namespace = XsiConstants.Namespace)]
    public class Claim
    {
        private static Claim s_system;

        [DataMember(Name = "ClaimType")]
        private string _claimType;
        [DataMember(Name = "Resource")]
        private object _resource;
        [DataMember(Name = "Right")]
        private string _right;

        private IEqualityComparer<Claim> _comparer;

        private Claim(string claimType, object resource, string right, IEqualityComparer<Claim> comparer)
        {
            if (claimType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(claimType));
            }

            if (claimType.Length <= 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("claimType", SRP.ArgumentCannotBeEmptyString);
            }

            if (right == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(right));
            }

            if (right.Length <= 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("right", SRP.ArgumentCannotBeEmptyString);
            }

            _claimType = claimType;
            _resource = resource;
            _right = right;
            _comparer = comparer;
        }

        public Claim(string claimType, object resource, string right) : this(claimType, resource, right, null)
        {
        }

        public static IEqualityComparer<Claim> DefaultComparer
        {
            get
            {
                return EqualityComparer<Claim>.Default;
            }
        }

        public static Claim System
        {
            get
            {
                if (s_system == null)
                {
                    s_system = new Claim(ClaimTypes.System, XsiConstants.System, Rights.Identity);
                }

                return s_system;
            }
        }

        public object Resource
        {
            get { return _resource; }
        }

        public string ClaimType
        {
            get { return _claimType; }
        }

        public string Right
        {
            get { return _right; }
        }

        // Turn key claims
        public static Claim CreateDnsClaim(string dns)
        {
            if (dns == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(dns));
            }

            return new Claim(ClaimTypes.Dns, dns, Rights.PossessProperty, ClaimComparer.Dns);
        }

        public static Claim CreateHashClaim(byte[] hash)
        {
            if (hash == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(hash));
            }

            return new Claim(ClaimTypes.Hash, SecurityUtils.CloneBuffer(hash), Rights.PossessProperty, ClaimComparer.Hash);
        }


        public static Claim CreateNameClaim(string name)
        {
            if (name == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(name));
            }

            return new Claim(ClaimTypes.Name, name, Rights.PossessProperty);
        }

        public static Claim CreateRsaClaim(RSA rsa)
        {
            if (rsa == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(rsa));

            return new Claim(ClaimTypes.Rsa, rsa, Rights.PossessProperty, ClaimComparer.Rsa);
        }

        public static Claim CreateSpnClaim(string spn)
        {
            if (spn == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(spn));
            }

            return new Claim(ClaimTypes.Spn, spn, Rights.PossessProperty);
        }

        public static Claim CreateThumbprintClaim(byte[] thumbprint)
        {
            if (thumbprint == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(thumbprint));
            }

            return new Claim(ClaimTypes.Thumbprint, SecurityUtils.CloneBuffer(thumbprint), Rights.PossessProperty, ClaimComparer.Thumbprint);
        }

        public static Claim CreateUpnClaim(string upn)
        {
            if (upn == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(upn));
            }

            return new Claim(ClaimTypes.Upn, upn, Rights.PossessProperty, ClaimComparer.Upn);
        }

        public static Claim CreateUriClaim(Uri uri)
        {
            if (uri == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(uri));
            }

            return new Claim(ClaimTypes.Uri, uri, Rights.PossessProperty);
        }

        public static Claim CreateWindowsSidClaim(SecurityIdentifier sid)
        {
            if (sid == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(sid));
            }

            return new Claim(ClaimTypes.Sid, sid, Rights.PossessProperty);
        }

        public static Claim CreateX500DistinguishedNameClaim(X500DistinguishedName x500DistinguishedName)
        {
            if (x500DistinguishedName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(x500DistinguishedName));
            }

            return new Claim(ClaimTypes.X500DistinguishedName, x500DistinguishedName, Rights.PossessProperty, ClaimComparer.X500DistinguishedName);
        }

        public override bool Equals(object obj)
        {
            if (_comparer == null)
            {
                _comparer = ClaimComparer.GetComparer(_claimType);
            }

            return _comparer.Equals(this, obj as Claim);
        }

        public override int GetHashCode()
        {
            if (_comparer == null)
            {
                _comparer = ClaimComparer.GetComparer(_claimType);
            }

            return _comparer.GetHashCode(this);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}: {1}", _right, _claimType);
        }
    }
}
