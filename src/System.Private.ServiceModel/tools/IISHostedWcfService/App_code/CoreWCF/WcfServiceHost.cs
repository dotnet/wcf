// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET

using CoreWCF;
using CoreWCF.Channels;

namespace WcfService
{
    public class ServiceHost
    {
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

        public virtual void ApplyConfiguration(ServiceHostBase host)
        {

        }

        public List<Endpoint> Endpoints => _endpoints;

        public Type ServiceType => _serviceType;

        public class Endpoint
        {
            public Type ContractType { get; set; }
            public Binding Binding { get; set; }
            public string Address { get; set; }
        }
    }
}
#endif
