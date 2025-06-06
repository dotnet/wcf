// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
#else
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
#endif

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "XmlSFAttribute.svc")]
    public class XmlSFAttributeTestServiceHost : TestServiceHostBase<IXmlSFAttribute>
    {
        protected override string Address { get { return "XmlSFAttribute"; } }

        protected override Binding GetBinding()
        {
            var binding = new BasicHttpBinding();
            return binding;
        }

        public XmlSFAttributeTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(XmlSFAttribute), baseAddresses)
        {
        }
    }
}
