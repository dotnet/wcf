// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;

namespace System.IdentityModel.Policy
{
    internal class DefaultAuthorizationContext : AuthorizationContext
    {
        private static DefaultAuthorizationContext s_empty;
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
                if (s_empty == null)
                    s_empty = new DefaultAuthorizationContext(new DefaultEvaluationContext());
                return s_empty;
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
