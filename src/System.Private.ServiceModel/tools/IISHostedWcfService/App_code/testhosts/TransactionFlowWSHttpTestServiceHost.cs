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
    [TestServiceDefinition(BasePath = "WSHttpTransactionFlow.svc", Schema = ServiceSchema.HTTP)]
    public class TransactionFlowWSHttpTestServiceHost : TestServiceHostBase<IWcfTransactionService>
    {
        protected override IList<Binding> GetBindings()
        {
            return new List<Binding> { GetWSHttpTransactionFlowBinding() };
        }

        private Binding GetWSHttpTransactionFlowBinding()
        {
            var binding = new WSHttpBinding(SecurityMode.None);
            binding.TransactionFlow = true;
            binding.Name = "TransactionFlow";
            return binding;
        }

        public TransactionFlowWSHttpTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfTransactionService), baseAddresses)
        {
        }
    }
}
#endif
