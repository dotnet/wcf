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

    internal class UnconditionalPolicy : IAuthorizationPolicy, IDisposable, ICloneable
    {
        private SecurityUniqueId _id;
        private ClaimSet _issuance;
        private ReadOnlyCollection<ClaimSet> _issuances;
        private IIdentity _primaryIdentity;
        private bool _disposed = false;

        public UnconditionalPolicy(ClaimSet issuance)
            : this(issuance, SecurityUtils.MaxUtcDateTime)
        {
        }

        public UnconditionalPolicy(ClaimSet issuance, DateTime expirationTime)
        {
            if (issuance == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(issuance));
            }

            Initialize(ClaimSet.System, issuance, null, expirationTime);
        }

        public UnconditionalPolicy(ReadOnlyCollection<ClaimSet> issuances, DateTime expirationTime)
        {
            if (issuances == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(issuances));
            }

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
            _primaryIdentity = from._primaryIdentity;
            if (from._issuance != null)
            {
                _issuance = from._issuance;
            }
            else
            {
                _issuances = from._issuances;
            }
            Issuer = from.Issuer;
            ExpirationTime = from.ExpirationTime;
        }

        private void Initialize(ClaimSet issuer, ClaimSet issuance, ReadOnlyCollection<ClaimSet> issuances, DateTime expirationTime)
        {
            Issuer = issuer;
            _issuance = issuance;
            _issuances = issuances;
            ExpirationTime = expirationTime;
        }

        public string Id
        {
            get
            {
                if (_id == null)
                {
                    _id = SecurityUniqueId.Create();
                }

                return _id.Value;
            }
        }

        public ClaimSet Issuer { get; private set; }

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
                        else if (_issuance is IPrincipal principal)
                        {
                            identity = principal.Identity;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _issuances.Count; ++i)
                        {
                            ClaimSet issuance = _issuances[i];
                            if (issuance is IIdentityInfo identityInfo)
                            {
                                identity = identityInfo.Identity;
                            }
                            else if (issuance is IPrincipal principal)
                            {
                                identity = principal.Identity;
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

        public DateTime ExpirationTime { get; private set; }

        internal UnconditionalPolicy Clone()
        {
            ThrowIfDisposed();
            return this;
        }

        object ICloneable.Clone() => Clone();

        public virtual void Dispose()
        {
            // Noop as only did anything for WindowsClaimSet which is only used with Service
            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(GetType().FullName));
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
            if (PrimaryIdentity != null && PrimaryIdentity != SecurityUtils.AnonymousIdentity)
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
                    identities.Add(PrimaryIdentity);
                }
            }

            evaluationContext.RecordExpirationTime(ExpirationTime);
            return true;
        }
    }
}
