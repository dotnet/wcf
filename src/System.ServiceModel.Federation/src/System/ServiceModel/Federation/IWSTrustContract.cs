// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace System.ServiceModel.Federation
{
    /// <summary>
    /// Defines the IWSTrustContract for sending WS-Trust messages to an STS.
    /// </summary>
    [ServiceContract]
    public interface IWSTrustContract
    {
        /// <summary>
        /// Method for Cancel binding for WS-Trust.
        /// </summary>
        /// <param name="message">The WS-Trust request to send to an STS.</param>
        /// <returns>A message containing a WS-Trust response.</returns>
        [OperationContract(Name = "Cancel", Action = "*", ReplyAction = "*")]
        Task<Message> CancelAsync(Message message);

        /// <summary>
        /// Method for Issue binding for WS-Trust.
        /// </summary>
        /// <param name="message">The WS-Trust request to send to an STS.</param>
        /// <returns>A message containing a WS-Trust response.</returns>
        [OperationContract(Name = "Issue", Action = "*", ReplyAction = "*")]
        Task<Message> IssueAsync(Message message);

        /// <summary>
        /// Method for Renew binding for WS-Trust.
        /// </summary>
        /// <param name="message">The WS-Trust request to send to an STS.</param>
        /// <returns>A message containing a WS-Trust response.</returns>
        [OperationContract(Name = "Renew", Action = "*", ReplyAction = "*")]
        Task<Message> RenewAsync(Message message);

        /// <summary>
        /// Method for Validate binding for WS-Trust.
        /// </summary>
        /// <param name="message">The WS-Trust request to send to an STS.</param>
        /// <returns>A message containing a WS-Trust response.</returns>
        [OperationContract(Name = "Validate", Action = "*", ReplyAction = "*")]
        Task<Message> ValidateAsync(Message message);
    }
}
