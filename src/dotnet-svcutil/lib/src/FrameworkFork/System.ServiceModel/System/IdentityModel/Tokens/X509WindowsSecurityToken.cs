//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.IdentityModel.Tokens
{
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Principal;

#if !disabled
    using WindowsIdentity = System.Object;
#endif


    public class X509WindowsSecurityToken : X509SecurityToken
    {
        WindowsIdentity windowsIdentity;
        bool disposed = false;
        string authenticationType;

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
#if disabled
            if (windowsIdentity == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("windowsIdentity");

            this.authenticationType = authenticationType;
            this.windowsIdentity = clone ? SecurityUtils.CloneWindowsIdentityIfNecessary(windowsIdentity, authenticationType) : windowsIdentity;
#else
            throw new NotImplementedException();
#endif
        }


        public WindowsIdentity WindowsIdentity
        {
            get
            {
                ThrowIfDisposed();
                return this.windowsIdentity;
            }
        }

        public string AuthenticationType
        {
            get
            {
                return this.authenticationType;
            }
        }

        public override void Dispose()
        {
            try
            {
                if (!this.disposed)
                {
                    this.disposed = true;
#if disabled
                    this.windowsIdentity.Dispose();
#endif
                    this.windowsIdentity = null;
                }
            }
            finally
            {
                base.Dispose();
            }
        }
    }
}
