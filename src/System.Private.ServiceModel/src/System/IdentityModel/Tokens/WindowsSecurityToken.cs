// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Security.Principal;
using System.ServiceModel;

namespace System.IdentityModel.Tokens
{
    public class WindowsSecurityToken : SecurityToken, IDisposable
    {
        private string _authenticationType;
        private string _id;
        private DateTime _effectiveTime;
        private DateTime _expirationTime;
        private WindowsIdentity _windowsIdentity;
        private bool _disposed = false;

        public WindowsSecurityToken(WindowsIdentity windowsIdentity)
            : this(windowsIdentity, SecurityUniqueId.Create().Value)
        {
        }

        public WindowsSecurityToken(WindowsIdentity windowsIdentity, string id)
            : this(windowsIdentity, id, null)
        {
        }

        public WindowsSecurityToken(WindowsIdentity windowsIdentity, string id, string authenticationType)
        {
            DateTime effectiveTime = DateTime.UtcNow;
            Initialize(id, authenticationType, effectiveTime, DateTime.UtcNow.AddHours(10), windowsIdentity, true);
        }

        protected WindowsSecurityToken()
        {
        }

        protected void Initialize(string id, DateTime effectiveTime, DateTime expirationTime, WindowsIdentity windowsIdentity, bool clone)
        {
            Initialize(id, null, effectiveTime, expirationTime, windowsIdentity, clone);
        }

        protected void Initialize(string id, string authenticationType, DateTime effectiveTime, DateTime expirationTime, WindowsIdentity windowsIdentity, bool clone)
        {
            if (windowsIdentity == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("windowsIdentity");

            if (id == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("id");

            _id = id;
            _authenticationType = authenticationType;
            _effectiveTime = effectiveTime;
            _expirationTime = expirationTime;
            _windowsIdentity = clone ? SecurityUtils.CloneWindowsIdentityIfNecessary(windowsIdentity, authenticationType) : windowsIdentity;
        }

        public override string Id
        {
            get { return _id; }
        }

        public string AuthenticationType
        {
            get { return _authenticationType; }
        }

        public override DateTime ValidFrom
        {
            get { return _effectiveTime; }
        }

        public override DateTime ValidTo
        {
            get { return _expirationTime; }
        }

        public virtual WindowsIdentity WindowsIdentity
        {
            get
            {
                ThrowIfDisposed();
                return _windowsIdentity;
            }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get { return EmptyReadOnlyCollection<SecurityKey>.Instance; }
        }

        public virtual void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_windowsIdentity != null)
                {
                    _windowsIdentity.Dispose();
                    _windowsIdentity = null;
                }
            }
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(this.GetType().FullName));
            }
        }
    }
}
