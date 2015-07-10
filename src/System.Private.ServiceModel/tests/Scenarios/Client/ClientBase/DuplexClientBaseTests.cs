// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public static class DuplexClientBaseTests
{
    static TimeSpan maxTestWaitTime = TimeSpan.FromSeconds(10);

    [Fact]
    [OuterLoop]
    public static void DuplexClientBaseOfT_OverNetTcp_Call()
    {
        DuplexClientBase<IWcfDuplexService> duplexService = null;
        StringBuilder errorBuilder = new StringBuilder();
        Guid guid = Guid.NewGuid();

        try
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;

            WcfDuplexServiceCallback callbackService = new WcfDuplexServiceCallback();
            InstanceContext context = new InstanceContext(callbackService);

            duplexService = new MyDuplexClientBase<IWcfDuplexService>(context, binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_Callback_Address));
            IWcfDuplexService proxy = duplexService.ChannelFactory.CreateChannel();

            // Ping on another thread.
            Task.Run(() => proxy.Ping(guid));
            Guid returnedGuid = callbackService.CallbackGuid;

            if (guid != returnedGuid)
            {
                errorBuilder.AppendLine(String.Format("The sent GUID does not match the returned GUID. Sent: {0} Received: {1}", guid, returnedGuid));
            }

            ((ICommunicationObject)duplexService).Close();
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
            for (Exception innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
            {
                errorBuilder.AppendLine(String.Format("Inner exception: {0}", innerException.ToString()));
            }
        }
        finally
        {
            if (duplexService != null && duplexService.State != CommunicationState.Closed)
            {
                duplexService.Abort();
            }
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: DuplexClientBaseOfT_OverNetTcp_Call FAILED with the following errors: {0}", errorBuilder));
    }

    public class WcfDuplexServiceCallback : IWcfDuplexServiceCallback
    {
        private TaskCompletionSource<Guid> _tcs;

        public WcfDuplexServiceCallback()
        {
            _tcs = new TaskCompletionSource<Guid>();
        }

        public Guid CallbackGuid
        {
            get
            {
                if (_tcs.Task.Wait(maxTestWaitTime))
                {
                    return _tcs.Task.Result;
                }
                throw new TimeoutException(string.Format("Not completed within the alloted time of {0}", maxTestWaitTime));
            }
        }

        public void OnPingCallback(Guid guid)
        {
            // Set the result in an async task with a 100ms delay to prevent a race condition
            // where the OnPingCallback hasn't sent the reply to the server before the channel is closed.
            Task.Run(async () =>
            {
                await Task.Delay(100);
                _tcs.SetResult(guid);
            });
        }
    }
}

