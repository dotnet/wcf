// Licensed to the .NET Foundation under one or more agreements.
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
