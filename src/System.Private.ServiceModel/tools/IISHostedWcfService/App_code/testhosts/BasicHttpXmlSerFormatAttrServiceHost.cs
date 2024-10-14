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

    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "BasicHttpRpcEncSingleNs.svc")]
    public class XmlSerializerICalculatorRpcEncServiceHost : TestServiceHostBase<ICalculatorRpcEnc>
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public XmlSerializerICalculatorRpcEncServiceHost(params Uri[] baseAddresses)
            : base(typeof(RpcEncSingleNsService), baseAddresses)
        {
        }
    }


    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "BasicHttpRpcLitSingleNs.svc")]
    public class XmlSerializerICalculatorRpcLitServiceHost : TestServiceHostBase<ICalculatorRpcLit>
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public XmlSerializerICalculatorRpcLitServiceHost(params Uri[] baseAddresses)
            : base(typeof(RpcLitSingleNsService), baseAddresses)
        {
        }
    }


    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "BasicHttpDocLitSingleNs.svc")]
    public class XmlSerializerICalculatorDocLitServiceHost : TestServiceHostBase<ICalculatorDocLit>
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public XmlSerializerICalculatorDocLitServiceHost(params Uri[] baseAddresses)
            : base(typeof(DocLitSingleNsService), baseAddresses)
        {
        }
    }


    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "BasicHttpRpcEncDualNs.svc")]
    public class XmlSerializerDualContractRpcEncServiceHost : TestServiceHostBase<ICalculatorRpcEnc, IHelloWorldRpcEnc>
    {
        protected static Binding s_binding;
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return s_binding = s_binding == null ? new BasicHttpBinding() : s_binding;
        }

        public XmlSerializerDualContractRpcEncServiceHost(params Uri[] baseAddresses)
            : base(typeof(RpcEncDualNsService), baseAddresses)
        {
        }
    }


    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "BasicHttpRpcLitDualNs.svc")]
    public class XmlSerializerDualContractRpcLitServiceHost : TestServiceHostBase<ICalculatorRpcLit, IHelloWorldRpcLit>
    {
        protected static Binding s_binding;
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return s_binding = s_binding == null ? new BasicHttpBinding() : s_binding;
        }

        public XmlSerializerDualContractRpcLitServiceHost(params Uri[] baseAddresses)
            : base(typeof(RpcLitDualNsService), baseAddresses)
        {
        }
    }


    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "BasicHttpDocLitDualNs.svc")]
    public class XmlSerializerDualContractDocLitServiceHost : TestServiceHostBase<ICalculatorDocLit, IHelloWorldDocLit>
    {
        protected static Binding s_binding;
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return s_binding = s_binding == null ? new BasicHttpBinding() : s_binding;
        }

        public XmlSerializerDualContractDocLitServiceHost(params Uri[] baseAddresses)
            : base(typeof(DocLitDualNsService), baseAddresses)
        {
        }
    }
}
