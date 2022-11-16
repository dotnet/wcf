// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


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
        private ReadOnlyCollection<SecurityKey> _securityKeys;
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
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(certificate));
            }

            _id = id ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(id));

            _certificate = clone ? new X509Certificate2(certificate) : certificate;

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
                if (_securityKeys == null)
                {
                    List<SecurityKey> temp = new List<SecurityKey>(1);
                    temp.Add(new X509AsymmetricSecurityKey(_certificate));
                    _securityKeys = temp.AsReadOnly();
                }
                return _securityKeys;
            }
        }

        public override DateTime ValidFrom
        {
            get
            {
                ThrowIfDisposed();
                if (_effectiveTime == SecurityUtils.MaxUtcDateTime)
                {
                    _effectiveTime = _certificate.NotBefore.ToUniversalTime();
                }

                return _effectiveTime;
            }
        }

        public override DateTime ValidTo
        {
            get
            {
                ThrowIfDisposed();
                if (_expirationTime == SecurityUtils.MinUtcDateTime)
                {
                    _expirationTime = _certificate.NotAfter.ToUniversalTime();
                }

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

        public override bool CanCreateKeyIdentifierClause<T>()
        {
            ThrowIfDisposed();
            if (typeof(T) == typeof(X509SubjectKeyIdentifierClause))
                return X509SubjectKeyIdentifierClause.CanCreateFrom(_certificate);

            return typeof(T) == typeof(X509ThumbprintKeyIdentifierClause) ||
                   typeof(T) == typeof(X509IssuerSerialKeyIdentifierClause) ||
                   typeof(T) == typeof(X509RawDataKeyIdentifierClause) ||
                   base.CanCreateKeyIdentifierClause<T>();
        }

        public override T CreateKeyIdentifierClause<T>()
        {
            ThrowIfDisposed();
            if (typeof(T) == typeof(X509SubjectKeyIdentifierClause))
            {
                X509SubjectKeyIdentifierClause x509KeyIdentifierClause;
                if (X509SubjectKeyIdentifierClause.TryCreateFrom(_certificate, out x509KeyIdentifierClause))
                    return x509KeyIdentifierClause as T;
            }
            else if (typeof(T) == typeof(X509ThumbprintKeyIdentifierClause))
            {
                return new X509ThumbprintKeyIdentifierClause(_certificate) as T;
            }
            else if (typeof(T) == typeof(X509IssuerSerialKeyIdentifierClause))
            {
                return new X509IssuerSerialKeyIdentifierClause(_certificate) as T;
            }
            else if (typeof(T) == typeof(X509RawDataKeyIdentifierClause))
            {
                return new X509RawDataKeyIdentifierClause(_certificate) as T;
            }

            return base.CreateKeyIdentifierClause<T>();
        }

        public override bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            ThrowIfDisposed();
            X509SubjectKeyIdentifierClause subjectKeyIdentifierClause = keyIdentifierClause as X509SubjectKeyIdentifierClause;
            if (subjectKeyIdentifierClause != null)
                return subjectKeyIdentifierClause.Matches(_certificate);

            X509ThumbprintKeyIdentifierClause thumbprintKeyIdentifierClause = keyIdentifierClause as X509ThumbprintKeyIdentifierClause;
            if (thumbprintKeyIdentifierClause != null)
                return thumbprintKeyIdentifierClause.Matches(_certificate);

            X509IssuerSerialKeyIdentifierClause issuerKeyIdentifierClause = keyIdentifierClause as X509IssuerSerialKeyIdentifierClause;
            if (issuerKeyIdentifierClause != null)
                return issuerKeyIdentifierClause.Matches(_certificate);

            X509RawDataKeyIdentifierClause rawCertKeyIdentifierClause = keyIdentifierClause as X509RawDataKeyIdentifierClause;
            if (rawCertKeyIdentifierClause != null)
                return rawCertKeyIdentifierClause.Matches(_certificate);

            return base.MatchesKeyIdentifierClause(keyIdentifierClause);
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(GetType().FullName));
            }
        }
    }
}
