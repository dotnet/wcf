// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel;

namespace WcfService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class XmlSFAttribute : IXmlSFAttribute
    {
        public void TestXmlSerializerSupportsFaults_True()
        {
            throw new FaultException<FaultDetailWithXmlSerializerFormatAttribute>(new FaultDetailWithXmlSerializerFormatAttribute { UsedDataContractSerializer = true } );
        }

        public void TestXmlSerializerSupportsFaults_False()
        {
            throw new FaultException<FaultDetailWithXmlSerializerFormatAttribute>(new FaultDetailWithXmlSerializerFormatAttribute { UsedDataContractSerializer = true } );
        }
    }
}
