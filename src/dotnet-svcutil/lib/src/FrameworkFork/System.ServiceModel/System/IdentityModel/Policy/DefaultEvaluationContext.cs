// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.ServiceModel;

namespace System.IdentityModel.Policy
{
    internal class DefaultEvaluationContext : EvaluationContext
    {
        private List<ClaimSet> _claimSets;
        private Dictionary<string, object> _properties;
        private DateTime _expirationTime = SecurityUtils.MaxUtcDateTime;
        private int _generation;

        ReadOnlyCollection<ClaimSet> readOnlyClaimSets;

        public DefaultEvaluationContext()
        {
            _properties = new Dictionary<string, object>();
            _generation = 0;
        }

        public override int Generation
        {
            get { return _generation; }
        }

        public override ReadOnlyCollection<ClaimSet> ClaimSets
        {
            get 
            {
                if (_claimSets == null)
                    return EmptyReadOnlyCollection<ClaimSet>.Instance;

                if (readOnlyClaimSets == null)
                    readOnlyClaimSets = new ReadOnlyCollection<ClaimSet>(_claimSets);

                return readOnlyClaimSets; 
            }
        }

        public override IDictionary<string, object> Properties
        {
            get { return _properties; }
        }

        public DateTime ExpirationTime
        {
            get { return _expirationTime; }
        }

        public override void AddClaimSet(IAuthorizationPolicy policy, ClaimSet claimSet)
        {
            if (claimSet == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("claimSet");

            if (_claimSets == null)
                _claimSets = new List<ClaimSet>();

            _claimSets.Add(claimSet);
            ++_generation;
        }

        public override void RecordExpirationTime(DateTime expirationTime)
        {
            if (_expirationTime > expirationTime)
                _expirationTime = expirationTime;
        }
    }
}
