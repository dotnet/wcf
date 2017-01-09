﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;

namespace WcfService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class WcfService_4_4_0 : IWcfService_4_4_0
    {
        public WcfService_4_4_0()
        {
        }

        public ReplyBankingData_4_4_0 MessageContractRequestReply(RequestBankingData_4_4_0 bt)
        {
            ReplyBankingData_4_4_0 bankingData = new ReplyBankingData_4_4_0();
            bankingData.accountName = bt.accountName;
            bankingData.transactionDate = bt.transactionDate;
            bankingData.amount = bt.amount;

            // post-1.1.0 features
            bankingData.replySingleValue = bt.requestSingleValue;
            bankingData.replyMultipleValues = bt.requestMultipleValues;
            bankingData.replyArrayMultipleValues = bt.requestArrayMultipleValues;

            return bankingData;
        }
        
        public XmlElementMessageHeaderResponse SendRequestWithXmlElementMessageHeader(XmlElementMessageHeaderRequest request)
        {
            return new XmlElementMessageHeaderResponse(request.TestHeader.HeaderValue);
        }
    }
}
