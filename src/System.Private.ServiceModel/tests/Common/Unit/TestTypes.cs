// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

    //// Needed by the MahjongApp Scenario tests
    [DataContract(Name = "ResultOf{0}", Namespace = "http://www.contoso.com/wcfnamespace")]
    public class ResultObject<TEntity>
    {
        private string _errorMessage;

        public ResultObject()
        {
            _errorMessage = "OK";
            this.HttpStatusCode = System.Net.HttpStatusCode.OK;
            this.ErrorCode = 0;
        }

        public static ResultObject<T> CopyResultErrorsStatus<T, D>(ResultObject<D> anotherResult)
        {
            return new ResultObject<T> { ErrorCode = anotherResult.ErrorCode, ErrorMessage = anotherResult.ErrorMessage, HttpStatusCode = anotherResult.HttpStatusCode };
        }

        public static ResultObject<T> CreateDefault<T>()
        {
            return new ResultObject<T> { Result = default(T), ErrorCode = 0, ErrorMessage = TestTypes.ErrorMessage.Get(TestTypes.ErrorCode.Ok) };
        }

        public void Exception(System.Exception ex)
        {
            this.ErrorCode = -1;
            this.ErrorMessage = (ex == null) ? "unexpected" : ex.Message;
        }

        [DataMember]
        public int ErrorCode { get; set; }

        [DataMember]
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

        [DataMember]
        public System.Net.HttpStatusCode HttpStatusCode { get; set; }

        [DataMember(Name = "Result")]
        public TEntity Result { get; set; }
    }

    public static class ErrorMessage
    {
        private static Dictionary<ErrorCode, string> s_localizedErrorCodes;

        public static string Get(ErrorCode errorCode)
        {
            if (s_localizedErrorCodes != null)
            {
                return (s_localizedErrorCodes.ContainsKey(errorCode) ? s_localizedErrorCodes[errorCode] : s_localizedErrorCodes[ErrorCode.UnknownException]);
            }
            return "Unexpected exception";
        }

        public static string GetErrorDescription(ErrorCode errorCode)
        {
            switch (errorCode)
            {
                case ErrorCode.Ok:
                    return "Success";

                case ErrorCode.DcXboxTokeNull:
                case ErrorCode.DcDailyFileNotAvailable:
                case ErrorCode.DcDailyFileBroken:
                    return "XboxErrorText";

                case ErrorCode.DcMonthlyFileNotAvailable:
                case ErrorCode.DcMonthlyFileBroken:
                    return "DCDownloadingDataErrorText";

                case ErrorCode.DcCanNotWriteMonthlyUserProgress:
                case ErrorCode.DcCanNotWriteDailyUserProgress:
                    return "XboxErrorSavingText";

                case ErrorCode.NotOwner:
                    return "Current user is not owner of theme and can't change it";

                case ErrorCode.ThemeNotFound:
                    return "Theme not found and can't be updated";

                case ErrorCode.AsyncOperationFault:
                    return "AsyncOperationFault";

                case ErrorCode.DataNotFound:
                    return "Data not found";

                case ErrorCode.CantShare:
                    return "Theme can't be shared due to internal error";

                case ErrorCode.GamePlayIsNotValid:
                    return "Game play is not valid";

                case ErrorCode.UserNotAuthenticated:
                    return "User not authenticated";

                case ErrorCode.UnknownException:
                    return "Exception cant be handled correctly";

                case ErrorCode.NullData:
                    return "Null Data was passed to the service";

                case ErrorCode.SameData:
                    return "Same data was requested";

                case ErrorCode.OnlineDataReceived:
                    return "Online Data received successfully";

                case ErrorCode.OfflineDataReceived:
                    return "Offline Data received successfully";

                case ErrorCode.OfflineOnlineDataReceived:
                    return "Online and Offline Data received successfully";

                case ErrorCode.LatencyOverhead:
                    return "Request latency overhead";
            }
            return "Unexpected exception";
        }

        public static void Init(Dictionary<ErrorCode, string> localizedErrorCodes)
        {
            ErrorMessage.s_localizedErrorCodes = localizedErrorCodes;
        }
    }

    public enum ErrorCode
    {
        AsyncOperationFault = 0x67,
        CantShare = 0x69,
        DataNotFound = 0x68,
        DcCanNotWriteDailyUserProgress = 7,
        DcCanNotWriteMonthlyUserProgress = 6,
        DcDailyFileBroken = 5,
        DcDailyFileNotAvailable = 4,
        DcFileBroken = 8,
        DcMonthlyFileBroken = 3,
        DcMonthlyFileNotAvailable = 2,
        DcUserMonthlyFileIsNotAvailable = 9,
        DcXboxTokeNull = 1,
        DeserializeError = 12,
        GamePlayIsNotValid = 0xc9,
        LatencyOverhead = 0x198,
        NotOwner = 0x65,
        NullData = 0x195,
        OfflineDataReceived = 0x321,
        OfflineOnlineDataReceived = 0x322,
        Ok = 0,
        OnlineDataReceived = 800,
        PremiumErrorNoInternetConnection = 0x1f7,
        SameData = 0x130,
        SponsorThemeIncorrectFormat = 15,
        ThemeNotFound = 0x66,
        UnknownException = 0x194,
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
            _queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
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

public class DuplexTaskReturnServiceCallback : IWcfDuplexTaskReturnCallback
{
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
        throw new FaultException<FaultDetail>(
          new FaultDetail("Throwing a Fault Exception from the Callback method."),
          new FaultReason("Reason: Testing FaultException returned from Duplex Callback"),
          new FaultCode("ServicePingFaultCallback"),
          "http://tempuri.org/IWcfDuplexTaskReturnCallback/ServicePingFaultCallback");
    }
}
