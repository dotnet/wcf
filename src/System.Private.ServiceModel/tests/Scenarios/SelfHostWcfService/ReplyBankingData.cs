// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;

namespace WcfService
{
    [MessageContract(IsWrapped = true, WrapperName = "CustomWrapperName", WrapperNamespace = "http://www.contoso.com")]
    internal class ReplyBankingData
    {
        [MessageBodyMember(Order = 1, Name = "Date_of_Request")]
        public DateTime transactionDate;
        [MessageBodyMember(Name = "Customer_Name", Namespace = "http://www.contoso.com", Order = 3)]
        public string accountName;
        [MessageBodyMember(Order = 2, Name = "Transaction_Amount")]
        public int amount;
    }
}
