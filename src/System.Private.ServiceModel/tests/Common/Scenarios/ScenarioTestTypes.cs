// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TestTypes
{
    // MessageContract-based class used for an IFeedbackService request
    [MessageContract(IsWrapped = false)]
    public class FeedbackRequest
    {
        [MessageBodyMember(Name = "Feedback", Namespace = "http://app.my.com/MyFeedback", Order = 0)]
        public FeedbackRequestBody Body;

        public FeedbackRequest()
        {
        }

        public FeedbackRequest(FeedbackRequestBody Body)
        {
            this.Body = Body;
        }
    }

    // DataContract-based class used for the body of the MessageContract-based FeedbackRequest
    [DataContract(Namespace = "http://app.my.com/MyFeedback")]
    public class FeedbackRequestBody
    {
        [DataMember(EmitDefaultValue = false, Order = 1)]
        public string email;
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public string Suggestion;
        [DataMember(EmitDefaultValue = false, Order = 3)]
        public string sysInfo;
        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string URL;

        public FeedbackRequestBody()
        {
        }

        public FeedbackRequestBody(string Suggestion, string email, string URL, string sysInfo)
        {
            this.Suggestion = Suggestion;
            this.email = email;
            this.URL = URL;
            this.sysInfo = sysInfo;
        }
    }

    // MessageContract-based class used for an IFeedbackService response
    [MessageContract(IsWrapped = false)]
    public class FeedbackResponse
    {
        [MessageBodyMember(Name = "FeedbackResponse", Namespace = "http://app.my.com/MyFeedback", Order = 0)]
        public FeedbackResponseBody Body;

        public FeedbackResponse()
        {
        }

        public FeedbackResponse(FeedbackResponseBody Body)
        {
            this.Body = Body;
        }
    }

    // DataContract-based class used for the body of the MessageContract-based FeedbackResponse
    [DataContract(Namespace = "http://app.my.com/MyFeedback")]
    public class FeedbackResponseBody
    {
        [DataMember(EmitDefaultValue = false, Order = 0)]
        public string FeedbackResult;

        public FeedbackResponseBody()
        {
        }

        public FeedbackResponseBody(string FeedbackResult)
        {
            this.FeedbackResult = FeedbackResult;
        }
    }

    [DataContract(Name = "ResultOf{0}", Namespace = "http://www.contoso.com/wcfnamespace")]
    public class ResultObject<TEntity>
    {
        private string _resultMessage;

        public static ResultObject<T> CreateSuccessObject<T>()
        {
            return new ResultObject<T>
            {
                Result = default(T),
                ErrorCode = (int)TestTypes.ErrorCode.Ok,
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                ResultMessage = TestTypes.ResultMessage.GetErrorDescription(TestTypes.ErrorCode.Ok)
            };
        }

        public static ResultObject<T> CreateFailureObject<T>()
        {
            return new ResultObject<T>
            {
                Result = default(T),
                ErrorCode = (int)TestTypes.ErrorCode.UserNotAuthenticated,
                HttpStatusCode = System.Net.HttpStatusCode.Unauthorized,
                ResultMessage = TestTypes.ResultMessage.GetErrorDescription(TestTypes.ErrorCode.UserNotAuthenticated)
            };
        }

        [DataMember]
        public int ErrorCode { get; set; }

        [DataMember]
        public string ResultMessage
        {
            get
            {
                return _resultMessage;
            }
            set
            {
                _resultMessage = value;
            }
        }

        [DataMember]
        public System.Net.HttpStatusCode HttpStatusCode { get; set; }

        [DataMember(Name = "Result")]
        public TEntity Result { get; set; }
    }

    public static class ResultMessage
    {
        public static string GetErrorDescription(ErrorCode errorCode)
        {
            switch (errorCode)
            {
                case ErrorCode.Ok:
                    return "Authentication Succeeded";

                case ErrorCode.UserNotAuthenticated:
                    return "Authentication Failed";
            }
            return "Unexpected exception";
        }
    }

    public enum ErrorCode
    {
        Ok = 0,
        UserNotAuthenticated = 0x191
    }

    // This class is used to test that we don't deadlock when running with a synchronization context
    // which executes all work on a single thread.
    public class SingleThreadSynchronizationContext : SynchronizationContext
    {
        private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object>> _queue =
           new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();

        public SingleThreadSynchronizationContext(bool trackOperations)
        {
            _trackOperations = trackOperations;
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            if (d == null) throw new ArgumentNullException("d");
            if (ExecutionContext.IsFlowSuppressed())
            {
                _queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
            }
            else
            {
                var ec = ExecutionContext.Capture();
                _queue.Add(new KeyValuePair<SendOrPostCallback, object>(RunOnCapturedState, (d, state, ec)));
                void RunOnCapturedState(object obj)
                {
                    var s = ((SendOrPostCallback d, object state, ExecutionContext ec))obj;
                    ExecutionContext.Run(s.ec, new ContextCallback(d), s.state);
                }
            }
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            throw new NotSupportedException("Synchronously sending is not supported.");
        }

        private int _operationCount = 0;
        private readonly bool _trackOperations;

        public override void OperationStarted()
        {
            if (_trackOperations)
                Interlocked.Increment(ref _operationCount);
        }

        public override void OperationCompleted()
        {
            if (_trackOperations &&
                        Interlocked.Decrement(ref _operationCount) == 0)
                Complete();
        }

        public void RunOnCurrentThread()
        {
            KeyValuePair<SendOrPostCallback, object> workItem;
            while (_queue.TryTake(out workItem, 5000))
            {
                workItem.Key(workItem.Value);
            }
        }

        public Task RunOnThreadPoolThread()
        {
            return Task.Run(() =>
            {
                SetSynchronizationContext(this);
                RunOnCurrentThread();
            });
        }

        public void Complete()
        {
            _queue.CompleteAdding();
        }

        public static void Run(Action asyncMethod)
        {
            var prevCtx = SynchronizationContext.Current;
            try
            {
                var syncCtx = new SingleThreadSynchronizationContext(true);
                SynchronizationContext.SetSynchronizationContext(syncCtx);

                syncCtx.OperationStarted();
                asyncMethod();
                syncCtx.OperationCompleted();

                syncCtx.RunOnCurrentThread();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevCtx);
            }
        }

        public static void Run(Func<Task> asyncMethod)
        {
            var prevCtx = SynchronizationContext.Current;
            try
            {
                var syncCtx = new SingleThreadSynchronizationContext(false);
                SynchronizationContext.SetSynchronizationContext(syncCtx);

                var t = asyncMethod();
                t.ContinueWith(delegate { syncCtx.Complete(); }, TaskScheduler.Default);

                syncCtx.RunOnCurrentThread();
                t.GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevCtx);
            }
        }
    }
}

[DataContract(Namespace = "http://www.contoso.com/wcfnamespace")]
public class CompositeType
{
    private bool _boolValue = true;
    private string _stringValue = "Hello ";

    [DataMember]
    public bool BoolValue
    {
        get { return _boolValue; }
        set { _boolValue = value; }
    }

    [DataMember]
    public string StringValue
    {
        get { return _stringValue; }
        set { _stringValue = value; }
    }
}

[System.Runtime.Serialization.DataContract(Name = "FaultDetail", Namespace = "http://www.contoso.com/wcfnamespace")]
public class FaultDetail
{
    private string _report;

    public FaultDetail()
    {
    }

    public FaultDetail(string message)
    {
        _report = message;
    }

    [DataMember]
    public string Message
    {
        get { return _report; }
        set { _report = value; }
    }
}


[DataContract(Name = "FaultDetail2", Namespace = "http://www.contoso.com/wcfnamespace")]
public class FaultDetail2
{
    private string _report;

    public FaultDetail2(string message)
    {
        _report = message;
    }

    [DataMember]
    public string Message
    {
        get { return _report; }
        set { _report = value; }
    }
}

[DataContract(Namespace = "http://www.contoso.com/wcfnamespace")]
public class ComplexCompositeType : IEquatable<ComplexCompositeType>
{
    private bool _boolValue;
    private char _charValue;
    private DateTime _dateTimeValue;

    private int _intValue;
    private short _shortValue;
    private float _floatValue;
    private long _longValue;
    private double _doubleValue;

    private uint _uintValue;
    private ushort _ushortValue;
    private ulong _ulongValue;

    private Guid _guidValue;

    private byte[] _byteArrayValue;
    private char[] _charArrayValue;

    private string _stringValue;
    private string _longerStringValue;

    private sbyte _sbyteValue;
    private TimeSpan _timeSpanValue;
    private DayOfWeek _dayOfWeekValue;

    [DataMember]
    public bool BoolValue
    {
        get { return _boolValue; }
        set { _boolValue = value; }
    }

    [DataMember]
    public char CharValue
    {
        get { return _charValue; }
        set { _charValue = value; }
    }

    [DataMember]
    public DateTime DateTimeValue
    {
        get { return _dateTimeValue; }
        set { _dateTimeValue = value; }
    }

    [DataMember]
    public int IntValue
    {
        get { return _intValue; }
        set { _intValue = value; }
    }

    [DataMember]
    public short ShortValue
    {
        get { return _shortValue; }
        set { _shortValue = value; }
    }

    [DataMember]
    public float FloatValue
    {
        get { return _floatValue; }
        set { _floatValue = value; }
    }

    [DataMember]
    public long LongValue
    {
        get { return _longValue; }
        set { _longValue = value; }
    }

    [DataMember]
    public double DoubleValue
    {
        get { return _doubleValue; }
        set { _doubleValue = value; }
    }

    [DataMember]
    public uint UintValue
    {
        get { return _uintValue; }
        set { _uintValue = value; }
    }

    [DataMember]
    public ushort UshortValue
    {
        get { return _ushortValue; }
        set { _ushortValue = value; }
    }

    [DataMember]
    public ulong UlongValue
    {
        get { return _ulongValue; }
        set { _ulongValue = value; }
    }

    [DataMember]
    public Guid GuidValue
    {
        get { return _guidValue; }
        set { _guidValue = value; }
    }

    [DataMember]
    public byte[] ByteArrayValue
    {
        get { return _byteArrayValue; }
        set { _byteArrayValue = value; }
    }

    [DataMember]
    public char[] CharArrayValue
    {
        get { return _charArrayValue; }
        set { _charArrayValue = value; }
    }

    [DataMember]
    public string StringValue
    {
        get { return _stringValue; }
        set { _stringValue = value; }
    }

    [DataMember]
    public string LongerStringValue
    {
        get { return _longerStringValue; }
        set { _longerStringValue = value; }
    }

    [DataMember]
    public sbyte SbyteValue
    {
        get { return _sbyteValue; }
        set { _sbyteValue = value; }
    }

    [DataMember]
    public TimeSpan TimeSpanValue
    {
        get { return _timeSpanValue; }
        set { _timeSpanValue = value; }
    }

    [DataMember]
    public DayOfWeek DayOfWeekValue
    {
        get { return _dayOfWeekValue; }
        set { _dayOfWeekValue = value; }
    }

    public bool Equals(ComplexCompositeType other)
    {
        if (other == null) { return false; }
        if (object.ReferenceEquals(this, other)) { return true; }

        if (_boolValue != other._boolValue) { return false; }
        if (_charValue != other._charValue) { return false; }
        if (_dateTimeValue != other._dateTimeValue) { return false; }

        if (_intValue != other._intValue) { return false; }
        if (_shortValue != other._shortValue) { return false; }
        if (_floatValue != other._floatValue) { return false; }
        if (_longValue != other._longValue) { return false; }
        if (_doubleValue != other._doubleValue) { return false; }

        if (_uintValue != other._uintValue) { return false; }
        if (_ushortValue != other._ushortValue) { return false; }
        if (_ulongValue != other._ulongValue) { return false; }

        if (_guidValue != other._guidValue) { return false; }

        if (_byteArrayValue.Length != other._byteArrayValue.Length) { return false; }
        for (int i = 0; i < _byteArrayValue.Length; i++)
        {
            if (_byteArrayValue[i] != other._byteArrayValue[i]) { return false; }
        }

        for (int i = 0; i < _charArrayValue.Length; i++)
        {
            if (_charArrayValue[i] != other._charArrayValue[i]) { return false; }
        }

        if (_stringValue != other._stringValue) { return false; }
        if (_longerStringValue != other._longerStringValue) { return false; }

        if (_sbyteValue != other._sbyteValue) { return false; }
        if (_timeSpanValue != other._timeSpanValue) { return false; }
        if (_dayOfWeekValue != other._dayOfWeekValue) { return false; }

        return true;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("BoolValue: " + _boolValue);
        sb.AppendLine("CharValue: " + _charValue);
        sb.AppendLine("DateTimeValue: " + _dateTimeValue);
        sb.AppendLine("IntValue: " + _intValue);
        sb.AppendLine("ShortValue: " + _shortValue);
        sb.AppendLine("FloatValue: " + _floatValue);
        sb.AppendLine("LongValue: " + _longValue);
        sb.AppendLine("DoubleValue: " + _doubleValue);
        sb.AppendLine("UintValue: " + _uintValue);
        sb.AppendLine("ushortValue: " + _ushortValue);
        sb.AppendLine("ulongValue: " + _ulongValue);
        sb.AppendLine("GuidValue: " + _guidValue);
        sb.AppendLine("ByteArrayValue: " + (_byteArrayValue != null ? "Length: " + _byteArrayValue.Length : "null"));
        sb.AppendLine("CharArrayValue: " + (_charArrayValue != null ? "Length: " + _charArrayValue.Length : "null"));
        sb.AppendLine("StringValue: " + (string.IsNullOrEmpty(_stringValue) ? "<empty>" : _stringValue.ToString()));
        sb.AppendLine("LongerStringValue: " + (string.IsNullOrEmpty(_longerStringValue) ? "<empty>" : _longerStringValue.ToString()));
        sb.AppendLine("SbyteValue: " + _sbyteValue);
        sb.AppendLine("TimeSpanValue: " + _timeSpanValue);
        sb.AppendLine("DayOfWeekValue: " + _dayOfWeekValue);

        return sb.ToString();
    }
}

[DataContract(Namespace = "http://www.contoso.com/wcfnamespace")]
public class TestHttpRequestMessageProperty
{
    private bool _suppressEntityBody;
    private string _method;
    private string _queryString;
    private Dictionary<string, string> _headers;

    public TestHttpRequestMessageProperty()
    {
        _headers = new Dictionary<string, string>();
    }

    [DataMember]
    public bool SuppressEntityBody
    {
        get { return _suppressEntityBody; }
        set { _suppressEntityBody = value; }
    }

    [DataMember]
    public string Method
    {
        get { return _method; }
        set { _method = value; }
    }

    [DataMember]
    public string QueryString
    {
        get { return _queryString; }
        set { _queryString = value; }
    }

    [DataMember]
    public Dictionary<string, string> Headers
    {
        get { return _headers; }
        set { _headers = value; }
    }
}

public class XmlCompositeType
{
    private bool _boolValue = true;
    private string _stringValue = "Hello ";

    public bool BoolValue
    {
        get { return _boolValue; }
        set { _boolValue = value; }
    }

    public string StringValue
    {
        get { return _stringValue; }
        set { _stringValue = value; }
    }
}

// This type should only be used by test Contract.DataContractTests.NetTcpBinding_DuplexCallback_ReturnsDataContractComplexType
// It tests a narrow scenario that returns a DataContract attributed type in the callback method that is not known by the ServiceContract attributed interface
// This test is designed to make sure the NET Native toolchain creates the needed serializer
[DataContract(Namespace = "http://www.contoso.com/wcfnamespace")]
public class ComplexCompositeTypeDuplexCallbackOnly : IEquatable<ComplexCompositeTypeDuplexCallbackOnly>
{
    private Guid _guidValue;

    [DataMember]
    public Guid GuidValue
    {
        get { return _guidValue; }
        set { _guidValue = value; }
    }

    public bool Equals(ComplexCompositeTypeDuplexCallbackOnly other)
    {
        if (other == null) { return false; }
        if (object.ReferenceEquals(this, other)) { return true; }

        if (_guidValue != other._guidValue) { return false; }

        return true;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("GuidValue: " + _guidValue);

        return sb.ToString();
    }
}

// This type should only be used by test Contract.DataContractTests.NetTcpBinding_DuplexCallback_ReturnsXmlComplexType
// It tests a narrow scenario that returns an Xml attributed type in the callback method that is not known by the ServiceContract attributed interface
// This test is designed to make sure the NET Native toolchain creates the needed serializer
public class XmlCompositeTypeDuplexCallbackOnly
{
    private bool _boolValue = true;
    private string _stringValue = "Hello ";

    public bool BoolValue
    {
        get { return _boolValue; }
        set { _boolValue = value; }
    }

    public string StringValue
    {
        get { return _stringValue; }
        set { _stringValue = value; }
    }
}

[CallbackBehavior(UseSynchronizationContext = false)]
public class WcfDuplexService_CallbackDebugBehavior_Callback : IWcfDuplexService_CallbackDebugBehavior_Callback
{
    public void ReplyThrow(string input)
    {
        throw new Exception(input);
    }
}

public class DuplexTaskReturnServiceCallback : IWcfDuplexTaskReturnCallback
{
    private bool _wrapExceptionInTask;

    public DuplexTaskReturnServiceCallback(bool wrapExceptionInTask = false)
    {
        _wrapExceptionInTask = wrapExceptionInTask;
    }

    public Task<Guid> ServicePingCallback(Guid guid)
    {
        // This returns the guid to the service which called this callback.
        // We could return Task.FromResult(guid) but that means we could execute the 
        // completion on the same thread. But if someone is using a task it means they 
        // would potentially have the completion on another thread.
        return Task.Run<Guid>(() => guid);
    }

    public Task<Guid> ServicePingFaultCallback(Guid guid)
    {
        var fault = new FaultException<FaultDetail>(
            new FaultDetail("Throwing a Fault Exception from the Callback method."),
            new FaultReason("Reason: Testing FaultException returned from Duplex Callback"),
            new FaultCode("ServicePingFaultCallback"),
            "http://tempuri.org/IWcfDuplexTaskReturnCallback/ServicePingFaultCallbackFaultDetailFault");

        if (_wrapExceptionInTask)
        {
            var tcs = new TaskCompletionSource<Guid>();
            tcs.TrySetException(fault);
            return tcs.Task;
        }

        throw fault;
    }
}

public class CustomBodyWriter : BodyWriter
{
    private string _bodyContent;

    public CustomBodyWriter()
        : base(true)
    { }

    public CustomBodyWriter(string message)
        : base(true)
    {
        _bodyContent = message;
    }

    protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
    {
        writer.WriteString(_bodyContent);
    }
}

//Helper class used in this test to allow construction of ContractDescription
public class MyClientBase<T> : ClientBase<T> where T : class
{
    public MyClientBase(Binding binding, EndpointAddress endpointAddress)
        : base(binding, endpointAddress)
    {
    }

    public MyClientBase(ServiceEndpoint serviceEndpoint)
        : base(serviceEndpoint)
    {
    }
}

// This helper class is used for ClientBase<T> tests
public class MyClientBase : ClientBase<IWcfServiceGenerated>
{
    public MyClientBase(Binding binding, EndpointAddress endpointAddress)
        : base(binding, endpointAddress)
    {
    }

    public IWcfServiceGenerated Proxy
    {
        get { return base.Channel; }
    }
}

// This helper class is used for ClientBase<T> tests
public class MyClientBaseWithChannelBase : ClientBase<IWcfServiceBeginEndGenerated>
{
    public MyClientBaseWithChannelBase(Binding binding, EndpointAddress endpointAddress)
        : base(binding, endpointAddress)
    {
    }

    public IWcfServiceBeginEndGenerated Proxy
    {
        get { return base.Channel; }
    }

    protected override IWcfServiceBeginEndGenerated CreateChannel()
    {
        return new MyChannelBase(this);
    }

    private class MyChannelBase : ChannelBase<IWcfServiceBeginEndGenerated>, IWcfServiceBeginEndGenerated
    {
        public MyChannelBase(ClientBase<IWcfServiceBeginEndGenerated> client) :
            base(client)
        {
        }

        public IAsyncResult BeginEcho(string message, AsyncCallback callback, object asyncState)
        {
            object[] _args = new object[1];
            _args[0] = message;
            return (IAsyncResult)base.BeginInvoke("Echo", _args, callback, asyncState);
        }

        public string EndEcho(IAsyncResult result)
        {
            object[] _args = new object[0];
            return (String)base.EndInvoke("Echo", _args, result);
        }

        public IAsyncResult BeginMessageRequestReply(Message request, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public string Echo(string message)
        {
            // Note: the public contract does not include base.Invoke(), so we are required
            // to use the Begin/End pattern over a sync method when using ChannelBase<T>
            object[] args = new object[] { message };
            IAsyncResult ar =  base.BeginInvoke(nameof(Echo), args, callback: null, state: null);
            return (String)base.EndInvoke(nameof(Echo), new object[1], ar);
        }

        public Message EndMessageRequestReply(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public Message MessageRequestReply(Message request)
        {
            throw new NotImplementedException();
        }
    }
}

// This helper class is used for DuplexClientBase<T> tests
public class MyDuplexClientBase<T> : DuplexClientBase<T> where T : class
{
    public MyDuplexClientBase(InstanceContext callbackInstance, Binding binding, EndpointAddress endpointAddress)
        : base(callbackInstance, binding, endpointAddress)
    {
    }
}

[CallbackBehavior(UseSynchronizationContext = false)]
public class WcfDuplexServiceCallback : IWcfDuplexServiceCallback, IWcfDuplexService_DataContract_Callback, IWcfDuplexService_Xml_Callback
{
    private TaskCompletionSource<Guid> _tcs;
    private TaskCompletionSource<XmlCompositeTypeDuplexCallbackOnly> _xml_tcs;
    private TaskCompletionSource<ComplexCompositeTypeDuplexCallbackOnly> _datacontract_tcs;

    public WcfDuplexServiceCallback()
    {
        _tcs = new TaskCompletionSource<Guid>();
        _xml_tcs = new TaskCompletionSource<XmlCompositeTypeDuplexCallbackOnly>();
        _datacontract_tcs = new TaskCompletionSource<ComplexCompositeTypeDuplexCallbackOnly>();
    }

    public Guid CallbackGuid
    {
        get
        {
            if (_tcs.Task.Wait(ScenarioTestHelpers.TestTimeout))
            {
                return _tcs.Task.Result;
            }
            throw new TimeoutException(string.Format("Not completed within the alloted time of {0}", ScenarioTestHelpers.TestTimeout));
        }
    }

    public XmlCompositeTypeDuplexCallbackOnly XmlCallbackGuid
    {
        get
        {
            if (_xml_tcs.Task.Wait(ScenarioTestHelpers.TestTimeout))
            {
                return _xml_tcs.Task.Result;
            }
            throw new TimeoutException(string.Format("Not completed within the alloted time of {0}", ScenarioTestHelpers.TestTimeout));
        }
    }

    public ComplexCompositeTypeDuplexCallbackOnly DataContractCallbackGuid
    {
        get
        {
            if (_datacontract_tcs.Task.Wait(ScenarioTestHelpers.TestTimeout))
            {
                return _datacontract_tcs.Task.Result;
            }
            throw new TimeoutException(string.Format("Not completed within the alloted time of {0}", ScenarioTestHelpers.TestTimeout));
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

    public void OnXmlPingCallback(XmlCompositeTypeDuplexCallbackOnly xmlCompositeType)
    {
        // Set the result in an async task with a 100ms delay to prevent a race condition
        // where the OnPingCallback hasn't sent the reply to the server before the channel is closed.
        Task.Run(async () =>
        {
            await Task.Delay(100);
            _xml_tcs.SetResult(xmlCompositeType);
        });
    }

    public void OnDataContractPingCallback(ComplexCompositeTypeDuplexCallbackOnly dataContractCompositeType)
    {
        // Set the result in an async task with a 100ms delay to prevent a race condition
        // where the OnPingCallback hasn't sent the reply to the server before the channel is closed.
        Task.Run(async () =>
        {
            await Task.Delay(100);
            _datacontract_tcs.SetResult(dataContractCompositeType);
        });
    }
}

public class BloatedStream : Stream
{
    private readonly long _length;
    private long _position;

    public BloatedStream(long length)
    {
        _length = length;
        _position = 0;
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override void Flush()
    {
        throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (_position < _length)
        {
            var numberOfBytesInThisRead = (int)Math.Min(count, _length - _position);
            _position += numberOfBytesInThisRead;
            return numberOfBytesInThisRead;
        }

        return 0;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }
}

public class FlowControlledStream : Stream
{
    private ManualResetEvent _waitEvent = new ManualResetEvent(false);
    // Used to control when Read will return 0.
    public bool StopStreaming { get; set; }
    //bool readCalledWithStopStreaming = false;

    public TimeSpan ReadThrottle { get; set; }

    // Only set this if you don't want to manually control when 
    // the stream stops.
    // Keep it low - less than 1 second.  The server can send bytes very quickly, so
    // sending a continuous stream will easily blow the MaxReceivedMessageSize buffer.
    public TimeSpan StreamDuration { get; set; }

    private DateTime _readStartedTime;
    private long _totalBytesRead = 0;

    public override bool CanRead
    {
        get { return !StopStreaming; }
    }

    public override bool CanSeek
    {
        get { return false; }
    }

    public override bool CanWrite
    {
        get { return false; }
    }

    public override void Flush()
    {
    }

    public override long Length
    {
        get { throw new NotImplementedException(); }
    }

    public override long Position
    {
        get
        {
            return _totalBytesRead;
        }
        set
        {
            _totalBytesRead = value;
        }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        // Duration-based streaming logic: Control the "StopStreaming" flag based on a Duration
        if (StreamDuration != TimeSpan.Zero)
        {
            if (_readStartedTime == DateTime.MinValue)
            {
                _readStartedTime = DateTime.Now;
            }
            if (DateTime.Now - _readStartedTime >= StreamDuration)
            {
                StopStreaming = true;
            }
        }

        if (StopStreaming)
        {
            buffer[offset] = 0;
            return 0;
        }

        // Allow Read to continue as long as StopStreaming is false.
        // Just fill buffer with as many random bytes as necessary.
        int seed = DateTime.Now.Millisecond;
        Random rand = new Random(seed);
        byte[] randomBuffer = new byte[count];
        rand.NextBytes(randomBuffer);
        randomBuffer.CopyTo(buffer, offset);
        _totalBytesRead += count;

        if (ReadThrottle != TimeSpan.Zero)
        {
            // Thread.Sleep and Thread.CurrentThread.Join are not available in NET Native
            _waitEvent.WaitOne(ReadThrottle);
        }
        return count;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}

public class ClientReceiver : IPushCallback, IDisposable
{
    private bool _disposed = false;

    public ManualResetEvent LogReceived { get; set; }
    public ManualResetEvent ReceiveDataInvoked { get; set; }
    public ManualResetEvent ReceiveDataCompleted { get; set; }
    public ManualResetEvent ReceiveStreamInvoked { get; set; }
    public ManualResetEvent ReceiveStreamCompleted { get; set; }
    public string Name { get; set; }

    public List<string> ServerLog { get; set; }

    public ClientReceiver()
    {
        LogReceived = new ManualResetEvent(false);
        ReceiveDataInvoked = new ManualResetEvent(false);
        ReceiveDataCompleted = new ManualResetEvent(false);
        ReceiveStreamInvoked = new ManualResetEvent(false);
        ReceiveStreamCompleted = new ManualResetEvent(false);
        Name = "ClientReceiver_" + DateTime.Now;
    }

    public void ReceiveData(string data)
    {
        ReceiveDataInvoked.Set();
        if (data == ScenarioTestHelpers.LastMessage)
        {
            ReceiveDataCompleted.Set();
        }
    }

    public void ReceiveStream(Stream stream)
    {
        ReceiveStreamInvoked.Set();

        int readResult;
        byte[] buffer = new byte[1000];
        do
        {
            try
            {
                readResult = stream.Read(buffer, 0, buffer.Length);
            }
            catch (Exception)
            {
                throw;
            }
        }
        while (readResult != 0);

        stream.Dispose();

        ReceiveStreamCompleted.Set();
    }

    public void ReceiveLog(List<string> log)
    {
        ServerLog = log;
        LogReceived.Set();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            LogReceived.Dispose();
            ReceiveDataInvoked.Dispose();
            ReceiveDataCompleted.Dispose();
            ReceiveStreamInvoked.Dispose();
            ReceiveStreamCompleted.Dispose();
        }

        _disposed = true;
    }
}

public class MyX509CertificateValidator : X509CertificateValidator
{
    private string _allowedIssuerName;
    public bool validateMethodWasCalled = false;

    public MyX509CertificateValidator(string allowedIssuerName)
    {
        if (string.IsNullOrEmpty(allowedIssuerName))
        {
            throw new ArgumentNullException("allowedIssuerName", "[MyX509CertificateValidator] The string parameter allowedIssuerName was null or empty.");
        }

        _allowedIssuerName = allowedIssuerName;
    }

    public override void Validate(X509Certificate2 certificate)
    {
        validateMethodWasCalled = true;

        // Check that there is a certificate.
        if (certificate == null)
        {
            throw new ArgumentNullException("certificate", "[MyX509CertificateValidator] The X509Certificate2 parameter certificate was null.");
        }

        // Check that the certificate issuer matches the configured issuer.
        if (!certificate.IssuerName.Name.Contains(_allowedIssuerName))
        {
            throw new Exception
                (string.Format("Certificate was not issued by a trusted issuer. Expected: {0}, Actual: {1}", _allowedIssuerName, certificate.IssuerName.Name));
        }
    }
}

[MessageContract(WrapperName = "login", WrapperNamespace = "http://www.contoso.com/", IsWrapped = true)]
public partial class LoginRequest
{
    [MessageBodyMember(Namespace = "http://www.contoso.com/", Order = 0)]
    [System.Xml.Serialization.XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string clientId;

    [MessageBodyMember(Namespace = "http://www.contoso.com/", Order = 1)]
    [System.Xml.Serialization.XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string user;

    [MessageBodyMember(Namespace = "http://www.contoso.com/", Order = 2)]
    [System.Xml.Serialization.XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string pwd;

    public LoginRequest()
    {
    }

    public LoginRequest(string clientId, string user, string pwd)
    {
        this.clientId = clientId;
        this.user = user;
        this.pwd = pwd;
    }
}

[MessageContract(WrapperName = "loginResponse", WrapperNamespace = "http://www.contoso.com/", IsWrapped = true)]
public partial class LoginResponse
{
    [MessageBodyMember(Namespace = "http://www.contoso.com/", Order = 0)]
    [System.Xml.Serialization.XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string @return;

    public LoginResponse()
    {
    }

    public LoginResponse(string @return)
    {
        this.@return = @return;
    }
}

// This type should be used by XmlSerializerFormat_EchoVeryComplexType only.
public class XmlVeryComplexType
{
    private int _id;
    private NonInstantiatedType _nonInstantiatedField = null;

    public NonInstantiatedType NonInstantiatedField
    {
        get
        {
            return _nonInstantiatedField;
        }
        set
        {
            _nonInstantiatedField = value;
        }
    }

    public int Id
    {
        get
        {
            return _id;
        }

        set
        {
            _id = value;
        }
    }
}

public class SoapComplexType
{
    private bool _boolValue;
    private string _stringValue;

    public bool BoolValue
    {
        get { return _boolValue; }
        set { _boolValue = value; }
    }

    public string StringValue
    {
        get { return _stringValue; }
        set { _stringValue = value; }
    }
}

[SoapType(Namespace = "WcfService")]
public class CustomerObject
{
    public string Name { get; set; }
    public object Data { get; set; }
}

[Serializable]
[SoapType(Namespace = "WcfService")]
public partial class AdditionalData
{
    public string Field
    {
        get; set;
    }
}

// This type should be used by XmlSerializerFormat_EchoVeryComplexType only.
// The type should not ever be instantiated. 
public class NonInstantiatedType
{
}

// This type is specifically not attributed in order to test default serialize/deserialize behavior.
public class UniqueType
{
    public string stringValue;
}

// This type should be exclusively used by test OperationContextScope_HttpRequestCustomMessageHeader_RoundTrip_Verify.
[XmlType(Namespace = "urn:TestWebServices/MyWebService/")]
public class MesssageHeaderCreateHeaderWithXmlSerializerTestType
{
    private string _message;

    [XmlElement(Order = 0)]
    public string Message
    {
        get
        {
            return _message;
        }
        set
        {
            _message = value;
        }
    }
}

// This type should only be used by test XmlSerializerFormatTests.
// This test is designed to make sure the NET Native toolchain creates the needed serializer
[MessageContract(WrapperName = "XmlMessageContractTestRequest", WrapperNamespace = "http://www.contoso.com/XmlMessageContarctTestMessages", IsWrapped = true)]
public partial class XmlMessageContractTestRequest
{
    [MessageBodyMember(Namespace = "http://www.contoso.com/XmlMessageContarctTestMessages", Order = 0)]
    public string Message;

    public XmlMessageContractTestRequest()
    {
    }

    public XmlMessageContractTestRequest(string message)
    {
        this.Message = message;
    }
}

// This type should only be used by test XmlSerializerFormatTests.
// This test is designed to make sure the NET Native toolchain creates the needed serializer
[MessageContract(WrapperName = "XmlMessageContractTestRequestWithMessageHeader", WrapperNamespace = "http://www.contoso.com/XmlMessageContarctTestMessages", IsWrapped = true)]
public partial class XmlMessageContractTestRequestWithMessageHeader
{
    [MessageHeader(Name = "XmlMessageContractTestRequestWithMessageHeaderMessage", Namespace = "http://www.contoso.com", MustUnderstand = false)]
    public string Message;

    public XmlMessageContractTestRequestWithMessageHeader()
    {
    }

    public XmlMessageContractTestRequestWithMessageHeader(string message)
    {
        this.Message = message;
    }
}

// This type should only be used by test XmlSerializerFormatTests.
// This test is designed to make sure the NET Native toolchain creates the needed serializer
[MessageContract(WrapperName = "XmlMessageContractTestResponse", WrapperNamespace = "http://www.contoso.com/XmlMessageContarctTestMessages", IsWrapped = true)]
public partial class XmlMessageContractTestResponse
{
    [MessageBodyMember(Namespace = "http://www.contoso.com/XmlMessageContarctTestMessages", Order = 0)]
    public string _message;

    public XmlMessageContractTestResponse()
    {
    }

    public XmlMessageContractTestResponse(string message)
    {
        this._message = message;
    }

    [MessageHeader(Name = "OutOfBandData", Namespace = "http://www.contoso.com", MustUnderstand = false)]
    public string Message
    {
        get
        {
            return _message;
        }
        set
        {
            _message = value;
        }
    }
}

[DataContract(Name = "KnownTypeA", Namespace = "http://www.contoso.com/wcfnamespace")]
public class KnownTypeA
{
    private string _content;

    public KnownTypeA()
    {
    }

    public KnownTypeA(string content)
    {
        _content = content;
    }

    [DataMember]
    public string Content
    {
        get { return _content; }
        set { _content = value; }
    }
}

[DataContract]
public class Employee
{
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string Age { get; set; }
}

[DataContract]
public class Manager : Employee
{
    [DataMember]
    public int OfficeId { get; set; }
}


[DataContract]
public class SessionTestsCompositeType
{
    [DataMember]
    public int MethodAValue { get; set; }
    [DataMember]
    public int MethodBValue { get; set; }
}

[DataContract(Name = "FaultDetailWithXmlSerializerFormatAttribute", Namespace = "http://www.contoso.com/wcfnamespace")]
public class FaultDetailWithXmlSerializerFormatAttribute
{
    private bool _usedXmlSerializer;
    private bool _usedDataContractSerializer;

    public FaultDetailWithXmlSerializerFormatAttribute()
    {
    }

    // If the Fault from the server was sent using the Xml Serializer this property will be set to 'true'
    [DataMember]
    [XmlElement]
    public bool UsedXmlSerializer
    {
        get { return _usedXmlSerializer; }
        set { _usedXmlSerializer = value; }
    }

    // If the Fault from the server was sent using the Data Contract Serializer this property will be set to 'true'
    [DataMember]
    [XmlElement]
    public bool UsedDataContractSerializer
    {
        get { return _usedDataContractSerializer; }
        set { _usedDataContractSerializer = value; }
    }
}

[MessageContract(WrapperName = "PingResponse", IsWrapped = true)]
public class PingEncodedResponse
{
    [MessageBodyMember(Namespace = "", Order = 0)]
    public int @Return;
}

[MessageContract(WrapperName = "Ping", IsWrapped = true)]
public class PingEncodedRequest
{
    [MessageBodyMember(Namespace = "", Order = 0)]
    public string Pinginfo;

    public PingEncodedRequest() { }

    public PingEncodedRequest(string pinginfo)
    {
        this.Pinginfo = pinginfo;
    }
}
