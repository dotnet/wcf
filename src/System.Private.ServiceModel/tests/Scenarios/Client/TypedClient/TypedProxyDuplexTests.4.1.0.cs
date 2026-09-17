// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public static class TypedProxyDuplexTests
{
    // ServiceContract typed proxy tests create a ChannelFactory using a provided [ServiceContract] Interface which...
    //       returns a generated proxy based on that Interface.
    // ChannelShape typed proxy tests create a ChannelFactory using a WCF understood channel shape which...
    //       returns a generated proxy based on the channel shape used, such as...
    //              IRequestChannel (for a request-reply message exchange pattern)
    //              IDuplexChannel (for a two-way duplex message exchange pattern)

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncTask_CallbackReturn()
    {
        DuplexChannelFactory<IWcfDuplexTaskReturnService> factory = null;
        Guid guid = Guid.NewGuid();

        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.None;

        DuplexTaskReturnServiceCallback callbackService = new DuplexTaskReturnServiceCallback();
        InstanceContext context = new InstanceContext(callbackService);

        try
        {
            factory = new DuplexChannelFactory<IWcfDuplexTaskReturnService>(context, binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_TaskReturn_Address));
            IWcfDuplexTaskReturnService serviceProxy = factory.CreateChannel();

            Task<Guid> task = serviceProxy.Ping(guid);

            Guid returnedGuid = task.Result;

            Assert.Equal(guid, returnedGuid);

            factory.Close();
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncTask_CallbackDispatchRuntime_MessageInspector_CorrelationStatePreserved()
    {
        CallbackDispatchInspectorBehavior behavior = new CallbackDispatchInspectorBehavior(addParameterInspector: false);
        Guid guid = Guid.NewGuid();

        Guid returnedGuid = CallDuplexTaskReturnService(guid, behavior);

        Assert.Equal(guid, returnedGuid);
        Assert.True(behavior.MessageInspector.AfterReceiveRequestCalled);
        Assert.True(behavior.MessageInspector.BeforeSendReplyCalled);
        Assert.Same(behavior.MessageInspector.CorrelationState, behavior.MessageInspector.CorrelationStateSeen);
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncTask_ClientAndCallbackDispatchRuntime_MultipleMessageInspectors_CorrelationStatesAndOrderPreserved()
    {
        const int InspectorCount = 3;
        MultipleMessageInspectorsBehavior behavior = new MultipleMessageInspectorsBehavior(InspectorCount);
        Guid guid = Guid.NewGuid();

        Guid returnedGuid = CallDuplexTaskReturnService(guid, behavior);

        Assert.Equal(guid, returnedGuid);

        int[] expectedOrder = new int[] { 0, 1, 2 };
        Assert.Equal(expectedOrder, behavior.ClientBeforeSendRequestOrder);
        Assert.Equal(expectedOrder, behavior.ClientAfterReceiveReplyOrder);
        Assert.Equal(expectedOrder, behavior.CallbackAfterReceiveRequestOrder);
        Assert.Equal(expectedOrder, behavior.CallbackBeforeSendReplyOrder);

        object[] correlationStates = behavior.ClientMessageInspectors
            .Select(inspector => inspector.CorrelationState)
            .Concat(behavior.CallbackMessageInspectors.Select(inspector => inspector.CorrelationState))
            .ToArray();
        Assert.Equal(InspectorCount * 2, correlationStates.Distinct().Count());

        foreach (OrderedClientMessageInspector inspector in behavior.ClientMessageInspectors)
        {
            Assert.Same(inspector.CorrelationState, inspector.CorrelationStateSeen);
        }

        foreach (OrderedCallbackDispatchMessageInspector inspector in behavior.CallbackMessageInspectors)
        {
            Assert.Same(inspector.CorrelationState, inspector.CorrelationStateSeen);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncTask_CallbackDispatchRuntime_MultipleMessageInspectors_NullAndObjectCorrelationStatesPreserved()
    {
        object expectedCorrelationState = new object();
        CallbackDispatchMessageInspectorsBehavior behavior = new CallbackDispatchMessageInspectorsBehavior(
            new object[] { null, expectedCorrelationState });
        Guid guid = Guid.NewGuid();

        Guid returnedGuid = CallDuplexTaskReturnService(guid, behavior);

        Assert.Equal(guid, returnedGuid);

        int[] expectedOrder = new int[] { 0, 1 };
        Assert.Equal(expectedOrder, behavior.AfterReceiveRequestOrder);
        Assert.Equal(expectedOrder, behavior.BeforeSendReplyOrder);

        Assert.Null(behavior.MessageInspectors[0].CorrelationStateReturned);
        Assert.Null(behavior.MessageInspectors[0].CorrelationStateSeen);
        Assert.Same(expectedCorrelationState, behavior.MessageInspectors[1].CorrelationStateReturned);
        Assert.Same(expectedCorrelationState, behavior.MessageInspectors[1].CorrelationStateSeen);
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncTask_CallbackDispatchRuntime_MessageAndParameterInspector_CorrelationStatesPreserved()
    {
        CallbackDispatchInspectorBehavior behavior = new CallbackDispatchInspectorBehavior(addParameterInspector: true);
        Guid guid = Guid.NewGuid();

        Guid returnedGuid = CallDuplexTaskReturnService(guid, behavior);

        Assert.Equal(guid, returnedGuid);
        Assert.True(behavior.MessageInspector.AfterReceiveRequestCalled);
        Assert.True(behavior.MessageInspector.BeforeSendReplyCalled);
        Assert.Same(behavior.MessageInspector.CorrelationState, behavior.MessageInspector.CorrelationStateSeen);
        Assert.True(behavior.ParameterInspector.BeforeCallCalled);
        Assert.True(behavior.ParameterInspector.AfterCallCalled);
        Assert.Same(behavior.ParameterInspector.CorrelationState, behavior.ParameterInspector.CorrelationStateSeen);
        Assert.NotSame(behavior.MessageInspector.CorrelationState, behavior.ParameterInspector.CorrelationState);
    }

    [WcfFact]
    [OuterLoop]
    public static void DuplexChanelFactory_Ctor_Type_Overload_E2E()
    {
        DuplexChannelFactory<IWcfDuplexTaskReturnService> factory = null;
        Guid guid = Guid.NewGuid();

        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.None;

        DuplexTaskReturnServiceCallback callbackService = new DuplexTaskReturnServiceCallback();
        InstanceContext context = new InstanceContext(callbackService);

        try
        {
            factory = new DuplexChannelFactory<IWcfDuplexTaskReturnService>(typeof(DuplexTaskReturnServiceCallback), binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_TaskReturn_Address));
            IWcfDuplexTaskReturnService serviceProxy = factory.CreateChannel(context);

            Task<Guid> task = serviceProxy.Ping(guid);

            Guid returnedGuid = task.Result;

            Assert.Equal(guid, returnedGuid);

            factory.Close();
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }
    }

    private static Guid CallDuplexTaskReturnService(Guid guid, IEndpointBehavior behavior)
    {
        DuplexChannelFactory<IWcfDuplexTaskReturnService> factory = null;

        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.None;

        DuplexTaskReturnServiceCallback callbackService = new DuplexTaskReturnServiceCallback();
        InstanceContext context = new InstanceContext(callbackService);

        try
        {
            factory = new DuplexChannelFactory<IWcfDuplexTaskReturnService>(context, binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_TaskReturn_Address));
            factory.Endpoint.EndpointBehaviors.Add(behavior);
            IWcfDuplexTaskReturnService serviceProxy = factory.CreateChannel();

            Guid returnedGuid = serviceProxy.Ping(guid).GetAwaiter().GetResult();

            factory.Close();
            return returnedGuid;
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }
    }

    private sealed class CallbackDispatchMessageInspectorsBehavior : IEndpointBehavior
    {
        public CallbackDispatchMessageInspectorsBehavior(object[] correlationStates)
        {
            MessageInspectors = new OrderedCallbackDispatchMessageInspector[correlationStates.Length];

            for (int i = 0; i < correlationStates.Length; i++)
            {
                MessageInspectors[i] = new OrderedCallbackDispatchMessageInspector(
                    i,
                    AfterReceiveRequestOrder,
                    BeforeSendReplyOrder,
                    correlationStates[i]);
            }
        }

        public OrderedCallbackDispatchMessageInspector[] MessageInspectors { get; }

        public List<int> AfterReceiveRequestOrder { get; } = new List<int>();

        public List<int> BeforeSendReplyOrder { get; } = new List<int>();

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            for (int i = 0; i < MessageInspectors.Length; i++)
            {
                clientRuntime.CallbackDispatchRuntime.MessageInspectors.Add(MessageInspectors[i]);
            }
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    private sealed class MultipleMessageInspectorsBehavior : IEndpointBehavior
    {
        public MultipleMessageInspectorsBehavior(int inspectorCount)
        {
            ClientMessageInspectors = new OrderedClientMessageInspector[inspectorCount];
            CallbackMessageInspectors = new OrderedCallbackDispatchMessageInspector[inspectorCount];

            for (int i = 0; i < inspectorCount; i++)
            {
                ClientMessageInspectors[i] = new OrderedClientMessageInspector(
                    i,
                    ClientBeforeSendRequestOrder,
                    ClientAfterReceiveReplyOrder);
                CallbackMessageInspectors[i] = new OrderedCallbackDispatchMessageInspector(
                    i,
                    CallbackAfterReceiveRequestOrder,
                    CallbackBeforeSendReplyOrder);
            }
        }

        public OrderedClientMessageInspector[] ClientMessageInspectors { get; }

        public OrderedCallbackDispatchMessageInspector[] CallbackMessageInspectors { get; }

        public List<int> ClientBeforeSendRequestOrder { get; } = new List<int>();

        public List<int> ClientAfterReceiveReplyOrder { get; } = new List<int>();

        public List<int> CallbackAfterReceiveRequestOrder { get; } = new List<int>();

        public List<int> CallbackBeforeSendReplyOrder { get; } = new List<int>();

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            for (int i = 0; i < ClientMessageInspectors.Length; i++)
            {
                clientRuntime.ClientMessageInspectors.Add(ClientMessageInspectors[i]);
                clientRuntime.CallbackDispatchRuntime.MessageInspectors.Add(CallbackMessageInspectors[i]);
            }
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    private sealed class OrderedClientMessageInspector : IClientMessageInspector
    {
        private readonly int _index;
        private readonly List<int> _beforeSendRequestOrder;
        private readonly List<int> _afterReceiveReplyOrder;

        public OrderedClientMessageInspector(int index, List<int> beforeSendRequestOrder, List<int> afterReceiveReplyOrder)
        {
            _index = index;
            _beforeSendRequestOrder = beforeSendRequestOrder;
            _afterReceiveReplyOrder = afterReceiveReplyOrder;
        }

        public object CorrelationState { get; } = new object();

        public object CorrelationStateSeen { get; private set; }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            _beforeSendRequestOrder.Add(_index);
            return CorrelationState;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            _afterReceiveReplyOrder.Add(_index);
            CorrelationStateSeen = correlationState;
        }
    }

    private sealed class OrderedCallbackDispatchMessageInspector : IDispatchMessageInspector
    {
        private readonly int _index;
        private readonly List<int> _afterReceiveRequestOrder;
        private readonly List<int> _beforeSendReplyOrder;

        public OrderedCallbackDispatchMessageInspector(int index, List<int> afterReceiveRequestOrder, List<int> beforeSendReplyOrder)
            : this(index, afterReceiveRequestOrder, beforeSendReplyOrder, new object())
        {
        }

        public OrderedCallbackDispatchMessageInspector(
            int index,
            List<int> afterReceiveRequestOrder,
            List<int> beforeSendReplyOrder,
            object correlationState)
        {
            _index = index;
            _afterReceiveRequestOrder = afterReceiveRequestOrder;
            _beforeSendReplyOrder = beforeSendReplyOrder;
            CorrelationState = correlationState;
        }

        public object CorrelationState { get; }

        public object CorrelationStateReturned { get; private set; }

        public object CorrelationStateSeen { get; private set; }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            _afterReceiveRequestOrder.Add(_index);
            CorrelationStateReturned = CorrelationState;
            return CorrelationState;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            _beforeSendReplyOrder.Add(_index);
            CorrelationStateSeen = correlationState;
        }
    }

    private sealed class CallbackDispatchInspectorBehavior : IEndpointBehavior
    {
        private const BindingFlags InstanceMembers = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private readonly bool _addParameterInspector;

        public CallbackDispatchInspectorBehavior(bool addParameterInspector)
        {
            _addParameterInspector = addParameterInspector;
        }

        public CallbackDispatchMessageInspector MessageInspector { get; } = new CallbackDispatchMessageInspector();

        public CallbackDispatchParameterInspector ParameterInspector { get; } = new CallbackDispatchParameterInspector();

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            DispatchRuntime callbackDispatchRuntime = clientRuntime.CallbackDispatchRuntime;
            callbackDispatchRuntime.MessageInspectors.Add(MessageInspector);

            if (_addParameterInspector)
            {
                AddParameterInspector(callbackDispatchRuntime, ParameterInspector);
            }
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        private static void AddParameterInspector(DispatchRuntime callbackDispatchRuntime, IParameterInspector inspector)
        {
            PropertyInfo operationsProperty = typeof(DispatchRuntime).GetProperty("Operations", InstanceMembers);
            Assert.NotNull(operationsProperty);

            IEnumerable operations = operationsProperty.GetValue(callbackDispatchRuntime) as IEnumerable;
            Assert.NotNull(operations);

            int operationCount = 0;
            foreach (object operation in operations)
            {
                PropertyInfo parameterInspectorsProperty = operation.GetType().GetProperty("ParameterInspectors", InstanceMembers);
                Assert.NotNull(parameterInspectorsProperty);

                object parameterInspectors = parameterInspectorsProperty.GetValue(operation);
                Assert.NotNull(parameterInspectors);

                MethodInfo addMethod = parameterInspectors.GetType().GetMethod("Add", new Type[] { typeof(IParameterInspector) });
                Assert.NotNull(addMethod);

                addMethod.Invoke(parameterInspectors, new object[] { inspector });
                operationCount++;
            }

            Assert.True(operationCount > 0);
        }
    }

    private sealed class CallbackDispatchMessageInspector : IDispatchMessageInspector
    {
        public object CorrelationState { get; } = new object();

        public bool AfterReceiveRequestCalled { get; private set; }

        public bool BeforeSendReplyCalled { get; private set; }

        public object CorrelationStateSeen { get; private set; }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            AfterReceiveRequestCalled = true;
            return CorrelationState;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            BeforeSendReplyCalled = true;
            CorrelationStateSeen = correlationState;
        }
    }

    private sealed class CallbackDispatchParameterInspector : IParameterInspector
    {
        public object CorrelationState { get; } = new object();

        public bool BeforeCallCalled { get; private set; }

        public bool AfterCallCalled { get; private set; }

        public object CorrelationStateSeen { get; private set; }

        public object BeforeCall(string operationName, object[] inputs)
        {
            BeforeCallCalled = true;
            return CorrelationState;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            AfterCallCalled = true;
            CorrelationStateSeen = correlationState;
        }
    }
}
