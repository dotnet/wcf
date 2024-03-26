// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
#else
using System.ServiceModel;
using System.Threading;
#endif

namespace WcfService
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
    public class WcfReliableService : IWcfReliableService, IWcfReliableDuplexService, IOneWayWcfReliableService
    {
        private int _currentNumber = 0;

        public int GetNextNumber()
        {
            return Interlocked.Increment(ref _currentNumber);
        }

        public string Echo(string echo)
        {
            return echo;
        }

        public string DuplexEcho(string echo)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IWcfReliableDuplexService>();
            return callback.DuplexEcho(echo);
        }

        public void OneWay(string text)
        {
            return;
        }
    }
}
