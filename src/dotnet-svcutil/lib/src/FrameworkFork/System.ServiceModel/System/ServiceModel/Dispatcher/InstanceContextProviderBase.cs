// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    abstract class InstanceContextProviderBase : IInstanceContextProvider
    {
        private readonly DispatchRuntime _dispatchRuntime;

        internal InstanceContextProviderBase(DispatchRuntime dispatchRuntime)
        {
            _dispatchRuntime = dispatchRuntime;
        }

        internal static bool IsProviderSingleton(IInstanceContextProvider provider)
        {
            return false;
        }

        internal static bool IsProviderSessionful(IInstanceContextProvider provider)
        {
            return (provider is PerSessionInstanceContextProvider);
        }

        internal static IInstanceContextProvider GetProviderForMode(InstanceContextMode instanceMode, DispatchRuntime runtime)
        {

            switch (instanceMode)
            {
                case InstanceContextMode.PerSession:
                    return new PerSessionInstanceContextProvider(runtime);
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        public abstract InstanceContext GetExistingInstanceContext(Message message, IContextChannel channel);

        internal ServiceChannel GetServiceChannelFromProxy(IContextChannel channel)
        {
            ServiceChannel serviceChannel = channel as ServiceChannel;
            if (serviceChannel == null)
            {
                serviceChannel = ServiceChannelFactory.GetServiceChannel(channel);
            }
            return serviceChannel;
        }
    }
}
