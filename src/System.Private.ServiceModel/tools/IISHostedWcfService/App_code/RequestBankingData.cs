// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
#else
using System;
using System.ServiceModel;
#endif

namespace WcfService
{
    [MessageContract(IsWrapped = true, WrapperName = "RequestBankingDataWrapper", WrapperNamespace = "http://www.contoso.com")]
    public class RequestBankingData
    {
#pragma warning disable 0649	// fields not assigned to statically
        [MessageBodyMember(Order = 1, Name = "Date_of_Request")]
        public DateTime transactionDate;
        [MessageBodyMember(Name = "Customer_Name", Namespace = "CustomNamespace", Order = 3)]
        public string accountName;
        [MessageBodyMember(Order = 2, Name = "Transaction_Amount")]
        public int amount;
#pragma warning restore 0649
    }
}
