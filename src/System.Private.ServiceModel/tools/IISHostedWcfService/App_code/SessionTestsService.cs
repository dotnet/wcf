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
        private int _AValue = 0, _BValue = 0;

        public int MethodAInitiating(int a)
        {
            return Interlocked.Exchange(ref _AValue, a);
        }

        public int MethodBNonInitiating(int b)
        {
            return Interlocked.Exchange(ref _BValue, b);
        }

        public SessionTestsCompositeType MethodCTerminating()
        {
            return new SessionTestsCompositeType() { MethodAValue = _AValue, MethodBValue = _BValue };
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class SessionTestsShortTimeoutService : SessionTestsDefaultService, ISessionTestsShortTimeoutService
    {
    }
}
