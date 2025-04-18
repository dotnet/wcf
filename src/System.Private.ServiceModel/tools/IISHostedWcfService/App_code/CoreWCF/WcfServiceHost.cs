// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Description;

namespace WcfService
{
    public class ServiceHost
    {
        private ServiceHostBase _serviceHostBase = null;
        private readonly Type _serviceType;
        private readonly List<Endpoint> _endpoints = new List<Endpoint>();

        public ServiceHost(Type serviceType, params Uri[] baseAddresses)
        {
            _serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        }

        public void AddServiceEndpoint(Type implementedContract, Binding binding, string address)
        {
            _endpoints.Add(new Endpoint
            {
                ContractType = implementedContract,
                Binding = binding,
                Address = address
            });
        }

        protected virtual void ApplyConfiguration() { }

        public void ApplyConfig(ServiceHostBase serviceHostBase)
        {
            _serviceHostBase = serviceHostBase;
            ApplyConfiguration();
            _serviceHostBase = null;
        }

        public List<Endpoint> Endpoints => _endpoints;

        public class Endpoint
        {
            public Type ContractType { get; set; }
            public Binding Binding { get; set; }
            public string Address { get; set; }
        }

        public Type ServiceType => _serviceHostBase != null ? _serviceHostBase.Description.ServiceType : _serviceType;

        public ServiceCredentials Credentials => _serviceHostBase != null ? _serviceHostBase.Credentials : new ServiceCredentials();

        public ServiceDescription Description => _serviceHostBase != null ? _serviceHostBase.Description : new ServiceDescription();
    }
}
#endif
