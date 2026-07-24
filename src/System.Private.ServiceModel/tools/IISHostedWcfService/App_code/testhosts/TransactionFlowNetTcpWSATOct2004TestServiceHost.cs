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
    [TestServiceDefinition(BasePath = "NetTcpTransactionFlowWSATOct2004.svc", Schema = ServiceSchema.NETTCP)]
    public class TransactionFlowNetTcpWSATOct2004TestServiceHost : TestServiceHostBase<IWcfTransactionService>
    {
        protected override IList<Binding> GetBindings()
        {
            return new List<Binding> { GetNetTcpTransactionFlowBinding() };
        }

        private Binding GetNetTcpTransactionFlowBinding()
        {
            var binding = new NetTcpBinding(SecurityMode.None);
            binding.TransactionFlow = true;
            binding.TransactionProtocol = TransactionProtocol.WSAtomicTransactionOctober2004;
            binding.Name = "TransactionFlow";
            return binding;
        }

        public TransactionFlowNetTcpWSATOct2004TestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfTransactionService), baseAddresses)
        {
        }
    }
}
#endif
