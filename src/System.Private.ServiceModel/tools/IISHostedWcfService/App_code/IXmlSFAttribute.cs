﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService
{
    [ServiceContract, XmlSerializerFormat]
    public interface IXmlSFAttribute
    {
        [OperationContract, XmlSerializerFormat(SupportFaults = true)]
        [FaultContract(typeof(FaultDetailWithXmlSerializerFormatAttribute),
            Action = "http://tempuri.org/IWcfService/FaultDetailWithXmlSerializerFormatAttribute",
            Name = "FaultDetailWithXmlSerializerFormatAttribute",
            Namespace = "http://www.contoso.com/wcfnamespace")]
        void TestXmlSerializerSupportsFaults_True();

        [OperationContract, XmlSerializerFormat]
        [FaultContract(typeof(FaultDetailWithXmlSerializerFormatAttribute),
            Action = "http://tempuri.org/IWcfService/FaultDetailWithXmlSerializerFormatAttribute",
            Name = "FaultDetailWithXmlSerializerFormatAttribute",
            Namespace = "http://www.contoso.com/wcfnamespace")]
        void TestXmlSerializerSupportsFaults_False();
    }
}
