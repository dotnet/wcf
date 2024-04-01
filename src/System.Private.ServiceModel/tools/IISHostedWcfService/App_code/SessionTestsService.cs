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

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SessionTestsDuplexService : ISessionTestsDuplexService
    {
        /// <summary>
        ///     This method is designed to test many intricacies of session-bound duplex calls 
        ///     especially when terminating methods are involved on both sides.
        ///     All calls are made sequentially to avoid possible inconsistencies due to concurrency
        /// </summary>
        /// <param name="callsToClientCallbackToMake">
        ///     number of times to call a non terminating "ClientCallback" client callback method </param>
        /// <param name="callsToTerminatingClientCallbackToMake">
        ///     number of times to call a terminating "TerminatingClientCallback" client callback method </param>
        /// <param name="callsToClientSideOnlyTerminatingClientCallbackToMake">
        ///     same as callsToTerminatingClientCallbackToMake except for the client callback is marked 
        ///     as IsTerminating=true only on the client side.
        ///
        ///     Even though this particular combination is allowed it steps into an uncommon area where 
        ///     the client's and server's OperationContract don't match. 
        ///     We only test this particular case because it affects which calls will fail or pass. </param>
        /// <param name="callsToNonTerminatingMethodToMakeInsideClientCallback">
        ///     number of times client callback needs to call our NonTerminatingMethod </param>
        /// <param name="callsToTerminatingMethodToMakeInsideTerminatingClientCallback">
        ///     number of times terminating client callback needs to call our "TerminatingMethod" </param>
        ///
        /// <returns>
        ///     total number of calls made
        /// </returns>
        public int NonTerminatingMethodCallingDuplexCallbacks(
            int callsToClientCallbackToMake,
            int callsToTerminatingClientCallbackToMake,
            int callsToClientSideOnlyTerminatingClientCallbackToMake,
            int callsToNonTerminatingMethodToMakeInsideClientCallback,
            int callsToTerminatingMethodToMakeInsideClientCallback)
        {
            return MethodCallingDuplexCallbacks(
                callsToClientCallbackToMake,
                callsToTerminatingClientCallbackToMake,
                callsToClientSideOnlyTerminatingClientCallbackToMake,
                callsToNonTerminatingMethodToMakeInsideClientCallback,
                callsToTerminatingMethodToMakeInsideClientCallback);
        }

        /// <summary>
        ///     Essentially the same as NonTerminatingMethodCallingDuplexCallbacks. 
        ///     The only difference is it is marked as IsTerminating=true on the client side
        /// </summary>
        public int TerminatingMethodCallingDuplexCallbacks(
            int callsToClientCallbackToMake,
            int callsToTerminatingClientCallbackToMake,
            int callsToClientSideOnlyTerminatingClientCallbackToMake,
            int callsToNonTerminatingMethodToMakeInsideClientCallback,
            int callsToTerminatingMethodToMakeInsideClientCallback)
        {
            return MethodCallingDuplexCallbacks(
                callsToClientCallbackToMake,
                callsToTerminatingClientCallbackToMake,
                callsToClientSideOnlyTerminatingClientCallbackToMake,
                callsToNonTerminatingMethodToMakeInsideClientCallback,
                callsToTerminatingMethodToMakeInsideClientCallback);
        }

        private int MethodCallingDuplexCallbacks(
            int callsToClientCallbackToMake,
            int callsToTerminatingClientCallbackToMake,
            int callsToClientSideOnlyTerminatingClientCallbackToMake,
            int callsToNonTerminatingMethodToMakeInsideClientCallback,
            int callsToTerminatingMethodToMakeInsideClientCallback)
        {
            int numCalls = 1;  // 1 - for this call
            var cb = OperationContext.Current.GetCallbackChannel<ISessionTestsDuplexCallback>();

            // we keep these for debugging purposes
            ((IContextChannel)cb).Closing += (sender, e) =>
            {
            };
            ((IContextChannel)cb).Closed += (sender, e) =>
            {
            };
            ((IContextChannel)cb).Faulted += (sender, e) =>
            {
            };

            // Rather than dealing with a large number of combinations of various exceptions arising from both sides,
            // the test is structured to return the total count of successful calls that were made both directions.

            // Terminating ones go first
            for (int i = 0; i < callsToTerminatingClientCallbackToMake; i++)
            {
                try
                {
                    numCalls += cb.TerminatingClientCallback(
                        callsToNonTerminatingMethodToMakeInsideClientCallback,
                        callsToTerminatingMethodToMakeInsideClientCallback);
                }
                catch
                {
                }
            }

            for (int i = 0; i < callsToClientSideOnlyTerminatingClientCallbackToMake; i++)
            {
                try
                {
                    numCalls += cb.ClientSideOnlyTerminatingClientCallback(
                        callsToNonTerminatingMethodToMakeInsideClientCallback,
                        callsToTerminatingMethodToMakeInsideClientCallback);
                }
                catch
                {
                }
            }

            for (int i = 0; i < callsToClientCallbackToMake; i++)
            {
                try
                {
                    numCalls += cb.ClientCallback(
                        callsToNonTerminatingMethodToMakeInsideClientCallback,
                        callsToTerminatingMethodToMakeInsideClientCallback);
                }
                catch
                {
                }
            }

            return numCalls;
        }

        public int NonTerminatingMethod()
        {
            return 1;
        }

        public int TerminatingMethod()
        {
            return 1;
        }
    }
}
