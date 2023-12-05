// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CoreWCF;

namespace Binding.UDS.IntegrationTests.ServiceContract
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class EchoService : IEchoService
    {
        public string Echo(string echo)
        {
            return echo;
        }
    }
}
