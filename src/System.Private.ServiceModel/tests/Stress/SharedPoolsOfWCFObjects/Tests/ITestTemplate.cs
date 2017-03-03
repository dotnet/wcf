// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using WcfService1;

namespace SharedPoolsOfWCFObjects
{
    public interface ITestTemplate<ChannelType, Params>
        where Params : IPoolTestParameter
    {
        EndpointAddress CreateEndPointAddress();
        Binding CreateBinding();
        ChannelFactory<ChannelType> CreateChannelFactory();
        void CloseFactory(ChannelFactory<ChannelType> factory);
        Task CloseFactoryAsync(ChannelFactory<ChannelType> factory);
        ChannelType CreateChannel(ChannelFactory<ChannelType> factory);
        void OpenChannel(ChannelType channel);
        Task OpenChannelAsync(ChannelType channel);
        void CloseChannel(ChannelType channel);
        Task CloseChannelAsync(ChannelType channel);
        bool ValidateChannel(ChannelType channel);
        bool ValidateFactory(ChannelFactory<ChannelType> factory);
        Func<ChannelType, int> UseChannel();
        Func<ChannelType, Task<int>> UseAsyncChannel();

        Params TestParameters { get; }
    }

    // This will likely expand into a more elaborate policy once we gain additional requirements
    // For now we only use it as an "hide exceptions on/off" switch
    public interface IExceptionPolicy
    {
        bool RelaxedExceptionPolicy { get; set; }
    }

    public interface IExceptionHandlingPolicyParameter
    {
        Func<Exception, bool> ExceptionHandler { get; }
        bool ReplaceInvalidPooledFactories { get; }
        bool ReplaceInvalidPooledChannels { get; }
        bool TrackServiceCallTimeouts { get; }
    }

    public interface IPoolTestParameter
    {
        int MaxPooledFactories { get; }
        int MaxPooledChannels { get; }
        int PreemptiveCloseChannelTimeout { get; }
        int PreemptiveCloseFactoryTimeout { get; }
    }


    public interface IStatsCollectingTestParameter
    {
        int SunnyDayMaxStatsSamples { get; }
        int RainyDayMaxStatsSamples { get; }
    }
}
