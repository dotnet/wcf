// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.Threading;

namespace WcfService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class SessionTestsDefaultService : ISessionTestsDefaultService
    {
        private int _aValue = 0, _bValue = 0;

        public int MethodAInitiating(int a)
        {
            return Interlocked.Exchange(ref _aValue, a);
        }

        public int MethodBNonInitiating(int b)
        {
            return Interlocked.Exchange(ref _bValue, b);
        }

        public SessionTestsCompositeType MethodCTerminating()
        {
            return new SessionTestsCompositeType() { MethodAValue = _aValue, MethodBValue = _bValue };
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class SessionTestsShortTimeoutService : SessionTestsDefaultService, ISessionTestsShortTimeoutService
    {
    }
}
