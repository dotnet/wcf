// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.WsTrust;

namespace System.ServiceModel.Federation
{
    /// <summary>
    /// A service contract that defines the Issue request.
    /// </summary>
    [ServiceContract]
    public interface IWSTrustChannelContract
    {
        /// <summary>
        /// Sends a WS-Trust Issue request to a STS.
        /// </summary>
        /// <param name="request">The <see cref="WsTrustRequest" /> to send to the STS.</param>
        /// <returns>A <see cref="SecurityToken" /> issued by the STS.</returns>
        [OperationContract(Name = "Issue", Action = "*", ReplyAction = "*")]
        Task<SecurityToken> IssueAsync(WsTrustRequest request);
    }
}
