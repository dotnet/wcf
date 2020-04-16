// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IdentityModel.Tokens
{
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Principal;
    using WindowsIdentity = System.Object;

    public class X509WindowsSecurityToken : X509SecurityToken
    {
        private WindowsIdentity _windowsIdentity;
        private bool _disposed = false;
        private string _authenticationType;

        public X509WindowsSecurityToken(X509Certificate2 certificate, WindowsIdentity windowsIdentity)
            : this(certificate, windowsIdentity, null, true)
        {
        }

        public X509WindowsSecurityToken(X509Certificate2 certificate, WindowsIdentity windowsIdentity, string id)
            : this(certificate, windowsIdentity, null, id, true)
        {
        }

        public X509WindowsSecurityToken(X509Certificate2 certificate, WindowsIdentity windowsIdentity, string authenticationType, string id)
            : this(certificate, windowsIdentity, authenticationType, id, true)
        {
        }

        internal X509WindowsSecurityToken(X509Certificate2 certificate, WindowsIdentity windowsIdentity, string authenticationType, bool clone)
            : this(certificate, windowsIdentity, authenticationType, SecurityUniqueId.Create().Value, clone)
        {
        }

        internal X509WindowsSecurityToken(X509Certificate2 certificate, WindowsIdentity windowsIdentity, string authenticationType, string id, bool clone)
            : base(certificate, id, clone)
        {
            throw new NotImplementedException();
        }


        public WindowsIdentity WindowsIdentity
        {
            get
            {
                ThrowIfDisposed();
                return _windowsIdentity;
            }
        }

        public string AuthenticationType
        {
            get
            {
                return _authenticationType;
            }
        }

        public override void Dispose()
        {
            try
            {
                if (!_disposed)
                {
                    _disposed = true;
                    _windowsIdentity = null;
                }
            }
            finally
            {
                base.Dispose();
            }
        }
    }
}
