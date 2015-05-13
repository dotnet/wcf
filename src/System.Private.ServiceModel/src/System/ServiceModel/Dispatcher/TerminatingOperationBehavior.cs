// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    internal class TerminatingOperationBehavior
    {
        private static void AbortChannel(object state)
        {
            ((IChannel)state).Abort();
        }

        public static TerminatingOperationBehavior CreateIfNecessary(DispatchRuntime dispatch)
        {
            if (IsTerminatingOperationBehaviorNeeded(dispatch))
            {
                return new TerminatingOperationBehavior();
            }
            else
            {
                return null;
            }
        }

        private static bool IsTerminatingOperationBehaviorNeeded(DispatchRuntime dispatch)
        {
            for (int i = 0; i < dispatch.Operations.Count; i++)
            {
                DispatchOperation operation = dispatch.Operations[i];

                if (operation.IsTerminating)
                {
                    return true;
                }
            }

            return false;
        }


        internal static void AfterReply(ref ProxyRpc rpc)
        {
            if (rpc.Operation.IsTerminating && rpc.Channel.HasSession)
            {
                IChannel sessionChannel = rpc.Channel.Binder.Channel;
                rpc.Channel.Close(rpc.TimeoutHelper.RemainingTime());
            }
        }
    }
}
