// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
#else
using System;
using System.ServiceModel;
#endif
using System.Xml.Serialization;

namespace WcfService
{
    public class WcfSoapService : IWcfSoapService
    {
        public string CombineStringXmlSerializerFormatSoap(string message1, string message2)
        {
            return message1 + message2;
        }

        public SoapComplexType EchoComositeTypeXmlSerializerFormatSoap(SoapComplexType complexObject)
        {
            return complexObject;
        }

        [return: MessageParameter(Name = "ProcessCustomerDataReturn"), SoapElement(DataType = "string")]
        public string ProcessCustomerData([MessageParameter(Name = "CustomerData")] CustomerObject customerData)
        {
            return customerData.Name + ((AdditionalData)customerData.Data).Field;
        }
        
        public PingEncodedResponse Ping(PingEncodedRequest request)
        {
            int requestIntValue;
            return new PingEncodedResponse() { @Return = Int32.TryParse(request.Pinginfo, out requestIntValue) ? requestIntValue : -1 };
        }
    }
}
