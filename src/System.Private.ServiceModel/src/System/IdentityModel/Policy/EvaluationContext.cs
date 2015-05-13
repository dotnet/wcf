// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;

namespace System.IdentityModel.Policy
{
    public abstract class EvaluationContext
    {
        public abstract ReadOnlyCollection<ClaimSet> ClaimSets { get; }
        public abstract IDictionary<string, object> Properties { get; }
        public abstract int Generation { get; }
        public abstract void AddClaimSet(IAuthorizationPolicy policy, ClaimSet claimSet);
        public abstract void RecordExpirationTime(DateTime expirationTime);
    }
}
