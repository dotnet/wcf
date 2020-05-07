// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.Security.Principal;
using System.ServiceModel;

namespace System.IdentityModel.Policy
{
    internal interface IIdentityInfo
    {
        IIdentity Identity { get; }
    }

    internal class UnconditionalPolicy : IAuthorizationPolicy, IDisposable
    {
        private SecurityUniqueId _id;
        private ClaimSet _issuer;
        private ClaimSet _issuance;
        private ReadOnlyCollection<ClaimSet> _issuances;
        private DateTime _expirationTime;
        private IIdentity _primaryIdentity;
        private bool _disposable = false;
        private bool _disposed = false;

        public UnconditionalPolicy(ClaimSet issuance)
            : this(issuance, SecurityUtils.MaxUtcDateTime)
        {
        }

        public UnconditionalPolicy(ClaimSet issuance, DateTime expirationTime)
        {
            if (issuance == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("issuance");

            Initialize(ClaimSet.System, issuance, null, expirationTime);
        }

        public UnconditionalPolicy(ReadOnlyCollection<ClaimSet> issuances, DateTime expirationTime)
        {
            if (issuances == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("issuances");

            Initialize(ClaimSet.System, null, issuances, expirationTime);
        }

        internal UnconditionalPolicy(IIdentity primaryIdentity, ClaimSet issuance)
            : this(issuance)
        {
            _primaryIdentity = primaryIdentity;
        }

        internal UnconditionalPolicy(IIdentity primaryIdentity, ClaimSet issuance, DateTime expirationTime)
            : this(issuance, expirationTime)
        {
            _primaryIdentity = primaryIdentity;
        }

        internal UnconditionalPolicy(IIdentity primaryIdentity, ReadOnlyCollection<ClaimSet> issuances, DateTime expirationTime)
            : this(issuances, expirationTime)
        {
            _primaryIdentity = primaryIdentity;
        }

        private UnconditionalPolicy(UnconditionalPolicy from)
        {
            _disposable = from._disposable;
            _primaryIdentity = from._disposable ? SecurityUtils.CloneIdentityIfNecessary(from._primaryIdentity) : from._primaryIdentity;
            if (from._issuance != null)
            {
                _issuance = from._disposable ? SecurityUtils.CloneClaimSetIfNecessary(from._issuance) : from._issuance;
            }
            else
            {
                _issuances = from._disposable ? SecurityUtils.CloneClaimSetsIfNecessary(from._issuances) : from._issuances;
            }
            _issuer = from._issuer;
            _expirationTime = from._expirationTime;
        }

        private void Initialize(ClaimSet issuer, ClaimSet issuance, ReadOnlyCollection<ClaimSet> issuances, DateTime expirationTime)
        {
            _issuer = issuer;
            _issuance = issuance;
            _issuances = issuances;
            _expirationTime = expirationTime;

            if (issuance != null)
            {
                _disposable = issuance is WindowsClaimSet;
            }
            else
            {
                for (int i = 0; i < issuances.Count; ++i)
                {
                    if (issuances[i] is WindowsClaimSet)
                    {
                        _disposable = true;
                        break;
                    }
                }
            }
        }

        public string Id
        {
            get
            {
                if (_id == null)
                    _id = SecurityUniqueId.Create();
                return _id.Value;
            }
        }

        public ClaimSet Issuer
        {
            get { return _issuer; }
        }

        internal IIdentity PrimaryIdentity
        {
            get
            {
                ThrowIfDisposed();
                if (_primaryIdentity == null)
                {
                    IIdentity identity = null;
                    if (_issuance != null)
                    {
                        if (_issuance is IIdentityInfo)
                        {
                            identity = ((IIdentityInfo)_issuance).Identity;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _issuances.Count; ++i)
                        {
                            ClaimSet issuance = _issuances[i];
                            if (issuance is IIdentityInfo)
                            {
                                identity = ((IIdentityInfo)issuance).Identity;
                                // Preferably Non-Anonymous
                                if (identity != null && identity != SecurityUtils.AnonymousIdentity)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    _primaryIdentity = identity ?? SecurityUtils.AnonymousIdentity;
                }
                return _primaryIdentity;
            }
        }

        internal ReadOnlyCollection<ClaimSet> Issuances
        {
            get
            {
                ThrowIfDisposed();
                if (_issuances == null)
                {
                    List<ClaimSet> issuances = new List<ClaimSet>(1);
                    issuances.Add(_issuance);
                    _issuances = new ReadOnlyCollection<ClaimSet>(issuances);
                }
                return _issuances;
            }
        }

        public DateTime ExpirationTime
        {
            get { return _expirationTime; }
        }

        internal bool IsDisposable
        {
            get { return _disposable; }
        }

        internal UnconditionalPolicy Clone()
        {
            ThrowIfDisposed();
            return (_disposable) ? new UnconditionalPolicy(this) : this;
        }

        public virtual void Dispose()
        {
            if (_disposable && !_disposed)
            {
                _disposed = true;
                SecurityUtils.DisposeIfNecessary(_primaryIdentity as IDisposable);
                SecurityUtils.DisposeClaimSetIfNecessary(_issuance);
                SecurityUtils.DisposeClaimSetsIfNecessary(_issuances);
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(this.GetType().FullName));
            }
        }

        public virtual bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            ThrowIfDisposed();
            if (_issuance != null)
            {
                evaluationContext.AddClaimSet(this, _issuance);
            }
            else
            {
                for (int i = 0; i < _issuances.Count; ++i)
                {
                    if (_issuances[i] != null)
                    {
                        evaluationContext.AddClaimSet(this, _issuances[i]);
                    }
                }
            }

            // Preferably Non-Anonymous
            if (this.PrimaryIdentity != null && this.PrimaryIdentity != SecurityUtils.AnonymousIdentity)
            {
                IList<IIdentity> identities;
                object obj;
                if (!evaluationContext.Properties.TryGetValue(SecurityUtils.Identities, out obj))
                {
                    identities = new List<IIdentity>(1);
                    evaluationContext.Properties.Add(SecurityUtils.Identities, identities);
                }
                else
                {
                    // null if other overrides the property with something else
                    identities = obj as IList<IIdentity>;
                }

                if (identities != null)
                {
                    identities.Add(this.PrimaryIdentity);
                }
            }

            evaluationContext.RecordExpirationTime(_expirationTime);
            return true;
        }
    }
}
