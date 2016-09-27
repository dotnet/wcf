// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public static class OperationBehaviorTest
{
    [WcfFact]
    public static void IOperationBehavior_Methods_AreCalled()
    {
        DuplexClientBase<ICustomOperationBehaviorDuplexService> duplexService = null;
        ICustomOperationBehaviorDuplexService proxy = null;

        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.None;

        WcfDuplexServiceCallback callbackService = new WcfDuplexServiceCallback();
        InstanceContext context = new InstanceContext(callbackService);

        duplexService = new MyDuplexClientBase<ICustomOperationBehaviorDuplexService>(context, binding, new EndpointAddress(FakeAddress.TcpAddress));
        proxy = duplexService.ChannelFactory.CreateChannel();

        // Wait to validate until the process has been given a reasonable time to complete.
        Task[] taskCollection = { MyOperationBehavior.validateMethodTcs.Task, MyOperationBehavior.addBindingParametersMethodTcs.Task, MyOperationBehavior.applyClientBehaviorMethodTcs.Task };
        bool waitAll = Task.WaitAll(taskCollection, 250);

        Assert.True(MyOperationBehavior.errorBuilder.Length == 0, "Test case FAILED with errors: " + MyOperationBehavior.errorBuilder.ToString());
        Assert.True(waitAll, "None of the IOperationBehavior methods were called.");

        ((ICommunicationObject)proxy).Close();
        ((ICommunicationObject)duplexService).Close();
    }
}
