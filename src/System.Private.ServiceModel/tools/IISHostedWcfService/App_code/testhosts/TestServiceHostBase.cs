// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF.Channels;
using CoreWCF.Description;
#else
using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
#endif

namespace WcfService
{
    public abstract class TestServiceHostBase<ContractType> : ServiceHost
    {
        private const string EmptyAddress = "___";
        protected static int SixtyFourMB = 64 * 1024 * 1024;

        protected virtual string Address { get { throw new NotImplementedException("Must override Address if not overriding GetBindings"); } }

        protected virtual Binding GetBinding() { throw new NotImplementedException("Must override either GetBinding or GetBindings"); }

        protected virtual IList<Binding> GetBindings()
        {
            var singleBinding = GetBinding();
            singleBinding.Name = string.IsNullOrEmpty(Address) ? EmptyAddress : Address;
            return new List<Binding> { singleBinding };
        }

        public TestServiceHostBase(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            foreach (var binding in GetBindings())
            {
                AddServiceEndpoint(typeof(ContractType), binding, binding.Name == EmptyAddress ? "" : binding.Name);
            }
        }

#if !NET
        //Overriding ApplyConfiguration() allows us to
        //alter the ServiceDescription prior to opening
        //the service host.
        protected override void ApplyConfiguration()
        {
            //First, we call base.ApplyConfiguration()
            //to read any configuration that was provided for
            //the service we're hosting. After this call,
            //this.ServiceDescription describes the service
            //as it was configured.
            base.ApplyConfiguration();

            ServiceDebugBehavior debugBehavior = this.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (debugBehavior == null)
            {
                debugBehavior = new ServiceDebugBehavior();
                this.Description.Behaviors.Add(debugBehavior);
            }
            debugBehavior.IncludeExceptionDetailInFaults = true;
            //Now that we've populated the ServiceDescription, we can reach into it
            //and do interesting things (in this case, we'll add an instance of
            //ServiceMetadataBehavior if it's not already there.
            ServiceMetadataBehavior mexBehavior = this.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (mexBehavior == null)
            {
                mexBehavior = new ServiceMetadataBehavior();
                this.Description.Behaviors.Add(mexBehavior);
            }
            else
            {
                //Metadata behavior has already been configured, 
                //so we don't have any work to do.
                return;
            }

            //Add a metadata endpoint at http base address
            foreach (Uri baseAddress in this.BaseAddresses)
            {
                if (baseAddress.Scheme == Uri.UriSchemeHttp)
                {
                    mexBehavior.HttpGetEnabled = true;
                    this.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                                            MetadataExchangeBindings.CreateMexHttpBinding(),
                                            "mex");
                }

                if (baseAddress.Scheme == Uri.UriSchemeNetTcp)
                {
                    mexBehavior.HttpGetEnabled = false;
                    this.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                                            MetadataExchangeBindings.CreateMexTcpBinding(),
                                            "mex");
                }
            }
        }
#endif
    }

    public abstract class TestServiceHostBase<ContractType1, ContractType2> : TestServiceHostBase<ContractType1>
    {
        public TestServiceHostBase(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            AddServiceEndpoint(typeof(ContractType2), GetBinding(), Address);
        }
    }
}
