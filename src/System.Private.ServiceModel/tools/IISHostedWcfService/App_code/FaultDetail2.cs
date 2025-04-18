// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !NET
using System.Runtime.Serialization;
#endif

namespace WcfService
{
    [DataContract(Name = "FaultDetail", Namespace = "http://www.contoso.com/wcfnamespace")]
    public class FaultDetail2
    {
        private string _report;

        public FaultDetail2(string message)
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
