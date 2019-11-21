﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;

namespace WcfService
{
    // This class depends on features added post-1.1.0
    [MessageContract(IsWrapped = true,
                     WrapperName = "CustomWrapperName",
                     WrapperNamespace = "http://www.contoso.com")]
    public class ReplyBankingData_4_4_0
    {
        [MessageBodyMember(Order = 1, Name = "Date_of_Request")]
        public DateTime transactionDate;

        [MessageBodyMember(Name = "Customer_Name",
                           Namespace = "http://www.contoso.com",
                           Order = 3)]
        public string accountName;

        [MessageBodyMember(Order = 2,
                           Name = "Transaction_Amount")]
        public decimal amount;

        // The following rely on features added post-1.1.0
        [MessageHeader(Name = "SingleElement")]
        public string replySingleValue;

        [MessageHeader(Name = "MultipleElement")]
        public string[] replyMultipleValues;

        [MessageHeaderArray(Name = "ArrayMultipleElement")]
        public string[] replyArrayMultipleValues;
    }
}
