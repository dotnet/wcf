// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace System.IdentityModel.Tokens
{
    public class X509SecurityToken : SecurityToken, IDisposable
    {
        private string _id;
        private X509Certificate2 _certificate;
        // private ReadOnlyCollection<SecurityKey> _securityKeys;
        private DateTime _effectiveTime = SecurityUtils.MaxUtcDateTime;
        private DateTime _expirationTime = SecurityUtils.MinUtcDateTime;
        private bool _disposed = false;
        private bool _disposable;

        public X509SecurityToken(X509Certificate2 certificate)
            : this(certificate, SecurityUniqueId.Create().Value)
        {
        }

        public X509SecurityToken(X509Certificate2 certificate, string id)
            : this(certificate, id, true)
        {
        }

        internal X509SecurityToken(X509Certificate2 certificate, bool clone)
            : this(certificate, SecurityUniqueId.Create().Value, clone)
        {
        }

        internal X509SecurityToken(X509Certificate2 certificate, bool clone, bool disposable)
            : this(certificate, SecurityUniqueId.Create().Value, clone, disposable)
        {
        }

        internal X509SecurityToken(X509Certificate2 certificate, string id, bool clone)
            : this(certificate, id, clone, true)
        {
        }

        internal X509SecurityToken(X509Certificate2 certificate, string id, bool clone, bool disposable)
        {
            if (certificate == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("certificate");
            if (id == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("id");

            _id = id;

            _certificate = clone ? new X509Certificate2(certificate.Handle) : certificate;
            // if the cert needs to be cloned then the token owns the clone and should dispose it
            _disposable = clone || disposable;
        }

        public override string Id
        {
            get { return _id; }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                ThrowIfDisposed();
                throw ExceptionHelper.PlatformNotSupported("X509SecurityToken.SecurityKeys");
                //if (_securityKeys == null)
                //{
                //    List<SecurityKey> temp = new List<SecurityKey>(1);
                //    temp.Add(new X509AsymmetricSecurityKey(_certificate));
                //    _securityKeys = temp.AsReadOnly();
                //}
                //return _securityKeys;
            }
        }

        public override DateTime ValidFrom
        {
            get
            {
                ThrowIfDisposed();
                if (_effectiveTime == SecurityUtils.MaxUtcDateTime)
                    _effectiveTime = _certificate.NotBefore.ToUniversalTime();
                return _effectiveTime;
            }
        }

        public override DateTime ValidTo
        {
            get
            {
                ThrowIfDisposed();
                if (_expirationTime == SecurityUtils.MinUtcDateTime)
                    _expirationTime = _certificate.NotAfter.ToUniversalTime();
                return _expirationTime;
            }
        }

        public X509Certificate2 Certificate
        {
            get
            {
                ThrowIfDisposed();
                return _certificate;
            }
        }

        public virtual void Dispose()
        {
            if (_disposable && !_disposed)
            {
                _disposed = true;

                _certificate.Dispose();
                _certificate = null;
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
