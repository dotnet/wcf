// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;

namespace System.IdentityModel.Policy
{
    internal class DefaultAuthorizationContext : AuthorizationContext
    {
        private static DefaultAuthorizationContext _empty;
        private SecurityUniqueId _id;
        private ReadOnlyCollection<ClaimSet> _claimSets;
        private DateTime _expirationTime;
        private IDictionary<string, object> _properties;

        public DefaultAuthorizationContext(DefaultEvaluationContext evaluationContext)
        {
            _claimSets = evaluationContext.ClaimSets;
            _expirationTime = evaluationContext.ExpirationTime;
            _properties = evaluationContext.Properties;
        }

        public static DefaultAuthorizationContext Empty
        {
            get
            {
                if (_empty == null)
                    _empty = new DefaultAuthorizationContext(new DefaultEvaluationContext());
                return _empty;
            }
        }

        public override string Id
        {
            get
            {
                if (_id == null)
                    _id = SecurityUniqueId.Create();
                return _id.Value;
            }
        }

        public override ReadOnlyCollection<ClaimSet> ClaimSets
        {
            get { return _claimSets; }
        }

        public override DateTime ExpirationTime
        {
            get { return _expirationTime; }
        }

        public override IDictionary<string, object> Properties
        {
            get { return _properties; }
        }
    }
}
