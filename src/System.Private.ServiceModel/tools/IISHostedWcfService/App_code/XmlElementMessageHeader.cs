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
using System.Xml.Serialization;

namespace WcfService
{
    [XmlType(Namespace = "http://tempuri.org/")]
    public partial class XmlElementMessageHeader
    {

        private string _headerValue;

        [XmlElement(Order = 0)]
        public string HeaderValue
        {
            get
            {
                return _headerValue;
            }
            set
            {
                _headerValue = value;
            }
        }
    }

    [MessageContract(WrapperName = "SendRequestWithXmlElementMessageHeader", WrapperNamespace = "http://tempuri.org/", IsWrapped = true)]
    public partial class XmlElementMessageHeaderRequest
    {

        [MessageHeader(Namespace = "http://tempuri.org/")]
        public XmlElementMessageHeader TestHeader;

        public XmlElementMessageHeaderRequest()
        {
        }

        public XmlElementMessageHeaderRequest(XmlElementMessageHeader testHeader)
        {
            TestHeader = testHeader;
        }
    }

    [MessageContract(WrapperName = "SendRequestWithXmlElementMessageHeaderResponse", WrapperNamespace = "http://tempuri.org/", IsWrapped = true)]
    public partial class XmlElementMessageHeaderResponse
    {

        [MessageBodyMember(Namespace = "http://tempuri.org/", Order = 0)]
        public string TestResult;

        public XmlElementMessageHeaderResponse()
        {
        }

        public XmlElementMessageHeaderResponse(string testResult)
        {
            TestResult = testResult;
        }
    }
}
