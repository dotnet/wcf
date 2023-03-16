// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;
using System.Runtime;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    public class ServiceChannelProxy : DispatchProxy, ICommunicationObject, IChannel, IClientChannel, IOutputChannel, IRequestChannel, IServiceChannel, IDuplexContextChannel
    {
        private const String activityIdSlotName = "E2ETrace.ActivityID";
        private Type _proxiedType;
        private ServiceChannel _serviceChannel;
        private ImmutableClientRuntime _proxyRuntime;
        private MethodDataCache _methodDataCache;

        // ServiceChannelProxy serves 2 roles.  It is the TChannel proxy called by the client,
        // and it is also the handler of those calls that dispatches them to the appropriate service channel.
        // In .Net Remoting terms, it is conceptually the same as a RealProxy and a TransparentProxy combined.
        internal static TChannel CreateProxy<TChannel>(MessageDirection direction, ServiceChannel serviceChannel)
        {
            TChannel proxy = DispatchProxy.Create<TChannel, ServiceChannelProxy>();
            if (proxy == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.FailedToCreateTypedProxy, typeof(TChannel))));
            }

            ServiceChannelProxy channelProxy = (ServiceChannelProxy)(object)proxy;
            channelProxy._proxiedType = typeof(TChannel);
            channelProxy._serviceChannel = serviceChannel;
            channelProxy._proxyRuntime = serviceChannel.ClientRuntime.GetRuntime();
            channelProxy._methodDataCache = new MethodDataCache();
            return proxy;
        }

        //Workaround is to set the activityid in remoting call's LogicalCallContext

        // Override ToString() to reveal only the expected proxy type, not the generated one
        public override string ToString()
        {
            return _proxiedType.ToString();
        }

        private MethodData GetMethodData(MethodCall methodCall)
        {
            MethodData methodData;
            MethodBase method = methodCall.MethodBase;
            if (_methodDataCache.TryGetMethodData(method, out methodData))
            {
                return methodData;
            }

            bool canCacheMessageData;

            Type declaringType = method.DeclaringType;
            if (declaringType == typeof(object) && method == typeof(object).GetMethod("GetType"))
            {
                canCacheMessageData = true;
                methodData = new MethodData(method, MethodType.GetType);
            }
            else if (declaringType.IsAssignableFrom(_serviceChannel.GetType()))
            {
                canCacheMessageData = true;
                methodData = new MethodData(method, MethodType.Channel);
            }
            else
            {
                ProxyOperationRuntime operation = _proxyRuntime.GetOperation(method, methodCall.Args, out canCacheMessageData);

                if (operation == null)
                {
                    if (_serviceChannel.Factory != null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.SFxMethodNotSupported1, method.Name)));
                    }
                    else
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.SFxMethodNotSupportedOnCallback1, method.Name)));
                    }
                }

                MethodType methodType;

                if (operation.IsTaskCall(methodCall))
                {
                    methodType = MethodType.TaskService;
                }
                else if (operation.IsSyncCall(methodCall))
                {
                    methodType = MethodType.Service;
                }
                else if (operation.IsBeginCall(methodCall))
                {
                    methodType = MethodType.BeginService;
                }
                else
                {
                    methodType = MethodType.EndService;
                }

                methodData = new MethodData(method, methodType, operation);
            }

            if (canCacheMessageData)
            {
                _methodDataCache.SetMethodData(methodData);
            }

            return methodData;
        }

        internal ServiceChannel GetServiceChannel()
        {
            return _serviceChannel;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (args == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(args));
            }

            if (targetMethod == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.InvalidTypedProxyMethodHandle, _proxiedType.Name)));
            }

            MethodCall methodCall = new MethodCall(targetMethod, args);
            MethodData methodData = GetMethodData(methodCall);

            switch (methodData.MethodType)
            {
                case MethodType.Service:
                    return InvokeService(methodCall, methodData.Operation);
                case MethodType.BeginService:
                    return InvokeBeginService(methodCall, methodData.Operation);
                case MethodType.EndService:
                    return InvokeEndService(methodCall, methodData.Operation);
                case MethodType.TaskService:
                    return InvokeTaskService(methodCall, methodData.Operation);
                case MethodType.Channel:
                    return InvokeChannel(methodCall);
                case MethodType.GetType:
                    return InvokeGetType(methodCall);
                default:
                    Fx.Assert("Invalid proxy method type");
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "Invalid proxy method type")));
            }
        }

        internal static class TaskCreator
        {
            public static Task CreateTask(ServiceChannel channel, MethodCall methodCall, ProxyOperationRuntime operation)
            {
                if (operation.TaskTResult == ServiceReflector.VoidType)
                {
                    return TaskCreator.CreateTask(channel, operation, methodCall.InArgs);
                }
                return TaskCreator.CreateGenericTask(channel, operation, methodCall.InArgs);
            }

            private static Task CreateGenericTask(ServiceChannel channel, ProxyOperationRuntime operation, object[] inputParameters)
            {
                TaskCompletionSourceProxy tcsp = new TaskCompletionSourceProxy(operation.TaskTResult);
                bool completedCallback = false;
                Action<IAsyncResult> endCallDelegate = (asyncResult) =>
                {
                    Contract.Assert(asyncResult != null, "'asyncResult' MUST NOT be NULL.");
                    completedCallback = true;
                    OperationContext originalOperationContext = OperationContext.Current;
                    OperationContext.Current = asyncResult.AsyncState as OperationContext;
                    try
                    {
                        object result = channel.EndCall(operation.Action, Array.Empty<object>(), asyncResult);
                        OperationContext.Current = originalOperationContext;
                        tcsp.TrySetResult(result);
                    }
                    catch (Exception e)
                    {
                        OperationContext.Current = originalOperationContext;
                        tcsp.TrySetException(e);
                    }
                };

                try
                {
                    IAsyncResult ar = ServiceChannel.BeginCall(channel, operation, inputParameters, new AsyncCallback(endCallDelegate), OperationContext.Current);
                    if (ar.CompletedSynchronously && !completedCallback)
                    {
                        endCallDelegate(ar);
                    }
                }
                catch (Exception e)
                {
                    tcsp.TrySetException(e);
                }

                return tcsp.Task;
            }

            private static Task CreateTask(ServiceChannel channel, ProxyOperationRuntime operation, object[] inputParameters)
            {
                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
                bool completedCallback = false;

                Action<IAsyncResult> endCallDelegate = (asyncResult) =>
                {
                    Contract.Assert(asyncResult != null, "'asyncResult' MUST NOT be NULL.");
                    completedCallback = true;
                    OperationContext originalOperationContext = OperationContext.Current;
                    OperationContext.Current = asyncResult.AsyncState as OperationContext;
                    try
                    {
                        channel.EndCall(operation.Action, Array.Empty<object>(), asyncResult);
                        OperationContext.Current = originalOperationContext;
                        tcs.TrySetResult(null);
                    }
                    catch (Exception e)
                    {
                        OperationContext.Current = originalOperationContext;
                        tcs.TrySetException(e);
                    }
                };

                try
                {
                    IAsyncResult ar = ServiceChannel.BeginCall(channel, operation, inputParameters, new AsyncCallback(endCallDelegate), OperationContext.Current);
                    if (ar.CompletedSynchronously && !completedCallback)
                    {
                        endCallDelegate(ar);
                    }
                }
                catch (Exception e)
                {
                    tcs.TrySetException(e);
                }

                return tcs.Task;
            }
        }

        private class TaskCompletionSourceProxy
        {
            private TaskCompletionSourceInfo _tcsInfo;
            private object _tcsInstance;

            public TaskCompletionSourceProxy(Type resultType)
            {
                _tcsInfo = TaskCompletionSourceInfo.GetTaskCompletionSourceInfo(resultType);
                _tcsInstance = _tcsInfo.CreateInstance();
            }

            public Task Task { get { return (Task)_tcsInfo.TaskProperty.GetValue(_tcsInstance); } }

            public bool TrySetResult(object result)
            {
                return (bool)_tcsInfo.TrySetResultMethod.Invoke(_tcsInstance, new object[] { result });
            }

            public bool TrySetException(Exception exception)
            {
                return (bool)_tcsInfo.TrySetExceptionMethod.Invoke(_tcsInstance, new object[] { exception });
            }

            public bool TrySetCanceled()
            {
                return (bool)_tcsInfo.TrySetCanceledMethod.Invoke(_tcsInstance, Array.Empty<object>());
            }
        }

        private class TaskCompletionSourceInfo
        {
            private static ConcurrentDictionary<Type, TaskCompletionSourceInfo> s_cache = new ConcurrentDictionary<Type, TaskCompletionSourceInfo>();
            private static MethodInfo s_createTcsOpenGenericMethod = typeof(TaskCompletionSourceInfo).GetMethod(nameof(CreateTaskCompletionSource), BindingFlags.Static | BindingFlags.NonPublic);
            private Func<object> _createDelegate;

            public TaskCompletionSourceInfo(Type resultType)
            {
                ResultType = resultType;
                Type tcsType = typeof(TaskCompletionSource<>);
                GenericType = tcsType.MakeGenericType(new Type[] { resultType });
                TaskProperty = GenericType.GetTypeInfo().GetDeclaredProperty("Task");
                TrySetResultMethod = GenericType.GetTypeInfo().GetDeclaredMethod("TrySetResult");
                TrySetExceptionMethod = GenericType.GetRuntimeMethod("TrySetException", new Type[] { typeof(Exception) });
                TrySetCanceledMethod = GenericType.GetRuntimeMethod("TrySetCanceled", Array.Empty<Type>());
            }

            public Type ResultType { get; private set; }
            public Type GenericType { get; private set; }
            public PropertyInfo TaskProperty { get; private set; }
            public MethodInfo TrySetResultMethod { get; private set; }
            public MethodInfo TrySetExceptionMethod { get; set; }
            public MethodInfo TrySetCanceledMethod { get; set; }

            internal object CreateInstance()
            {
                if (_createDelegate == null)
                {
                    var createTcsGenericMethod = s_createTcsOpenGenericMethod.MakeGenericMethod(ResultType);
                    _createDelegate = (Func<object>)Delegate.CreateDelegate(typeof(Func<object>), createTcsGenericMethod);
                }

                return _createDelegate();
            }

            private static object CreateTaskCompletionSource<T>()
            {
                return new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            }

            public static TaskCompletionSourceInfo GetTaskCompletionSourceInfo(Type resultType)
            {
                return s_cache.GetOrAdd(resultType, t => new TaskCompletionSourceInfo(t));
            }
        }

        private object InvokeTaskService(MethodCall methodCall, ProxyOperationRuntime operation)
        {
            Task task = TaskCreator.CreateTask(_serviceChannel, methodCall, operation);
            return task;
        }

        private object InvokeChannel(MethodCall methodCall)
        {
            string activityName = null;
            ActivityType activityType = ActivityType.Unknown;
            if (DiagnosticUtility.ShouldUseActivity)
            {
                if (ServiceModelActivity.Current == null ||
                    ServiceModelActivity.Current.ActivityType != ActivityType.Close)
                {
                    MethodData methodData = GetMethodData(methodCall);
                    if (methodData.MethodBase.DeclaringType == typeof(ICommunicationObject)
                        && methodData.MethodBase.Name.Equals("Close", StringComparison.Ordinal))
                    {
                        activityName = SRP.Format(SRP.ActivityClose, _serviceChannel.GetType().FullName);
                        activityType = ActivityType.Close;
                    }
                }
            }

            using (ServiceModelActivity activity = string.IsNullOrEmpty(activityName) ? null : ServiceModelActivity.CreateBoundedActivity())
            {
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    ServiceModelActivity.Start(activity, activityName, activityType);
                }
                return ExecuteMessage(_serviceChannel, methodCall);
            }
        }

        private object InvokeGetType(MethodCall methodCall)
        {
            return _proxiedType;
        }

        private object InvokeBeginService(MethodCall methodCall, ProxyOperationRuntime operation)
        {
            AsyncCallback callback;
            object asyncState;
            object[] ins = operation.MapAsyncBeginInputs(methodCall, out callback, out asyncState);
            object ret = _serviceChannel.BeginCall(operation.Action, operation.IsOneWay, operation, ins, callback, asyncState);
            return ret;
        }

        private object InvokeEndService(MethodCall methodCall, ProxyOperationRuntime operation)
        {
            IAsyncResult result;
            object[] outs;
            operation.MapAsyncEndInputs(methodCall, out result, out outs);
            object ret = _serviceChannel.EndCall(operation.Action, outs, result);
            operation.MapAsyncOutputs(methodCall, outs, ref ret);
            return ret;
        }

        private object InvokeService(MethodCall methodCall, ProxyOperationRuntime operation)
        {
            object[] outs;
            object[] ins = operation.MapSyncInputs(methodCall, out outs);
            object ret = _serviceChannel.Call(operation.Action, operation.IsOneWay, operation, ins, outs);
            operation.MapSyncOutputs(methodCall, outs, ref ret);
            return ret;
        }

        private object ExecuteMessage(object target, MethodCall methodCall)
        {
            MethodBase targetMethod = methodCall.MethodBase;

            object[] args = methodCall.Args;
            object returnValue = null;
            try
            {
                returnValue = targetMethod.Invoke(target, args);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

            return returnValue;
        }

        internal class MethodDataCache
        {
            private MethodData[] _methodDatas;

            public MethodDataCache()
            {
                _methodDatas = new MethodData[4];
            }

            private object ThisLock
            {
                get { return this; }
            }

            public bool TryGetMethodData(MethodBase method, out MethodData methodData)
            {
                lock (ThisLock)
                {
                    MethodData[] methodDatas = _methodDatas;
                    int index = FindMethod(methodDatas, method);
                    if (index >= 0)
                    {
                        methodData = methodDatas[index];
                        return true;
                    }
                    else
                    {
                        methodData = new MethodData();
                        return false;
                    }
                }
            }

            private static int FindMethod(MethodData[] methodDatas, MethodBase methodToFind)
            {
                for (int i = 0; i < methodDatas.Length; i++)
                {
                    MethodBase method = methodDatas[i].MethodBase;
                    if (method == null)
                    {
                        break;
                    }
                    if (method == methodToFind)
                    {
                        return i;
                    }
                }
                return -1;
            }

            public void SetMethodData(MethodData methodData)
            {
                lock (ThisLock)
                {
                    int index = FindMethod(_methodDatas, methodData.MethodBase);
                    if (index < 0)
                    {
                        for (int i = 0; i < _methodDatas.Length; i++)
                        {
                            if (_methodDatas[i].MethodBase == null)
                            {
                                _methodDatas[i] = methodData;
                                return;
                            }
                        }
                        MethodData[] newMethodDatas = new MethodData[_methodDatas.Length * 2];
                        Array.Copy(_methodDatas, newMethodDatas, _methodDatas.Length);
                        newMethodDatas[_methodDatas.Length] = methodData;
                        _methodDatas = newMethodDatas;
                    }
                }
            }
        }

        internal enum MethodType
        {
            Service,
            BeginService,
            EndService,
            Channel,
            Object,
            GetType,
            TaskService
        }

        internal struct MethodData
        {
            private ProxyOperationRuntime _operation;

            public MethodData(MethodBase methodBase, MethodType methodType)
                : this(methodBase, methodType, null)
            {
            }

            public MethodData(MethodBase methodBase, MethodType methodType, ProxyOperationRuntime operation)
            {
                MethodBase = methodBase;
                MethodType = methodType;
                _operation = operation;
            }

            public MethodBase MethodBase { get; }

            public MethodType MethodType { get; }

            public ProxyOperationRuntime Operation
            {
                get { return _operation; }
            }
        }

        #region Channel interfaces
        // These channel methods exist only to implement additional channel interfaces for ServiceChannelProxy.
        // This is required because clients can down-cast typed proxies to the these channel interfaces.
        // On the desktop, the .Net Remoting layer allowed that type cast, and subsequent calls against the
        // interface went back through the RealProxy and invoked the underlying ServiceChannel.
        // Net Native and CoreClr do not have .Net Remoting and therefore cannot use that mechanism.
        // But because typed proxies derive from ServiceChannelProxy, implementing these interfaces
        // on ServiceChannelProxy permits casting the typed proxy to these interfaces.
        // All interface implementations delegate directly to the underlying ServiceChannel.
        T IChannel.GetProperty<T>()
        {
            return _serviceChannel.GetProperty<T>();
        }

        CommunicationState ICommunicationObject.State
        {
            get { return _serviceChannel.State; }
        }

        event EventHandler ICommunicationObject.Closed
        {
            add { _serviceChannel.Closed += value; }
            remove { _serviceChannel.Closed -= value; }
        }

        event EventHandler ICommunicationObject.Closing
        {
            add { _serviceChannel.Closing += value; }
            remove { _serviceChannel.Closing -= value; }
        }

        event EventHandler ICommunicationObject.Faulted
        {
            add { _serviceChannel.Faulted += value; }
            remove { _serviceChannel.Faulted -= value; }
        }

        event EventHandler ICommunicationObject.Opened
        {
            add { _serviceChannel.Opened += value; }
            remove { _serviceChannel.Opened -= value; }
        }

        event EventHandler ICommunicationObject.Opening
        {
            add { _serviceChannel.Opening += value; }
            remove { _serviceChannel.Opening -= value; }
        }

        void ICommunicationObject.Abort()
        {
            _serviceChannel.Abort();
        }

        void ICommunicationObject.Close()
        {
            _serviceChannel.Close();
        }

        void ICommunicationObject.Close(TimeSpan timeout)
        {
            _serviceChannel.Close(timeout);
        }

        IAsyncResult ICommunicationObject.BeginClose(AsyncCallback callback, object state)
        {
            return _serviceChannel.BeginClose(callback, state);
        }

        IAsyncResult ICommunicationObject.BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _serviceChannel.BeginClose(timeout, callback, state);
        }

        void ICommunicationObject.EndClose(IAsyncResult result)
        {
            _serviceChannel.EndClose(result);
        }

        void ICommunicationObject.Open()
        {
            _serviceChannel.Open();
        }

        void ICommunicationObject.Open(TimeSpan timeout)
        {
            _serviceChannel.Open(timeout);
        }

        IAsyncResult ICommunicationObject.BeginOpen(AsyncCallback callback, object state)
        {
            return _serviceChannel.BeginOpen(callback, state);
        }

        IAsyncResult ICommunicationObject.BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _serviceChannel.BeginOpen(timeout, callback, state);
        }

        void ICommunicationObject.EndOpen(IAsyncResult result)
        {
            _serviceChannel.EndOpen(result);
        }

        bool IClientChannel.AllowInitializationUI
        {
            get
            {
                return ((IClientChannel)_serviceChannel).AllowInitializationUI;
            }
            set
            {
                ((IClientChannel)_serviceChannel).AllowInitializationUI = value;
            }
        }

        bool IClientChannel.DidInteractiveInitialization
        {
            get { return ((IClientChannel)_serviceChannel).DidInteractiveInitialization; }
        }

        Uri IClientChannel.Via
        {
            get { return _serviceChannel.Via; }
        }

        event EventHandler<UnknownMessageReceivedEventArgs> IClientChannel.UnknownMessageReceived
        {
            add { ((IClientChannel)_serviceChannel).UnknownMessageReceived += value; }
            remove { ((IClientChannel)_serviceChannel).UnknownMessageReceived -= value; }
        }

        IAsyncResult IClientChannel.BeginDisplayInitializationUI(AsyncCallback callback, object state)
        {
            return _serviceChannel.BeginDisplayInitializationUI(callback, state);
        }

        void IClientChannel.DisplayInitializationUI()
        {
            _serviceChannel.DisplayInitializationUI();
        }

        void IClientChannel.EndDisplayInitializationUI(IAsyncResult result)
        {
            _serviceChannel.EndDisplayInitializationUI(result);
        }

        void IDisposable.Dispose()
        {
            ((IClientChannel)_serviceChannel).Dispose();
        }

        bool IContextChannel.AllowOutputBatching
        {
            get
            {
                return ((IContextChannel)_serviceChannel).AllowOutputBatching;
            }
            set
            {
                ((IContextChannel)_serviceChannel).AllowOutputBatching = value;
            }
        }

        IInputSession IContextChannel.InputSession
        {
            get { return ((IContextChannel)_serviceChannel).InputSession; }
        }

        EndpointAddress IContextChannel.LocalAddress
        {
            get { return ((IContextChannel)_serviceChannel).LocalAddress; }
        }

        TimeSpan IContextChannel.OperationTimeout
        {
            get
            {
                return ((IContextChannel)_serviceChannel).OperationTimeout;
            }
            set
            {
                ((IContextChannel)_serviceChannel).OperationTimeout = value;
            }
        }

        IOutputSession IContextChannel.OutputSession
        {
            get { return ((IContextChannel)_serviceChannel).OutputSession; }
        }

        EndpointAddress IOutputChannel.RemoteAddress
        {
            get { return ((IContextChannel)_serviceChannel).RemoteAddress; }
        }

        Uri IOutputChannel.Via
        {
            get { return _serviceChannel.Via; }
        }

        EndpointAddress IContextChannel.RemoteAddress
        {
            get { return ((IContextChannel)_serviceChannel).RemoteAddress; }
        }

        string IContextChannel.SessionId
        {
            get { return ((IContextChannel)_serviceChannel).SessionId; }
        }

        IExtensionCollection<IContextChannel> IExtensibleObject<IContextChannel>.Extensions
        {
            get { return ((IContextChannel)_serviceChannel).Extensions; }
        }

        IAsyncResult IOutputChannel.BeginSend(Message message, AsyncCallback callback, object state)
        {
            return _serviceChannel.BeginSend(message, callback, state);
        }

        IAsyncResult IOutputChannel.BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _serviceChannel.BeginSend(message, timeout, callback, state);
        }

        void IOutputChannel.EndSend(IAsyncResult result)
        {
            _serviceChannel.EndSend(result);
        }

        void IOutputChannel.Send(Message message)
        {
            _serviceChannel.Send(message);
        }

        void IOutputChannel.Send(Message message, TimeSpan timeout)
        {
            _serviceChannel.Send(message, timeout);
        }

        Message IRequestChannel.Request(Message message)
        {
            return _serviceChannel.Request(message);
        }

        Message IRequestChannel.Request(Message message, TimeSpan timeout)
        {
            return _serviceChannel.Request(message, timeout);
        }

        IAsyncResult IRequestChannel.BeginRequest(Message message, AsyncCallback callback, object state)
        {
            return _serviceChannel.BeginRequest(message, callback, state);
        }

        IAsyncResult IRequestChannel.BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _serviceChannel.BeginRequest(message, timeout, callback, state);
        }

        Message IRequestChannel.EndRequest(IAsyncResult result)
        {
            return _serviceChannel.EndRequest(result);
        }

        public IAsyncResult BeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return ((IDuplexContextChannel)_serviceChannel).BeginCloseOutputSession(timeout, callback, state);
        }

        public void EndCloseOutputSession(IAsyncResult result)
        {
            ((IDuplexContextChannel)_serviceChannel).EndCloseOutputSession(result);
        }

        public void CloseOutputSession(TimeSpan timeout)
        {
            ((IDuplexContextChannel)_serviceChannel).CloseOutputSession(timeout);
        }

        EndpointAddress IRequestChannel.RemoteAddress
        {
            get { return ((IContextChannel)_serviceChannel).RemoteAddress; }
        }

        Uri IRequestChannel.Via
        {
            get { return _serviceChannel.Via; }
        }

        Uri IServiceChannel.ListenUri
        {
            get { return _serviceChannel.ListenUri; }
        }

        public bool AutomaticInputSessionShutdown
        {
            get
            {
                return ((IDuplexContextChannel)_serviceChannel).AutomaticInputSessionShutdown;
            }

            set
            {
                ((IDuplexContextChannel)_serviceChannel).AutomaticInputSessionShutdown = value;
            }
        }

        public InstanceContext CallbackInstance
        {
            get
            {
                return ((IDuplexContextChannel)_serviceChannel).CallbackInstance;
            }

            set
            {
                ((IDuplexContextChannel)_serviceChannel).CallbackInstance = value;
            }
        }
        #endregion // Channel interfaces
    }
}
