// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
#else
using System.ServiceModel;
using System.ServiceModel.Channels;
#endif

namespace WcfService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class UnderstoodHeaders : IUnderstoodHeaders
    {
        public void CreateMessageHeader(string headerName, string headerNameSpace, bool mustUnderstand)
        {
            // Create a MessageHeader using the parameters provided by the client.
            MessageHeader customHeader = MessageHeader.CreateHeader(headerName, headerNameSpace, "Any Object", mustUnderstand);

            // Add it to the outgoing response.
            OperationContext.Current.OutgoingMessageHeaders.Add(customHeader);
        }
    }
}
