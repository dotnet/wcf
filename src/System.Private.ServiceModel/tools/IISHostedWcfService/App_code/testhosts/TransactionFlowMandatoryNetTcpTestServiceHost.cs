// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService
{
    [TestServiceDefinition(BasePath = "NetTcpTransactionFlowMandatory.svc", Schema = ServiceSchema.NETTCP)]
    public class TransactionFlowMandatoryNetTcpTestServiceHost : TestServiceHostBase<IWcfTransactionMandatoryService>
    {
        protected override IList<Binding> GetBindings()
        {
            return new List<Binding> { GetNetTcpTransactionFlowBinding() };
        }

        private Binding GetNetTcpTransactionFlowBinding()
        {
            var binding = new NetTcpBinding(SecurityMode.None);
            binding.TransactionFlow = true;
            binding.Name = "TransactionFlow";
            return binding;
        }

        public TransactionFlowMandatoryNetTcpTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfTransactionMandatoryService), baseAddresses)
        {
        }
    }
}
#endif
