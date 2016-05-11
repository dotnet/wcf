// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Serialization;

namespace WcfService
{
    [DataContract(Name = "FaultDetail", Namespace = "http://www.contoso.com/wcfnamespace")]
    public class FaultDetail
    {
        private string _report;

        public FaultDetail(string message)
        {
            _report = message;
        }

        [DataMember]
        public string Message
        {
            get { return _report; }
            set { _report = value; }
        }
    }
}
