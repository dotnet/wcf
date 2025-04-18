// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
#else
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
#endif

namespace WcfService
{
    [TestServiceDefinition(BasePath = "ReliableSessionDuplexService.svc", Schema = ServiceSchema.NETTCP)]
    public class ReliableSessionDuplexServiceHost : TestServiceHostBase<IWcfReliableDuplexService>
    {
        protected override IList<Binding> GetBindings()
        {
            var bindings = new List<Binding>();
            AddBindings(bindings, new NetTcpBinding(SecurityMode.None, true), "NetTcp");
            return bindings;
        }

        private void AddBindings(List<Binding> bindingList, Binding binding, string baseName)
        {
            AddBinding(bindingList, binding, true, ReliableMessagingVersion.WSReliableMessaging11, baseName);
            AddBinding(bindingList, binding, false, ReliableMessagingVersion.WSReliableMessaging11, baseName);
            AddBinding(bindingList, binding, true, ReliableMessagingVersion.WSReliableMessagingFebruary2005, baseName);
            AddBinding(bindingList, binding, false, ReliableMessagingVersion.WSReliableMessagingFebruary2005, baseName);
        }

        private void AddBinding(List<Binding> bindingList, Binding binding, bool ordered, ReliableMessagingVersion rmVersion, string baseName)
        {
            var customBinding = new CustomBinding(binding);
            var reliableSessionBindingElement = customBinding.Elements.Find<ReliableSessionBindingElement>();
            reliableSessionBindingElement.Ordered = ordered;
            reliableSessionBindingElement.ReliableMessagingVersion = rmVersion;
            customBinding.Name = baseName + (ordered ? "Ordered" : "Unordered") + "_" + rmVersion.ToString();
            bindingList.Add(customBinding);
        }

        public ReliableSessionDuplexServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfReliableService), baseAddresses)
        {
        }
    }
}
