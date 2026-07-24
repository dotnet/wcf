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
    [TestServiceDefinition(BasePath = "WS2007HttpTransactionFlow.svc", Schema = ServiceSchema.HTTP)]
    public class TransactionFlowWS2007HttpTestServiceHost : TestServiceHostBase<IWcfTransactionService>
    {
        protected override IList<Binding> GetBindings()
        {
            return new List<Binding> { GetWS2007HttpTransactionFlowBinding() };
        }

        private Binding GetWS2007HttpTransactionFlowBinding()
        {
            var binding = new WS2007HttpBinding(SecurityMode.None);
            binding.TransactionFlow = true;
            binding.Name = "TransactionFlow";
            return binding;
        }

        public TransactionFlowWS2007HttpTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfTransactionService), baseAddresses)
        {
        }
    }
}
#endif
